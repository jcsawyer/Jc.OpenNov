using static Jc.OpenNov.Utilities.EncodableExtensions;

using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Data;

public sealed class Configuration : Encodable
{
    public ushort Id { get; }
    public ushort Handle { get; }
    public int NbSegment { get; }
    public int TotalEntries { get; }
    public int UnitCode { get; }
    public int TotalStorage { get; }
    public List<Attribute> Attributes { get; }

    public Configuration(
        ushort id,
        ushort handle,
        int nbSegment,
        int totalEntries,
        int unitCode,
        int totalStorage,
        List<Attribute> attributes)
    {
        Id = id;
        Handle = handle;
        NbSegment = nbSegment;
        TotalEntries = totalEntries;
        UnitCode = unitCode;
        TotalStorage = totalStorage;
        Attributes = attributes;
        
        Field(() => Id, WriteUnsignedShort, SizeOf);
        Field(() => Attributes.Count, WriteInt, SizeOf);

        Field(GetEncodedSize, (w, len) =>
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            
            foreach (var attr in Attributes)
            {
                var attrBytes = attr.ToByteArray();
                bw.Write(attrBytes);
            }

            var data = ms.ToArray();

            WriteShort(w, (short)data.Length);
            w.Write(data);
        }, len => 2 + len);
    }

    public static Configuration ReadFrom(BinaryReader reader)
    {
        var id = reader.GetUnsignedShort();
        var count = reader.GetUnsignedShort();
        _ = reader.GetUnsignedShort(); // skip len

        var nbSegment = -1;
        var totalEntries = -1;
        var unitCode = -1;
        var totalStorage = -1;

        var attributes = new List<Attribute>();

        for (var i = 0; i < count; i++)
        {
            _ = reader.GetUnsignedShort(); // cls (ignored)
            _ = reader.GetUnsignedShort(); // handle (ignored)
            var attrCount = reader.GetUnsignedShort();
            _ = reader.GetUnsignedShort(); // attrLen (ignored)

            for (var j = 0; j < attrCount; j++)
            {
                var attribute = Attribute.ReadFrom(reader);

                switch (attribute.Type)
                {
                    case Attribute.AttrNumSeg:
                        nbSegment = attribute.Value;
                        break;
                    case Attribute.AttrMetricStoreUsageCnt:
                        totalEntries = attribute.Value;
                        break;
                    case Attribute.AttrUnitCode:
                        unitCode = attribute.Value;
                        break;
                    case Attribute.AttrMetricStoreCapacCnt:
                        totalStorage = attribute.Value;
                        break;
                    case Attribute.AttrAttributeValMap:
                        // TODO
                        break;
                }

                attributes.Add(attribute);
            }
        }

        return new Configuration(id, 0, nbSegment, totalEntries, unitCode, totalStorage, attributes);
    }
}