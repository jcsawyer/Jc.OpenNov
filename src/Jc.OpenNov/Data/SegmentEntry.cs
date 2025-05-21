using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Data;

public sealed class SegmentEntry
{
    public int ClassId { get; }
    public int MetricType { get; }
    public int Otype { get; }
    public int Handle { get; }
    public int AmCount { get; }
    public byte[] Data { get; }

    public SegmentEntry(int classId, int metricType, int otype, int handle, int amCount, byte[] data)
    {
        ClassId = classId;
        MetricType = metricType;
        Otype = otype;
        Handle = handle;
        AmCount = amCount;
        Data = data;
    }

    public static SegmentEntry FromBinaryReader(BinaryReader reader)
    {
        int classId = reader.GetUnsignedShort();
        int metricType = reader.GetUnsignedShort();
        int otype = reader.GetUnsignedShort();
        int handle = reader.GetUnsignedShort();
        int amCount = reader.GetUnsignedShort();
        var data = reader.GetBytes();

        return new SegmentEntry(classId, metricType, otype, handle, amCount, data);
    }
}