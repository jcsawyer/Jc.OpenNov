using System.Text;

namespace Jc.OpenNov.Utilities;

public static class ByteBufferExtensions
{
    public static byte GetByte(this BinaryReader reader)
    {
        return (byte)(reader.ReadByte() & 0xff);
    }

    public static void PutByte(this BinaryWriter writer, byte value)
    {
        writer.Write((byte)(value & 0xff));
    }
    
    public static short GetShort(this BinaryReader reader)
    {
        var bytes = reader.ReadBytes(2);
        if (bytes.Length < 2)
            throw new EndOfStreamException();

        // Big-endian: first byte is high byte, second byte is low byte
        return (short)((bytes[0] << 8) | bytes[1]);
    }
    
    public static void PutShort(this BinaryWriter writer, short value)
    {
        // Write as big-endian: high byte first
        writer.Write((byte)((value >> 8) & 0xff));
        writer.Write((byte)(value & 0xff));
    }

    public static ushort GetUnsignedShort(this BinaryReader reader)
    {
        var bytes = reader.ReadBytes(2);
        if (bytes.Length < 2)
            throw new EndOfStreamException();

        // Big-endian: first byte is high byte, second byte is low byte
        return (ushort)((bytes[0] << 8) | bytes[1]);
    }

    public static void PutUnsignedShort(this BinaryWriter writer, ushort value)
    {
        // Write as big-endian: high byte first
        writer.Write((byte)((value >> 8) & 0xff));
        writer.Write((byte)(value & 0xff));
    }
    
    public static int GetInt(this BinaryReader reader)
    {
        var bytes = reader.ReadBytes(4);
        if (bytes.Length < 4)
            throw new EndOfStreamException();

        // Big-endian: combine 4 bytes
        return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
    }

    public static void PutInt(this BinaryWriter writer, int value)
    {
        // Write as big-endian: highest byte first
        writer.Write((byte)((value >> 24) & 0xff));
        writer.Write((byte)((value >> 16) & 0xff));
        writer.Write((byte)((value >> 8) & 0xff));
        writer.Write((byte)(value & 0xff));
    }

    public static uint GetUnsignedInt(this BinaryReader reader)
    {
        var bytes = reader.ReadBytes(4);
        if (bytes.Length < 4)
            throw new EndOfStreamException();

        // Big-endian: combine 4 bytes
        return (uint)((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]);
    }

    public static void PutUnsignedInt(this BinaryWriter writer, uint value)
    {
        // Write as big-endian: highest byte first
        writer.Write((byte)((value >> 24) & 0xff));
        writer.Write((byte)((value >> 16) & 0xff));
        writer.Write((byte)((value >> 8) & 0xff));
        writer.Write((byte)(value & 0xff));
    }

    public static long GetLong(this BinaryReader reader)
    {
        var bytes = reader.ReadBytes(8);
        if (bytes.Length < 8)
            throw new EndOfStreamException();

        return ((long)bytes[0] << 56) |
               ((long)bytes[1] << 48) |
               ((long)bytes[2] << 40) |
               ((long)bytes[3] << 32) |
               ((long)bytes[4] << 24) |
               ((long)bytes[5] << 16) |
               ((long)bytes[6] << 8)  |
               bytes[7];
    }
    
    public static void PutLong(this BinaryWriter writer, long value)
    {
        writer.Write((byte)((value >> 56) & 0xFF));
        writer.Write((byte)((value >> 48) & 0xFF));
        writer.Write((byte)((value >> 40) & 0xFF));
        writer.Write((byte)((value >> 32) & 0xFF));
        writer.Write((byte)((value >> 24) & 0xFF));
        writer.Write((byte)((value >> 16) & 0xFF));
        writer.Write((byte)((value >> 8) & 0xFF));
        writer.Write((byte)(value & 0xFF));
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

    public static string GetIndexedString(this BinaryReader reader)
    {
        return Encoding.ASCII.GetString(reader.GetBytes()).Replace("\0", "");
    }
}