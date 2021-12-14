using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _14
{
    class Program
    {
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
            Queue<char> characters = new Queue<char>(template.ToCharArray());
            var charList = new Queue<char>();

            int maxSteps = 40;
            var start = DateTime.Now;
            for (int i = 1; i <= maxSteps; i++)
            {
                while (characters.Count > 1)
                {
                    var char1 = characters.Dequeue();
                    var char2 = characters.Peek();
                    var match = lookup[char1][char2];
                    charList.Enqueue(char1);
                    charList.Enqueue(match);
                }
                charList.Enqueue(characters.Dequeue());
                var elapsed = (DateTime.Now - start).TotalMilliseconds;
                characters = charList;

                System.Console.WriteLine($"After step {i}: {charList.Count}  - elapsed: " + elapsed);
                if (i != maxSteps)
                {
                    charList = new Queue<char>();
                }
            }

            var countedOccurrences = new string(charList.ToArray()).GroupBy(c => c).Select(g => (character: g.Key, count: g.Count())).OrderBy(el => el.count);
            var mostCommon = countedOccurrences.Last();
            var leastCommon = countedOccurrences.First();

            var score = mostCommon.count - leastCommon.count;
            System.Console.WriteLine($"Score: {score} Most common: {mostCommon.character}({mostCommon.count}) least common: {leastCommon.character}({leastCommon.count})");
        }
    }
}
