using System.Buffers.Binary;
using Jc.OpenNov.Utilities;
using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public sealed class DataApdu : Encodable
{
    public const ushort EventReportChosen = 0x0100;
    public const ushort ConfirmedEventReportChosen = 0x0101;
    public const ushort SconfirmedEventReportChosen = 0x0201;
    public const ushort GetChosen = 0x0203;
    public const ushort SgetChosen = 0x0103;
    public const ushort ConfirmedAction = 0x0107;
    public const ushort ConfirmedActionChosen = 0x0207;
    public const ushort MdcActSegGetInfo = 0x0C0D;
    public const ushort MdcActSegTrigXfer = 0x0C1C;

    public ushort InvokeId { get; }
    public ushort DChoice { get; }
    public Encodable? Payload { get; }

    public DataApdu(ushort invokeId, ushort dChoice, Encodable? payload = null)
    {
        InvokeId = invokeId;
        DChoice = dChoice;
        Payload = payload;
        
        Field(() => InvokeId, WriteUnsignedShort, SizeOf);
        Field(() => DChoice, WriteUnsignedShort, SizeOf);

        if (Payload != null)
        {
            Field(() => Payload.GetEncodedSize() + 2, (w, len) =>
            {
                WriteByteArray(w, Payload.ToByteArray());
            }, len => 2 + len);
        }
    }

    public static DataApdu FromBinaryReader(BinaryReader reader)
    {
        _ = reader.GetUnsignedShort(); // olen
        var invokeId = reader.GetUnsignedShort();
        var dChoice = reader.GetUnsignedShort();
        _ = reader.GetUnsignedShort(); // dlen

        var payload = dChoice switch
        {
            ConfirmedActionChosen => ParseConfirmedActionPayload(reader),
            ConfirmedEventReportChosen => EventReport.ReadFrom(reader),
            SgetChosen or GetChosen => ParseGetPayload(reader),
            _ => null
        };

        return new DataApdu(invokeId, dChoice, payload);
    }

    private static Encodable? ParseConfirmedActionPayload(BinaryReader reader)
    {
        _ = reader.GetUnsignedShort(); // handle
        var actionType = reader.GetUnsignedShort();
        _ = reader.GetUnsignedShort(); // actionLen

        return actionType switch
        {
            MdcActSegGetInfo => SegmentInfoList.FromBinaryReader(reader),
            MdcActSegTrigXfer => TriggerSegmentDataXfer.FromByteBuffer(reader),
            _ => null
        };
    }

    private static Encodable? ParseGetPayload(BinaryReader reader)
    {
        _ = reader.GetUnsignedShort(); // handle
        var count = reader.GetUnsignedShort();
        _ = reader.GetUnsignedShort(); // len

        var attributes = new List<Attribute>();
        for (var i = 0; i < count; i++)
        {
            attributes.Add(Attribute.ReadFrom(reader));
        }

        return FullSpecification.FromAttributes(attributes);
    }

    private int EncodedSize()
    {
        return 6 + (Payload?.GetEncodedSize() + 2 ?? 0);
    }

    public override byte[] ToByteArray()
    {
        var payloadSize = Payload?.GetEncodedSize() + 2 ?? 0;
        var buffer = new byte[EncodedSize()];
        var span = buffer.AsSpan();

        BinaryPrimitives.WriteUInt16BigEndian(span[..2], (ushort)(payloadSize + 4));
        BinaryPrimitives.WriteUInt16BigEndian(span[2..], InvokeId);
        BinaryPrimitives.WriteUInt16BigEndian(span[4..], DChoice);

        if (Payload == null)
        {
            return buffer;
        }
        
        BinaryPrimitives.WriteUInt16BigEndian(span[6..], (ushort)Payload.GetEncodedSize());
        Payload.ToByteArray().CopyTo(span[8..]);

        return buffer;
    }
}