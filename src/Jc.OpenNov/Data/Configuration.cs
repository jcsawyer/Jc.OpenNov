namespace Jc.OpenNov.Data;

public sealed class Configuration : Encodable
{
    public int Id { get; }
    public int Handle { get; }
    public int NbSegment { get; }
    public int TotalEntries { get; }
    public int UnitCode { get; }
    public int TotalStorage { get; }
    public List<Attribute> Attributes { get; }

    public Configuration(
        int id,
        int handle,
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
    }

    public static Configuration ReadFrom(BinaryReader reader)
    {
        var id = reader.ReadUInt16();
        var count = reader.ReadUInt16();
        _ = reader.ReadUInt16(); // skip len

        var nbSegment = -1;
        var totalEntries = -1;
        var unitCode = -1;
        var totalStorage = -1;

        var attributes = new List<Attribute>();

        for (var i = 0; i < count; i++)
        {
            _ = reader.ReadUInt16(); // cls (ignored)
            _ = reader.ReadUInt16(); // handle (ignored)
            var attrCount = reader.ReadUInt16();
            _ = reader.ReadUInt16(); // attrLen (ignored)

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