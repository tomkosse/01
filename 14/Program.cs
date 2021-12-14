using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _14
{
    static class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);
            Stopwatch s = Stopwatch.StartNew();

            var template = lines[0];
            var highestCharacterValue = 0;
            var lookup = new long[26, 26];
            for (int i = 2; i < lines.Length; i++)
            {
                var line = lines[i];
                var char1 = line[0] - 'A';
                highestCharacterValue = char1 > highestCharacterValue ? char1 : highestCharacterValue;
                var char2 = line[1] - 'A';
                var element = line[6] - 'A';
                lookup[char1, char2] = element;
            }

            highestCharacterValue++;

            var pairs = new long[highestCharacterValue, highestCharacterValue];
            var countPerCharacter = new long[highestCharacterValue];
            for (int i = 0; i < template.Length - 1; i++)
            {
                var firstChar = template[i] - 'A';
                var secondChar = template[i + 1] - 'A';
                pairs[firstChar, secondChar]++;
                countPerCharacter[firstChar]++;
            }
            countPerCharacter[template[^1] - 'A']++;

            int maxSteps = 40; // 10 for part 1
            for (int step = 1; step <= maxSteps; step++)
            {
                var tempPairs = new long[highestCharacterValue, highestCharacterValue];
                for (int i = 0; i < highestCharacterValue; i++)
                {
                    for (int j = 0; j < highestCharacterValue; j++)
                    {
                        var count = pairs[i, j];
                        if (count > 0)
                        {
                            tempPairs[i, lookup[i, j]] += count;
                            tempPairs[lookup[i, j], j] += count;
                            countPerCharacter[lookup[i, j]] += count;

                            pairs[i, j] = 0;
                        }
                    }
                }
                pairs = tempPairs;
            }

            var lowest = long.MaxValue;
            var highest = long.MinValue;
            for (int i = 0; i < highestCharacterValue; i++)
            {
                var count = countPerCharacter[i];
                lowest = count < lowest && count > 0 ? count : lowest;
                highest = count > highest ? count : highest;
            }
            var score = highest - lowest;

            System.Console.WriteLine($"Score: {score} - elapsed: {s.ElapsedMilliseconds}ms which is {s.ElapsedTicks} ticks");
        }
    }
}