namespace Jc.OpenNov.Data;

public interface IEncodableField
{
    int GetSize();
    void Write(BinaryWriter writer);
}