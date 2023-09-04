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
        // Dependências injetadas no componente
        [Inject]
        public IScreenRecorderService ScreenRecorderService { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        private bool isInitialized = false;

        private bool isShowing = false;

        // Propriedade para controlar a visibilidade do componente
        public bool ShowScreenRecorder { get; set; } = false;

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private string logText = "";
        private bool downloadLinkVisible = false;
        private string downloadUrl = "";
        private ScreenCapture screenCapture;
        private ElementReference previewElement; // Referência ao elemento HTML

        private async Task Log(string msg)
        {
            logText += msg + "\n";
        }

        private async Task StartRecording()
        {
            // Inicia a gravação de tela usando a API de mídia do navegador
            var stream = await JSRuntime.InvokeAsync<IJSObjectReference>("navigator.mediaDevices.getDisplayMedia",
                new { video = true, audio = true });

            // Inicia a gravação com a stream obtida
            await JSRuntime.InvokeVoidAsync("startRecording", stream, 10000, previewElement);
        }

        private async Task StopRecording()
        {
            // Para a gravação em andamento
            await JSRuntime.InvokeVoidAsync("stopRecording", previewElement);
        }

        [JSInvokable]
        public void HandleRecordingData(byte[] data)
        {
            // Lida com os dados recebidos durante a gravação
            Log($"Received {data.Length} bytes of recorded data.");

            // Quando a gravação estiver completa, permite que o usuário faça o download
            if (data.Length > 0)
            {
                CreateBlobAndDownload(data);
            }
        }

        private async Task CreateBlobAndDownload(byte[] data)
        {
            if (data.Length > 0)
            {
                // Cria um objeto Blob a partir dos dados gravados
                var objectURL = await JSRuntime.InvokeAsync<string>("createBlobAndGetObjectURL", data, "video/webm");
                downloadUrl = objectURL;
                downloadLinkVisible = true;
            }
        }

        // Métodos para mostrar, fechar e redefinir o componente
        public async Task ShowAsync()
        {
            // Mostra o componente assíncronamente
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
            // Reseta a captura de tela
            screenCapture = new ScreenCapture { }; // Cria uma nova instância de ScreenCapture
        }

        public partial class ScreenCapture : ComponentBase
        {
            [Parameter]
            public EventCallback<bool> CloseEventCallback { get; set; }

            [Inject]
            private IJSRuntime JSRuntime { get; set; }
            public bool ShowScreenRecorder { get; private set; }

            private int recordingTimeMS = 10000;
            private bool firstRender = true;

            // Método personalizado para forçar a atualização do estado
            public void Refresh()
            {
                InvokeAsync(StateHasChanged);
            }

            protected override async Task OnAfterRenderAsync(bool firstRender)
            {
                if (this.firstRender)
                {
                    this.firstRender = false;

                    // Adiciona ouvintes de eventos para os botões de início e parada
                    await JSRuntime.InvokeVoidAsync("startButton.addEventListener", "click", DotNetObjectReference.Create(this));
                    await JSRuntime.InvokeVoidAsync("stopButton.addEventListener", "click", DotNetObjectReference.Create(this));
                }
            }

            public async void Close()
            {
                // Fecha o componente e invoca o retorno de chamada
                ShowScreenRecorder = false;
                await CloseEventCallback.InvokeAsync(true);
                StateHasChanged();
            }
        }
    }
}