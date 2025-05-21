using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Tests.Buffers;

[TestFixture]
internal sealed class ByteBufferTests
{
    private MemoryStream _stream;
    private BinaryReader _reader;
    private BinaryWriter _writer;

    [SetUp]
    public void SetUp()
    {
        _stream = new MemoryStream();
        _writer = new BinaryWriter(_stream);
        _reader = new BinaryReader(_stream);
    }

    [TearDown]
    public void TearDown()
    {
        _writer.Dispose();
        _reader.Dispose();
        _stream.Dispose();
    }

    [Test]
    public void GetUnsignedByte_Should_Read_Unsigned_Byte()
    {
        _writer.Write((byte)0xAB);
        _stream.Position = 0;

        var value = _reader.GetByte();

        Assert.That(value, Is.EqualTo(0xAB));
    }

    [Test]
    public void PutUnsignedByte_Should_Write_Unsigned_Byte()
    {
        _writer.PutByte(0xCD);
        _stream.Position = 0;

        var value = _reader.ReadByte();

        Assert.That(value, Is.EqualTo(0xCD));
    }

    [Test]
    public void GetUnsignedShort_Should_Read_Unsigned_Short()
    {
        _writer.PutUnsignedShort(0xBEEF);
        _stream.Position = 0;

        var value = _reader.GetUnsignedShort();

        Assert.That(value, Is.EqualTo(0xBEEF));
    }

    [Test]
    public void PutUnsignedShort_Should_Write_Unsigned_Short()
    {
        _writer.PutUnsignedShort(0xCAFE);
        _stream.Position = 0;

        var value = _reader.GetUnsignedShort();

        Assert.That(value, Is.EqualTo(0xCAFE));
    }

    [Test]
    public void GetUnsignedInt_Should_Read_Unsigned_Int()
    {
        _writer.PutUnsignedInt(0xDEADBEEF);
        _stream.Position = 0;

        var value = _reader.GetUnsignedInt();

        Assert.That(value, Is.EqualTo(0xDEADBEEF));
    }

    [Test]
    public void PutUnsignedInt_Should_Write_Unsigned_Int()
    {
        _writer.PutUnsignedInt(0x12345678);
        _stream.Position = 0;

        var value = _reader.GetUnsignedInt();

        Assert.That(value, Is.EqualTo(0x12345678));
    }

    [Test]
    public void GetBytes_Without_Length_Should_Read_Length_Prefixed_Bytes()
    {
        _writer.Write((ushort)2);
        _writer.Write(new byte[] { 0xAA, 0xBB });
        _stream.Position = 0;

        var bytes = _reader.GetBytes();

        Assert.Multiple(() =>
        {
            Assert.That(bytes.Length, Is.EqualTo(2));
            Assert.That(bytes[0], Is.EqualTo(0xAA));
            Assert.That(bytes[1], Is.EqualTo(0xBB));
        });
    }

    [Test]
    public void GetBytes_With_Length_Should_Read_Specified_Bytes()
    {
        _writer.Write(new byte[] { 0x12, 0x34, 0x56 });
        _stream.Position = 0;

        var bytes = _reader.GetBytes(3);

        Assert.Multiple(() =>
        {
            Assert.That(bytes.Length, Is.EqualTo(3));
            Assert.That(bytes[0], Is.EqualTo(0x12));
            Assert.That(bytes[1], Is.EqualTo(0x34));
            Assert.That(bytes[2], Is.EqualTo(0x56));
        });
    }

    [Test]
    public void GetBits_Should_Read_Bytes_And_Return_BitSet()
    {
        _writer.Write(new byte[] { 0xAA, 0xBB });
        _stream.Position = 0;

        var bitSet = _reader.GetBits(2);

        Assert.Multiple(() =>
        {
            Assert.That(bitSet.GetBits(0, 8), Is.EqualTo(0xAA));
            Assert.That(bitSet.GetBits(8, 8), Is.EqualTo(0xBB));
        });
    }

    [Test]
    public void GetBits_With_Reverse_Should_Return_Reversed_BitSet()
    {
        _writer.Write(new byte[] { 0x11, 0x22 });
        _stream.Position = 0;

        var bitSet = _reader.GetBits(2, true);

        Assert.Multiple(() =>
        {
            Assert.That(bitSet.GetBits(0, 8), Is.EqualTo(0x22));
            Assert.That(bitSet.GetBits(8, 8), Is.EqualTo(0x11));
        });
    }
}