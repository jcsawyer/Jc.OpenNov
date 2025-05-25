using System.Diagnostics;

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
        OpCode = unchecked((byte)-1);
        TypeLen = -1;
        PayloadLen = -1;
        HeaderLen = null;
        Header = null;
        Chk = 0;
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

        _ = reader.ReadBytes(3); // proto id
        var header = hasId ? reader.ReadBytes(headerLen) : null;

        int chk = reader.ReadByte();
        var remaining = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        var realLen = Math.Min(remaining, payloadLen);
        var content = reader.ReadBytes(realLen);

        return new PhdPacket(
            opcode, typeLen, realLen,
            hasId ? headerLen : null,
            header, chk & 0x0F, chk, content
        );
    }

    public byte[] ToByteArray()
    {
        var iLen = Content.Length;
        var idLen = Header?.Length ?? 0;
        var hasId = idLen > 0;

        var totalLen = idLen + iLen + 7;
        var buffer = new byte[totalLen];
        var span = buffer.AsSpan();
        var offset = 0;

        var flags = (byte)(Mb | Me | Sr | (hasId ? Il : 0) | WellKnown);
        Debug.WriteLine($"Flags: ox{flags:X2}");
        span[offset++] = flags;
        span[offset++] = 3; // Type length for "PHD"
        span[offset++] = (byte)(iLen + 1); // Payload + 1 for seq/chk

        if (hasId)
        {
            Header.CopyTo(span.Slice(offset, idLen));
            offset += idLen;
        }

        // Write "PHD"
        var phdBytes = "PHD"u8;
        phdBytes.CopyTo(span.Slice(offset, 3));
        offset += 3;

        span[offset++] = (byte)(Seq & 0x0F | 0x80 | Chk);

        if (iLen > 0)
        {
            Content.CopyTo(span.Slice(offset, iLen));
        }

        return buffer;
    }
}