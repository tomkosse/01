using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _09
{
    public class Cell
    {
        public int Value;

        public int X;
        public int Y;

        public bool InBasin;

        public Cell(int value, int x, int y)
        {
            Value = value;
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"Cell {X}:{Y} - {Value}";
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = File.ReadAllLines(args[0]).Select(lines => lines.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
            var start = DateTime.Now;
            var cellMatrix = GetCellMatrix(numbers);

            List<int> lowPoints = new List<int>();
            List<List<Cell>> basins = new List<List<Cell>>();

            for (int y = 0; y < cellMatrix.Length; y++)
            {
                for (int x = 0; x < cellMatrix[y].Length; x++)
                {
                    var cell = cellMatrix[y][x];
                    var allAdjecent = GetAdjecentValuesWithoutPeaks(cellMatrix, y, x).ToList();
                    if (allAdjecent.All(a => a.Value > cell.Value))
                    {
                        lowPoints.Add(cell.Value);
                    }
                    var basin = new List<Cell>();
                    while (allAdjecent.Count > 0)
                    {
                        var adjecentCell = allAdjecent[0];
                        if (!adjecentCell.InBasin)
                        {
                            basin.Add(adjecentCell);
                            adjecentCell.InBasin = true;
                            allAdjecent.AddRange(GetAdjecentValuesWithoutPeaks(cellMatrix, adjecentCell.Y, adjecentCell.X).Where(a => !a.InBasin));
                        }
                        allAdjecent.Remove(adjecentCell);
                    }
                    if(basin.Count > 0)
                    {
                        basins.Add(basin);
                    }
                }
            }
            var largestBasinSizes = basins.Select(b => b.Count).OrderByDescending(b => b).Take(3).ToArray();
            int sizeOfThreeLargestBasinsMultiplied = largestBasinSizes[0] * largestBasinSizes[1] * largestBasinSizes[2];
            int lowPointScore = lowPoints.Sum(lp => lp + 1);
            var elapsed = (DateTime.Now - start).TotalMilliseconds;

            System.Console.WriteLine("Part 1: " + lowPointScore);
            System.Console.WriteLine("Amount of basins: " + basins.Count);
            System.Console.WriteLine("Size of three largest basins multiplied: " + sizeOfThreeLargestBasinsMultiplied);
            System.Console.WriteLine("Elapsed: " + elapsed + "ms");
        }

        private static Cell[][] GetCellMatrix(int[][] numberMatrix)
        {
            Cell[][] cells = new Cell[numberMatrix.Length][];

            for (int y = 0; y < numberMatrix.Length; y++)
            {
                var row = numberMatrix[y];
                cells[y] = new Cell[row.Length];
                for (int x = 0; x < row.Length; x++)
                {
                    var cell = numberMatrix[y][x];
                    cells[y][x] = new Cell(cell, x, y);
                }
            }
            return cells;
        }

        private static Cell[] GetAdjecentValuesWithoutPeaks(Cell[][] cellMatrix, int y, int x)
        {
            List<Cell> cells = new List<Cell>();
            var row = cellMatrix[y];
            if(y > 0)
            {
                cells.Add(cellMatrix[y - 1][x]);
            }
            if(y < cellMatrix.Length - 1)
            {
                cells.Add(cellMatrix[y + 1][x]);
            }
            if(x > 0)
            {
                cells.Add(cellMatrix[y][x-1]);
            }
            if(x < row.Length - 1)
            {
                cells.Add(cellMatrix[y][x+1]);
            }
            return cells.Where(c => c.Value != 9).ToArray();
        }
    }
}
