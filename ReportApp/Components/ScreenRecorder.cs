using ReportApp.Services;
using Microsoft.AspNetCore.Components;
using ReportApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using ReportApp.Components;

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

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private string logText = "";
        private bool downloadLinkVisible = false;
        private string downloadUrl = "";
        private ScreenCapture screenCapture;
        private ElementReference previewElement; // Adicione uma referência ao elemento HTML

        private async Task Log(string msg)
        {
            logText += msg + "\n";
        }

        private async Task StartRecording()
        {
            var stream = await JSRuntime.InvokeAsync<IJSObjectReference>("navigator.mediaDevices.getDisplayMedia",
                new { video = true, audio = true });

            // Use 'previewElement' para acessar o ElementReference
            await JSRuntime.InvokeVoidAsync("startRecording", stream, 10000, previewElement);
        }

        private async Task StopRecording()
        {
            await JSRuntime.InvokeVoidAsync("stopRecording", previewElement);
        }

        [JSInvokable]
        public void HandleRecordingData(byte[] data)
        {
            // Lidar com os dados recebidos aqui
            Log($"Received {data.Length} bytes of recorded data.");

            // Quando a gravação estiver completa, permita que o usuário faça o download
            if (data.Length > 0)
            {
                CreateBlobAndDownload(data);
            }
        }

        private async Task CreateBlobAndDownload(byte[] data)
        {
            if (data.Length > 0)
            {
                var objectURL = await JSRuntime.InvokeAsync<string>("createBlobAndGetObjectURL", data, "video/webm");
                downloadUrl = objectURL;
                downloadLinkVisible = true;
            }
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
        }

        private void ResetScreenCapture()
        {
            screenCapture = new ScreenCapture { }; // Crie uma instância de ScreenCapture
        }

        public partial class ScreenCapture : ComponentBase
        {
            [Inject]
            private IJSRuntime JSRuntime { get; set; }
            public bool ShowScreenRecorder { get; private set; }

            private int recordingTimeMS = 10000;
            private bool firstRender = true;

            // Custom method to force state update
            public void Refresh()
            {
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

            public void Close()
            {
                ShowScreenRecorder = false;

                StateHasChanged();
            }

        }
    }
}