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

            var maximumYPosition = int.MinValue;
            var maxSettings = (xVel: 0, yVel: 0);
            var allValidVelocities = new List<(int xVel, int yVel)>();
            for (int x = 0; x < xRange[1] + 1; x++)
            {
                var lowerYBound = -1 * (Math.Abs(yRange[1] * 2));
                var higherYBound = Math.Abs(yRange[1] * 2);
                for (int y = lowerYBound; y < higherYBound; y++)
                {
                    int highestY = int.MinValue;
                    var probe = (X: 0, Y: 0);

                    var velocity = (xVel: x, yVel: y);

                    while (HasNotOvershot(probe, velocity, xRange[1], yRange[1]))
                    {
                        probe.X += velocity.xVel;
                        probe.Y += velocity.yVel;
                        var towardsZero = -1 * velocity.xVel.CompareTo(0);
                        velocity.xVel += towardsZero;
                        velocity.yVel--;
                        if (probe.Y > highestY)
                        {
                            highestY = probe.Y;
                        }

                        if (IsInRange(probe.X, xRange[0], xRange[1]) && IsInRange(probe.Y, yRange[0], yRange[1]))
                        {
                            if (highestY > maximumYPosition)
                            {
                                maximumYPosition = highestY;
                                maxSettings = (xVel: x, yVel: y);
                            }
                            allValidVelocities.Add((xVel: x, yVel: y));
                            break;
                        }
                    }
                }
            }
            var time = sw.ElapsedMilliseconds;
            System.Console.WriteLine(allValidVelocities.Count);
            System.Console.WriteLine($"Reached {maximumYPosition} with velocity {maxSettings.xVel},{maxSettings.yVel}");
            System.Console.WriteLine("Done in " + time + "ms");
        }

        private static bool HasNotOvershot((int X, int Y) probe, (int xVel, int yVel) velocity, int furthestXValue, int furthestYValue)
        {
            var nextXPoint = (probe.X + velocity.xVel);
            var nextYPoint = (probe.Y + velocity.yVel);
            //return probe.X <= furthestXPoint || Math.Abs(furthestYPoint - nextYPoint) < Math.Abs(furthestYPoint - probe.Y);
            return probe.X <= furthestXValue && IsInRange(probe.Y, -10000, 10000);
        }

        public static bool IsInRange(int a, int min, int max)
        {
            return a >= min && a <= max;
        }
    }
}
