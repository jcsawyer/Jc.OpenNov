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
        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(Cla);
            writer.Write(UpdateCommand);
            writer.Write((byte)0x00); // First byte of ushort(0)
            writer.Write((byte)0x00); // Second byte of ushort(0)
            writer.Write((byte)(_bytes.Length + 2)); // Lc field
            writer.Write((byte)((_bytes.Length >> 8) & 0xFF)); // High byte of length
            writer.Write((byte)(_bytes.Length & 0xFF));        // Low byte of length
            writer.Write(_bytes);
            return stream.ToArray();
        }
    }
}