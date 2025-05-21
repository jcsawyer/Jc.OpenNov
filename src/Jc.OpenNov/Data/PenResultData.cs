namespace Jc.OpenNov.Data;

public sealed class PenResultData
{
    public string Model { get; init; }
    public string Serial { get; init; }
    public long StartTime { get; init; }
    public List<InsulinDose> Doses { get; init; }
}