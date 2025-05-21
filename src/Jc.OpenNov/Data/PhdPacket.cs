using Jc.OpenNov.Buffers;

namespace Jc.OpenNov.Data;

public sealed class PhdPacket
{
    private const byte Mb = 1 << 7;
    private const byte Me = 1 << 6;
    private const byte Cf = 1 << 5;
    private const byte Sr = 1 << 4;
    private const byte Il = 1 << 3;

    private const byte WellKnown = 0x01;

    public byte OpCode { get; }
    public int TypeLen { get; }
    public int PayloadLen { get; }
    public int? HeaderLen { get; }
    public byte[] Header { get; }
    public int Seq { get; }
    public int Chk { get; }
    public byte[] Content { get; }

    public PhdPacket(
        byte opCode,
        int typeLen,
        int payloadLen,
        int? headerLen,
        byte[] header,
        int seq,
        int chk,
        byte[] content)
    {
        OpCode = opCode;
        TypeLen = typeLen;
        PayloadLen = payloadLen;
        HeaderLen = headerLen;
        Header = header;
        Seq = seq;
        Chk = chk;
        Content = content;
    }

    public static PhdPacket FromBinaryReader(BinaryReader reader)
    {
        byte opcode = reader.ReadByte();
        int typeLen = reader.ReadByte();
        int payloadLen = reader.ReadByte() - 1;

        bool hasId = (opcode & Il) != 0;
        int headerLen = hasId ? reader.ReadByte() : 0;

        byte[] protoId = reader.GetBytes(3);
        byte[] header = hasId ? reader.ReadBytes(headerLen) : null;

        int chk = reader.ReadByte();
        int remaining = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        int realLen = Math.Min(remaining, payloadLen);
        byte[] content = reader.ReadBytes(realLen);

        return new PhdPacket(
            opcode, typeLen, realLen,
            hasId ? headerLen : (int?)null,
            header, chk & 0x0F, chk, content
        );
    }

    public byte[] ToByteArray()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        bool hasId = Header != null && Header.Length > 0;
        byte flags = (byte)(Mb | Me | Sr | (hasId ? Il : 0) | WellKnown);

        writer.Write(flags);
        writer.Write((byte)3); // TypeLen fixed
        writer.Write((byte)(Content.Length + 1));

        if (hasId)
        {
            writer.Write((byte)Header.Length);
        }

        writer.Write("PHD"u8.ToArray());

        if (hasId)
        {
            writer.Write(Header);
        }

        writer.Write((byte)((Seq & 0x0F) | 0x80 | Chk));

        if (Content.Length > 0)
        {
            writer.Write(Content);
        }

        return ms.ToArray();
    }
}