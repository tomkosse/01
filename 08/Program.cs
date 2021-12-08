using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _08
{
    public class Digit
    {
        private string[] _validDigits =
        {
            "abcefg", "cf", "acdeg", "acdfg", "bcdf", "abdfg", "abdefg", "acf", "abcdefg", "abcdfg"
        };
        private readonly string input;

        public int? Value
        {
            get
            {
                var findCorrectDigit = Enumerable.Range(0, _validDigits.Length).Where(i => _validDigits[i] == input);
                return findCorrectDigit.Any() ? findCorrectDigit.First() : -1;
            }
        }

        public bool IsValid
        {
            get
            {
                return _validDigits.Contains(input);
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

            // Part 1
            var answerPart1 = 0;
            foreach (var line in lines)
            {
                var splitted = line.Split("|").Select(el => el.Trim()).ToArray();
                var outputValues = splitted[1].Split(" ");
                var hits = new int[] { 2, 3, 4, 7 };
                answerPart1 += outputValues.Count(ov => hits.Contains(ov.Length));
            }
            System.Console.WriteLine("Part 1 answer: " + answerPart1);

            // Part 2
            int answerPart2 = 0;
            foreach (var line in lines)
            {
                var splitted = line.Split("|").Select(el => el.Trim()).ToArray();
                var signalPattern = splitted[0].Split(" ");
                var outputValues = splitted[1].Split(" ");

                var digits = signalPattern
                                .Select(sp => new EncodedDigit(sp))
                                .OrderBy(d => d.Length)
                                .ToList();

                var possiblePairs = GenerateAllPossibleDecodingPairs(digits);

                var possibleDecodingTables = new List<IEnumerable<(char, char)>>() { new List<(char, char)>() };
                foreach (var pm in possiblePairs.GroupBy(e => e.Item1))
                {
                    var copiedTables = new List<IEnumerable<(char, char)>>();
                    foreach (var option in pm)
                    {
                        foreach (var variant in possibleDecodingTables)
                        {
                            var copiedTable = variant.ToList();
                            copiedTable.Add(option);
                            copiedTables.Add(copiedTable);
                        }
                    }
                    possibleDecodingTables = copiedTables;
                }

                IEnumerable<(char, char)> decodingTable = possibleDecodingTables
                                    .Where(table =>
                                        signalPattern
                                            .Select(pattern => new Digit(pattern, table))
                                            .All(digit => digit.IsValid)
                                    )
                                    .First();

                int outputValue = int.Parse(string.Join("", outputValues.Select(sp => new Digit(sp, decodingTable).Value)));
                var translated = string.Join(" ", signalPattern.Select(sp => new Digit(sp, decodingTable).Value));
                System.Console.WriteLine("Signal pattern: " + translated + " Output value: " + outputValue);
                answerPart2 += outputValue;
            }

            System.Console.WriteLine("======================================================");
            System.Console.WriteLine("                                  Total output: " + answerPart2);
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

                var allCombos = sigSegs.SelectMany(s => correctSegments.Select(cs => (s, cs)));
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
            switch (digit.Length)
            {
                case 2:
                    return new[] { "cf" };
                case 4:
                    return new[] { "bcdf" };
                case 3:
                    return new[] { "acf" };
                case 5:
                    return new[] { "acdeg", "acdfg", "abdfg" };
                case 6:
                    return new[] { "abcdefg", "abcdfg", "abdefg" };
                case 7:
                    return new[] { "abcdefg" };
                default:
                    throw new InvalidOperationException("We're not supposed to get here");
            }
        }
    }
}
