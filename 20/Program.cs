using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _20
{
    public class Pixel
    {
        public bool IsLit { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public Pixel[] Neighbours { get; set; }

        public Pixel(int x, int y, bool isLit)
        {
            IsLit = isLit;
            X = x;
            Y = y;
        }

        public int AlgorithmValue
        {
            get
            {
                var bitString = new string(Neighbours.Select(n => n.IsLit ? '1' : '0').ToArray());
                return Convert.ToInt32(bitString, 2);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);
            var parsed = lines.Select(line => line.ToCharArray().Select(c => c == '#' ? 1 : 0).ToArray()).ToArray(); ;

            var algo = parsed[0];

            var pixels = CreatePixels(parsed.Skip(2).ToArray(), 6, false);
            int iterations = 50;
            for(int i=1; i <= iterations; i++)
            {
                pixels = ApplyAlgorithm(algo, pixels);
                if(i == 2)
                {
                    System.Console.WriteLine("Part 1: " + pixels.Count(p => p.IsLit));
                }
            }
            System.Console.WriteLine("Part 2: " + pixels.Count(p => p.IsLit));
        }
        private static List<Pixel> ApplyAlgorithm(int[] algo, List<Pixel> pixels)
        {
            var enlargedPixels = CreatePixels(ToBits(pixels), 6, pixels[0].IsLit);
            var pixelsToChange = enlargedPixels.Select(p => (p.X, p.Y, val: algo[p.AlgorithmValue])).ToArray();
            foreach (var pixel in pixelsToChange)
            {
                var correspondingPixel = enlargedPixels.First(p => p.X == pixel.X && p.Y == pixel.Y);
                correspondingPixel.IsLit = pixel.val == 1;
            }
            return enlargedPixels;
        }

        private static List<Pixel> CreatePixels(int[][] numberMatrix, int enlargeBy, bool paddingLit)
        {
            var padding = (enlargeBy / 2);

            int height = numberMatrix.Length + enlargeBy;
            int width = numberMatrix[0].Length + enlargeBy;
            Pixel[][] pixels = new Pixel[height][];
            for (int y = 0; y < padding; y++)
            {
                pixels[y] = new Pixel[width];
                for (int i = 0; i < width; i++)
                {
                    pixels[y][i] = new Pixel(i, y, paddingLit);
                }
            }
            for (int y = padding; y < height - padding; y++)
            {
                var row = numberMatrix[y - padding];
                pixels[y] = new Pixel[width];

                for (int i = 0; i < padding; i++)
                {
                    pixels[y][i] = new Pixel(i, y, paddingLit);
                }

                for (int x = padding; x < width - padding; x++)
                {
                    var cell = numberMatrix[y - padding][x - padding];
                    pixels[y][x] = new Pixel(x, y, cell == 1);
                }
                for (int i = width - padding; i < width; i++)
                {
                    pixels[y][i] = new Pixel(i, y, paddingLit);
                }
            }

            for (int y = height - padding; y < height; y++)
            {
                pixels[y] = new Pixel[width];
                for (int x = 0; x < width; x++)
                {
                    pixels[y][x] = new Pixel(x, y, paddingLit);
                }
            }

            foreach (var pixel in pixels.SelectMany(o => o))
            {
                pixel.Neighbours = GetBitPixels(pixels, pixel);
            }
            return pixels.SelectMany(a => a).ToList();
        }

        private static void PrintGrid(List<Pixel> pixels)
        {
            var startX = pixels.Min(p => p.X);
            var startY = pixels.Min(p => p.Y);
            var endX = pixels.Max(p => p.X);
            var endY = pixels.Max(p => p.Y);
            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    var pixel = pixels.First(p => p.X == x && p.Y == y);
                    Console.Write(pixel.IsLit ? "#" : ".");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static int[][] ToBits(List<Pixel> pixels)
        {
            var startX = pixels.Min(p => p.X);
            var startY = pixels.Min(p => p.Y);
            var endX = pixels.Max(p => p.X);
            var endY = pixels.Max(p => p.Y);
            int[][] outPix = new int[endY + 1][];
            for (int y = startY; y <= endY; y++)
            {
                outPix[y] = new int[endX + 1];
                for (int x = startX; x <= endX; x++)
                {
                    var pixel = pixels.First(p => p.X == x && p.Y == y);
                    outPix[y][x] = pixel.IsLit ? 1 : 0;
                }
            }
            return outPix;
        }

        private static Pixel[] GetBitPixels(Pixel[][] pixelMatrix, Pixel pixel)
        {
            List<Pixel> pixels = new List<Pixel>();
            var row = pixelMatrix[pixel.Y];
            if (pixel.Y > 0)
            {
                if (pixel.X > 0)
                {
                    pixels.Add(pixelMatrix[pixel.Y - 1][pixel.X - 1]);
                }
                pixels.Add(pixelMatrix[pixel.Y - 1][pixel.X]);
                if (pixel.X < row.Length - 1)
                {
                    pixels.Add(pixelMatrix[pixel.Y - 1][pixel.X + 1]);
                }
            }
            if (pixel.X > 0)
            {
                pixels.Add(pixelMatrix[pixel.Y][pixel.X - 1]);
            }
            pixels.Add(pixel);
            if (pixel.X < row.Length - 1)
            {
                pixels.Add(pixelMatrix[pixel.Y][pixel.X + 1]);
            }
            if (pixel.Y < pixelMatrix.Length - 1)
            {
                if (pixel.X > 0)
                {
                    pixels.Add(pixelMatrix[pixel.Y + 1][pixel.X - 1]);
                }
                pixels.Add(pixelMatrix[pixel.Y + 1][pixel.X]);
                if (pixel.X < row.Length - 1)
                {
                    pixels.Add(pixelMatrix[pixel.Y + 1][pixel.X + 1]);
                }
            }
            return pixels.ToArray();
        }
    }
}