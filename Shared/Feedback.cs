using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReportApp.Shared
{
	public class Feedback
	{
        public int FeedbackId { get; set; }


        public int UserId { get; set; }


        public DateTime Timestamp { get; set; }


        public int Ranking { get; set; }


        [StringLength(1500, ErrorMessage = "The comments length can't exceed 1500 characters.")]
        public string Comments { get; set; }


        public int? AttachmentId { get; set; }


        /* How did you first learn about our app? */

        [StringLength(150, ErrorMessage = "The answer length can't exceed 150 characters.")]
        public string? Question1 { get; set; }


        /* On a scale of 1 to 5, rate the interface of the mobile application. */
        public int? Question2 { get; set; }


        /* Would you recommend this app to your family and friends? */
        public bool? Question3 { get; set; }

        public User? User { get; set; }

        public Attachments? Attachment { get; set; }
    }
}
