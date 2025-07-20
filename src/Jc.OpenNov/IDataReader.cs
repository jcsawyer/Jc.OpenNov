using System.Buffers.Binary;

namespace Jc.OpenNov;

public interface IDataReader
{
    byte[]? ReadData(byte[] input);
    void DataSent(byte[] data);
    void DataReceived(byte[] data);
}