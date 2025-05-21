namespace Jc.OpenNov.Data;

public interface IEncodable
{
    int GetEncodedSize();
    void WriteTo(BinaryWriter writer);
}