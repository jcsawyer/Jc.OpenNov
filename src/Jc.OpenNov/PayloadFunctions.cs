using System.Buffers.Binary;
using Jc.OpenNov.Data;

namespace Jc.OpenNov;

public static class PayloadFunctions
{
    public static byte[] ApplicationSelect() =>
        CreateTransceivePayload(NvpController.ByName, 0, NvpController.NdefTagApplicationSelect, le: true);

    public static byte[] CapabilityContainerSelect() =>
        CreateTransceivePayload(0, NvpController.FirstOnly, NvpController.CapabilityContainerSelect);

    public static byte[] NdefSelect() =>
        CreateTransceivePayload(0, NvpController.FirstOnly, NvpController.NdefSelect);

    public static byte[] CreateReadPayload(int offset, int length)
    {
        var buffer = new byte[5];
        var span = buffer.AsSpan();
        span[0] = NvpController.Cla;
        span[1] = NvpController.InsRb;
        BinaryPrimitives.WriteUInt16BigEndian(span[2..], (ushort)offset);
        span[4] = (byte)length;
        return buffer;
    }

    private static byte[] CreateTransceivePayload(int p1, int p2, byte[] data, bool le = false)
    {
        var length = data.Length + (le ? 6 : 5);
        var buffer = new byte[length];
        var span = buffer.AsSpan();
        var offset = 0;
        span[offset++] = NvpController.Cla;
        span[offset++] = NvpController.InsSl;
        span[offset++] = (byte)p1;
        span[offset++] = (byte)p2;
        span[offset++] = (byte)data.Length;
        data.CopyTo(span.Slice(offset, data.Length));
        offset += data.Length;
        if (le) span[offset] = 0x00;
        return buffer;
    }

    public static Apdu RetrieveInformation(ushort invokeId, Configuration config)
    {
        var buffer = new byte[4];
        var span = buffer.AsSpan();
        BinaryPrimitives.WriteUInt16BigEndian(span.Slice(0, 2), config.Id);
        BinaryPrimitives.WriteUInt16BigEndian(span.Slice(2, 2), 0);
        return new Apdu(
            Apdu.Prst,
            new DataApdu(
                invokeId: invokeId,
                dChoice: 0x0201,
                payload: new EventRequest(
                    handle: 0,
                    currentTime: 0,
                    type: EventReport.MdcNotiConfig,
                    data: buffer
                )
            )
        );
    }

    public static Apdu AskInformation(ushort invokeId, Configuration config) =>
        new Apdu(
            Apdu.Prst,
            new DataApdu(
                invokeId: invokeId,
                dChoice: 0x0103,
                payload: new ArgumentsSimple(handle: config.Handle)
            )
        );

    public static Apdu ConfirmedAction(ushort invokeId) =>
        new Apdu(
            Apdu.Prst,
            new DataApdu(
                invokeId: invokeId,
                dChoice: DataApdu.ConfirmedAction,
                payload: Data.ConfirmedAction.AllSegment(
                    handle: Data.ConfirmedAction.StoreHandle,
                    type: DataApdu.MdcActSegGetInfo
                )
            )
        );

    public static Apdu XferAction(ushort invokeId, ushort segment) =>
        new Apdu(
            Apdu.Prst,
            new DataApdu(
                invokeId: invokeId,
                dChoice: DataApdu.ConfirmedAction,
                payload: Data.ConfirmedAction.Segment(
                    handle: Data.ConfirmedAction.StoreHandle,
                    type: DataApdu.MdcActSegTrigXfer,
                    segment: segment
                )
            )
        );

    public static Apdu ConfirmedXfer(ushort invokeId, byte[] data) =>
        new Apdu(
            Apdu.Prst,
            new DataApdu(
                invokeId: invokeId,
                dChoice: DataApdu.SconfirmedEventReportChosen,
                payload: new EventRequest(
                    handle: Data.ConfirmedAction.StoreHandle,
                    currentTime: -1,
                    type: EventReport.MdcNotiSegmentData,
                    data: data
                )
            )
        );

    public static byte[] EventRequestData(ushort instance, int index, int count, bool confirmed)
    {
        var buffer = new byte[12];
        var span = buffer.AsSpan();
        var offset = 0;
        BinaryPrimitives.WriteUInt16BigEndian(span[offset..], instance);
        offset += 2;
        BinaryPrimitives.WriteUInt16BigEndian(span[offset..], 0);
        offset += 2;
        BinaryPrimitives.WriteUInt16BigEndian(span[offset..], (ushort)index);
        offset += 2;
        BinaryPrimitives.WriteUInt16BigEndian(span[offset..], 0);
        offset += 2;
        BinaryPrimitives.WriteUInt16BigEndian(span[offset..], (ushort)count);
        offset += 2;
        span[offset++] = 0x00; // block
        span[offset] = (byte)(confirmed ? 0x80 : 0x00);
        return buffer;
    }
}