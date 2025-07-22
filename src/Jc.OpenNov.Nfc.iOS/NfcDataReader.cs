using System.Diagnostics;
using CoreNFC;

namespace Jc.OpenNov.Nfc.iOS;

public class NfcDataReader : IDataReader
{
    private readonly INFCIso7816Tag _isoTag;

    public event EventHandler<byte[]>? OnDataSent;
    public event EventHandler<byte[]>? OnDataReceived;

    public NfcDataReader(INFCIso7816Tag isoTag)
    {
        _isoTag = isoTag;
    }

    public async Task<byte[]?> ReadDataAsync(byte[] input)
    {
        try
        {
            OnDataSent?.Invoke(this, input);

            if (input == null || input.Length < 4)
                throw new ArgumentException("APDU must be at least 4 bytes");

            var apdu = new NFCIso7816Apdu(NSData.FromArray(input));
            var tcs = new TaskCompletionSource<byte[]>();

            _isoTag.SendCommand(apdu, (responseData, sw1, sw2, error) =>
            {
                if (error is not null)
                {
                    tcs.TrySetException(new NSErrorException(error));
                    return;
                }

                var result = responseData?.ToArray() ?? [];
                var final = new byte[result.Length + 2];

                Buffer.BlockCopy(result, 0, final, 0, result.Length);
                final[^2] = sw1;
                final[^1] = sw2;

                OnDataReceived?.Invoke(this, final);
                tcs.TrySetResult(final);
            });

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"APDU error: {ex.Message}");
            throw;
        }
    }

    public void DataSent(byte[] data)
    {
        Debug.WriteLine($"Sent: {BitConverter.ToString(data)}");
        OnDataSent?.Invoke(this, data);
    }

    public void DataReceived(byte[] data)
    {
        Debug.WriteLine($"Received: {BitConverter.ToString(data)}");
        OnDataReceived?.Invoke(this, data);
    }
}