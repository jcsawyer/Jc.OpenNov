using Jc.OpenNov.Data;

namespace Jc.OpenNov.Avalonia;

public class InternalOpenNov : IOpenNov
{
    INfc? IOpenNov.Nfc { get; set; }
    
    public event EventHandler<PenResult>? OnDataRead;
    public event EventHandler<ITag>? OnTagDetected;
    public event EventHandler<byte[]>? OnDataSent;
    public event EventHandler<byte[]>? OnDataReceived;
    public event EventHandler<Exception>? OnError;
    
    public void MonitorNfc(Func<string, List<InsulinDose>, bool>? stopCondition = null)
    {
        stopCondition ??= (_, _) => false;

        if (((IOpenNov)this).Nfc is null)
        {
            return;
        }
        
        ((IOpenNov)this).Nfc.OnDataRead += DataReadHandler;
        ((IOpenNov)this).Nfc.OnTagDetected += TagDetectedHandler;
        ((IOpenNov)this).Nfc.OnDataSent += DataSentHandler;
        ((IOpenNov)this).Nfc.OnDataReceived += DataReceivedHandler;
        ((IOpenNov)this).Nfc.OnError += ErrorHandler;

        ((IOpenNov)this).Nfc.MonitorNfc(stopCondition);
    }
    
    private void DataReadHandler(object? sender, PenResult result)
    {
        OnDataRead?.Invoke(sender, result);
    }
    
    private void TagDetectedHandler(object? sender, ITag tag)
    {
        OnTagDetected?.Invoke(sender, tag);
    }
    
    private void DataSentHandler(object? sender, byte[] data)
    {
        OnDataSent?.Invoke(sender, data);
    }
    
    private void DataReceivedHandler(object? sender, byte[] data)
    {
        OnDataReceived?.Invoke(sender, data);
    }
    
    private void ErrorHandler(object? sender, Exception exception)
    {
        OnError?.Invoke(sender, exception);
    }

    public void StopNfc()
    {
        if (((IOpenNov)this).Nfc is null)
        {
            return;
        }
        
        ((IOpenNov)this).Nfc.OnDataRead -= DataReadHandler;
        ((IOpenNov)this).Nfc.OnTagDetected -= TagDetectedHandler;
        ((IOpenNov)this).Nfc.OnDataSent -= DataSentHandler;
        ((IOpenNov)this).Nfc.OnDataReceived -= DataReceivedHandler;
        ((IOpenNov)this).Nfc.OnError -= ErrorHandler;
        
        ((IOpenNov)this).Nfc.StopNfc();
    }
}