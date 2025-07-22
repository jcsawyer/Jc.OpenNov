namespace Jc.OpenNov.Data;

public class Encodable : IEncodable
{
    protected readonly List<IEncodableField> Fields = [];

    protected void Field<T>(Func<T> getter, Action<BinaryWriter, T> serializer, Func<T, int> sizeFunc)
    {
        Fields.Add(new EncodableField<T>(getter, serializer, sizeFunc));
    }

    public virtual int GetEncodedSize() => Fields
        .Sum(field => (int)field.GetSize());

    public virtual void WriteTo(BinaryWriter writer)
    {
        foreach (var field in Fields)
        {
            field.Write(writer);
        }
    }

    public virtual byte[] ToByteArray()
    {
        var size = GetEncodedSize();
        var buffer = new byte[size];
        using var writer = new BinaryWriter(new MemoryStream(buffer));
        WriteTo(writer);
        return buffer;
    }
}