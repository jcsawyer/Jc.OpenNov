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

    public byte OpCode { get; init; }
    public int TypeLen { get; init; }
    public int PayloadLen { get; init; }
    public int? HeaderLen { get; init; }
    public byte[] Header { get; init; }
    public int Seq { get; init; }
    public int Chk { get; init; }
    public byte[] Content { get; init; }

    public PhdPacket(int seq, byte[] data)
    {
        Seq = seq;
        Content = data;
    }

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
        var opcode = reader.ReadByte();
        int typeLen = reader.ReadByte();
        var payloadLen = reader.ReadByte() - 1;

        var hasId = (opcode & Il) != 0;
        var headerLen = hasId ? reader.ReadByte() : 0;

        var protoId = reader.GetBytes(3);
        var header = hasId ? reader.ReadBytes(headerLen) : null;

        int chk = reader.ReadByte();
        var remaining = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        var realLen = Math.Min(remaining, payloadLen);
        var content = reader.ReadBytes(realLen);

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

        var hasId = Header != null && Header.Length > 0;
        var flags = (byte)(Mb | Me | Sr | (hasId ? Il : 0) | WellKnown);

        writer.PutByte(flags);
        writer.PutByte(3); // TypeLen fixed
        writer.PutByte((byte)(Content.Length + 1));

        if (hasId)
        {
            writer.PutByte((byte)Header.Length);
        }
        
        if (hasId)
        {
            writer.Write(Header);
        }

        writer.Write("PHD"u8.ToArray());

        writer.PutByte((byte)((Seq & 0x0F) | 0x80 | Chk));

        if (Content.Length > 0)
        {
            writer.Write(Content);
        }

        return ms.ToArray();
    }
}