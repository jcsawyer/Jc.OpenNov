using Jc.OpenNov.Data;

namespace Jc.OpenNov.Avalonia;

public interface IOpenNov
{
    internal INfc? Nfc { get; set; }
    
    event EventHandler<PenResult>? OnDataRead;
    event EventHandler<ITag>? OnTagDetected;
    event EventHandler<Exception>? OnError;

    void MonitorNfc(Func<string, List<InsulinDose>, bool>? stopCondition = null);
    void StopNfc();
}