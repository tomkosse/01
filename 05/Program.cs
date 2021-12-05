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

                if (x1 != x2 && y1 == y2)
                {
                    var minX = Math.Min(x1, x2);
                    var maxX = Math.Max(x1, x2);
                    for (int i = minX; i <= maxX; i++)
                    {
                        matrix[i, y1]++;
                    }
                }
                else if (y1 != y2 && x1 == x2)
                {
                    var minY = Math.Min(y1, y2);
                    var maxY = Math.Max(y1, y2);
                    for (int i = minY; i <= maxY; i++)
                    {
                        matrix[x1, i]++;
                    }
                }
                else
                { // Diagonal
                    int cursorY = y1;
                    int cursorX = x1;
                    while (cursorX != x2)
                    {
                        matrix[cursorX, cursorY]++;
                        cursorX += cursorX >= x2 ? -1 : 1;
                        cursorY += cursorY >= y2 ? -1 : 1;
                    }
                    matrix[cursorX, cursorY]++;
                }
            }

            int intersections = 0;
            for (int i = 0; i < globalMaxY; i++)
            {
                for (int n = 0; n < globalMaxX; n++)
                {
                    var cell = matrix[n, i];
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
