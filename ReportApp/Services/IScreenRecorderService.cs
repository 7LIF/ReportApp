using Microsoft.JSInterop;
using System.Threading.Tasks;
using ReportApp.Shared;
using ReportApp.Components;

public interface IScreenRecorderService
{
    Task StartRecording(MediaStream stream, int lengthInMS, DotNetObjectReference<ScreenRecorder> dotnetRef);
    Task StopRecording(MediaStream stream);
    Task ResetMedia();
}