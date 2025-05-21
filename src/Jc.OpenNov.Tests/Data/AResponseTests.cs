using Jc.OpenNov.Data;

namespace Jc.OpenNov.Tests.Data;

[TestFixture]
internal sealed class AResponseTests
{
    private short _result;
        private short _protocol;
        private ApoepElement _apoep;

        [SetUp]
        public void SetUp()
        {
            _result = 42;
            _protocol = 7;
            _apoep = new ApoepElement(
                1, 2, 3, 4, 5,
                [0xAA, 0xBB, 0xCC],
                6, 7, 8, 9
            );
        }

        [Test]
        public void Constructor_Should_Set_Properties_And_Override_Apoep_Fields()
        {
            var response = new AResponse(_result, _protocol, _apoep);

            Assert.Multiple(() =>
            {
                Assert.That(response.Result, Is.EqualTo(_result));
                Assert.That(response.Protocol, Is.EqualTo(_protocol));
                Assert.That(response.Apoep, Is.EqualTo(_apoep));
                Assert.That(response.Apoep.RecMode, Is.EqualTo(0));
                Assert.That(response.Apoep.ConfigId, Is.EqualTo(0));
                Assert.That(response.Apoep.SystemType, Is.EqualTo(ApoepElement.SysTypeManager));
                Assert.That(response.Apoep.ListCount, Is.EqualTo(0));
                Assert.That(response.Apoep.ListLen, Is.EqualTo(0));
            });
        }

        [Test]
        public void Encoded_Output_Should_Contain_Correct_Fields()
        {
            var response = new AResponse(_result, _protocol, _apoep);

            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            response.WriteTo(writer);
            writer.Flush();
            ms.Position = 0;

            using var reader = new BinaryReader(ms);
            var result = reader.ReadInt16();
            var protocol = reader.ReadInt16();
            var apoepLen = reader.ReadInt16();
            var apoepBytes = reader.ReadBytes(apoepLen);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(_result));
                Assert.That(protocol, Is.EqualTo(_protocol));
                Assert.That(apoepLen, Is.EqualTo( // check encoded size
                    new ApoepElement(
                        _apoep.Version, _apoep.Encoding, _apoep.Nomenclature, _apoep.Functional,
                        ApoepElement.SysTypeManager, _apoep.SystemId, 0, 0, 0, 0
                    ).GetEncodedSize()
                ));
                Assert.That(apoepBytes.Length, Is.EqualTo(apoepLen));
            });
        }
}