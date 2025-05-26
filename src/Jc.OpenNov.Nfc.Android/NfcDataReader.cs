using Android.Nfc.Tech;

namespace Jc.OpenNov.Nfc.Android;

public class NfcDataReader : IDataReader
{
    private readonly IsoDep _isoDep;

    public NfcDataReader(IsoDep isoDep)
    {
        _isoDep = isoDep;
    }

    public virtual byte[]? ReadData(byte[] input)
    {
        return _isoDep.Transceive(input);
    }

    public virtual void OnDataSent(byte[] data)
    {
    }

    public virtual void OnDataReceived(byte[] data)
    {
    }
}