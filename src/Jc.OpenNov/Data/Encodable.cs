namespace Jc.OpenNov.Data;

public abstract class Encodable : IEncodable
{
    protected readonly List<object> _fields = new();

    protected void Field<T>(Func<T> getter, Action<BinaryWriter, T> serializer, Func<T, int> sizeFunc)
    {
        _fields.Add(new EncodableField<T>(getter, serializer, sizeFunc));
    }

    public virtual int GetEncodedSize() => _fields
        .Cast<dynamic>()
        .Sum(field => (int)field.GetSize());

    public virtual void WriteTo(BinaryWriter writer)
    {
        foreach (dynamic field in _fields)
        {
            field.Write(writer);
        }
    }
}