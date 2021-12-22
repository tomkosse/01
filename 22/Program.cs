using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _22
{
    static class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines(args[0]);
            var sw = Stopwatch.StartNew();

            var boundsPerLinePart1 = input.Select(l => UglyParse(l.ToCharArray())).ToArray();

            var part1 = Part1(boundsPerLinePart1);

            var boundsPerLinePart2 = input.Select(l => UglyParse(l.ToCharArray())).ToArray();
            var part2 = Part2(boundsPerLinePart2);
            sw.Stop();

            System.Console.WriteLine("Part 1: " + part1);
            System.Console.WriteLine("Part 2: " + part2);
            System.Console.WriteLine("Done in : " + sw.ElapsedMilliseconds + "ms");
        }

        private static long Part2((bool isOn, int[] bounds)[] cubes)
        {
            var processedCubes = new List<(bool isOn, int[] bounds)>();
            
            for(int i=0; i < cubes.Length; i++)
            {
                var cube = cubes[i];
                processedCubes = processedCubes.GetUndoubledCubes(cube);
            }

            long runningOnCount = 0;
            foreach(var cube in processedCubes)
            {
                var vol = cube.bounds.Volume();
                System.Console.WriteLine("Cube " + (cube.isOn ? 1 : 0) + " " + string.Join(",", cube.bounds) + " vol. " + vol);
                if(cube.isOn)
                {
                    runningOnCount += vol;
                }
                else 
                {
                    runningOnCount -= vol;
                }
                System.Console.WriteLine("   => " + runningOnCount);
            }
            return runningOnCount;
        }
        
        private static long Volume(this int[] bounds)
        {
            return (bounds[1] - bounds[0] + 1) * (bounds[3] - bounds[2] + 1) * (bounds[5] - bounds[4] + 1);
        }

        private static List<(bool isOn, int[] bounds)> GetUndoubledCubes(this IEnumerable<(bool isOn, int[] bounds)> cubes, (bool isOn, int[] bounds) cube)
        {
            List<(bool isOn, int[] bounds)> newCubes = new List<(bool isOn, int[] bounds)>();
            foreach(var c in cubes)
            {
                int endX = Math.Min(c.bounds[1], cube.bounds[1]);
                int startX = Math.Max(c.bounds[0], cube.bounds[0]);
                int endY = Math.Min(c.bounds[3], cube.bounds[3]);
                int startY = Math.Max(c.bounds[2], cube.bounds[2]);
                int endZ = Math.Min(c.bounds[5], cube.bounds[5]);
                int startZ = Math.Max(c.bounds[4], cube.bounds[4]);

                var xOverlap = Math.Max(0, endX - startX + 1);
                var yOverlap = Math.Max(0, endY - startY + 1);
                var zOverlap = Math.Max(0, endZ - startZ + 1);
                var volume = xOverlap * yOverlap * zOverlap;
                newCubes.Add(c);
                if(volume != 0 && cube.isOn == c.isOn)
                {
                    var newCoords = new int[] { startX, endX, startY, endY, startZ, endZ };
                    newCubes.Add((!cube.isOn, newCoords));
                }
                else if (volume != 0 && cube.isOn != c.isOn)
                {
                    var newCoords = new int[] { startX, endX, startY, endY, startZ, endZ};
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

            foreach (var bound in cubes)
            {
                var startX = Math.Max(bound.bounds[0] + 50, 0);
                var endX = Math.Min(100, bound.bounds[1] + 50);
                var startY = Math.Max(bound.bounds[2] + 50, 0);
                var endY = Math.Min(100, bound.bounds[3] + 50);
                var startZ = Math.Max(bound.bounds[4] + 50, 0);
                var endZ = Math.Min(100, bound.bounds[5] + 50);
                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        for (int z = startZ; z <= endZ; z++)
                        {
                            reactor[x, y, z] = bound.isOn;
                        }
                    }
                }
            }

            int count = 0;
            foreach (var el in reactor)
            {
                if (el)
                {
                    count++;
                }
            }
            return count;
        }

        private static (bool isOn, int[] bounds) UglyParse(char[] chars)
        {
            int[] numbers = new int[6];
            int idx = 0;
            bool skipping = false;
            bool isNegative = false;
            var startIdx = 5;
            var isOn = chars[1] == 'n';
            if(!isOn)
            {
                startIdx = 6;
            }
            for (int i = startIdx; i < chars.Length; i++)
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
