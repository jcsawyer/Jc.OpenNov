namespace Jc.OpenNov.Buffers;

public abstract class ByteBuffer
{
    public static int GetUnsignedByte(BitSet bits, int bitOffset)
    {
        return (int)bits.GetBits(bitOffset, 8);
    }

    public static void PutUnsignedByte(BitSet bits, int bitOffset, int value)
    {
        bits.SetBits(bitOffset, 8, (ulong)(value & 0xFF));
    }

    public static int GetUnsignedShort(BitSet bits, int bitOffset)
    {
        return (int)bits.GetBits(bitOffset, 16);
    }

    public static void PutUnsignedShort(BitSet bits, int bitOffset, int value)
    {
        bits.SetBits(bitOffset, 16, (ulong)(value & 0xFFFF));
    }

    public static long GetUnsignedInt(BitSet bits, int bitOffset)
    {
        return (long)bits.GetBits(bitOffset, 32);
    }

    public static void PutUnsignedInt(BitSet bits, int bitOffset, long value)
    {
        bits.SetBits(bitOffset, 32, (ulong)(value & 0xFFFFFFFF));
    }

    public static byte[] GetBytes(BitSet bits, int bitOffset, int byteLen)
    {
        var result = new byte[byteLen];
        for (var i = 0; i < byteLen; i++)
        {
            result[i] = (byte)bits.GetBits(bitOffset + i * 8, 8);
        }
        return result;
    }

    public static BitSet GetBits(BitSet bits, int bitOffset, int byteLen, bool reverse = false)
    {
        var extracted = new byte[byteLen];
        for (var i = 0; i < byteLen; i++)
        {
            extracted[i] = (byte)bits.GetBits(bitOffset + i * 8, 8);
        }

        if (reverse)
        {
            Array.Reverse(extracted);
        }

        return BitSet.ValueOf(extracted);
    }
}