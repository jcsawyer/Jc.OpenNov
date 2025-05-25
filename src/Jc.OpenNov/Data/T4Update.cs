using System.Buffers.Binary;

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
        // Allocate the buffer on the heap
        var buffer = new byte[_bytes.Length + 7];
        var span = buffer.AsSpan();

        // Write directly to the span without slicing repeatedly
        span[0] = Cla;
        span[1] = UpdateCommand;
        BinaryPrimitives.WriteUInt16BigEndian(span[2..], 0);
        span[4] = (byte)(_bytes.Length + 2);
        BinaryPrimitives.WriteUInt16BigEndian(span[5..], (ushort)_bytes.Length);
        _bytes.CopyTo(span[7..]);

        return buffer;
    }
}