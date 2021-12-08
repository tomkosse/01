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
            "abcefg", "cf", "acdeg", "acdfg", "acdfg", "bcdf", "abdfg", "abdefg", "acf", "abcdefg", "abcdfg"
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
            get {
                System.Console.WriteLine("Validating " + input + " against table");
                return _validDigits.Contains(input);
            }
        }

        public Digit(string input, IEnumerable<(char, char)> decodingTable)
        {
            System.Console.WriteLine($"Creating digit with input {input} and decodingtable {string.Join(" - ", decodingTable)}");
            var decoded = input
                            .ToCharArray()
                            .Select(c => decodingTable.Where(combo => combo.Item1 == c).First().Item2)
                            .ToArray();

            this.input = new String(decoded);
        }
    }

    public class EncodedDigit
    {
        
        public EncodedDigit(int? digit, char[] signalSegments)
        {
            Digit = digit;
            SignalSegments = signalSegments;
        }

        public int? Digit { get; }
        public char[] SignalSegments { get; }


        public int Length
        {
            get
            {
                return SignalSegments.Length;
            }
        }

        public override string ToString()
        {
            return "Digit: " + (Digit?.ToString() ?? "(unknown)") + "  | SignalSegments: " + new String(SignalSegments);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var lines = File
                .ReadAllLines(args[0]);

            var digits = new List<EncodedDigit>();
            foreach (var line in lines)
            {
                var splitted = line.Split("|").Select(el => el.Trim()).ToArray();
                var signalPattern = splitted[0].Split(" ");

                signalPattern.Select(pat => new string(pat.ToCharArray().OrderBy(c => c).ToArray())).Distinct().ToList().ForEach(seg =>
                {
                    var encodedDigit = CreateEncodedDigits(seg);
                    if (encodedDigit != null)
                    {
                        digits.Add(encodedDigit);
                    }
                });
            }
            System.Console.WriteLine("+++++");
            foreach(var digit in digits)
            {
                System.Console.WriteLine(digit);
            }
            System.Console.WriteLine("+++++");

            var certainMapping = new List<(char, char)>();
            var possiblePairs = GenerateAllPossiblePairs(digits, certainMapping);
            System.Console.WriteLine(string.Join(" - ", possiblePairs));

            var possibleMapping = new List<(char, char)>();

            foreach (var digit in digits)
            {
                foreach (var otherDigit in digits)
                {
                    var differentUnmappedSignalSegments = digit.SignalSegments.Except(otherDigit.SignalSegments);
                    if (differentUnmappedSignalSegments.Count() == 1)
                    {
                        var uniqueSegment = differentUnmappedSignalSegments.First();

                        System.Console.WriteLine("Unique segment: " + uniqueSegment);
                        Console.WriteLine(digit);
                        Console.WriteLine(otherDigit);

                        var pairsForMatch = PossibleCorrectSegmentsForDigit(otherDigit, certainMapping.Select(cm => cm.Item2).ToArray()).SelectMany(c => c);

                        var pairsForCurrent = PossibleCorrectSegmentsForDigit(digit, certainMapping.Select(cm => cm.Item2).ToArray()).SelectMany(c => c);
                        var options = possiblePairs.Where(pp => pp.Item1 == uniqueSegment && pairsForCurrent.Contains(pp.Item2));

                        if (options.Count() == 1)
                        {
                            var sol = options.Single();
                            certainMapping.Add(sol);
                        }
                        else {
                            possibleMapping.AddRange(options.Where(o => !possibleMapping.Contains(o)));
                        }
                    }
                }
            }
            System.Console.WriteLine("Certain Mapping: " + string.Join(" - ", certainMapping));
            System.Console.WriteLine("Possible Mapping: " + string.Join(" - ", possibleMapping));
            System.Console.WriteLine("======");

            var variants = new List<IEnumerable<(char, char)>>();
            variants.Add(certainMapping);
            foreach(var pm in possibleMapping.GroupBy(e => e.Item1))
            {
                var copiedVariants = new List<IEnumerable<(char, char)>>();
                foreach(var option in pm)
                {
                    foreach(var variant in variants)
                    {
                        var copiedVariant = variant.ToList();
                        copiedVariant.Add(option);
                        copiedVariants.Add(copiedVariant);
                    }
                }
                variants = copiedVariants;
            }

            foreach(var variant in variants)
            {
                System.Console.WriteLine("Possible variant: " + string.Join(" - ", variant));
            }

            foreach (var line in lines)
            {
                var splitted = line.Split("|").Select(el => el.Trim()).ToArray();
                var signalPattern = splitted[0].Split(" ");
                var validVariant = variants
                                    .Where(v => signalPattern.Select(sp => new Digit(sp, v)).All(d => d.IsValid))
                                    .ToList();
                System.Console.WriteLine("Valid variant: " + string.Join(" - ", validVariant));
            }
        }


        private static EncodedDigit CreateEncodedDigits(string segment)
        {
            switch (segment.Length)
            {
                case 2:
                    return new EncodedDigit(1, segment.ToCharArray());
                case 3:
                    return new EncodedDigit(7, segment.ToCharArray());
                case 4:
                    return new EncodedDigit(4, segment.ToCharArray());
                case 7:
                    return new EncodedDigit(8, segment.ToCharArray());
                default:
                    return new EncodedDigit(null, segment.ToCharArray());
            }
        }

        private static IEnumerable<(char, char)> GenerateAllPossiblePairs(List<EncodedDigit> knownDigits, List<(char, char)> certainMapping)
        {
            List<(char, char)> doNotGenerateFurther = certainMapping.ToList();

            List<(char, char)> possiblePairs = new List<(char, char)>();
            foreach (var digit in knownDigits.OrderBy(kus => kus.Length))
            {
                var correctSegments = PossibleCorrectSegmentsForDigit(digit, doNotGenerateFurther.Select(cm => cm.Item2).ToArray()).SelectMany(c => c).ToArray();
                var sigSegs = digit.SignalSegments.Except(doNotGenerateFurther.Select(cm => cm.Item1)).ToArray();

                var allCombos = sigSegs.SelectMany(s => correctSegments.Select(cs => (s, cs)));
                possiblePairs.AddRange(allCombos);
                
                if(correctSegments.Length == sigSegs.Length)
                {
                    doNotGenerateFurther.AddRange(allCombos);
                }
            }
            return possiblePairs.Distinct();
        }

        private static IEnumerable<char[]> PossibleCorrectSegmentsForDigit(EncodedDigit digit, char[] exclude)
        {
            IEnumerable<char[]> answer = null;
            switch (digit.Length)
            {
                case 2:
                    answer = new[] { "cf".ToCharArray() };
                    break;
                case 4:
                    answer = new[] { "bcdf".ToCharArray() };
                    break;
                case 3:
                    answer = new[] { "acf".ToCharArray() };
                    break;
                case 5:
                    answer = new[]
                    {
                        "acdeg".ToCharArray(), "acdfg".ToCharArray(), "abdfg".ToCharArray()
                    };
                    break;
                case 6:
                    answer = new[]
                    {
                        "abcdefg".ToCharArray(), "abcdfg".ToCharArray(), "abdefg".ToCharArray()
                    };
                    break;
                case 7:
                    answer = new[] { "abcdefg".ToCharArray()};
                    break;
                default:
                    break;
            }

            return answer.Select(a => a.Except(exclude).ToArray());
        }
    }
}
