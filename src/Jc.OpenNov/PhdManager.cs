using Jc.OpenNov.Buffers;
using Jc.OpenNov.Data;

namespace Jc.OpenNov;

public sealed class PhdManager
{
    private const int MaxReadSize = 255;

    private readonly IDataReader _dataReader;
    private int sequence = 0;

    public PhdManager(IDataReader dataReader)
    {
        _dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
    }

    private TransceiveResult Request(byte[] data)
    {
        return _dataReader.ReadResult(data);
    }

    public byte[] SendEmptyRequest()
    {
        return SendRequest(Array.Empty<byte>());
    }

    private byte[] SendRequest(byte[] data)
    {
        var phd = new PhdPacket(sequence++, data);
        var update = new T4Update(phd.ToByteArray());

        Request(update.ToByteArray());

        var readLen = Request(CreateReadPayload(0, 2));
        if (!readLen.Success || readLen.Content.Length < 2)
        {
            throw new InvalidOperationException("Failed to read length of PHD packet.");
        }
        
        int len = new BinaryReader(readLen.Content).GetUnsignedShort();

        var reads = DecomposeNumber(len, MaxReadSize);
        var fullResult = new byte[len];
        int offset = 0;

        for (int index = 0; index < reads.Count; index++)
        {
            int readLength = reads[index];
            var readResult = Request(CreateReadPayload(2 + index * MaxReadSize, readLength));
            Array.Copy(readResult.Content.ToArray(), 0, fullResult, offset, readLength);
            offset += readLength;
        }

        using var buffer = new BinaryReader(new MemoryStream(fullResult));
        var resultPhd = PhdPacket.FromBinaryReader(buffer);

        sequence = resultPhd.Seq + 1;

        var ack = new T4Update(new byte[] { 0xd0, 0x00, 0x00 });
        _dataReader.ReadResult(ack.ToByteArray());

        return resultPhd.Content;
    }

    public byte[] SendApduRequest(Apdu apdu)
    {
        return SendRequest(apdu.ToByteArray());
    }

    public T DecodeDataApduRequest<T>(Apdu inputApdu) where T : Encodable
    {
        var byteArray = SendApduRequest(inputApdu);
        using var stream = new MemoryStream(byteArray);
        using var reader = new BinaryReader(stream);
        var outputApdu = Apdu.FromBinaryReader(reader);
        var dataApdu = (DataApdu)outputApdu.Payload;
        return (T)dataApdu.Payload;
    }

    private byte[] CreateReadPayload(int offset, int length)
    {
        return
        [
            NvpController.Cla,
            NvpController.InsRb,
            (byte)((offset >> 8) & 0xFF),
            (byte)(offset & 0xFF),
            (byte)length
        ];
    }
    
    private static List<int> DecomposeNumber(int n, int maxValue)
    {
        var times = n / maxValue;
        var remainder = n % maxValue;

        var list = Enumerable.Repeat(maxValue, times).ToList();

        if (remainder != 0)
        {
            list.Add(remainder);
        }

        return list;
    }
}