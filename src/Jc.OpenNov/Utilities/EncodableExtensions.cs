namespace Jc.OpenNov.Utilities;

public static class EncodableExtensions
{
    public static int SizeOf(short _) => sizeof(short);
    public static int SizeOf(ushort _) => sizeof(ushort);
    public static int SizeOf(int _) => sizeof(int);
    public static int SizeOf(uint _) => sizeof(uint);
    public static int SizeOf(byte _) => sizeof(byte);
    public static int SizeOf(byte[] arr) => 2 + arr.Length;

    public static void WriteShort(BinaryWriter writer, short value) => writer.Write(value);
    public static void WriteUnsignedShort(BinaryWriter writer, ushort value) => writer.Write(value);
    public static void WriteInt(BinaryWriter writer, int value) => writer.Write(value);
    public static void WriteUnsignedInt(BinaryWriter writer, uint value) => writer.Write(value);
    public static void WriteByte(BinaryWriter writer, byte value) => writer.Write(value);
    public static void WriteUnsignedByte(BinaryWriter writer, byte value) => writer.Write(value);
    public static void WriteByteArray(BinaryWriter writer, byte[] arr)
    {
        writer.Write((short)arr.Length);
        writer.Write(arr);
    }
}