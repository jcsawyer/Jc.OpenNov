using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Data;

public sealed class T4Update
{
    private readonly byte[] _bytes;

    private const byte Cla = 0x00;
    private const byte UpdateCommand = 0xD6;

    public T4Update(byte[] bytes)
    {
        _bytes = bytes;
    }

    public byte[] ToByteArray()
    {
        var buffer = new byte[_bytes.Length + 7];
        using var stream = new MemoryStream(buffer);
        using var writer = new BinaryWriter(stream);
        
        writer.PutByte(Cla);
        writer.PutByte(UpdateCommand);
        writer.PutUnsignedShort(0);
        writer.PutByte((byte)(_bytes.Length + 2));
        writer.PutUnsignedShort((ushort)_bytes.Length);
        writer.Write(_bytes);
        
        return buffer;
    }
}