using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _07
{
    class Program
    {
        static void Main(string[] args)
        {            
            var horizontalPositions = File
                .ReadAllLines(args[0])
                .SelectMany(line => line.Split(","))
                .Select(int.Parse)
                .ToArray();

            var min = horizontalPositions.Min();
            var max = horizontalPositions.Max();
            var answerPartOne = Enumerable
                        .Range(min, max - min)
                        .Min(point => horizontalPositions.Sum(hp =>  Math.Abs(hp - point)));

            System.Console.WriteLine("Part 1: " + answerPartOne);
            
            var start = DateTime.Now;
            var fuelLookup = CreateFuelLookup(max + 1);

            var lowestFuelConsumption = int.MaxValue;
            for(int i = min; i <= max; i++)
            {
                var consumption = DetermineFuelConsumptionPart2(i, horizontalPositions, lowestFuelConsumption, fuelLookup);
                if(consumption < lowestFuelConsumption)
                {
                    lowestFuelConsumption = consumption;
                }
            }

            System.Console.WriteLine($"Time elapsed: {(DateTime.Now - start).TotalMilliseconds}ms");
                
            System.Console.WriteLine("Part 2: " + lowestFuelConsumption);
        }

        private static int[] CreateFuelLookup(int maxRequiredSize)
        {
            int[] lookupArray = new int[maxRequiredSize];
            for(int i=1; i < maxRequiredSize; i++)
            {
                lookupArray[i] = lookupArray[i-1] + i;
            }
            return lookupArray;
        }

        private static int DetermineFuelConsumptionPart2(int point, IEnumerable<int> horizontalPositions, int currentLowestFuelConsumption, int[] fuelLookup)
        {
            var expended = 0;
            foreach(var hp in horizontalPositions)
            {
                var dist = Math.Abs(hp - point);
                expended += fuelLookup[dist];
                if(expended > currentLowestFuelConsumption)
                {
                    return int.MaxValue;
                }
            }
            return expended;
        }
    }
}