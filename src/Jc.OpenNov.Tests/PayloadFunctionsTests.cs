namespace Jc.OpenNov.Tests;

[TestFixture]
internal sealed class PayloadFunctionsTests
{
    [Test]
    public void CreateReadPayload_Returns_Correct_Buffer_For_Valid_Input()
    {
        var result = PayloadFunctions.CreateReadPayload(36864, 16);

        Assert.Multiple(() =>
        {
            Assert.That(result.Length, Is.EqualTo(5));
            Assert.That(result[0], Is.EqualTo(NvpController.Cla));
            Assert.That(result[1], Is.EqualTo(NvpController.InsRb));
            Assert.That(result[2..4], Is.EqualTo(new byte[] { 0x90, 0x00 }));
            Assert.That(result[4], Is.EqualTo(16));
        });
    }

    [Test]
    public void CreateReadPayload_Handles_Zero_Offset_And_Length()
    {
        var result = PayloadFunctions.CreateReadPayload(0, 0);

        Assert.Multiple(() =>
        {
            Assert.That(result.Length, Is.EqualTo(5));
            Assert.That(result[2..4], Is.EqualTo(new byte[] { 0x00, 0x00 }));
            Assert.That(result[4], Is.EqualTo(0));
        });
    }

    [Test]
    public void EventRequestData_Returns_Correct_Buffer_For_Valid_Input()
    {
        var result = PayloadFunctions.EventRequestData(1, 2, 3, true);

        Assert.Multiple(() =>
        {
            Assert.That(result.Length, Is.EqualTo(12));
            Assert.That(result[0..2], Is.EqualTo(new byte[] { 0x00, 0x01 }));
            Assert.That(result[4..6], Is.EqualTo(new byte[] { 0x00, 0x02 }));
            Assert.That(result[8..10], Is.EqualTo(new byte[] { 0x00, 0x03 }));
            Assert.That(result[11], Is.EqualTo(0x80));
        });
    }

    [Test]
    public void EventRequestData_Handles_Unconfirmed_Request()
    {
        var result = PayloadFunctions.EventRequestData(1, 2, 3, false);

        Assert.Multiple(() =>
        {
            Assert.That(result.Length, Is.EqualTo(12));
            Assert.That(result[11], Is.EqualTo(0x00));
        });
    }
}