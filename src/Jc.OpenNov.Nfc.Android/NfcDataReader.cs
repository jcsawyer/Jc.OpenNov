using Android.Nfc.Tech;

namespace Jc.OpenNov.Nfc.Android;

public class NfcDataReader : IDataReader
{
    private readonly IsoDep _isoDep;
    
    public event EventHandler<byte[]>? OnDataSent;
    public event EventHandler<byte[]>? OnDataReceived;

    public NfcDataReader(IsoDep isoDep)
    {
        _isoDep = isoDep;
    }

    public virtual byte[] ReadData(byte[] input)
    {
        // According to the docs, Transceive(input) should not return null.
        return _isoDep.Transceive(input)!;
    }

    public virtual void DataSent(byte[] data)
    {
        OnDataSent?.Invoke(this, data);
    }

    public virtual void DataReceived(byte[] data)
    {
        OnDataReceived?.Invoke(this, data);
    }
}