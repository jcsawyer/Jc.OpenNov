using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public sealed class ArgumentsSimple : Encodable
{
    public ushort Handle { get; }
    public ushort Size { get; }
    public ushort Size2 { get; }

    public ArgumentsSimple(ushort handle, ushort size = 0, ushort size2 = 0)
    {
        Handle = handle;
        Size = size;
        Size2 = size2;

        Field(() => Handle, WriteUnsignedShort, SizeOf);
        Field(() => Size, WriteUnsignedShort, SizeOf);
        Field(() => Size2, WriteUnsignedShort, SizeOf);
    }
}