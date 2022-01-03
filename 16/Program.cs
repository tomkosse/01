using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _16
{
    public abstract class Packet
    {
        public int Version { get; }
        public int Type { get; }

        public Packet(int version, int type)
        {
            Version = version;
            Type = type;            
        }
        public abstract long GetValue();        
        public abstract int GetVersionSum();
    }

    public class OperatorPacket : Packet
    {
        static Func<long[], long>[] operations = new Func<long[], long>[]
        {
            (longArr) => longArr.Sum(),
            (longArr) => longArr.Aggregate((long)1, (acc, el) => acc * el),
            (longArr) => longArr.Min(),
            (longArr) => longArr.Max(),
            (longArr) => throw new Exception("RESERVED"),
            (longArr) => longArr[0] > longArr[1] ? 1 : 0,
            (longArr) => longArr[0] < longArr[1] ? 1 : 0,
            (longArr) => longArr[0] == longArr[1] ? 1 : 0,
        };
        public Packet[] SubPackets { get; }
        public OperatorPacket(int version, int type, Packet[] subPackets) : base(version, type)
        {
            this.SubPackets = subPackets;
        }

        public override long GetValue()
        {
            return operations[Type](SubPackets.Select(sp => sp.GetValue()).ToArray());
        }
        public override int GetVersionSum()
        {
            return SubPackets.Sum(sp => sp.GetVersionSum()) + Version;
        }
    }

    public class LiteralPacket : Packet
    {
        private readonly long value;
        public LiteralPacket(int version, int type, long value) : base(version, type)
        {
            this.value = value;
        }
        public override long GetValue()
        {
            return value;
        }
        public override int GetVersionSum()
        {
            return Version;
        }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            var hexLine = File.ReadAllLines(args[0])[0];
            var sw = Stopwatch.StartNew();
            var bitstring = string.Join("", Convert.FromHexString(hexLine).Select(b => Convert.ToString((byte)(b >> 4 & 0xF), 2).PadLeft(4, '0') + Convert.ToString((byte)(b & 0xF), 2).PadLeft(4, '0')));
            
            var rootPacket = ParsePackets(bitstring, out int _);
            int versionSum = rootPacket.GetVersionSum();
            long value = rootPacket.GetValue();
            sw.Stop();
            System.Console.WriteLine($"Part 1: {versionSum}");
            System.Console.WriteLine($"Part 2: {value}");
            System.Console.WriteLine($"Done in {sw.ElapsedMilliseconds}ms");
        }

        private static Packet ParsePackets(string inputString, out int charsRead)
        {
            var packetVersion = Convert.ToInt32(inputString.Substring(0, 3), 2);
            var packetType = Convert.ToInt32(inputString.Substring(3, 3), 2);
            var payload = inputString.Substring(6);
            if (packetType == 4)
            {
                var literal = CreateLiteralPacket(packetVersion, packetType, payload, out int lpCharsRead);
                charsRead = lpCharsRead + 6;
                return literal;
            }
            else
            {
                var remainder = payload;
                bool isTotalLength = payload.First() == '0';
                remainder = payload.Substring(1);
                List<Packet> subPackets = new List<Packet>();
                if (isTotalLength)
                {
                    var totalLength = Convert.ToInt32(remainder.Substring(0, 15), 2);
                    remainder = remainder.Substring(15);
                    int localCharactersUsed = 0;
                    while (localCharactersUsed < totalLength)
                    {
                        subPackets.Add(ParsePackets(remainder.Substring(localCharactersUsed, totalLength - localCharactersUsed), out int localCharsRead));
                        localCharactersUsed += localCharsRead;
                    }
                    charsRead = localCharactersUsed + 1 + 15 + 6;
                }
                else
                {
                    var numberOfSubpackets = Convert.ToInt32(remainder.Substring(0, 11), 2);
                    remainder = remainder.Substring(11);
                    int localCharactersUsed = 0;
                    for (int i = 0; i < numberOfSubpackets; i++)
                    {
                        subPackets.Add(ParsePackets(remainder.Substring(localCharactersUsed), out int localCharsRead));
                        localCharactersUsed += localCharsRead;
                    }
                    charsRead = localCharactersUsed + 1 + 11 + 6;
                }
                var op = new OperatorPacket(packetVersion, packetType, subPackets.ToArray());
                return op;
            }
        }

        private static LiteralPacket CreateLiteralPacket(int version, int type, string remainder, out int charsRead)
        {
            int read = 0;
            var unpackedPayload = "";
            bool keepReading = true;
            while (keepReading)
            {
                keepReading = remainder[0] == '1';
                unpackedPayload += remainder.Substring(1, 4);
                remainder = remainder.Substring(5);
                read += 5;
            }
            charsRead = read;
            return new LiteralPacket(version, type, Convert.ToInt64(unpackedPayload, 2));
        }
    }
}