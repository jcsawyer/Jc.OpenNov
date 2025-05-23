using Jc.OpenNov.Data;

namespace Jc.OpenNov.Tests;

[TestFixture]
internal sealed class NvpControllerTests
{
    private NvpController _controller;
    private string _testFilePath;

    [SetUp]
    public void SetUp()
    {
        _testFilePath = "nvp_datatest.txt"; // Ensure file is copied to output dir
        var stream = File.OpenRead(_testFilePath);
        _controller = new NvpController(new TestingDataReader(stream));
    }

    [Test]
    public void DataRead()
    {
        var result = _controller.DataRead();

        Assert.That(result, Is.TypeOf<PenResult.Success>());

        var success = (PenResult.Success)result;
        var data = success.Data;

        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        
        Assert.Multiple(() =>
        {
            Assert.That(data.Serial, Is.EqualTo("ABERGX"));
            Assert.That(data.Doses.Count, Is.EqualTo(96));
            Assert.That(data.StartTime, Is.EqualTo(6206432L));

            var expectedUnits = new[] { 20, 20, 100, 160, 140, 20 };
            var actualUnits = data.Doses.Take(6).Select(d => d.Units).ToArray();
            Assert.That(actualUnits, Is.EqualTo(expectedUnits));

            var expectedTimes = new long[] { -86, -87, -90, -92, -260474, -260480 };
            var actualTimes = data.Doses.Take(6)
                .Select(d => (d.Time - currentTime) / 1000)
                .ToArray();
            Assert.That(actualTimes, Is.EqualTo(expectedTimes));
        });
    }

    [Test]
    public void DataReadStopCondition()
    {
        var result = _controller.DataRead((_, list) =>
            list.FirstOrDefault(d => d.Time > 1726330347000L) != null
        );

        Assert.That(result, Is.TypeOf<PenResult.Success>());

        var success = (PenResult.Success)result;
        var data = success.Data;

        Assert.Multiple(() =>
        {
            Assert.That(data.Serial, Is.EqualTo("ABERGX"));
            Assert.That(data.Doses.Count, Is.EqualTo(18));
        });
    }
}