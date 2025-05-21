using System.Buffers.Binary;

namespace Jc.OpenNov;

public interface IDataReader
{
    byte[] ReadData(byte[] input);
    void OnDataSent(byte[] data);
    void OnDataReceived(byte[] data);
}