﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _13
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines(args[0]);
            IEnumerable<(int X, int Y)> dots = lines.Where(line => line.Contains(",")).Select(line => line.Split(",")).Select(arr => (X: int.Parse(arr[0]), Y: int.Parse(arr[1]))).ToArray();

            foreach(var instruction in lines.Where(line => line.Contains("=")))
            {
                int line = int.Parse(instruction.Split("=").Last().ToString());
                if(instruction.Contains("x"))
                {
                    dots = dots.FoldHorizontally(line);
                }
                else 
                {
                    dots = dots.FoldVertically(line);
                }
            }

            PrintDots(dots, dots.Max(d => d.X), dots.Max(d => d.Y));
        }

        private static IEnumerable<(int X, int Y)> FoldHorizontally(this IEnumerable<(int X, int Y)> dots, int line)
        {
            return dots.Select(dot => dot.X > line ? (line - (dot.X - line), dot.Y) : dot).Distinct().ToArray();
        }

        private static IEnumerable<(int X, int Y)> FoldVertically(this IEnumerable<(int X, int Y)> dots, int line)
        {
            return dots.Select(dot => dot.Y > line ? (dot.X, line - (dot.Y - line)) : dot).Distinct().ToArray();
        }

        private static void PrintDots(IEnumerable<(int X, int Y)> dots, int maxX, int maxY)
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