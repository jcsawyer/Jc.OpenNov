using Jc.OpenNov.Data;

namespace Jc.OpenNov.Avalonia;

internal interface INfc
{
    event EventHandler<PenResult>? OnDataRead;
    event EventHandler<ITag>? OnTagDetected;
    event EventHandler<byte[]>? OnDataSent;
    event EventHandler<byte[]>? OnDataReceived;
    event EventHandler<Exception>? OnError;

    void MonitorNfc(Func<string, List<InsulinDose>, bool>? stopCondition = null);
    void StopNfc();
}