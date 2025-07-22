using CoreNFC;

namespace Jc.OpenNov.Avalonia.iOS;

public class Tag : ITag
{
    private readonly INFCIso7816Tag _tag;

    public Tag(INFCIso7816Tag tag)
    {
        _tag = tag;
    }
    
    public byte[]? GetId()
    {
        return _tag.Identifier.ToArray();
    }
}
