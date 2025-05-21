using static Jc.OpenNov.Utilities.EncodableExtensions;

using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Data;

public sealed class InsulinDose : Encodable
{
    public long Time { get; }
    public int Units { get; }
    public int Flags { get; }

    public InsulinDose(long time, int units, int flags)
    {
        Time = time;
        Units = units;
        Flags = flags;
        
        Field(() => Time, WriteLong, SizeOf);
        Field(() => Units, WriteInt, SizeOf);
        Field(() => Flags, WriteInt, SizeOf);
    }

    public static InsulinDose ReadFrom(BinaryReader reader)
    {
        var relativeTime = reader.GetUnsignedInt();
        var units = (int)(reader.GetUnsignedInt() & 0xFFFF);
        var flags = (int)reader.GetUnsignedInt();

        return new InsulinDose(relativeTime, units, flags);
    }

    public InsulinDose WithUtcTime(uint relativeTime, long currentTime)
    {
        var correctedTime = currentTime - ((relativeTime - Time) * 1000L);
        return new InsulinDose(correctedTime, Units, Flags);
    }
}