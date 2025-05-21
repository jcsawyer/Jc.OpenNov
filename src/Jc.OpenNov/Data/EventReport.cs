using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public sealed class EventReport : Encodable
{
    public const int MDC_NOTI_CONFIG = 3356;
    public const int MDC_NOTI_SEGMENT_DATA = 3361;

    public short Handle { get; }
    public int RelativeTime { get; }
    public short EventType { get; }

    public Configuration? Configuration { get; }
    public int Instance { get; }
    public int Index { get; }
    public List<InsulinDose> InsulinDoses { get; }

    public EventReport(
        short handle,
        int relativeTime,
        short eventType,
        Configuration? configuration = null,
        int instance = -1,
        int index = -1,
        List<InsulinDose>? insulinDoses = null)
    {
        Handle = handle;
        RelativeTime = relativeTime;
        EventType = eventType;
        Configuration = configuration;
        Instance = instance;
        Index = index;
        InsulinDoses = insulinDoses ?? new List<InsulinDose>();

        Field(() => Handle, WriteShort, SizeOf);
        Field(() => RelativeTime, WriteInt, SizeOf);
        Field(() => EventType, WriteShort, SizeOf);

        if (EventType == MDC_NOTI_SEGMENT_DATA)
        {
            Field(() => Instance, WriteInt, SizeOf);
            Field(() => Index, WriteInt, SizeOf);
            Field(() => (int)InsulinDoses.Count, WriteInt, SizeOf);
            Field(() => (short)0, WriteShort, SizeOf); // status placeholder
            Field(() => (short)0, WriteShort, SizeOf); // bcount placeholder

            foreach (var dose in InsulinDoses)
            {
                Field(() => dose.GetEncodedSize(), (writer, _) => { dose.WriteTo(writer); }, len => len);
            }
        }
        else if (EventType == MDC_NOTI_CONFIG && Configuration != null)
        {
            Field(() => Configuration!.GetEncodedSize(), (writer, _) =>
            {
                writer.Write((short)Configuration.GetEncodedSize());
                Configuration.WriteTo(writer);
            }, len => 2 + len);
        }
    }

    public static EventReport ReadFrom(BinaryReader reader)
    {
        var handle = reader.ReadInt16();
        var relativeTime = reader.ReadInt32();
        var eventType = reader.ReadInt16();
        _ = reader.ReadInt16(); // len

        if (eventType == MDC_NOTI_SEGMENT_DATA)
        {
            var instance = reader.ReadInt16();
            var index = reader.ReadInt32();
            var count = reader.ReadInt32();
            _ = reader.ReadInt16(); // status
            _ = reader.ReadInt16(); // bcount

            var doses = new List<InsulinDose>();
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            for (var i = 0; i < count; i++)
            {
                var dose = InsulinDose.ReadFrom(reader);
                dose.WithUtcTime(relativeTime, currentTime);
                doses.Add(dose);
            }

            return new EventReport(handle, relativeTime, eventType, null, instance, index, doses);
        }
        else if (eventType == MDC_NOTI_CONFIG)
        {
            var config = Configuration.ReadFrom(reader);
            return new EventReport(handle, relativeTime, eventType, config);
        }

        return new EventReport(handle, relativeTime, eventType);
    }
}