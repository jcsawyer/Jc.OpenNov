using Jc.OpenNov.Data;

namespace Jc.OpenNov.Tests.Data;

[TestFixture]
internal sealed class ARequestTests
{
    private ushort _protocol;
        private uint _version;
        private ushort _elements;
        private ApoepElement _apoep;

        [SetUp]
        public void SetUp()
        {
            _protocol = ApoepElement.Apoep;
            _version = 42;
            _elements = 1;
            _apoep = new ApoepElement(
                1, 2, 3, 4, 5,
                [0xAA, 0xBB],
                6, 7, 8, 9
            );
        }

        [Test]
        public void Constructor_Should_Set_All_Properties()
        {
            var request = new ARequest(_protocol, _version, _elements, _apoep);

            Assert.Multiple(() =>
            {
                Assert.That(request.Protocol, Is.EqualTo(_protocol));
                Assert.That(request.Version, Is.EqualTo(_version));
                Assert.That(request.Elements, Is.EqualTo(_elements));
                Assert.That(request.Apoep, Is.EqualTo(_apoep));
            });
        }

        [Test]
        public void FromByteBuffer_Should_Read_ARequest_Correctly()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            // Write Version, Elements, Length (dummy)
            writer.Write(_version);
            writer.Write(_elements);
            writer.Write((ushort)0); // Length (not used)

            // Write protocol, then ApoepElement as bytes
            writer.Write(_protocol);

            // ApoepElement as bytes (length-prefixed)
            using (var apoepStream = new MemoryStream())
            using (var apoepWriter = new BinaryWriter(apoepStream))
            {
                _apoep.WriteTo(apoepWriter);
                var apoepBytes = apoepStream.ToArray();
                writer.Write((ushort)apoepBytes.Length);
                writer.Write(apoepBytes);
            }

            writer.Flush();
            ms.Position = 0;

            using var reader = new BinaryReader(ms);
            var request = ARequest.FromByteBuffer(reader);

            Assert.Multiple(() =>
            {
                Assert.That(request.Protocol, Is.EqualTo(_protocol));
                Assert.That(request.Version, Is.EqualTo(_version));
                Assert.That(request.Elements, Is.EqualTo(_elements));
                Assert.That(request.Apoep.Version, Is.EqualTo(_apoep.Version));
                Assert.That(request.Apoep.Encoding, Is.EqualTo(_apoep.Encoding));
                Assert.That(request.Apoep.Nomenclature, Is.EqualTo(_apoep.Nomenclature));
                Assert.That(request.Apoep.Functional, Is.EqualTo(_apoep.Functional));
                Assert.That(request.Apoep.SystemType, Is.EqualTo(_apoep.SystemType));
                Assert.That(request.Apoep.SystemId, Is.EqualTo(_apoep.SystemId));
                Assert.That(request.Apoep.ConfigId, Is.EqualTo(_apoep.ConfigId));
                Assert.That(request.Apoep.RecMode, Is.EqualTo(_apoep.RecMode));
                Assert.That(request.Apoep.ListCount, Is.EqualTo(_apoep.ListCount));
                Assert.That(request.Apoep.ListLen, Is.EqualTo(_apoep.ListLen));
            });
        }

        [Test]
        public void FromByteBuffer_Should_Throw_When_Apoep_Not_Found()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            // Write Version, Elements, Length (dummy)
            writer.Write(_version);
            writer.Write((ushort)1);
            writer.Write((ushort)0);

            // Write protocol that is not ApoepElement.Apoep
            writer.Write((ushort)0xFFFF);
            writer.Write((ushort)0); // Length
            writer.Flush();
            ms.Position = 0;

            using var reader = new BinaryReader(ms);

            Assert.That(() => ARequest.FromByteBuffer(reader), Throws.TypeOf<InvalidOperationException>());
        }
}