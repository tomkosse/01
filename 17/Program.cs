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
            var coordSpace = line.Split(": ")[1].Split(", ").Select(str => str.Substring(2).Split("..").Select(int.Parse).ToArray()).ToArray();
            var xRange = coordSpace[0];
            var yRange = coordSpace[1];

            var maxSettings = (maxHeight: int.MinValue, xVel: 0, yVel: 0);
            int amountOfValidVelocities = 0;
            var lowerYBound = -1 * (Math.Abs(yRange[0]));
            var higherYBound = Math.Abs(yRange[0]);
            for (int x = 1; x <= xRange[1]; x++)
            {
                for (int y = lowerYBound; y < higherYBound; y++)
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
                }
            }
            var time = sw.ElapsedMilliseconds;
            System.Console.WriteLine(amountOfValidVelocities);
            System.Console.WriteLine($"Reached {maxSettings.maxHeight} with velocity {maxSettings.xVel},{maxSettings.yVel}");
            System.Console.WriteLine("Done in " + time + "ms");
        }

        public static bool IsInRange(int a, int min, int max) => a >= min && a <= max;
    }
}
