using Jc.OpenNov.Data;
using Jc.OpenNov.Utilities;

namespace Jc.OpenNov;

public sealed class NvpController
{
    internal const byte Cla = 0x00;
    internal const byte InsSl = 0xA4;
    internal const byte InsRb = 0xB0;
    internal const byte ByName = 0x04;
    internal const byte FirstOnly = 0x0C;

    internal static readonly byte[] NdefTagApplicationSelect = [0xD2, 0x76, 0x00, 0x00, 0x85, 0x01, 0x01];
    internal static readonly byte[] CapabilityContainerSelect = [0xE1, 0x03];
    internal static readonly byte[] NdefSelect = [0xE1, 0x04];

    public const ushort CommandCompleted = 0x9000;

    private readonly IDataReader _dataReader;
    private readonly PhdManager _phdManager;

    public NvpController(IDataReader dataReader)
    {
        _dataReader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
        _phdManager = new PhdManager(_dataReader);
    }
    
    public async Task<PenResult> DataRead(Func<string, List<InsulinDose>, bool>? stopCondition = null)
        {
            await using (await _dataReader.ReadResult(PayloadFunctions.ApplicationSelect()));
            await using (await _dataReader.ReadResult(PayloadFunctions.CapabilityContainerSelect()));
            await using (await _dataReader.ReadResult(PayloadFunctions.CreateReadPayload(0, 15)));
            await using (await _dataReader.ReadResult(PayloadFunctions.NdefSelect()));

            return await RetrieveConfiguration(stopCondition ?? ((_, _) => false)).ConfigureAwait(false);
        }

        private async Task<PenResult> RetrieveConfiguration(Func<string, List<InsulinDose>, bool> stopCondition)
        {
            await using var lengthResult = await _dataReader.ReadResult(PayloadFunctions.CreateReadPayload(0, 2)).ConfigureAwait(false);
            int length = new BinaryReader(lengthResult.Content).GetShort();

            await using var fullRead = await _dataReader.ReadResult(PayloadFunctions.CreateReadPayload(2, length)).ConfigureAwait(false);

            var ack = new T4Update([0xD0, 0x00, 0x00]);
            await using (await _dataReader.ReadResult(ack.ToByteArray()).ConfigureAwait(false));

            using var fullReadReader = new BinaryReader(fullRead.Content);
            var phdPacket = PhdPacket.FromBinaryReader(fullReadReader);
            using var phdMs = new MemoryStream(phdPacket.Content);
            using var phdReader = new BinaryReader(phdMs);
            var apdu = Apdu.FromBinaryReader(phdReader);

            var aRequest = (ARequest)apdu.Payload;

            var sendApdu = new Apdu(
                Apdu.Aare,
                new AResponse(result: 3, protocol: ApoepElement.Apoep, apoep: aRequest.Apoep)
            );

            var result = await _phdManager.SendApduRequest(sendApdu).ConfigureAwait(false);
            using var adpuMs = new MemoryStream(result);
            using var adpuReader = new BinaryReader(adpuMs);
            var resultApdu = Apdu.FromBinaryReader(adpuReader);

            var dataApdu = (DataApdu)resultApdu.Payload;
            var configuration = ((EventReport)dataApdu.Payload).Configuration;

            if (configuration != null)
            {
                await _phdManager.SendApduRequest(PayloadFunctions.RetrieveInformation(dataApdu.InvokeId, configuration));

                var info = await _phdManager.DecodeDataApduRequest<FullSpecification>(
                    PayloadFunctions.AskInformation(dataApdu.InvokeId, configuration)
                ).ConfigureAwait(false);

                var model = string.Join(" ", info.Model);
                var serial = info.Specification.Serial;
                var startTime = info.RelativeTime;

                var doseList = new List<InsulinDose>();

                var storageArray = await _phdManager.SendApduRequest(PayloadFunctions.ConfirmedAction(dataApdu.InvokeId)).ConfigureAwait(false);
                using var saMs = new MemoryStream(storageArray);
                using var saReader = new BinaryReader(saMs);
                var storage = Apdu.FromBinaryReader(saReader);

                if (storage.Payload is DataApdu storageDataApdu &&
                    storageDataApdu.Payload is SegmentInfoList segmentInfoList)
                {
                    var segment = segmentInfoList.Items.First();
                    await ReadSegment(segment, storageDataApdu.InvokeId, doseList, list => stopCondition(serial, list)).ConfigureAwait(false);

                    return new PenResult.Success(new PenResultData
                    {
                        Model = model,
                        Serial = serial,
                        StartTime = startTime,
                        Doses = doseList
                    });
                }
            }

            return new PenResult.Failure("Unknown error");
        }

        private async Task ReadSegment(SegmentInfo segment, ushort invokeId, List<InsulinDose> doseList, Func<List<InsulinDose>, bool> stopCondition)
        {
            var xferArray = await _phdManager.SendApduRequest(PayloadFunctions.XferAction(invokeId, segment.InstNum)).ConfigureAwait(false);
            using var xferMs = new MemoryStream(xferArray);
            using var xferReader = new BinaryReader(xferMs);
            Apdu.FromBinaryReader(xferReader); // xfer

            var result = await _phdManager.SendEmptyRequest().ConfigureAwait(false);

            var finished = false;

            do
            {
                if (result.Length == 0)
                    result = await _phdManager.SendEmptyRequest().ConfigureAwait(false);

                using var ms = new MemoryStream(result);
                using var reader = new BinaryReader(ms);
                var logApdu = Apdu.FromBinaryReader(reader);

                var eventReport = logApdu.GetEventReport();
                if (eventReport != null)
                {
                    doseList.AddRange(eventReport.InsulinDoses);

                    if (eventReport.InsulinDoses.Count == 0 || stopCondition(eventReport.InsulinDoses))
                    {
                        finished = true;
                        continue;
                    }

                    var currentInstance = (ushort)eventReport.Instance;
                    var currentIndex = eventReport.Index;

                    result = await _phdManager.SendApduRequest(PayloadFunctions.ConfirmedXfer(
                        logApdu.GetDataApdu()?.InvokeId ?? 0,
                        PayloadFunctions.EventRequestData(currentInstance, currentIndex, eventReport.InsulinDoses.Count, true)
                    )).ConfigureAwait(false);
                }
                else
                {
                    finished = true;
                }
            }
            while (!finished);
        }
}