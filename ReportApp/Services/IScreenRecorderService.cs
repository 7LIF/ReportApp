using System.Threading.Tasks;
using Microsoft.JSInterop;
using ReportApp.Shared;
using static ReportApp.Components.AddScreenRecorder;

public interface IScreenRecorderService
{
    Task StartRecording(MediaStream stream, int lengthInMS, DotNetObjectReference<ScreenCapture> dotnetRef);
    Task StopRecording(MediaStream stream);
    Task Reset();
}