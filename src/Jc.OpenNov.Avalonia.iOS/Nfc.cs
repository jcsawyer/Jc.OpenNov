using CoreNFC;
using Jc.OpenNov.Data;
using Jc.OpenNov.Nfc.iOS;

namespace Jc.OpenNov.Avalonia.iOS;

internal sealed class Nfc : INfc
{
    private readonly NfcController _nfcController;

    public Nfc(NfcController nfcController)
    {
        _nfcController = nfcController ?? throw new ArgumentNullException(nameof(nfcController));
    }
    
    public event EventHandler<PenResult>? OnDataRead;
    public event EventHandler<ITag>? OnTagDetected;
    public event EventHandler<byte[]>? OnDataSent;
    public event EventHandler<byte[]>? OnDataReceived;
    public event EventHandler<Exception>? OnError;
    public void MonitorNfc(Func<string, List<InsulinDose>, bool>? stopCondition = null)
    {
        _nfcController.MonitorNfc(stopCondition);
        
        _nfcController.OnDataRead += OnDataRead;
        _nfcController.OnTagDetected += TagDetectedHandler;
        _nfcController.OnDataSent += OnDataSent;
        _nfcController.OnDataReceived += OnDataReceived;
        _nfcController.OnError += OnError;
    }

    private void TagDetectedHandler(object? sender, INFCIso7816Tag e)
    {
        OnTagDetected?.Invoke(this, new Tag(e));
    }

    public void StopNfc()
    {
        _nfcController.StopNfc();
        
        _nfcController.OnDataRead -= OnDataRead;
        _nfcController.OnTagDetected -= TagDetectedHandler;
        _nfcController.OnDataSent -= OnDataSent;
        _nfcController.OnDataReceived -= OnDataReceived;
        _nfcController.OnError -= OnError;
    }
}