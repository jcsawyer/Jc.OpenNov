using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public sealed class ConfirmedAction : Encodable
{
    public const int StoreHandle = 0x0100;
    public const int AllSegments = 0x0001;

    public ushort Handle { get; }
    public ushort Type { get; }
    public byte[] Bytes { get; }

    private ConfirmedAction(ushort handle, ushort type, byte[] bytes)
    {
        Handle = handle;
        Type = type;
        Bytes = bytes;

        Field(() => Handle, WriteUnsignedShort, SizeOf);
        Field(() => Type, WriteUnsignedShort, SizeOf);
        Field(() => Bytes, WriteByteArray, SizeOf);
    }

    public static ConfirmedAction AllSegment(ushort handle, ushort type)
    {
        return new ConfirmedAction(handle, type, [0, 1, 0, 2, 0, 0]);
    }

    public static ConfirmedAction Segment(ushort handle, ushort type, ushort segment)
    {
        byte[] bytes;
        using (var ms = new MemoryStream())
        using (var writer = new BinaryWriter(ms))
        {
            WriteUnsignedShort(writer, segment);
            bytes = ms.ToArray();
        }

        return new ConfirmedAction(handle, type, bytes);
    }
}