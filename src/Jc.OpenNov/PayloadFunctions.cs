using Jc.OpenNov.Buffers;
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
            using var ms = new MemoryStream(5);
            using var writer = new BinaryWriter(ms);
            writer.PutByte(NvpController.Cla);
            writer.PutByte(NvpController.InsRb);
            writer.PutUnsignedShort((ushort)offset);
            writer.PutByte((byte)length);
            return ms.ToArray();
        }

        private static byte[] CreateTransceivePayload(int p1, int p2, byte[] data, bool le = false)
        {
            int length = data.Length + (le ? 6 : 5);
            using var ms = new MemoryStream(length);
            using var writer = new BinaryWriter(ms);
            writer.PutByte(NvpController.Cla);
            writer.PutByte(NvpController.InsSl);
            writer.PutByte((byte)p1);
            writer.PutByte((byte)p2);
            writer.PutByte((byte)data.Length);
            ms.Write(data, 0, data.Length);
            if (le) writer.PutByte(0x00);
            return ms.ToArray();
        }

        // === APDU Builders ===

        public static Apdu RetrieveInformation(ushort invokeId, Configuration config)
        {
            using var ms = new MemoryStream(4);
            using var writer = new BinaryWriter(ms);
            writer.PutUnsignedShort(config.Id);
            writer.PutUnsignedShort(0);
            ms.Seek(0, SeekOrigin.Begin);
            return new Apdu(
                Apdu.Prst,
                new DataApdu(
                    invokeId: invokeId,
                    dChoice: 0x0201,
                    payload: new EventRequest(
                        handle: 0,
                        currentTime: 0,
                        type: EventReport.MdcNotiConfig,
                        data: ms.ToArray()
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
            using var ms = new MemoryStream(12);
            using var writer = new BinaryWriter(ms);
            writer.PutUnsignedShort((ushort)instance);
            writer.PutUnsignedShort(0);
            writer.PutUnsignedShort((ushort)index);
            writer.PutUnsignedShort(0);
            writer.PutUnsignedShort((ushort)count);
            writer.PutByte(0x00); // block
            writer.PutByte((byte)(confirmed ? 0x80 : 0x00));
            return ms.ToArray();
        }
    }