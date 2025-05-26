using Moq;

namespace Jc.OpenNov.Tests;

[TestFixture]
internal sealed class DataReaderExtensionTests
{
    private Mock<IDataReader> _mockReader;
    private byte[] _command;

    [SetUp]
    public void SetUp()
    {
        _mockReader = new Mock<IDataReader>();
        _command = [0x01, 0x02];
    }

    [Test]
    public void ReadResult_Returns_Success_When_Command_Completed()
    {
        var responseData = new byte[] { 0x01, 0x00, 0x90, 0x00 };
        _mockReader.Setup(r => r.ReadData(_command)).Returns(responseData);

        var result = _mockReader.Object.ReadResult(_command);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.Content.ToArray(), Is.EqualTo(new byte[] { 0x01, 0x00 }));
        });
    }

    [Test]
    public void ReadResult_Returns_Failure_When_Command_Not_Completed()
    {
        var responseData = new byte[] { 0x00, 0x01, 0x03, 0x04 };
        _mockReader.Setup(r => r.ReadData(_command)).Returns(responseData);

        var result = _mockReader.Object.ReadResult(_command);

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.Content.ToArray(), Is.EqualTo(new byte[] { 0x0, 0x01 }));
        });
    }

    [Test]
    public void ReadResult_Handles_Empty_Response_Data()
    {
        var responseData = Array.Empty<byte>();
        _mockReader.Setup(r => r.ReadData(_command)).Returns(responseData);

        Assert.That(() => _mockReader.Object.ReadResult(_command), Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void ReadResult_Handles_Null_Response_Data()
    {
        _mockReader.Setup(r => r.ReadData(_command)).Returns((byte[])null);

        Assert.That(() => _mockReader.Object.ReadResult(_command), Throws.ArgumentNullException);
    }
}