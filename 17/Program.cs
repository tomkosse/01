using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _17
{
    class Program
    {
        static void Main(string[] args)
        {
            var line = File.ReadAllLines(args[0])[0];
            var sw = Stopwatch.StartNew();

            int[] numbers = UglyParse(line.ToCharArray());

            var xRange = new int[] { numbers[0], numbers[1] };
            var yRange = new int[] { numbers[2], numbers[3] };

            var maxSettings = (maxHeight: int.MinValue, xVel: 0, yVel: 0);
            int amountOfValidVelocities = 0;
            var lowerYBound = -1 * (Math.Abs(yRange[0])); // A y lower than the lowest Y value would instantly undershoot
            var higherYBound = Math.Abs(yRange[0]);

            // The x distance travelled is (x^2 / 2) + 1 so the minimum x required to reach the lower bound can be determined
            int lowerXBound = (int)Math.Sqrt(xRange[0] * 2 - 1);

            for (int y = lowerYBound; y < higherYBound; y++)
            {
                for (int x = lowerXBound; x <= xRange[1]; x++)
                {
                    int highestHeight = int.MinValue;
                    var probe = (X: 0, Y: 0);
                    var velocity = (xVel: x, yVel: y);

                    while (probe.X <= xRange[1] && probe.Y >= yRange[0])
                    {
                        probe.X += velocity.xVel;
                        probe.Y += velocity.yVel;
                        velocity.xVel += -1 * velocity.xVel.CompareTo(0);
                        velocity.yVel--;
                        if (probe.Y > highestHeight)
                        {
                            highestHeight = probe.Y;
                        }

                        if (IsInRange(probe.X, xRange[0], xRange[1]) && IsInRange(probe.Y, yRange[0], yRange[1]))
                        {
                            if (highestHeight > maxSettings.maxHeight)
                            {
                                maxSettings = (maxHeight: highestHeight, xVel: x, yVel: y);
                            }
                            amountOfValidVelocities++;
                            break;
                        }
                    }
                    if (probe.X > xRange[1] && probe.Y > Math.Max(yRange[0], yRange[1]))
                    {
                        break; // Overshoot. Een nog hogere X gaat niet meer helpen.
                    }
                }
            }
            var time = sw.ElapsedMilliseconds;
            var ticks = sw.ElapsedTicks;
            System.Console.WriteLine($"Valid velocity count: {amountOfValidVelocities}");
            System.Console.WriteLine($"Reached {maxSettings.maxHeight} with velocity {maxSettings.xVel},{maxSettings.yVel}");
            System.Console.WriteLine($"Done in {time}ms ({ticks} ticks)");
        }

        private static int[] UglyParse(char[] chars)
        {
            int[] numbers = new int[4];
            int idx = 0;
            bool skipping = false;
            bool isNegative = false;

            for (int i = 15; i < chars.Length; i++)
            {
                var c = chars[i];
                if (Char.IsNumber(c))
                {
                    skipping = false;
                    numbers[idx] = numbers[idx] * 10 + (int)Char.GetNumericValue(c);
                }
                else if (c == '-')
                {
                    isNegative = true;
                }
                else
                {
                    if (!skipping)
                    {
                        if (isNegative)
                        {
                            numbers[idx] *= -1;
                        }
                        idx++;
                    }
                    skipping = true;
                    isNegative = false;
                }
            }
            if (isNegative)
            {
                numbers[idx] *= -1;
            }

            return numbers;
        }

        public static bool IsInRange(int a, int min, int max) => a >= min && a <= max;
    }
}
