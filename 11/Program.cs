using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _11
{
    public class Octopus
    {
        public int EnergyLevel { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public bool HasFlashed { get; set; }

        public bool CanFlash
        {
            get
            {
                return EnergyLevel > 9 && !HasFlashed;
            }
        }

        public Octopus[] Neighbours { get; set; }

        public Octopus(int energyLevel, int x, int y)
        {
            EnergyLevel = energyLevel;
            X = x;
            Y = y;
        }

        public void Flash()
        {
            HasFlashed = true;
            foreach (var adjecentOctopus in Neighbours)
            {
                adjecentOctopus.EnergyLevel++;
            }
        }

        public void Reset()
        {
            EnergyLevel = 0;
            HasFlashed = false;
        }

        public override string ToString()
        {
            return $"Octopus {X}:{Y} - {EnergyLevel}";
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var numbers = File.ReadAllLines(args[0]).Select(lines => lines.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
            var octopuses = CreateOctopusMatrix(numbers);

            var allOctopuses = octopuses.SelectMany(o => o);
            int flashCounter = 0;
            int stepCounter = 1;
            bool stop = false;

            while (!stop)
            {
                Console.WriteLine("Step " + stepCounter);

                PrintGrid(octopuses);

                foreach (Octopus octo in allOctopuses)
                {
                    octo.EnergyLevel++;
                }

                while (allOctopuses.Any(o => o.CanFlash))
                {
                    foreach (Octopus octo in allOctopuses.Where(o => o.CanFlash))
                    {
                        octo.Flash();
                        flashCounter++;
                    }
                }

                if (allOctopuses.All(o => o.HasFlashed))
                {
                    System.Console.WriteLine("All octopuses flashed in one step! Step " + stepCounter);
                    stop = true;
                }
                
                foreach (var flashedOcto in allOctopuses.Where(o => o.HasFlashed))
                {
                    flashedOcto.Reset();
                }
                System.Console.WriteLine("Amount of flashes after step " + stepCounter + " = " + flashCounter);
                stepCounter++;
            }
        }

        private static void PrintGrid(Octopus[][] octopuses)
        {
            for (int x = 0; x < octopuses.Length; x++)
            {
                for (int y = 0; y < octopuses[x].Length; y++)
                {
                    Console.Write(octopuses[x][y].EnergyLevel);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static Octopus[][] CreateOctopusMatrix(int[][] numberMatrix)
        {
            Octopus[][] octopuses = new Octopus[numberMatrix.Length][];

            for (int y = 0; y < numberMatrix.Length; y++)
            {
                var row = numberMatrix[y];
                octopuses[y] = new Octopus[row.Length];
                for (int x = 0; x < row.Length; x++)
                {
                    var cell = numberMatrix[y][x];
                    octopuses[y][x] = new Octopus(cell, x, y);
                }
            }
            foreach (var octopus in octopuses.SelectMany(o => o))
            {
                octopus.Neighbours = GetAdjecentOctopuses(octopuses, octopus);
            }
            return octopuses;
        }

        private static Octopus[] GetAdjecentOctopuses(Octopus[][] octopusMatrix, Octopus octopus)
        {
            List<Octopus> cells = new List<Octopus>();
            var row = octopusMatrix[octopus.Y];
            if (octopus.Y > 0)
            {
                cells.Add(octopusMatrix[octopus.Y - 1][octopus.X]);
                if (octopus.X > 0)
                {
                    cells.Add(octopusMatrix[octopus.Y - 1][octopus.X - 1]);
                }
                if (octopus.X < row.Length - 1)
                {
                    cells.Add(octopusMatrix[octopus.Y - 1][octopus.X + 1]);
                }
            }
            if (octopus.Y < octopusMatrix.Length - 1)
            {
                cells.Add(octopusMatrix[octopus.Y + 1][octopus.X]);
                if (octopus.X > 0)
                {
                    cells.Add(octopusMatrix[octopus.Y + 1][octopus.X - 1]);
                }
                if (octopus.X < row.Length - 1)
                {
                    cells.Add(octopusMatrix[octopus.Y + 1][octopus.X + 1]);
                }
            }
            if (octopus.X > 0)
            {
                cells.Add(octopusMatrix[octopus.Y][octopus.X - 1]);
            }
            if (octopus.X < row.Length - 1)
            {
                cells.Add(octopusMatrix[octopus.Y][octopus.X + 1]);
            }
            return cells.ToArray();
        }
    }
}
