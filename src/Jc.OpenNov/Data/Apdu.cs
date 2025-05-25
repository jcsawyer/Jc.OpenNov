using Jc.OpenNov.Utilities;
using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public sealed class Apdu : Encodable
{
    public const ushort Aarq = 0xE200;
    public const ushort Aare = 0xE300;
    public const ushort Rlrq = 0xE400;
    public const ushort Rlre = 0xE500;
    public const ushort Abrt = 0xE600;
    public const ushort Prst = 0xE700;

    public ushort At { get; }
    public Encodable Payload { get; }

    public Apdu(ushort at, Encodable payload)
    {
        At = at;
        Payload = payload;
        
        Field(() => At, WriteUnsignedShort, SizeOf);
        Field(Payload.GetEncodedSize, (w, len) =>
        {
            WriteByteArray(w, Payload.ToByteArray());
        }, len => 2 + len);
    }

    public static Apdu FromBinaryReader(BinaryReader reader)
    {
        var at = reader.GetUnsignedShort();
        _ = reader.GetUnsignedShort(); // payloadLen

        var payload = at switch
        {
            Aarq or Aare => ARequest.FromByteBuffer(reader),
            Prst => DataApdu.FromBinaryReader(reader),
            Rlrq or Rlre or Abrt => new Encodable(),
            _ => throw new InvalidOperationException($"Unknown at value {at}")
        };

        return new Apdu(at, payload);
    }
}

public static class ApduExtensions
{
    public static DataApdu? GetDataApdu(this Apdu apdu)
    {
        return apdu.Payload as DataApdu;
    }

    public static EventReport? GetEventReport(this Apdu apdu)
    {
        var dataApdu = apdu.GetDataApdu();
        return dataApdu?.Payload as EventReport;
    }
}