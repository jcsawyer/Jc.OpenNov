using static Jc.OpenNov.Utilities.EncodableExtensions;

namespace Jc.OpenNov.Data;

public sealed class AResponse : Encodable
{
    public ushort Result { get; }
    public ushort Protocol { get; }
    public ApoepElement Apoep { get; }

    public AResponse(ushort result, ushort protocol, ApoepElement apoep)
    {
        apoep.RecMode = 0;
        apoep.ConfigId = 0;
        apoep.SystemType = ApoepElement.SysTypeManager;
        apoep.ListCount = 0;
        apoep.ListLen = 0;

        Result = result;
        Protocol = protocol;
        Apoep = apoep;

        Field(() => Result, WriteUnsignedShort, SizeOf);
        Field(() => Protocol, WriteUnsignedShort, SizeOf);

        Field(() => Apoep.GetEncodedSize(), (w, _) =>
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            Apoep.WriteTo(bw);
            var data = ms.ToArray();

            WriteShort(w, (short)data.Length);
            w.Write(data);
        }, len => 2 + len);
    }

}