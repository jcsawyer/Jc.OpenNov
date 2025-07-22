namespace Jc.OpenNov;

public interface IDataReader
{
    Task<byte[]?> ReadDataAsync(byte[] input);
    void DataSent(byte[] data);
    void DataReceived(byte[] data);
}