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

    public void OnDataSent(byte[] data) => SentData.Add(data);

    public void OnDataReceived(byte[] data) => ReceivedData.Add(data);
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
                    var hexLine = ParseHexDumpLine(line);
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
            writer.WriteLine(DumpHex(bytes));
        }
    }

    public void Clear()
    {
        _byteList.Clear();
    }
    
    private static readonly Regex HexDumpLineRegex = new(@"^([0-9a-fA-F]{4})\s+((?:[0-9a-fA-F]{2}\s+)+)", RegexOptions.Compiled);

    // You need to implement or translate these two methods accordingly
    private static HexDumpLine ParseHexDumpLine(string line)
    {
        // Trim just in case
        line = line.Trim();

        if (line.Length < 5)
            throw new ArgumentException("Line too short");

        var offsetStr = line.Substring(0, 4);
        if (!int.TryParse(offsetStr, NumberStyles.HexNumber, null, out var index))
            throw new ArgumentException("Invalid offset");

        // Split on whitespace but skip the first token (offset)
        var tokens = line.Substring(4).Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        var byteList = new List<byte>();
        foreach (var token in tokens)
        {
            if (token.Length != 2) break; // Stop before ASCII part
            if (byte.TryParse(token, NumberStyles.HexNumber, null, out var b))
                byteList.Add(b);
            else
                break;
        }

        if (byteList.Count == 0)
            throw new ArgumentException($"No bytes parsed from line: {line}");

        return new HexDumpLine { Index = index, Data = byteList.ToArray() };
    }

    private static string DumpHex(byte[] bytes)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            if (i % 16 == 0)
            {
                if (i > 0) sb.AppendLine();
                sb.Append(i.ToString("X4")).Append("   ");
            }
            sb.Append(bytes[i].ToString("X2")).Append(" ");
        }
        return sb.ToString().TrimEnd();
    }

    private class HexDumpLine
    {
        public int Index { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}