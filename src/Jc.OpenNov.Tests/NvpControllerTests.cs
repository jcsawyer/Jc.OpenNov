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
        _testFilePath = "datatest.txt";
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
            Assert.That(data.Serial, Is.EqualTo("ACUKKE"));
            Assert.That(data.Doses.Count, Is.EqualTo(23));
            Assert.That(data.StartTime, Is.EqualTo(462022L));

            var expectedUnits = new[] { 60, 100, 50, 10, 160, 90 };
            var actualUnits = data.Doses.Take(6).Select(d => d.Units).ToArray();
            Assert.That(actualUnits, Is.EqualTo(expectedUnits));

            var expectedTimes = new long[] { -3591, -4188, -5597, -6098, -6120, -6225 };
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
            Assert.That(data.Serial, Is.EqualTo("ACUKKE"));
            Assert.That(data.Doses.Count, Is.EqualTo(18));
        });
    }
}