using Jc.OpenNov.Utilities;

namespace Jc.OpenNov.Data;

public sealed class SegmentInfoMap
{
    public int Bits { get; }
    public int Count { get; }
    public int Length { get; }
    public List<SegmentEntry> Items { get; }

    public SegmentInfoMap(int bits, int count, int length, List<SegmentEntry> items)
    {
        Bits = bits;
        Count = count;
        Length = length;
        Items = items;
    }

    public static SegmentInfoMap FromBinaryReader(BinaryReader reader)
    {
        int bits = reader.GetUnsignedShort();
        int count = reader.GetUnsignedShort();
        int length = reader.GetUnsignedShort();

        var items = new List<SegmentEntry>();
        for (var i = 0; i < count; i++)
        {
            items.Add(SegmentEntry.FromBinaryReader(reader));
        }

        return new SegmentInfoMap(bits, count, length, items);
    }
}