using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _08
{
    public class Digit
    {
        public static string[] ValidDigits
        {
            get
            {
                return new string[] {
                    "abcefg", "cf", "acdeg", "acdfg", "bcdf", "abdfg", "abdefg", "acf", "abcdefg", "abcdfg"
                };
            }
        }

        private readonly string input;

        public int? Value
        {
            get
            {
                var findCorrectDigit = Enumerable.Range(0, ValidDigits.Length).Where(i => ValidDigits[i] == input);
                return findCorrectDigit.Any() ? findCorrectDigit.First() : -1;
            }
        }

        public bool IsValid
        {
            get
            {
                return ValidDigits.Contains(input);
            }
        }

        public Digit(string input, IEnumerable<(char, char)> decodingTable)
        {
            var decoded = input
                            .ToCharArray()
                            .Select(c => decodingTable.Where(combo => combo.Item1 == c).First().Item2)
                            .OrderBy(c => c)
                            .ToArray();

            this.input = new String(decoded);
        }
    }

    public class EncodedDigit
    {
        public EncodedDigit(string signalSegments)
        {
            SignalSegments = signalSegments
                                .ToCharArray()
                                .OrderBy(c => c)
                                .ToArray();
        }

        public char[] SignalSegments { get; }

        public int Length
        {
            get
            {
                return SignalSegments.Length;
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var lines = File
                .ReadAllLines(args[0]);

            var hits = new int[] { 2, 3, 4, 7 };

            var answerPart1 = 0;
            int answerPart2 = 0;
            var start = DateTime.Now;
            foreach (var line in lines)
            {
                var splitted = line.Split(" | ").ToArray();
                var signalPattern = splitted[0].Split(" ");
                var outputValues = splitted[1].Split(" ");
                answerPart1 += outputValues.Count(ov => hits.Contains(ov.Length));

                var digits = signalPattern
                                .Select(sp => new EncodedDigit(sp))
                                .OrderBy(d => d.Length)
                                .ToList();

                var possiblePairs = GenerateAllPossibleDecodingPairs(digits);

                IEnumerable<IEnumerable<(char, char)>> possibleDecodingTables = new List<IEnumerable<(char, char)>>() { new List<(char, char)>() };
                foreach (var pm in possiblePairs.GroupBy(e => e.Item1))
                {
                    possibleDecodingTables = pm.Join(possibleDecodingTables, a => 1, b => 1, (a, b) => b.Append(a));
                }

                IEnumerable<(char, char)> decodingTable = possibleDecodingTables
                                    .Where(table =>
                                        signalPattern
                                            .Select(pattern => new Digit(pattern, table))
                                            .All(digit => digit.IsValid)
                                    )
                                    .First();

                int outputValue = int.Parse(string.Join("", outputValues.Select(sp => new Digit(sp, decodingTable).Value)));
                answerPart2 += outputValue;
            }
            var end = DateTime.Now;
            System.Console.WriteLine($"Part 1: {answerPart1}                             Part 2: {answerPart2}");
            System.Console.WriteLine((end - start).TotalMilliseconds + "ms");
        }

        private static IEnumerable<(char, char)> GenerateAllPossibleDecodingPairs(List<EncodedDigit> knownDigits)
        {
            List<(char, char)> doNotGenerateFurther = new List<(char, char)>();

            List<(char, char)> possiblePairs = new List<(char, char)>();
            foreach (var digit in knownDigits)
            {
                var correctSegments = PossibleCorrectSegmentsForDigit(digit)
                                        .SelectMany(c => c)
                                        .Except(doNotGenerateFurther.Select(d => d.Item2))
                                        .ToArray();

                var sigSegs = digit.SignalSegments.Except(doNotGenerateFurther.Select(cm => cm.Item1));

                var allCombos = sigSegs
                    .SelectMany(s => correctSegments.Select(cs => (s, cs)));
                    
                possiblePairs.AddRange(allCombos);

                if (correctSegments.Length == sigSegs.Count())
                {
                    doNotGenerateFurther.AddRange(allCombos);
                }
            }
            return possiblePairs.Distinct();
        }

        private static IEnumerable<string> PossibleCorrectSegmentsForDigit(EncodedDigit digit)
        {
            return Digit.ValidDigits.Where(vd => vd.Length == digit.Length).ToArray();
        }
    }
}
