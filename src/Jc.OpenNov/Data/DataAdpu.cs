using static Jc.OpenNov.Utilities.EncodableExtensions;

using Jc.OpenNov.Buffers;

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
            Field(() => Payload, (w, p) => WriteByteArray(w, p.ToByteArray()), _ => (Payload?.GetEncodedSize() ?? 0) + 4);
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

    public int EncodedSize()
    {
        return (Payload?.GetEncodedSize() ?? 0);
    }

    public override byte[] ToByteArray()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.PutUnsignedShort(InvokeId);
        writer.PutUnsignedShort(DChoice);

        if (Payload != null)
        {
            writer.PutUnsignedShort((ushort)Payload.GetEncodedSize());
            writer.Write(Payload.ToByteArray());
        }

        return ms.ToArray();
    }
}