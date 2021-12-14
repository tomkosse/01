using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _14
{
    class Program
    {
        private static Dictionary<char, long> _countPerCharacter = new Dictionary<char, long>();
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);
            var template = lines.First();

            var insertions = lines.Where(l => l.Contains("->")).Select(l => l.Split(" -> ")).Select(arr => (searchPattern1: arr[0][0], searchPattern2: arr[0][1], element: arr[1][0])).ToArray();
            var lookup = new Dictionary<char, Dictionary<char, char>>();
            foreach (var insertion in insertions)
            {
                if (!lookup.ContainsKey(insertion.searchPattern1))
                {
                    var innerDict = new Dictionary<char, char>();
                    lookup.Add(insertion.searchPattern1, innerDict);
                }
                var innerDictionary = lookup[insertion.searchPattern1];
                innerDictionary.Add(insertion.searchPattern2, insertion.element);
            }

            System.Console.WriteLine("Template:     " + template);
            var pairCounts = new Dictionary<string, long>();

            foreach(var c in template)
            {
                AddToTally(c, 1);
            }

            while (template.Length > 1)
            {
                var pair = template.Substring(0, 2);
                pairCounts.Add(pair, 1);
                System.Console.WriteLine("Added pair " + pair);
                template = template.Substring(1);
            }

            int maxSteps = 40;
            var start = DateTime.Now;
            for (int i = 1; i <= maxSteps; i++)
            {
                Dictionary<string, long> toInsert = new Dictionary<string, long>();
                foreach(var pair in pairCounts.Keys)
                {
                    var match = lookup[pair[0]][pair[1]];
                    var newPair1 = pair[0].ToString() + match.ToString();
                    var newPair2 = match.ToString() + pair[1].ToString();
                    if(!toInsert.ContainsKey(newPair1))
                    {
                        toInsert.Add(newPair1, 0);
                    }
                    if(!toInsert.ContainsKey(newPair2))
                    {
                        toInsert.Add(newPair2, 0);
                    }
                    toInsert[newPair1] += pairCounts[pair];
                    toInsert[newPair2] += pairCounts[pair];
                    
                    AddToTally(match, pairCounts[pair]);

                    pairCounts[pair] -= pairCounts[pair];
                    if(pairCounts[pair] == 0)
                    {
                        pairCounts.Remove(pair);
                    }
                    System.Console.WriteLine("Replacing " + pair + " with " + newPair1 + " and " + newPair2);
                }

                toInsert.ToList().ForEach(ti => { if(pairCounts.ContainsKey(ti.Key)) { pairCounts[ti.Key] += ti.Value; } else { pairCounts.Add(ti.Key, ti.Value); } });

                var elapsed = (DateTime.Now - start).TotalMilliseconds;
                System.Console.WriteLine($"After step {i}: {pairCounts.Sum(pc => pc.Value) + 1}  - elapsed: " + elapsed);
                pairCounts.ToList().ForEach(pc => System.Console.WriteLine(pc.Key + " - " + pc.Value));
            }

            var mostCommon = _countPerCharacter.OrderBy(pc => pc.Value).Last();
            var leastCommon = _countPerCharacter.OrderBy(pc => pc.Value).First();

            var score = mostCommon.Value - leastCommon.Value;
            System.Console.WriteLine($"Score: {score} Most common: {mostCommon.Key}({mostCommon.Value}) least common: {leastCommon.Key}({leastCommon.Value})");
        }

        private static void AddToTally(char c, long amount)
        {
            if (!_countPerCharacter.ContainsKey(c))
            {
                _countPerCharacter.Add(c, amount);
            }
            else
            {
                _countPerCharacter[c] += amount;
            }
        }
    }
}
