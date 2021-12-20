using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _20
{
    public abstract class Lit
    {
        public abstract bool IsLit { get; }
    }

    public class BinaryValue : Lit
    {
        public override bool IsLit { get { return _isLit; } }
        private bool _isLit;

        public BinaryValue(char value)
        {
            _isLit = value == '#';
        }
    }

    public class Pixel : Lit
    {
        public override bool IsLit { get { return _isLit; } }

        private bool _isLit;
        public int X { get; set; }
        public int Y { get; set; }

        public Pixel(int x, int y, bool isLit)
        {
            _isLit = isLit;
            X = x;
            Y = y;
        }

        public int AlgorithmIndex { get; set; }

        public void SetLit(bool lit)
        {
            _isLit = lit;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);
            var sw = Stopwatch.StartNew();
            var parsed = lines.Select(line => line.ToCharArray().Select(c => new BinaryValue(c)).ToArray()).ToArray();

            var pixels = EnlargeCanvas(parsed.Skip(2).ToArray(), false);
            int iterations = 50;
            int afterTwoIterations = 0;
            int afterFiftyIterations = 0;
            for (int i = 1; i <= iterations; i++)
            {
                var applied = ApplyAlgorithm(parsed[0], pixels);
                pixels = applied.matrix;
                if (i == 2)
                {
                    afterTwoIterations = applied.amountLit;
                }
                else if (i == 50)
                {
                    afterFiftyIterations = applied.amountLit;
                }
            }
            sw.Stop();
            System.Console.WriteLine("Part 1: " + afterTwoIterations);
            System.Console.WriteLine("Part 2: " + afterFiftyIterations);
            System.Console.WriteLine("Done in " + sw.ElapsedMilliseconds + "ms");
        }
        private static (int amountLit, Pixel[][] matrix) ApplyAlgorithm(BinaryValue[] algorithmValues, Pixel[][] pixels)
        {
            var enlargedPixels = EnlargeCanvas(pixels, pixels[0][0].IsLit);
            var pixelsToChange = enlargedPixels.SelectMany(p => p).Select(p => (p.X, p.Y, shouldBeLit: algorithmValues[p.AlgorithmIndex].IsLit)).ToArray();
            int amountLit = 0;
            foreach (var toChange in pixelsToChange)
            {
                enlargedPixels[toChange.Y][toChange.X].SetLit(toChange.shouldBeLit);
                amountLit += toChange.shouldBeLit ? 1 : 0;
            }
            return (amountLit, enlargedPixels);
        }

        private static Pixel[][] EnlargeCanvas(Lit[][] matrix, bool paddingLit)
        {
            int height = matrix.Length + 2;
            int width = matrix[0].Length + 2;
            Pixel[][] pixels = new Pixel[height][];
            pixels[0] = new Pixel[width];
            for (int i = 0; i < width; i++)
            {
                pixels[0][i] = new Pixel(i, 0, paddingLit);
            }
            for (int y = 1; y < height - 1; y++)
            {
                var row = matrix[y - 1];
                pixels[y] = new Pixel[width];
                pixels[y][0] = new Pixel(0, y, paddingLit);

                for (int x = 1; x < width - 1; x++)
                {
                    var cell = matrix[y - 1][x - 1];
                    pixels[y][x] = new Pixel(x, y, cell.IsLit);
                }
                pixels[y][width - 1] = new Pixel(width - 1, y, paddingLit);
            }
            pixels[height - 1] = new Pixel[width];
            for (int x = 0; x < width; x++)
            {
                pixels[height - 1][x] = new Pixel(x, height - 1, paddingLit);
            }
            foreach (var pixel in pixels.SelectMany(o => o))
            {
                pixel.AlgorithmIndex = GetAlgorithmIndex(pixels, pixel);
            }
            return pixels;
        }

        private static int GetAlgorithmIndex(Pixel[][] pixelMatrix, Pixel pixel)
        {
            int val = 0;
            var row = pixelMatrix[pixel.Y];
            if (pixel.Y > 0)
            {
                if (pixel.X > 0)
                {
                    val = (val << 1) + (pixelMatrix[pixel.Y - 1][pixel.X - 1].IsLit ? 1 : 0);
                }
                val = (val << 1) + (pixelMatrix[pixel.Y - 1][pixel.X].IsLit ? 1 : 0);
                if (pixel.X < row.Length - 1)
                {
                    val = (val << 1) + (pixelMatrix[pixel.Y - 1][pixel.X + 1].IsLit ? 1 : 0);
                }
            }
            if (pixel.X > 0)
            {
                val = (val << 1) + (pixelMatrix[pixel.Y][pixel.X - 1].IsLit ? 1 : 0);
            }
            val = (val << 1) + (pixel.IsLit ? 1 : 0);

            if (pixel.X < row.Length - 1)
            {
                val = (val << 1) + (pixelMatrix[pixel.Y][pixel.X + 1].IsLit ? 1 : 0);
            }
            if (pixel.Y < pixelMatrix.Length - 1)
            {
                if (pixel.X > 0)
                {
                    val = (val << 1) + (pixelMatrix[pixel.Y + 1][pixel.X - 1].IsLit ? 1 : 0);
                }
                val = (val << 1) + (pixelMatrix[pixel.Y + 1][pixel.X].IsLit ? 1 : 0);
                if (pixel.X < row.Length - 1)
                {
                    val = (val << 1) + (pixelMatrix[pixel.Y + 1][pixel.X + 1].IsLit ? 1 : 0);
                }
            }
            return val;
        }
    }
}