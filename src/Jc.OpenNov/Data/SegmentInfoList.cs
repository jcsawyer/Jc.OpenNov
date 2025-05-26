using Jc.OpenNov.Utilities;

namespace Jc.OpenNov.Data;

public sealed class SegmentInfoList : Encodable
{
    public List<SegmentInfo> Items { get; }

    public SegmentInfoList(List<SegmentInfo>? items = null)
    {
        Items = items ?? [];
    }

    public static SegmentInfoList FromBinaryReader(BinaryReader reader)
    {
        var count = reader.GetUnsignedShort();
        _ = reader.GetUnsignedShort(); // length

        var items = Enumerable.Range(0, count)
            .Select(_ => SegmentInfo.FromBinaryReader(reader))
            .ToList();

        return new SegmentInfoList(items);
    }
}