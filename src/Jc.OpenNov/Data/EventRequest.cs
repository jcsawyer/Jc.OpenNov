using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public class EventRequest : Encodable
{
    public ushort Handle { get; }
    public int CurrentTime { get; }
    public ushort Type { get; }
    public byte[] Data { get; }

    public EventRequest(ushort handle, int currentTime, ushort type, byte[] data)
    {
        Handle = handle;
        CurrentTime = currentTime;
        Type = type;
        Data = data;

        Field(() => Handle, WriteUnsignedShort, SizeOf);
        Field(() => CurrentTime, WriteInt, SizeOf);
        Field(() => Type, WriteUnsignedShort, SizeOf);
        Field(() => Data, WriteByteArray, SizeOf);
    }
}