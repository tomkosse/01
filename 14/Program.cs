using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _14
{
    static class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]).ToArray();
            var start = DateTime.Now;

            var template = lines[0];
            var lookup = lines.Skip(2).Select(l => l.Split(" -> ")).ToDictionary(i => i[0], i => i[1][0]);

            var pairCounts = new Dictionary<string, long>();
            for (int i = 0; i < template.Length - 1; i++)
            {
                var pair = template[i].ToString() + template[i + 1];
                pairCounts.Add(pair, 1);
            }

            var countPerCharacter = new Dictionary<char, long>();
            template.ToList().ForEach(c => countPerCharacter.EnterOrAdd(c, 1));

            int maxSteps = 40; // 10 for part 1
            for (int i = 1; i <= maxSteps; i++)
            {
                Dictionary<string, long> toInsert = new Dictionary<string, long>();
                foreach (var pair in pairCounts.Keys.ToList())
                {
                    toInsert.EnterOrAdd(pair[0].ToString() + lookup[pair].ToString(), pairCounts[pair]);
                    toInsert.EnterOrAdd(lookup[pair].ToString() + pair[1].ToString(), pairCounts[pair]);
                    countPerCharacter.EnterOrAdd(lookup[pair], pairCounts[pair]);
                    pairCounts.Remove(pair);
                }
                foreach (var ti in toInsert)
                {
                    pairCounts.EnterOrAdd(ti.Key, ti.Value);
                }
            }
            var ordered = countPerCharacter.OrderBy(pc => pc.Value).ToArray();
            var score = ordered[^1].Value - ordered[0].Value;
            System.Console.WriteLine($"Score: {score} - elapsed: {(DateTime.Now - start).TotalMilliseconds}");
        }

        public static void EnterOrAdd<K>(this Dictionary<K, long> dict, K key, long value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
            else
            {
                dict[key] = dict[key] + value;
            }
        }
    }
}