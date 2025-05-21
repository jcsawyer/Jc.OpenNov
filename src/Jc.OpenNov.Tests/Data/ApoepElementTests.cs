using Jc.OpenNov.Data;

namespace Jc.OpenNov.Tests.Data;

[TestFixture]
internal sealed class ApoepElementTests
{
    private int _version;
    private short _encoding;
    private int _nomenclature;
    private int _functional;
    private int _systemType;
    private byte[] _systemId;
    private short _configId;
    private int _recMode;
    private short _listCount;
    private short _listLen;

    [SetUp]
    public void SetUp()
    {
        _version = 1;
        _encoding = 2;
        _nomenclature = 3;
        _functional = 4;
        _systemType = 5;
        _systemId = new byte[] { 0xAA, 0xBB, 0xCC };
        _configId = 6;
        _recMode = 7;
        _listCount = 8;
        _listLen = 9;
    }

    [Test]
    public void Constructor_Should_Set_All_Properties()
    {
        var element = new ApoepElement(
            _version, _encoding, _nomenclature, _functional, _systemType,
            _systemId, _configId, _recMode, _listCount, _listLen);

        Assert.Multiple(() =>
        {
            Assert.That(element.Version, Is.EqualTo(_version));
            Assert.That(element.Encoding, Is.EqualTo(_encoding));
            Assert.That(element.Nomenclature, Is.EqualTo(_nomenclature));
            Assert.That(element.Functional, Is.EqualTo(_functional));
            Assert.That(element.SystemType, Is.EqualTo(_systemType));
            Assert.That(element.SystemId, Is.EqualTo(_systemId));
            Assert.That(element.ConfigId, Is.EqualTo(_configId));
            Assert.That(element.RecMode, Is.EqualTo(_recMode));
            Assert.That(element.ListCount, Is.EqualTo(_listCount));
            Assert.That(element.ListLen, Is.EqualTo(_listLen));
        });
    }

    [Test]
    public void FromBinaryReader_Should_Read_All_Fields_Correctly()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write(_version);
        writer.Write(_encoding);
        writer.Write(_nomenclature);
        writer.Write(_functional);
        writer.Write(_systemType);
        writer.Write((short)_systemId.Length);
        writer.Write(_systemId);
        writer.Write(_configId);
        writer.Write(_recMode);
        writer.Write(_listCount);
        writer.Write(_listLen);
        writer.Flush();
        ms.Position = 0;

        using var reader = new BinaryReader(ms);
        var element = ApoepElement.FromBinaryReader(reader);

        Assert.Multiple(() =>
        {
            Assert.That(element.Version, Is.EqualTo(_version));
            Assert.That(element.Encoding, Is.EqualTo(_encoding));
            Assert.That(element.Nomenclature, Is.EqualTo(_nomenclature));
            Assert.That(element.Functional, Is.EqualTo(_functional));
            Assert.That(element.SystemType, Is.EqualTo(_systemType));
            Assert.That(element.SystemId, Is.EqualTo(_systemId));
            Assert.That(element.ConfigId, Is.EqualTo(_configId));
            Assert.That(element.RecMode, Is.EqualTo(_recMode));
            Assert.That(element.ListCount, Is.EqualTo(_listCount));
            Assert.That(element.ListLen, Is.EqualTo(_listLen));
        });
    }

    [Test]
    public void FromBinaryReader_Should_Handle_Empty_SystemId()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write(_version);
        writer.Write(_encoding);
        writer.Write(_nomenclature);
        writer.Write(_functional);
        writer.Write(_systemType);
        writer.Write((short)0);
        writer.Write(_configId);
        writer.Write(_recMode);
        writer.Write(_listCount);
        writer.Write(_listLen);
        writer.Flush();
        ms.Position = 0;

        using var reader = new BinaryReader(ms);
        var element = ApoepElement.FromBinaryReader(reader);

        Assert.Multiple(() =>
        {
            Assert.That(element.SystemId, Is.Empty);
            Assert.That(element.Version, Is.EqualTo(_version));
        });
    }
}