using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Data;

public class TriggerSegmentDataXfer : Encodable
{
    public int SegmentId { get; }
    public int ResponseCode { get; }

    public TriggerSegmentDataXfer(int segmentId, int responseCode)
    {
        SegmentId = segmentId;
        ResponseCode = responseCode;
    }

    public static TriggerSegmentDataXfer FromByteBuffer(BinaryReader reader)
    {
        int segmentId = reader.GetUnsignedShort();
        int responseCode = reader.GetUnsignedShort();

        return new TriggerSegmentDataXfer(segmentId, responseCode);
    }

    public bool IsOkay()
    {
        return SegmentId != 0 && ResponseCode == 0;
    }
}