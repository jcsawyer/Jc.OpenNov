using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public sealed class ApoepElement : Encodable
{
    public int Version { get; set; }
    public short Encoding { get; set; }
    public int Nomenclature { get; set; }
    public int Functional { get; set; }
    public int SystemType { get; set; }
    public byte[] SystemId { get; set; }
    public short ConfigId { get; set; }
    public int RecMode { get; set; }
    public short ListCount { get; set; }
    public short ListLen { get; set; }

    public const ushort Apoep = 20601;
    public const int SysTypeManager = unchecked((int)0x80000000);
    public const int SysTypeAgent = 0x00800000; 

    public ApoepElement(
        int version,
        short encoding,
        int nomenclature,
        int functional,
        int systemType,
        byte[] systemId,
        short configId,
        int recMode,
        short listCount,
        short listLen)
    {
        Version = version;
        Encoding = encoding;
        Nomenclature = nomenclature;
        Functional = functional;
        SystemType = systemType;
        SystemId = systemId;
        ConfigId = configId;
        RecMode = recMode;
        ListCount = listCount;
        ListLen = listLen;
        
        Field(() => Version, WriteInt, SizeOf);
        Field(() => Encoding, WriteShort, SizeOf);
        Field(() => Nomenclature, WriteInt, SizeOf);
        Field(() => Functional, WriteInt, SizeOf);
        Field(() => SystemType, WriteInt, SizeOf);
        Field(() => SystemId, WriteByteArray, SizeOf);
        Field(() => ConfigId, WriteShort, SizeOf);
        Field(() => RecMode, WriteInt, SizeOf);
        Field(() => ListCount, WriteShort, SizeOf);
        Field(() => ListLen, WriteShort, SizeOf);
    }

    public static ApoepElement FromBinaryReader(BinaryReader reader)
    {
        var version = reader.ReadInt32();
        var encoding = reader.ReadInt16();
        var nomenclature = reader.ReadInt32();
        var functional = reader.ReadInt32();
        var systemType = reader.ReadInt32();

        var systemIdLength = reader.ReadInt16();
        var systemId = reader.ReadBytes(systemIdLength);

        var configId = reader.ReadInt16();
        var recMode = reader.ReadInt32();
        var listCount = reader.ReadInt16();
        var listLen = reader.ReadInt16();

        return new ApoepElement(
            version,
            encoding,
            nomenclature,
            functional,
            systemType,
            systemId,
            configId,
            recMode,
            listCount,
            listLen
        );
    }
}