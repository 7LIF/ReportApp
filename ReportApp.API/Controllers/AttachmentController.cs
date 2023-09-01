using Microsoft.AspNetCore.Mvc;
using ReportApp.API.Models;
using ReportApp.Services;
using ReportApp.Shared;

namespace ReportApp.API.Controllers
{
    [Route("api/att")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {

        private const string AttachmentsFolder = "Attachments";
        private readonly IAttachment _attachmentModel;

        public AttachmentController(IAttachment attachmentModel, IWebHostEnvironment env)
        {
            _attachmentsFolderPath = Path.Combine(env.ContentRootPath, "Attachments");
            _attachmentModel = attachmentModel;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAttachments()
        {
            try
            {

                return Ok(_attachmentModel.GetAllAttachments());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        private readonly string _attachmentsFolderPath;



        [HttpGet("{fileName}")]
        public IActionResult GetAttachment(string fileName)
        {

            try
            {
                string filePath = Path.Combine(_attachmentsFolderPath, fileName);




                if (System.IO.File.Exists(filePath))
                {
                    var fileStream = System.IO.File.OpenRead(filePath);
                    return File(fileStream, "image/jpeg");
                }

                return NotFound();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        //Focused controller for the files we will upload
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] List<AttachmentDto> attachmentsDto)
        {
            try
            {
                var attachmentsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), AttachmentsFolder);
                if (!Directory.Exists(attachmentsFolderPath))
                {
                    Directory.CreateDirectory(attachmentsFolderPath);
                }

                foreach (var attachmentDto in attachmentsDto)
                {
                    var buf = Convert.FromBase64String(attachmentDto.Base64data);
                    var extension = Path.GetExtension(attachmentDto.FileName);
                    var fileName = Path.GetFileNameWithoutExtension(attachmentDto.FileName);
                    var attachmentPath = Path.Combine(attachmentsFolderPath, fileName + extension);

                    await System.IO.File.WriteAllBytesAsync(attachmentPath, buf);

                    var attachment = new Attachments
                    {
                        FeedbackId = attachmentDto.FeedbackId,
                        BugReportId = attachmentDto.BugReportId,
                        Base64data = attachmentDto.Base64data,
                        ContentType = attachmentDto.ContentType,
                        FileName = attachmentDto.FileName,
                        FilePath = attachmentPath
                    };

                    await _attachmentModel.AddAttachments(attachment);
                }

                return Ok("Files uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await _attachmentModel.DeleteAll();
            return Ok();
        }
    }
}
