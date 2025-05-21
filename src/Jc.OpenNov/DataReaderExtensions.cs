using Jc.OpenNov.Buffers;

namespace Jc.OpenNov;

public static class DataReaderExtensions
{
    public static TransceiveResult ReadResult(this IDataReader reader, byte[] command)
    {
        reader.OnDataSent(command);
        var data = reader.ReadData(command);
        reader.OnDataReceived(data);

        using var ms = new MemoryStream(data);
        using var br = new BinaryReader(ms);

        var dataSize = data.Length - 2;
        var content = br.ReadBytes(dataSize);
        var status = br.GetUnsignedShort() & 0xFFFF;

        return new TransceiveResult {
            Content = new MemoryStream(content), 
            Success = status == NvpController.CommandCompleted
        };
    }
}