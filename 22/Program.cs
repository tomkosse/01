using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace _22
{
    static class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines(args[0]);
            var sw = Stopwatch.StartNew();

            var bounds = input.Select(l => QuickParse(l.ToCharArray())).ToArray();
            var part1 = Part1(bounds);
            var part2 = Part2(bounds);

            sw.Stop();

            System.Console.WriteLine("Part 1: " + part1);
            System.Console.WriteLine("Part 2: " + part2);
            System.Console.WriteLine("Done in : " + sw.ElapsedMilliseconds + "ms");
        }

        private static BigInteger Part2((bool isOn, int[] bounds)[] cubes)
        {
            var processedCubes = new List<(bool isOn, int[] bounds)>();
            
            foreach(var cube in cubes)
            {
                processedCubes = processedCubes.GetUndoubledCubes(cube);
            }

            BigInteger runningOnCount = 0;
            foreach(var cube in processedCubes)
            {
                var vol = cube.bounds.Volume();
                runningOnCount += cube.isOn ? vol : vol * -1;
            }
            return runningOnCount;
        }
        
        private static long Volume(this int[] bounds)
        {
            return (bounds[1] - bounds[0] + 1L) * (bounds[3] - bounds[2] + 1L) * (bounds[5] - bounds[4] + 1L);
        }

        private static List<(bool isOn, int[] bounds)> GetUndoubledCubes(this List<(bool isOn, int[] bounds)> cubes, (bool isOn, int[] bounds) cube)
        {
            List<(bool isOn, int[] bounds)> newCubes = new List<(bool isOn, int[] bounds)>(cubes.Count + 2);
            foreach(var c in cubes)
            {
                newCubes.Add(c);

                int endX = Math.Min(c.bounds[1], cube.bounds[1]);
                int startX = Math.Max(c.bounds[0], cube.bounds[0]);
                int endY = Math.Min(c.bounds[3], cube.bounds[3]);
                int startY = Math.Max(c.bounds[2], cube.bounds[2]);
                int endZ = Math.Min(c.bounds[5], cube.bounds[5]);
                int startZ = Math.Max(c.bounds[4], cube.bounds[4]);

                var xOverlap = Math.Max(0, endX - startX + 1);
                var yOverlap = Math.Max(0, endY - startY + 1);
                var zOverlap = Math.Max(0, endZ - startZ + 1);

                bool overlapt = xOverlap != 0 && yOverlap != 0 && zOverlap != 0;
                if(overlapt && cube.isOn == c.isOn)
                {
                    var newCoords = new int[] { startX, endX, startY, endY, startZ, endZ };
                    newCubes.Add((!cube.isOn, newCoords));
                }
                else if (overlapt && cube.isOn != c.isOn)
                {
                    var newCoords = new int[] { startX, endX, startY, endY, startZ, endZ };
                    newCubes.Add((cube.isOn, newCoords));
                }
            }
            if(cube.isOn)
            {
                newCubes.Add(cube);
            }
            return newCubes;
        }

        private static int Part1((bool isOn, int[] bounds)[] cubes)
        {
            bool[,,] reactor = new bool[101, 101, 101];

            foreach (var cube in cubes)
            {
                var startX = Math.Max(cube.bounds[0] + 50, 0);
                var endX = Math.Min(100, cube.bounds[1] + 50);
                var startY = Math.Max(cube.bounds[2] + 50, 0);
                var endY = Math.Min(100, cube.bounds[3] + 50);
                var startZ = Math.Max(cube.bounds[4] + 50, 0);
                var endZ = Math.Min(100, cube.bounds[5] + 50);
                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        for (int z = startZ; z <= endZ; z++)
                        {
                            reactor[x, y, z] = cube.isOn;
                        }
                    }
                }
            }

            int count = 0;
            foreach (var cube in reactor)
            {
                if (cube)
                {
                    count++;
                }
            }
            return count;
        }

        private static (bool isOn, int[] bounds) QuickParse(char[] chars)
        {
            int[] numbers = new int[6];
            int idx = 0;
            bool skipping = false;
            bool isNegative = false;
            var isOn = chars[1] == 'n';
            
            var cursorStart = isOn ? 5 : 6;

            for (int i = cursorStart; i < chars.Length; i++)
            {
                var c = chars[i];
                if (Char.IsNumber(c))
                {
                    skipping = false;
                    numbers[idx] = numbers[idx] * 10 + (int)Char.GetNumericValue(c);
                }
                else if (c == '-')
                {
                    isNegative = true;
                }
                else
                {
                    if (!skipping)
                    {
                        if (isNegative)
                        {
                            numbers[idx] *= -1;
                        }
                        idx++;
                    }
                    skipping = true;
                    isNegative = false;
                }
            }
            if (isNegative)
            {
                numbers[idx] *= -1;
            }
            return (isOn, numbers);
        }
    }
}