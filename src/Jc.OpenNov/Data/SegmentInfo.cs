using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Data;

public sealed class SegmentInfo
{
    public ushort InstNum { get; }
    public int Usage { get; }
    public List<Attribute> Items { get; }
    public SegmentInfoMap? SegmentInfoMap { get; }

    public SegmentInfo(ushort instNum, int usage, List<Attribute> items, SegmentInfoMap? segmentInfoMap = null)
    {
        InstNum = instNum;
        Usage = usage;
        Items = items;
        SegmentInfoMap = segmentInfoMap;
    }

    public static SegmentInfo FromBinaryReader(BinaryReader reader)
    {
        var instNum = reader.GetUnsignedShort();
        var count = reader.GetUnsignedShort();
        _ = reader.GetUnsignedShort(); // length

        var usage = -1;
        SegmentInfoMap? segmentInfoMap = null;
        var items = new List<Attribute>();

        for (var i = 0; i < count; i++)
        {
            var attribute = Attribute.ReadFrom(reader);

            if (attribute.Type == Attribute.AttrPmSegMap)
            {
                using var segMapStream = new MemoryStream(attribute.Data);
                using var segMapReader = new BinaryReader(segMapStream);
                segmentInfoMap = SegmentInfoMap.FromBinaryReader(segMapReader);
            }
            else if (attribute.Type == Attribute.AttrSegUsageCnt)
            {
                usage = attribute.Value;
            }

            items.Add(attribute);
        }

        return new SegmentInfo(instNum, usage, items, segmentInfoMap);
    }
}