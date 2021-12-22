using System.Collections.Immutable;
using System.Text;

namespace AdventOfCode.Year2021.Day16
{
    public class HexSolver : ISolver
    {
        private static readonly ImmutableDictionary<char, string> HexMapping = new Dictionary<char, string>()
        {
            { '0', "0000" },
            { '1', "0001" },
            { '2', "0010" },
            { '3', "0011" },
            { '4', "0100" },
            { '5', "0101" },
            { '6', "0110" },
            { '7', "0111" },
            { '8', "1000" },
            { '9', "1001" },
            { 'A', "1010" },
            { 'B', "1011" },
            { 'C', "1100" },
            { 'D', "1101" },
            { 'E', "1110" },
            { 'F', "1111" }
        }.ToImmutableDictionary();
        private const int HeaderSize = 6;

        public void SolvePartOne(List<string> data)
        {
            char[] bitString = Parse(data).ToCharArray();
            Packet rootPacket = DecodePacket(bitString, out _);
            Console.WriteLine(rootPacket.VersionSum());
        }

        private Packet DecodePacket(char[] data, out int nextByte)
        {
            (int version, int type) = ReadHeader(data[0..HeaderSize]);
            if (type == 4)
            {
                char[] remainder = data[HeaderSize..];
                long value = ParseLiteral(remainder, out int nextByteOffset);
                if (value < 0)
                {
                    throw new InvalidDataException("NEGATIVE VALUE!");
                }

                nextByte = HeaderSize + nextByteOffset;

                return new Packet(version, type) { Value = value };
            }
            else
            {
                int lengthType = data[6] - '0';
                if (lengthType == 0)
                {
                    int numBits = Convert.ToInt32(new string(data[7..22]), 2);

                    var done = false;
                    var start = 22;
                    var end = start + numBits;
                    List<Packet> subpackets = new();
                    while (!done)
                    {
                        Packet subPacket = DecodePacket(data[start..end], out int nextSubpacketByte);
                        subpackets.Add(subPacket);
                        if (start + nextSubpacketByte == end)
                        {
                            done = true;
                        }
                        else
                        {
                            start += nextSubpacketByte;
                        }
                    }

                    nextByte = end;
                    return new Packet(version, type) { Subpackets = subpackets };
                }
                else
                {
                    int numSubpackets = Convert.ToInt32(new string(data[7..18]), 2);

                    var done = false;
                    var start = 18;
                    List<Packet> subpackets = new();
                    while (!done)
                    {
                        Packet subPacket = DecodePacket(data[start..], out int nextOffset);
                        subpackets.Add(subPacket);
                        start += nextOffset;
                        if (subpackets.Count == numSubpackets)
                        {
                            done = true;
                        }
                    }

                    nextByte = start;
                    return new Packet(version, type) { Subpackets = subpackets };
                }
            }
        }

        private static long ParseLiteral(char[] data, out int nextByteOffset)
        {
            var done = false;
            var start = 0;
            var builder = new StringBuilder();

            while (!done)
            {
                char[] slice = data[start..(start + 5)];
                builder.Append(slice[1..]);

                if (slice[0] == '0')
                {
                    done = true;
                }
                else
                {
                    start += 5;
                }
            }

            nextByteOffset = start + 5;
            return Convert.ToInt64(builder.ToString(), 2);
        }

        private static (int Version, int Type) ReadHeader(char[] header)
        {
            var version = Convert.ToInt32(new string(header[0..3]), 2);
            var type = Convert.ToInt32(new string(header[3..6]), 2);
            return (version, type);
        }

        public static string Parse(List<string> data)
        {
            return string.Concat(from character in data[0]
                                 select HexMapping[character]);
        }

        public void SolvePartTwo(List<string> data)
        {
            char[] bitString = Parse(data).ToCharArray();
            Packet rootPacket = DecodePacket(bitString, out _);
            Console.WriteLine(rootPacket.Evaluate());
        }

        internal class Packet
        {
            public int Version { get; init; }
            public int Type { get; init; }

            public long Value { get; init; }

            public List<Packet> Subpackets { get; init; }

            public Packet(int version, int type)
            {
                Version = version;
                Type = type;
                Subpackets = new();
            }

            public void AddSubpacket(Packet subPacket)
            {
                Subpackets.Add(subPacket);
            }

            public int VersionSum()
            {
                return (from packet in Subpackets
                        select packet.VersionSum()).Sum() + Version;
            }

            public long Evaluate()
            {
                if (Type == 4)
                {
                    return Value;
                }
                else if (Type == 0)
                {
                    return (from packet in Subpackets
                            select packet.Evaluate()).Sum();
                }
                else if (Type == 1)
                {
                    return (from packet in Subpackets
                            select packet.Evaluate()).Aggregate(1L, (a, c) => a * c);
                }
                else if (Type == 2)
                {
                    return (from packet in Subpackets
                            select packet.Evaluate()).Min();
                }
                else if (Type == 3)
                {
                    return (from packet in Subpackets
                            select packet.Evaluate()).Max();
                }
                else if (Type == 5)
                {
                    return Subpackets[0].Evaluate() > Subpackets[1].Evaluate() ? 1 : 0;
                }
                else if (Type == 6)
                {
                    return Subpackets[0].Evaluate() < Subpackets[1].Evaluate() ? 1 : 0;
                }
                else if (Type == 7)
                {
                    return Subpackets[0].Evaluate() == Subpackets[1].Evaluate() ? 1 : 0;
                }
                else
                {
                    throw new InvalidOperationException("Illegal type specified!");
                }
            }
        }
    }
}