using System.Diagnostics;

namespace Jc.OpenNov.Tests;

public class TestingDataReader : IDataReader
{
    private readonly Queue<byte[]> _queue;

    public List<byte[]> SentData { get; } = [];
    public List<byte[]> ReceivedData { get; } = [];

    public TestingDataReader(Stream inputStream)
    {
        var dataStore = ByteArrayStore.FromInputStream(inputStream);
        _queue = new Queue<byte[]>(dataStore.Content);
    }

    public Task<byte[]> ReadDataAsync(byte[] input)
    {
        if (_queue.Count == 0)
            throw new EndOfStreamException("No more test data available.");
        return Task.FromResult(_queue.Dequeue());
    }

    public void DataSent(byte[] data)
    {
        Console.WriteLine("Data sent: " + BitConverter.ToString(data));
        Debug.WriteLine("Data sent: " + BitConverter.ToString(data));
        SentData.Add(data);
    }

    public void DataReceived(byte[] data)
    {
        Console.WriteLine("Data received: " + BitConverter.ToString(data));
        Debug.WriteLine("Data received: " + BitConverter.ToString(data));
        ReceivedData.Add(data);
    }
}

public class ByteArrayStore
{
    private readonly List<byte[]> _byteList = [];
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