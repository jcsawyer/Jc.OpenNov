using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Tests.Buffers;

[TestFixture]
internal sealed class ByteBufferTests
{
    private BitSet _bitSet;

        [SetUp]
        public void SetUp()
        {
            _bitSet = new BitSet(32);
        }

        [Test]
        public void GetUnsignedByte_Should_Return_Correct_Value()
        {
            _bitSet.SetBits(0, 8, 0xAB);
            var value = ByteBuffer.GetUnsignedByte(_bitSet, 0);

            Assert.That(value, Is.EqualTo(0xAB));
        }

        [Test]
        public void PutUnsignedByte_Should_Set_Correct_Bits()
        {
            ByteBuffer.PutUnsignedByte(_bitSet, 8, 0xCD);

            Assert.That(_bitSet.GetBits(8, 8), Is.EqualTo(0xCD));
        }

        [Test]
        public void GetUnsignedShort_Should_Return_Correct_Value()
        {
            _bitSet.SetBits(0, 16, 0xBEEF);
            var value = ByteBuffer.GetUnsignedShort(_bitSet, 0);

            Assert.That(value, Is.EqualTo(0xBEEF));
        }

        [Test]
        public void PutUnsignedShort_Should_Set_Correct_Bits()
        {
            ByteBuffer.PutUnsignedShort(_bitSet, 0, 0xCAFE);

            Assert.That(_bitSet.GetBits(0, 16), Is.EqualTo(0xCAFE));
        }

        [Test]
        public void GetUnsignedInt_Should_Return_Correct_Value()
        {
            _bitSet.SetBits(0, 32, 0xDEADBEEF);
            var value = ByteBuffer.GetUnsignedInt(_bitSet, 0);

            Assert.That(value, Is.EqualTo(0xDEADBEEF));
        }

        [Test]
        public void PutUnsignedInt_Should_Set_Correct_Bits()
        {
            ByteBuffer.PutUnsignedInt(_bitSet, 0, 0x12345678);

            Assert.That(_bitSet.GetBits(0, 32), Is.EqualTo(0x12345678));
        }

        [Test]
        public void GetBytes_Should_Return_Correct_Byte_Array()
        {
            _bitSet.SetBits(0, 8, 0x12);
            _bitSet.SetBits(8, 8, 0x34);
            _bitSet.SetBits(16, 8, 0x56);

            var bytes = ByteBuffer.GetBytes(_bitSet, 0, 3);

            Assert.Multiple(() =>
            {
                Assert.That(bytes[0], Is.EqualTo(0x12));
                Assert.That(bytes[1], Is.EqualTo(0x34));
                Assert.That(bytes[2], Is.EqualTo(0x56));
            });
        }

        [Test]
        public void GetBits_Should_Return_Correct_BitSet()
        {
            _bitSet.SetBits(0, 8, 0xAA);
            _bitSet.SetBits(8, 8, 0xBB);

            var result = ByteBuffer.GetBits(_bitSet, 0, 2);

            Assert.Multiple(() =>
            {
                Assert.That(result.GetBits(0, 8), Is.EqualTo(0xAA));
                Assert.That(result.GetBits(8, 8), Is.EqualTo(0xBB));
            });
        }

        [Test]
        public void GetBits_With_Reverse_Should_Return_Reversed_Bytes()
        {
            _bitSet.SetBits(0, 8, 0x11);
            _bitSet.SetBits(8, 8, 0x22);

            var result = ByteBuffer.GetBits(_bitSet, 0, 2, true);

            Assert.Multiple(() =>
            {
                Assert.That(result.GetBits(0, 8), Is.EqualTo(0x22));
                Assert.That(result.GetBits(8, 8), Is.EqualTo(0x11));
            });
        }
}