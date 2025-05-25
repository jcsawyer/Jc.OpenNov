using Jc.OpenNov.Data;
using Jc.OpenNov.Utilities;

namespace Jc.OpenNov.Tests.Data;

public class DataApduTests
{
    private byte[] adpuEventReportBytes;
    private byte[] adpuSpecificationBytes;
    private byte[] apduSegmentInfoBytes;
    private byte[] apduDoseListBytes;

    [SetUp]
    public void SetUp()
    {
        adpuEventReportBytes = HexToBytes("""
                                              D1 03 C9 50 48 44 82 E7 00 00 C4 00 C2 00 00 01 
                                              01 00 BC 00 00 00 00 00 00 0D 1C 00 B2 40 0A 00 
                                              04 00 AC 00 3D 01 00 00 08 00 38 0A 4D 00 02 08 
                                              00 09 43 00 02 00 00 09 41 00 04 00 00 03 20 09 
                                              53 00 02 00 00 0A 57 00 04 00 02 50 4D 09 51 00 
                                              02 00 01 0A 63 00 04 00 00 00 00 09 44 00 04 00 
                                              00 01 11 00 06 00 02 00 04 00 20 09 2F 00 04 00 
                                              82 34 01 0A 46 00 02 F0 40 09 96 00 02 15 60 0A 
                                              55 00 08 00 01 00 04 0A 56 00 04 00 05 00 03 00 
                                              03 00 1A 09 2F 00 04 00 82 34 02 0A 46 00 02 F0 
                                              40 0A 55 00 08 00 01 00 04 0A 66 00 02 00 06 00 
                                              04 00 03 00 1A 09 2F 00 04 00 82 F0 00 0A 46 00 
                                              02 F0 40 0A 55 00 08 00 01 00 04 0A 66 00 02 90 
                                              00
                                          """);

        adpuSpecificationBytes = HexToBytes("""
                                                D1 03 D8 50 48 44 86 E7 00 00 D3 00 D1 00 00 02
                                                03 00 CB 00 00 00 08 00 C5 09 84 00 0A 00 08 00
                                                14 65 00 40 13 3E A8 09 8F 00 04 00 26 21 E4 0A
                                                45 00 10 20 00 1F 00 FF FF FF FF 00 00 1F 40 00
                                                00 00 00 09 2D 00 4B 00 04 00 47 00 01 00 01 00
                                                06 41 42 47 50 56 49 00 02 00 01 00 20 44 32 32
                                                34 35 31 37 32 39 36 30 30 30 30 30 20 44 32 32
                                                34 35 31 37 32 39 36 30 30 30 30 30 20 00 03 00
                                                01 00 01 00 00 04 00 01 00 08 30 31 2E 30 38 2E
                                                30 31 0A 5A 00 08 00 01 00 04 10 48 00 01 09 28
                                                00 1C 00 10 4E 6F 76 6F 20 4E 6F 72 64 69 73 6B
                                                20 41 2F 53 00 08 4E 6F 76 6F 50 65 6E 00 0A 44
                                                00 02 40 0A 0A 4B 00 16 00 02 00 12 02 01 00 08
                                                04 00 00 01 00 02 A0 48 02 02 00 02 00 00
                                            """);

        apduSegmentInfoBytes = HexToBytes("""
                                              D1 03 81 50 48 44 88 E7 00 00 7C 00 7A 00 00 02
                                              07 00 74 01 00 0C 0D 00 6E 00 01 00 6A 00 10 00
                                              06 00 64 09 22 00 02 00 10 0A 4E 00 36 40 00 00
                                              03 00 30 00 06 00 82 34 01 00 02 00 01 00 04 0A
                                              56 00 04 00 05 00 82 34 02 00 03 00 01 00 04 0A
                                              66 00 02 00 06 00 82 F0 00 00 04 00 01 00 04 0A
                                              66 00 02 09 53 00 02 00 00 0A 58 00 0A 00 08 44
                                              6F 73 65 20 4C 6F 67 09 7B 00 04 00 00 00 59 0A
                                              64 00 04 00 02 71 00
                                          """);

        apduDoseListBytes = HexToBytes("""
                                           D1 03 FD 50 48 44 8E E7 00 00 F8 00 F6 80 01 01
                                           01 00 F0 01 00 00 B0 81 59 0D 21 00 E6 00 10 00
                                           00 00 00 00 00 00 12 80 00 00 D8 00 AF 3C BA FF
                                           00 01 A4 08 00 00 00 00 AF 3C A9 FF 00 01 A4 08
                                           00 00 00 00 AF 3C 96 FF 00 00 14 08 00 00 00 00
                                           AF 3C 95 FF 00 00 14 08 00 00 00 00 AF 3C 94 FF
                                           00 00 14 08 00 00 00 00 AF 3C 93 FF 00 00 14 08
                                           00 00 00 00 AD EA DD FF 00 01 A4 08 00 00 00 00
                                           AD EA D3 FF 00 01 A4 08 00 00 00 00 AD EA C8 FF
                                           00 00 14 08 00 00 00 00 AC A2 4A FF 00 01 A4 08
                                           00 00 00 00 AC A2 3B FF 00 01 A4 08 00 00 00 00
                                           AC A2 28 FF 00 00 14 08 00 00 00 00 AB 53 DB FF
                                           00 01 A4 08 00 00 00 00 AB 53 CF FF 00 01 A4 08
                                           00 00 00 00 AB 53 C1 FF 00 00 14 08 00 00 00 00
                                           AA 02 90 FF 00 01 AE 08 00 00 00 00 AA 02 7D FF
                                           00 00 14 08 00 00 00 00 AA 02 7C FF 00 00 14 08
                                           00 00 00
                                       """);
    }

    [Test]
    public void Data_Apdu_Segment_Info_Parsing()
    {
        var phd = PhdPacket.FromBinaryReader(new BinaryReader(new MemoryStream(apduSegmentInfoBytes)));
        var resultApdu = Apdu.FromBinaryReader(new BinaryReader(new MemoryStream(phd.Content)));

        Assert.Multiple(() =>
        {
            Assert.That(resultApdu.At, Is.EqualTo(0x0000e700));
            Assert.That(resultApdu.Payload, Is.TypeOf<DataApdu>());

            var data = (DataApdu)resultApdu.Payload;
            Assert.That(data.Payload, Is.TypeOf<SegmentInfoList>());

            var segmentInfo = (SegmentInfoList)data.Payload;
            Assert.That(segmentInfo.Items.Count, Is.EqualTo(1));
            Assert.That(segmentInfo.Items[0].Items.Count, Is.EqualTo(6));
        });
    }

    [Test]
    public void Data_Apdu_Dose_List_Parsing()
    {
        var phd = PhdPacket.FromBinaryReader(new BinaryReader(new MemoryStream(apduDoseListBytes)));
        var resultApdu = Apdu.FromBinaryReader(new BinaryReader(new MemoryStream(phd.Content)));

        Assert.Multiple(() =>
        {
            Assert.That(resultApdu.At, Is.EqualTo(0x0000e700));
            Assert.That(resultApdu.Payload, Is.TypeOf<DataApdu>());

            var data = (DataApdu)resultApdu.Payload;
            Assert.That(data.Payload, Is.TypeOf<EventReport>());

            var report = (EventReport)data.Payload;
            Assert.That(report.InsulinDoses.Count, Is.EqualTo(18));
            Assert.That(report.InsulinDoses[0].Units, Is.EqualTo(420));
        });
    }

    [Test]
    public void Data_Apdu_Event_Report_Parsing()
    {
        var phd = PhdPacket.FromBinaryReader(new BinaryReader(new MemoryStream(adpuEventReportBytes)));
        var resultApdu = Apdu.FromBinaryReader(new BinaryReader(new MemoryStream(phd.Content)));

        Assert.Multiple(() =>
        {
            Assert.That(resultApdu.At, Is.EqualTo(0x0000e700));
            Assert.That(resultApdu.Payload, Is.TypeOf<DataApdu>());

            var data = (DataApdu)resultApdu.Payload;
            Assert.That(data.Payload, Is.TypeOf<EventReport>());

            var eventReport = (EventReport)data.Payload;
            Assert.That(eventReport.Configuration, Is.Not.Null);
            Assert.That(eventReport.Configuration.Id, Is.EqualTo(16394));
            Assert.That(eventReport.Configuration.NbSegment, Is.EqualTo(1));
            Assert.That(eventReport.Configuration.TotalEntries, Is.EqualTo(273));
            Assert.That(eventReport.Configuration.UnitCode, Is.EqualTo(5472));
            Assert.That(eventReport.Configuration.TotalStorage, Is.EqualTo(800));
        });
    }

    [Test]
    public void Specification_Parsing()
    {
        var phd = PhdPacket.FromBinaryReader(new BinaryReader(new MemoryStream(adpuSpecificationBytes)));
        var resultApdu = Apdu.FromBinaryReader(new BinaryReader(new MemoryStream(phd.Content)));

        Assert.Multiple(() =>
        {
            Assert.That(resultApdu.At, Is.EqualTo(0x0000e700));
            Assert.That(resultApdu.Payload, Is.TypeOf<DataApdu>());

            var data = (DataApdu)resultApdu.Payload;
            Assert.That(data.Payload, Is.TypeOf<FullSpecification>());

            var fullSpec = (FullSpecification)data.Payload;
            Assert.That(fullSpec.Specification, Is.Not.Null);
            Assert.That(fullSpec.Specification.Serial, Is.EqualTo("ABGPVI"));
            Assert.That(fullSpec.Specification.SoftwareRevision, Is.EqualTo("01.08.01"));
        });
    }

    [Test]
    public void Apdu_Encoding_With_Event_Request()
    {
        var expectedResult = HexToBytes(@"
            E7 00 00 16 00 14 00 00 02 01 00 0E 00 00 00 00
            00 00 0D 1C 00 04 40 0A 00 00
        ");

        using var buffer = new MemoryStream();
        using var writer = new BinaryWriter(buffer);
        writer.PutUnsignedShort(16394);
        writer.PutUnsignedShort(0);
        var eventData = buffer.ToArray();

        var eventRequest = new EventRequest(
            handle: 0,
            currentTime: 0,
            type: EventReport.MdcNotiConfig,
            data: eventData
        );

        var dataApdu = new DataApdu(
            invokeId: 0,
            dChoice: 0x0201,
            payload: eventRequest
        );

        var apdu = new Apdu(
            at: Apdu.Prst,
            payload: dataApdu
        );

        var resultBytes = apdu.ToByteArray();

        Assert.That(resultBytes, Is.EqualTo(expectedResult));
    }

    [Test]
    public void Arguments_Simple()
    {
        var expectedResult = HexToBytes(@"
            E7 00 00 0E 00 0C 00 01 01 03 00 06 00 37 00 00
            00 00
        ");

        var dataApdu = new DataApdu(
            invokeId: 1,
            dChoice: 0x0103,
            payload: new ArgumentsSimple(handle: 55)
        );

        var apdu = new Apdu(
            at: Apdu.Prst,
            payload: dataApdu
        );

        var resultBytes = apdu.ToByteArray();

        Assert.That(resultBytes, Is.EqualTo(expectedResult));
    }


    private byte[] HexToBytes(string hex)
    {
        return string.Concat(hex.Where(c => !char.IsWhiteSpace(c)))
            .Chunk(2)
            .Select(pair => Convert.ToByte(new string(pair), 16))
            .ToArray();
    }
}