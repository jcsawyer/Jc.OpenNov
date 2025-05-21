using Jc.OpenNov.Buffers;
using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public sealed class ARequest : Encodable
{
    public ushort Protocol { get; init; }
    public uint Version { get; init; }
    public ushort Elements { get; init; }
    public ApoepElement Apoep { get; init; }

    public ARequest(
        ushort protocol,
        uint version,
        ushort elements,
        ApoepElement apoep)
    {
        Protocol = protocol;
        Version = version;
        Elements = elements;
        Apoep = apoep;
        
        Field(() => Protocol, WriteUnsignedShort, SizeOf);
        Field(() => Version, WriteUnsignedInt, SizeOf);
        Field(() => Elements, WriteUnsignedShort, SizeOf);

        Field(() => Apoep.GetEncodedSize(), (w, _) =>
        {
            var buffer = new MemoryStream();
            using var subWriter = new BinaryWriter(buffer);
            Apoep.WriteTo(subWriter);
            var data = buffer.ToArray();

            WriteShort(w, (short)data.Length);
            w.Write(data);
        }, len => 2 + len);
    }

    public static ARequest FromByteBuffer(BinaryReader reader)
    {
        var version = reader.GetUnsignedInt();
        var elements = reader.GetUnsignedShort();
        _ = reader.GetUnsignedShort(); // Length

        ApoepElement? apoep = null;
        for (var i = 0; i < elements; i++)
        {
            var protocol = reader.GetUnsignedShort();
            var bytes = reader.GetBytes();
            if (protocol == ApoepElement.Apoep)
            {
                using var memoryStream = new MemoryStream(bytes);
                using var binaryReader = new BinaryReader(memoryStream);
                apoep = ApoepElement.FromBinaryReader(binaryReader);
            }
        }

        if (apoep is not null)
        {
            return new ARequest(ApoepElement.Apoep, version, elements, apoep);
        }

        throw new InvalidOperationException("APOEP packet not found");
    }
}