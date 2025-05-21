namespace Jc.OpenNov.Buffers;

public sealed class BitSet
{
    private byte[] _bits;

    public BitSet(int sizeInBits)
    {
        _bits = new byte[(sizeInBits + 7) / 8];
    }

    private BitSet(byte[] data)
    {
        _bits = data;
    }

    public static BitSet ValueOf(byte[] data)
    {
        return new BitSet((byte[])data.Clone());
    }

    public ulong GetBits(int bitOffset, int length)
    {
        if (length > 64)
            throw new ArgumentOutOfRangeException(nameof(length), "Can only extract up to 64 bits");

        ulong result = 0;
        for (var i = 0; i < length; i++)
        {
            var byteIndex = (bitOffset + i) / 8;
            var bitIndex = (bitOffset + i) % 8;

            var bit = (_bits[byteIndex] & (1 << bitIndex)) != 0;
            if (bit)
            {
                result |= (1UL << i);
            }
        }
        return result;
    }

    public void SetBits(int bitOffset, int length, ulong value)
    {
        for (var i = 0; i < length; i++)
        {
            var byteIndex = (bitOffset + i) / 8;
            var bitIndex = (bitOffset + i) % 8;

            var bit = ((value >> i) & 1UL) != 0;

            if (bit)
            {
                _bits[byteIndex] |= (byte)(1 << bitIndex);
            }
            else
            {
                _bits[byteIndex] &= (byte)~(1 << bitIndex);
            }
        }
    }

    public byte[] ToByteArray()
    {
        return (byte[])_bits.Clone();
    }
}