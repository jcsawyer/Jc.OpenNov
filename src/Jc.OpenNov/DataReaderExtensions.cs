using System.Diagnostics;
using Jc.OpenNov.Utilities;

namespace Jc.OpenNov;

public static class DataReaderExtensions
{
    public static async Task<TransceiveResult> ReadResult(this IDataReader reader, byte[] command)
    {
        reader.DataSent(command);
        var data = await reader.ReadDataAsync(command).ConfigureAwait(false);

        if (data is not null)
        {
            reader.DataReceived(data);
        }

        using var ms = new MemoryStream(data);
        using var br = new BinaryReader(ms);

        var dataSize = data.Length - 2;
        var content = br.GetBytes(dataSize);
        var status = br.GetUnsignedShort() & 0xFFFF;

        return new TransceiveResult
        {
            Content = new MemoryStream(content),
            Success = status == NvpController.CommandCompleted
        };
    }
}