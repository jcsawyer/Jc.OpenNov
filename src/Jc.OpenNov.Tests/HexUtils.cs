using System.Globalization;
using System.Text;

namespace Jc.OpenNov.Tests;

public static class HexUtils
    {
        private static readonly char[] HexDigits = {
            '0', '1', '2', '3', '4', '5', '6', '7',
            '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        private const int LineSize = 16;

        public static string DumpHex(this byte[] array) => DumpHexString(array);

        public static string DumpHexString(byte[]? array)
        {
            return array == null ? "<null>" : DumpHex(array, 0, array.Length);
        }

        public static string DumpHex(byte[]? array, int offset, int length)
        {
            if (array == null) return "<null>";
            var result = new StringBuilder();
            var line = new byte[LineSize];
            var lineIndex = 0;

            result.Append("\n0x").Append(ToHexString(offset));

            for (int i = offset; i < offset + length; i++)
            {
                if (lineIndex == LineSize)
                {
                    result.Append(" ");
                    for (int j = 0; j < LineSize; j++)
                    {
                        byte b = line[j];
                        result.Append((b > 0x20 && b < 0x7E) ? (char)b : '.');
                    }
                    result.Append("\n0x").Append(ToHexString(i));
                    lineIndex = 0;
                }

                byte b2 = array[i];
                result.Append(" ")
                      .Append(HexDigits[(b2 >> 4) & 0x0F])
                      .Append(HexDigits[b2 & 0x0F]);
                line[lineIndex++] = b2;
            }

            if (lineIndex != LineSize)
            {
                int count = (LineSize - lineIndex) * 3 + 1;
                result.Append(' ', count);
                for (int i = 0; i < lineIndex; i++)
                {
                    byte b = line[i];
                    result.Append((b > 0x20 && b < 0x7E) ? (char)b : '.');
                }
            }

            return result.ToString();
        }

        public static string ToHexString(byte[] array, int offset = 0, int length = -1)
        {
            if (length == -1) length = array.Length;
            var buf = new char[length * 2];
            int bufIndex = 0;

            for (int i = offset; i < offset + length; i++)
            {
                byte b = array[i];
                buf[bufIndex++] = HexDigits[(b >> 4) & 0x0F];
                buf[bufIndex++] = HexDigits[b & 0x0F];
            }

            return new string(buf);
        }

        public static string ToHexString(int i)
        {
            return ToHexString(ToByteArray(i));
        }

        public static byte[] ToByteArray(int i)
        {
            return new byte[]
            {
                (byte)((i >> 24) & 0xFF),
                (byte)((i >> 16) & 0xFF),
                (byte)((i >> 8) & 0xFF),
                (byte)(i & 0xFF)
            };
        }

        private static int ToByte(char c)
        {
            if (c is >= '0' and <= '9') return c - '0';
            if (c is >= 'A' and <= 'F') return c - 'A' + 10;
            if (c is >= 'a' and <= 'f') return c - 'a' + 10;
            throw new ArgumentException($"Invalid hex char '{c}'");
        }

        public static byte[] StringToByteArray(string hexString)
        {
            return Enumerable.Range(0, hexString.Length / 2)
                .Select(i => Convert.ToByte(hexString.Substring(i * 2, 2), 16))
                .ToArray();
        }

        public static string RemoveMultiple(this string input, params string[] items)
        {
            return items.Aggregate(input, (current, item) => current.Replace(item, string.Empty));
        }

        public static byte[] HexToByteArray(this string input)
        {
            return StringToByteArray(input.RemoveMultiple("\n", " ", "\t"));
        }

        public record HexDumpLine(int Index, byte[] Data);

        public static HexDumpLine ParseHexDumpLine(string input)
        {
            int len = (input.Length - 11) / 4;
            if (len < 0 || len > 16 || !input.StartsWith("0x"))
                throw new ArgumentException("Wrong hexdump input");

            int index = int.Parse(input.Substring(2, 8), NumberStyles.HexNumber);
            var data = new List<byte>();

            for (int i = 0; i < len; i++)
            {
                try
                {
                    string num = input.Substring(11 + i * 3, 2);
                    if (!string.IsNullOrWhiteSpace(num))
                        data.Add(Convert.ToByte(num, 16));
                }
                catch (FormatException) { }
            }

            return new HexDumpLine(index, data.ToArray());
        }
    }