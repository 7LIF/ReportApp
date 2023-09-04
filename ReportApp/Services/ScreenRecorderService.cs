using Microsoft.JSInterop;
using ReportApp.Components;
using ReportApp.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ReportApp.Components.AddScreenRecorder;

public class ScreenRecorderService : IScreenRecorderService
{
    private static bool recordingInProgress;
    private MediaStream currentStream;

    private IJSRuntime JSRuntime { get; }

    public ScreenRecorderService(IJSRuntime jsRuntime)
    {
        JSRuntime = jsRuntime;
    }

    public async Task StartRecording(MediaStream stream, int lengthInMS, DotNetObjectReference<ScreenCapture> dotnetRef)
    {
        try
        {
            var recorder = await JSRuntime.InvokeAsync<IJSObjectReference>("startRecording", stream, lengthInMS);

            await JSRuntime.InvokeVoidAsync("addEventListener", recorder, "dataavailable", DotNetObjectReference.Create(new
            {
                OnDataAvailable = (Func<object, Task>)(async e =>
                {
                    var dataArray = new List<byte>();
                    var data = await JSRuntime.InvokeAsync<byte[]>("getData", e);

                    if (data.Length > 0)
                    {
                        dataArray.AddRange(data);

                        var options = new { type = "video/webm" };
                        var recordedBlob = await JSRuntime.InvokeAsync<IJSObjectReference>("new Blob", new object[] { new object[] { dataArray.ToArray() }, options });

                        var recordingUrl = await JSRuntime.InvokeAsync<string>("createObjectURL", recordedBlob);

                        await JSRuntime.InvokeVoidAsync("setRecordingSource", recordingUrl);

                        await JSRuntime.InvokeVoidAsync("log", $"Successfully recorded {data.Length} bytes of video/webm media.");
                    }
                })
            }));
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("log", $"Error: {ex.Message}");
        }
    }

    public async Task StopRecording(MediaStream stream)
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("stopRecording", stream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping recording: {ex.Message}");
        }
    }


    internal async Task Reset()
    {
        if (recordingInProgress)
        {
            await StopRecording(currentStream); // Pare a gravação atual
            recordingInProgress = false;
        }
    }

}