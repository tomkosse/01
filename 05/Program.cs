using System;
using System.IO;
using System.Linq;

namespace _05
{
    class Program
    {
        static void Main(string[] args)
        {
            var coordPairs = File
                    .ReadAllLines(args[0])
                    .Select(line => line.Split(" -> "))
                    .Select(str =>
                        (
                            leftHand: str[0].Split(",").Select(int.Parse),
                            rightHand: str[1].Split(",").Select(int.Parse)
                        )
                    );
            var allPairs = coordPairs
                            .Select(pair => pair.leftHand)
                            .Union(coordPairs.Select(pair => pair.rightHand));
            int globalMaxX = allPairs.Max(p => p.First()) + 1;
            int globalMaxY = allPairs.Max(p => p.Last()) + 1;

            int[,] matrix = new int[globalMaxX, globalMaxY];
            foreach (var coordPair in coordPairs)
            {
                var x1 = coordPair.leftHand.First();
                var y1 = coordPair.leftHand.Last();
                var x2 = coordPair.rightHand.First();
                var y2 = coordPair.rightHand.Last();

                while (x1 != x2 || y1 != y2)
                {
                    matrix[x1, y1]++;
                    x1 += x1 > x2 ? -1 : (x1 == x2 ? 0 : 1);
                    y1 += y1 > y2 ? -1 : (y1 == y2 ? 0 : 1);
                }
                matrix[x1, y1]++;
            }

            int intersections = 0;
            for (int y = 0; y < globalMaxY; y++)
            {
                for (int x = 0; x < globalMaxX; x++)
                {
                    var cell = matrix[x, y];
                    Console.Write(cell);
                    if (cell > 1)
                    {
                        intersections++;
                    }
                }
                System.Console.WriteLine();
            }

            System.Console.WriteLine("Intersections: " + intersections);
        }
    }
}
