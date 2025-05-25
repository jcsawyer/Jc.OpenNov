using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Utilities;

public static class EncodableExtensions
{
    public static int SizeOf(short _) => sizeof(short);
    public static int SizeOf(ushort _) => sizeof(ushort);
    public static int SizeOf(int _) => sizeof(int);
    public static int SizeOf(uint _) => sizeof(uint);
    public static int SizeOf(long _) => sizeof(long);
    public static int SizeOf(byte[] arr) => 2 + arr.Length;

    public static void WriteShort(BinaryWriter writer, short value) => writer.PutShort(value);
    public static void WriteUnsignedShort(BinaryWriter writer, ushort value) => writer.PutUnsignedShort(value);
    public static void WriteInt(BinaryWriter writer, int value) => writer.PutInt(value);
    public static void WriteUnsignedInt(BinaryWriter writer, uint value) => writer.PutUnsignedInt(value);
    public static void WriteLong(BinaryWriter writer, long value) => writer.PutLong(value);
    public static void WriteByteArray(BinaryWriter writer, byte[] arr)
    {
        writer.PutUnsignedShort((ushort)arr.Length);
        writer.Write(arr);
    }
}