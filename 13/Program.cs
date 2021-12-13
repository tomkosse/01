using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace _13
{
    public class Dot
    {
        public int X { get; }
        public int Y { get; }

        public Dot(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public static class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines(args[0]);
            IEnumerable<Dot> dots = lines.Where(line => line.Contains(",")).Select(line => line.Split(",")).Select(arr => new Dot(int.Parse(arr[0]), int.Parse(arr[1]))).ToArray();

            var maxX = dots.Max(d => d.X);
            var maxY = dots.Max(d => d.Y);

            foreach(var instruction in lines.Where(line => line.Contains("=")))
            {
                int line = int.Parse(instruction.Split("=").Last().ToString());
                if(instruction.Contains("x"))
                {
                    dots = dots.FoldHorizontally(line);
                    maxX -= line + 1;
                }
                else 
                {
                    dots = dots.FoldVertically(line);
                    maxY -= line + 1;
                }
            }
            PrintDots(dots, maxX, maxY);
        }

        private static IEnumerable<Dot> DistinctDots(this IEnumerable<Dot> dots)
        {
            return dots.GroupBy(dot => (dot.X, dot.Y)).Select(gr => gr.First()).ToArray();
        }

        private static IEnumerable<Dot> FoldHorizontally(this IEnumerable<Dot> dots, int line)
        {
            return dots.Select(dot => dot.X > line ? new Dot(line - (dot.X - line), dot.Y) : dot).DistinctDots();
        }

        private static IEnumerable<Dot> FoldVertically(this IEnumerable<Dot> dots, int line)
        {
            return dots.Select(dot => dot.Y > line ? new Dot(dot.X, line - (dot.Y - line)) : dot).DistinctDots();
        }

        private static void PrintDots(IEnumerable<Dot> dots, int maxX, int maxY)
        {
            for (int y = 0; y <= maxY; y++)
            {
                string line = "";
                for (int x = 0; x <= maxX; x++)
                {
                    line += dots.Any(d => d.X == x && d.Y == y) ? "#" : ".";
                }
                Console.WriteLine(line);
            }
            Console.WriteLine($"{dots.Count()} dots visible");
        }
    }
}
