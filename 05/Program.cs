using System;
using System.IO;
using System.Linq;

namespace _05
{
    class Program
    {
        static void Main(string[] args)
        {
            var rangePairs = File
                    .ReadAllLines(args[0])
                    .Select(line => line.Split(" -> "))
                    .Select(str =>
                        (
                            leftHand: str[0].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse),
                            rightHand: str[1].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)
                        )
                    );
            var allPairs = rangePairs
                            .Select(pair => pair.leftHand)
                            .Union(rangePairs.Select(pair => pair.rightHand));
            int globalMaxX = allPairs.Max(p => p.First()) + 1;
            int globalMaxY = allPairs.Max(p => p.Last()) + 1;
            
            int[,] matrix = new int[globalMaxX, globalMaxY];
            foreach (var rangePair in rangePairs)
            {
                var x1 = rangePair.leftHand.First();
                var y1 = rangePair.leftHand.Last();
                var x2 = rangePair.rightHand.First();
                var y2 = rangePair.rightHand.Last();
                var minY = Math.Min(y1, y2);
                var maxY = Math.Max(y1, y2);
                var minX = Math.Min(x1, x2);
                var maxX = Math.Max(x1, x2);

                if (x1 != x2 && y1 == y2)
                {
                    for (int i = minX; i <= maxX; i++)
                    {
                        matrix[i, y1]++;
                    }
                }
                else if (y1 != y2 && x1 == x2)
                {
                    for (int i = minY; i <= maxY; i++)
                    {
                        matrix[x1, i]++;
                    }
                }
                else
                { // Diagonal
                    int cursorY = y1;
                    int cursorX = x1;
                    while (cursorX != x2 && cursorY != y2)
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
                    var column = matrix[n, i];
                    Console.Write(column);
                    if (column > 1)
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
