using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Jc.OpenNov.Tests;

public class TestingDataReader : IDataReader
{
    private readonly Queue<byte[]> _queue;

    public List<byte[]> SentData { get; } = new();
    public List<byte[]> ReceivedData { get; } = new();

    public TestingDataReader(Stream inputStream)
    {
        var dataStore = ByteArrayStore.FromInputStream(inputStream);
        _queue = new Queue<byte[]>(dataStore.Content);
    }

    public byte[] ReadData(byte[] input)
    {
        if (_queue.Count == 0)
            throw new EndOfStreamException("No more test data available.");
        return _queue.Dequeue();
    }

    public void OnDataSent(byte[] data)
    {
        Console.WriteLine("Data sent: " + BitConverter.ToString(data));
        Debug.WriteLine("Data sent: " + BitConverter.ToString(data));
        SentData.Add(data);
    }

    public void OnDataReceived(byte[] data)
    {
        Console.WriteLine("Data received: " + BitConverter.ToString(data));
        Debug.WriteLine("Data received: " + BitConverter.ToString(data));
        ReceivedData.Add(data);
    }
}

public class ByteArrayStore
{
    private readonly List<byte[]> _byteList = new();
    public IReadOnlyList<byte[]> Content => _byteList;

    public static ByteArrayStore FromInputStream(Stream input)
    {
        var store = new ByteArrayStore();

        using var reader = new StreamReader(input);
        string? line;
        byte[]? current = null;

        while ((line = reader.ReadLine()) != null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var hexLine = HexUtils.ParseHexDumpLine(line);
                    if (hexLine.Index == 0)
                    {
                        if (current != null)
                            store.AddByteArray(current);
                        current = hexLine.Data;
                    }
                    else
                    {
                        current = current?.Concat(hexLine.Data).ToArray();
                    }
                }
            }
            catch (ArgumentException)
            {
                // skip invalid lines
            }
        }

        if (current != null)
        {
            store.AddByteArray(current);
        }

        return store;
    }

    public void AddByteArray(byte[] byteArray)
    {
        _byteList.Add(byteArray);
    }

    public void ToOutputStream(Stream output)
    {
        using var writer = new StreamWriter(output);
        foreach (var bytes in _byteList)
        {
            writer.WriteLine(HexUtils.DumpHex(bytes));
        }
    }

    public void Clear()
    {
        _byteList.Clear();
    }
}