namespace Jc.OpenNov.Buffers;

public static class ByteBufferExtensions
{
    public static byte GetUnsignedByte(this BinaryReader reader)
    {
        return (byte)(reader.ReadByte() & 0xff);
    }

    public static void PutUnsignedByte(this BinaryWriter writer, int value)
    {
        writer.Write((byte)(value & 0xff));
    }

    public static ushort GetUnsignedShort(this BinaryReader reader)
    {
        return reader.ReadUInt16();
    }

    public static void PutUnsignedShort(this BinaryWriter writer, int value)
    {
        writer.Write((ushort)(value & 0xffff));
    }

    public static uint GetUnsignedInt(this BinaryReader reader)
    {
        return reader.ReadUInt32();
    }

    public static void PutUnsignedInt(this BinaryWriter writer, long value)
    {
        writer.Write((uint)(value & 0xffffffffL));
    }

    public static byte[] GetBytes(this BinaryReader reader)
    {
        var length = reader.GetUnsignedShort();
        return reader.ReadBytes(length);
    }
    
    public static byte[] GetBytes(this BinaryReader reader, int length)
    {
        return reader.ReadBytes(length);
    }

    public static BitSet GetBits(this BinaryReader reader, int byteLength, bool reverse)
    {
        var bytes = reader.ReadBytes(byteLength);
        if (reverse)
        {
            Array.Reverse(bytes);
        }
        return BitSet.ValueOf(bytes);
    }

    public static BitSet GetBits(this BinaryReader reader, int byteLength)
    {
        return GetBits(reader, byteLength, false);
    }
}