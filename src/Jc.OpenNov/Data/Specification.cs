using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Data;

public sealed class Specification
{
    public string Serial { get; }
    public string PartNumber { get; }
    public string HardwareRevision { get; }
    public string SoftwareRevision { get; }

    public Specification(
        string serial = "",
        string partNumber = "",
        string hardwareRevision = "",
        string softwareRevision = "")
    {
        Serial = serial;
        PartNumber = partNumber;
        HardwareRevision = hardwareRevision;
        SoftwareRevision = softwareRevision;
    }

    private const int SerialNumber = 1;
    private const int PartNumberVersion = 2;
    private const int HwVersion = 3;
    private const int SwVersion = 4;

    public static Specification FromByteBuffer(BinaryReader reader)
    {
        var serial = "";
        var partNumber = "";
        var hardwareRevision = "";
        var softwareRevision = "";

        int count = reader.GetUnsignedShort();
        reader.GetUnsignedShort(); // size

        for (var i = 0; i < count; i++)
        {
            int type = reader.GetUnsignedShort();
            reader.GetUnsignedShort(); // component
            var value = reader.GetIndexedString();

            switch (type)
            {
                case SerialNumber:
                    serial = value;
                    break;
                case PartNumberVersion:
                    partNumber = value;
                    break;
                case HwVersion:
                    hardwareRevision = value;
                    break;
                case SwVersion:
                    softwareRevision = value;
                    break;
            }
        }

        return new Specification(serial, partNumber, hardwareRevision, softwareRevision);
    }
}