using Jc.OpenNov.Buffers;
using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public sealed class EventReport : Encodable
{
    public const int MdcNotiConfig = 3356;
    public const int MdcNotiSegmentData = 3361;

    public ushort Handle { get; }
    public uint RelativeTime { get; }
    public ushort EventType { get; }

    public Configuration? Configuration { get; }
    public short Instance { get; }
    public int Index { get; }
    public List<InsulinDose> InsulinDoses { get; }

    public EventReport(
        ushort handle,
        uint relativeTime,
        ushort eventType,
        Configuration? configuration = null,
        short instance = -1,
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

        Field(() => Handle, WriteUnsignedShort, SizeOf);
        Field(() => RelativeTime, WriteUnsignedInt, SizeOf);
        Field(() => EventType, WriteUnsignedShort, SizeOf);

        if (EventType == MdcNotiSegmentData)
        {
            Field(() => Instance, WriteShort, SizeOf);
            Field(() => Index, WriteInt, SizeOf);
            Field(() => (int)InsulinDoses.Count, WriteInt, SizeOf);
            Field(() => (short)0, WriteShort, SizeOf); // status placeholder
            Field(() => (short)0, WriteShort, SizeOf); // bcount placeholder

            foreach (var dose in InsulinDoses)
            {
                Field(() => dose.GetEncodedSize(), (writer, _) => { dose.WriteTo(writer); }, len => len);
            }
        }
        else if (EventType == MdcNotiConfig && Configuration != null)
        {
            Field(() => Configuration!.GetEncodedSize(), (writer, _) =>
            {
                writer.PutUnsignedShort((ushort)Configuration.GetEncodedSize());
                Configuration.WriteTo(writer);
            }, len => 2 + len);
        }
    }

    public static EventReport ReadFrom(BinaryReader reader)
    {
        var handle = reader.GetUnsignedShort();
        var relativeTime = reader.GetUnsignedInt();
        var eventType = reader.GetUnsignedShort();
        _ = reader.GetUnsignedShort(); // len

        if (eventType == MdcNotiSegmentData)
        {
            var instance = reader.GetUnsignedShort();
            var index = (int)reader.GetUnsignedInt();
            var count = reader.GetUnsignedInt();
            _ = reader.GetUnsignedShort(); // status
            _ = reader.GetUnsignedShort(); // bcount

            var doses = new List<InsulinDose>();
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            for (var i = 0; i < count; i++)
            {
                var dose = InsulinDose.ReadFrom(reader);
                dose = dose.WithUtcTime(relativeTime, currentTime);
                doses.Add(dose);
            }

            return new EventReport(handle, relativeTime, eventType, null, (short)instance, index, doses);
        }
        else if (eventType == MdcNotiConfig)
        {
            var config = Configuration.ReadFrom(reader);
            return new EventReport(handle, relativeTime, eventType, config);
        }

        return new EventReport(handle, relativeTime, eventType);
    }
}