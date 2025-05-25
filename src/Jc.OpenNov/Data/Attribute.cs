using Jc.OpenNov.Utilities;
using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public sealed class Attribute : Encodable
{
    public const int AttrSysId = 2436;
    public const int AttrIdInstno = 2338;
    public const int AttrIdModel = 2344;
    public const int AttrIdProdSpecn = 2349;
    public const int AttrIdType = 2351;
    public const int AttrMetricStoreCapacCnt = 2369;
    public const int AttrMetricStoreSampleAlg = 2371;
    public const int AttrMetricStoreUsageCnt = 2372;
    public const int AttrNumSeg = 2385;
    public const int AttrOpStat = 2387;
    public const int AttrSegUsageCnt = 2427;
    public const int AttrTimeRel = 2447;
    public const int AttrUnitCode = 2454;
    public const int AttrDevConfigId = 2628;
    public const int AttrMdsTimeInfo = 2629;
    public const int AttrMetricSpecSmall = 2630;
    public const int AttrRegCertDataList = 2635;
    public const int AttrPmStoreCapab = 2637;
    public const int AttrPmSegMap = 2638;
    public const int AttrAttributeValMap = 2645;
    public const int AttrNuValObsSimp = 2646;
    public const int AttrPmStoreLabelString = 2647;
    public const int AttrPmSegLabelString = 2648;
    public const int AttrSysTypeSpecList = 2650;
    public const int AttrClearTimeout = 2659;
    public const int AttrTransferTimeout = 2660;
    public const int AttrEnumObsValBasicBitStr = 2662;

    public int Type { get; }
    public byte[] Data { get; }
    public int Value { get; }

    public Attribute(int type, byte[] data, int value)
    {
        Type = type;
        Data = data;
        Value = value;
        
        Field(() => Type, WriteInt, SizeOf);
        Field(() => Data.Length, WriteInt, SizeOf);
        Field(() => Data, (w, d) => w.Write(d), len => Data.Length);
    }

    public static Attribute ReadFrom(BinaryReader reader)
    {
        int type = reader.GetUnsignedShort();
        int len = reader.GetUnsignedShort();
        var data = reader.ReadBytes(len);

        var value = -1;
        if (data.Length == 2)
        {
            value = (data[0] << 8) | data[1];
        }
        else if (data.Length == 4)
        {
            value = (int)((uint)(data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3]));
        }

        return new Attribute(type, data, value);
    }


    public BinaryReader Wrap()
    {
        return new BinaryReader(new MemoryStream(Data));
    }
}