﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ReportApp.Components;
using ReportApp.Services;


namespace ReportApp.Shared
{
    public partial class FeedbackMenuComponent
    {
        public bool showmenu = false;

        public bool showBugReportComponent = false;

        public bool showFeedReportComponent = false;

        protected bool isAddBugReportInitialized = false;


        public IEnumerable<BugReport> BugReports { get; set; }

        public IEnumerable<Feedback> Feedbacks { get; set; }    

        [Inject]
        public IBugReportDataService BugReportDataService { get; set; }
        [Inject]
        public IFeedbackDataService FeedbackDataService { get; set; }

        [Inject]
        public IUserDataService UserDataService { get; set; }

        protected AddBugReport AddBugReport { get; set; }
        protected AddFeedbackReport AddFeedbackReport { get; set; }

        [Parameter]
        public EventCallback<bool> OnClickEventCallback { get; set; }
      


        protected async override Task OnInitializedAsync()
        {
           var BugReports = (await BugReportDataService.GetAllBugReports()).ToList();
			var Feedbacks = (await FeedbackDataService.GetAllFeedbacks()).ToList();
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


        private void ToggleMenu()
        {
            showmenu = !showmenu;
           
            showBugReportComponent = false;

         showFeedReportComponent = false;
        }

 

	
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
