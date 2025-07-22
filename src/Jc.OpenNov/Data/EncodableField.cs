namespace Jc.OpenNov.Data;

public class EncodableField<T> : IEncodableField
{
    public Func<T> Getter { get; }
    public Action<BinaryWriter, T> Serializer { get; }
    public Func<T, int> SizeFunc { get; }

    public EncodableField(Func<T> getter, Action<BinaryWriter, T> serializer, Func<T, int> sizeFunc)
    {
        Getter = getter;
        Serializer = serializer;
        SizeFunc = sizeFunc;
    }

    public int GetSize() => SizeFunc(Getter());
    public void Write(BinaryWriter writer) => Serializer(writer, Getter());
}