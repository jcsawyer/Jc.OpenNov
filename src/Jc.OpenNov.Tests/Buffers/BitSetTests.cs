using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Tests.Buffers;

[TestFixture]
internal sealed class BitSetTests
{
    private BitSet _bitSet;
    private const int BitSetSize = 16;

    [SetUp]
    public void SetUp()
    {
        _bitSet = new BitSet(BitSetSize);
    }

    [Test]
    public void Constructor_Should_Initialize_All_Bits_To_Zero()
    {
        var bytes = _bitSet.ToByteArray();
        Assert.That(bytes, Is.All.EqualTo(0));
    }
    
    [Test]
    public void SetBits_And_GetBits_Should_Set_And_Get_Multiple_Bits()
    {
        _bitSet.SetBits(2, 4, 0b1011);
        var value = _bitSet.GetBits(2, 4);

        Assert.That(value, Is.EqualTo(0b1011));
    }

    [Test]
    public void SetBits_And_GetBits_Should_Set_And_Get_Single_Bit()
    {
        _bitSet.SetBits(3, 1, 1);
        var value = _bitSet.GetBits(3, 1);

        Assert.That(value, Is.EqualTo(1));
    }

    [Test]
    public void GetBits_Should_Throw_When_Length_Exceeds_64()
    {
        Assert.That(() => _bitSet.GetBits(0, 65), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void ValueOf_Should_Create_Independent_Copy()
    {
        var data = new byte[] { 0xAA, 0x55 };
        var copy = BitSet.ValueOf(data);

        Assert.Multiple(() =>
        {
            Assert.That(copy.ToByteArray(), Is.EqualTo(data));
            data[0] = 0xFF;
            Assert.That(copy.ToByteArray()[0], Is.EqualTo(0xAA));
        });
    }

    [Test]
    public void ToByteArray_Should_Return_Clone_Of_Internal_Array()
    {
        _bitSet.SetBits(0, 8, 0xF0);
        var arr1 = _bitSet.ToByteArray();
        arr1[0] = 0x00;
        var arr2 = _bitSet.ToByteArray();

        Assert.That(arr2[0], Is.EqualTo(0xF0));
    }
}