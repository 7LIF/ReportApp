﻿using ReportApp.Services;
using Microsoft.AspNetCore.Components;
using ReportApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace ReportApp.Components
{
    public partial class AddScreenRecorder
    {
        // Dependencies
        [Inject]
        public IScreenRecorderService ScreenRecorderService { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        private bool isInitialized = false;
        private bool isShowing = false;
        public bool ShowScreenRecorder { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            isInitialized = true;
        }

        // Methods for showing, closing, and resetting the component
        public async Task ShowAsync()
        {
            if (!isInitialized)
            {
                return;
            }

            isShowing = true;
            await Task.Delay(10);
            ShowScreenRecorder = isShowing;
            StateHasChanged();
        }

        private void ResetScreenCapture()
        {
            // Replace 'ScreenRecorder' with 'ScreenCapture'
            ScreenCapture = new ScreenCapture { };
        }

        public void Close()
        {
            ShowScreenRecorder = false;
            StateHasChanged();
        }

        public partial class ScreenCapture
        {
            [Inject]
            private IJSRuntime JSRuntime { get; set; }

            private int recordingTimeMS = 10000;
            private bool firstRender = true;

            // Custom method to force state update
            public void Refresh()
            {
                // Replace 'StateHasChanged()' with 'InvokeAsync(StateHasChanged)'
                InvokeAsync(StateHasChanged);
            }

            protected override async Task OnAfterRenderAsync(bool firstRender)
            {
                if (this.firstRender)
                {
                    this.firstRender = false;
                    await JSRuntime.InvokeVoidAsync("startButton.addEventListener", "click", DotNetObjectReference.Create(this));
                    await JSRuntime.InvokeVoidAsync("stopButton.addEventListener", "click", DotNetObjectReference.Create(this));
                }
            }
        }
    }
}