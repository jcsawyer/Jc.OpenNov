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
    }

    public static InsulinDose ReadFrom(BinaryReader reader)
    {
        uint relativeTime = reader.ReadUInt32();
        int units = (int)(reader.ReadUInt32() & 0xFFFF);
        int flags = (int)reader.ReadUInt32();

        return new InsulinDose(relativeTime, units, flags);
    }

    public InsulinDose WithUtcTime(int relativeTime, long currentTime)
    {
        long correctedTime = currentTime - ((relativeTime - Time) * 1000L);
        return new InsulinDose(correctedTime, Units, Flags);
    }
}