using AndroidTag = Android.Nfc.Tag;

namespace Jc.OpenNov.Avalonia.Android;

public class Tag : ITag
{
    private readonly AndroidTag _tag;

    public Tag(AndroidTag tag)
    {
        _tag = tag;
    }
    
    public byte[]? GetId()
    {
        return _tag.GetId();
    }
}