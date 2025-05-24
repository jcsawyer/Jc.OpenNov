using Jc.OpenNov.Buffers;
using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public sealed class ApoepElement : Encodable
{
    public uint Version { get; set; }
    public ushort Encoding { get; set; }
    public uint Nomenclature { get; set; }
    public uint Functional { get; set; }
    public uint SystemType { get; set; }
    public byte[] SystemId { get; set; }
    public ushort ConfigId { get; set; }
    public uint RecMode { get; set; }
    public ushort ListCount { get; set; }
    public ushort ListLen { get; set; }

    public const ushort Apoep = 20601;
    public const uint SysTypeManager = 0x80000000;
    public const int SysTypeAgent = 0x00800000; 

    public ApoepElement(
        uint version,
        ushort encoding,
        uint nomenclature,
        uint functional,
        uint systemType,
        byte[] systemId,
        ushort configId,
        uint recMode,
        ushort listCount,
        ushort listLen)
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
        
        Field(() => Version, WriteUnsignedInt, SizeOf);
        Field(() => Encoding, WriteUnsignedShort, SizeOf);
        Field(() => Nomenclature, WriteUnsignedInt, SizeOf);
        Field(() => Functional, WriteUnsignedInt, SizeOf);
        Field(() => SystemType, WriteUnsignedInt, SizeOf);
        Field(() => SystemId, WriteByteArray, SizeOf);
        Field(() => ConfigId, WriteUnsignedShort, SizeOf);
        Field(() => RecMode, WriteUnsignedInt, SizeOf);
        Field(() => ListCount, WriteUnsignedShort, SizeOf);
        Field(() => ListLen, WriteUnsignedShort, SizeOf);
    }

    public static ApoepElement FromBinaryReader(BinaryReader reader)
    {
        var version = reader.GetUnsignedInt();
        var encoding = reader.GetUnsignedShort();
        var nomenclature = reader.GetUnsignedInt();
        var functional = reader.GetUnsignedInt();
        var systemType = reader.GetUnsignedInt();

        var systemId = reader.GetBytes();

        var configId = reader.GetUnsignedShort();
        var recMode = reader.GetUnsignedInt();
        var listCount = reader.GetUnsignedShort();
        var listLen = reader.GetUnsignedShort();

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