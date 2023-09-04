using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ReportApp.Components;
using ReportApp.Services;


namespace ReportApp.Shared
{
    public partial class FeedbackMenuComponent
    {
        //Dependencies
        public bool showmenu = false;

        public bool showScreenRecorderComponent = false;

        public bool showBugReportComponent = false;

        public bool showFeedReportComponent = false;

        protected bool isAddBugReportInitialized = false;

        private bool recordingInProgress;

        public IEnumerable<MediaStream> MediaStreams { get; set; }

        public IEnumerable<BugReport> BugReports { get; set; }

        public IEnumerable<Feedback> Feedbacks { get; set; }


        [Inject]
        public IScreenRecorderService ScreenRecorderService { get; set; }

        [Inject]
        public IBugReportDataService BugReportDataService { get; set; }
        [Inject]
        public IFeedbackDataService FeedbackDataService { get; set; }

        [Inject]
        public IUserDataService UserDataService { get; set; }

        protected ScreenRecorder ScreenRecorder { get; set; }
        protected AddBugReport AddBugReport { get; set; }
        protected AddFeedbackReport AddFeedbackReport { get; set; }

        [Parameter]
        public EventCallback<bool> OnClickEventCallback { get; set; }


        protected async override Task OnInitializedAsync()
        {
            var BugReports = (await BugReportDataService.GetAllBugReports()).ToList();
			var Feedbacks = (await FeedbackDataService.GetAllFeedbacks()).ToList();
        }

        public async Task ScreenRecorder_OnDialogClose()
        {
            await ScreenRecorderService.Reset();
            StateHasChanged();
        }

        public async void AddBugReport_OnDialogClose()
        {
			var BugReports = (await BugReportDataService.GetAllBugReports()).ToList();
         
            StateHasChanged();
        }

        public async void AddFeedReport_OnDialogClose()
        {
			var Feedbacks = (await FeedbackDataService.GetAllFeedbacks()).ToList();
         
            StateHasChanged();
        }

        //Method for ShowMenu Feedback Button
        private void ToggleMenu()
        {
            showmenu = !showmenu;

            showScreenRecorderComponent = false;

            showBugReportComponent = false;

            showFeedReportComponent = false;
        }

        //show component screen recorder
        protected async Task QuickRecorder()
        {
            StateHasChanged();
            await Task.Delay(5);
            showmenu = false;

            showScreenRecorderComponent = true;
            StateHasChanged();
        }

        //show component bug
        protected async Task QuickAddBug()
        {
            StateHasChanged();
            await Task.Delay(5);
            showmenu = false;
          
            showBugReportComponent = true;
            StateHasChanged();
           
            while (AddBugReport == null)
            {
                await Task.Delay(5);
            }

            AddBugReport.ShowAsync();
       
        }


        //show component feedback
        protected async Task QuickAddFeed()
        {
          
            StateHasChanged(); 
            await Task.Delay(5);
            showmenu = false;
         
            showFeedReportComponent = true;
            StateHasChanged();
            
            while (AddFeedbackReport == null)
            {
                await Task.Delay(5);
            }

            AddFeedbackReport.Show();
         
         }

        
    }
}
