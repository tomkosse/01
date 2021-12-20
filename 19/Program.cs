using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _19
{
    public static class Program
    {
        private static int _maxDistance;

        static void Main(string[] args)
        {
            var input = File.ReadAllLines(args[0]);
            var sw = Stopwatch.StartNew();
            List<List<(int x, int y, int z)>> reports = new List<List<(int x, int y, int z)>>();
            var inputQueue = new Queue<string>(input);
            while (inputQueue.Any())
            {
                inputQueue.Dequeue(); // scanner header
                var list = new List<(int x, int y, int z)>();
                while (inputQueue.Any())
                {
                    var coord = inputQueue.Dequeue();
                    if (coord.Trim() == string.Empty)
                    {
                        break;
                    }
                    list.Add(StringToCoords(coord));
                }
                reports.Add(list);
            }

            var beacons = FindAllBeacons(reports);
            sw.Stop();
            System.Console.WriteLine("Part 1: " + beacons.Count());
            System.Console.WriteLine("Part 2: " + _maxDistance);
            System.Console.WriteLine("Done in " + sw.ElapsedMilliseconds + "ms");
        }

        private static IEnumerable<(int x, int y, int z)> FindAllBeacons(List<List<(int x, int y, int z)>> reports)
        {
            var scannerLocations = new Nullable<(int x, int y, int z)>[reports.Count];
            scannerLocations[0] = (0, 0, 0);
            while (scannerLocations.Any(l => l == null))
            {
                for (int i = 0; i < reports.Count; i++)
                {
                    for (int j = 0; j < reports.Count; j++)
                    {
                        if (scannerLocations[i] != null && scannerLocations[j] == null)
                        {
                            var absoluteScannerLocation = TryDetermineScannerLocation(reports[i], reports[j]);
                            if (absoluteScannerLocation.HasValue)
                            {
                                reports[j] = reports[j].Select(c => GetAbsoluteCoordinate(absoluteScannerLocation.Value.coord, GetTransformations(c)[absoluteScannerLocation.Value.transformationId])).ToList();
                                scannerLocations[j] = absoluteScannerLocation.Value.coord;
                            }
                        }
                    }
                }
            }

            var maxDistance = int.MinValue;
            foreach (var scannerA in scannerLocations)
            {
                foreach (var scannerB in scannerLocations)
                {
                    var relativeDistanceCoord = GetRelativeCoordinate(scannerA.Value, scannerB.Value);
                    var manhattanDistance = relativeDistanceCoord.ManhattanDistance();
                    if (manhattanDistance > maxDistance)
                    {
                        maxDistance = manhattanDistance;
                    }
                }
            }
            _maxDistance = maxDistance;

            return reports.SelectMany(ar => ar).Distinct();
        }

        public static int ManhattanDistance(this (int x, int y, int z) coord)
        {
            return Math.Abs(coord.x) + Math.Abs(coord.y) + Math.Abs(coord.z);
        }

        private static Nullable<(int transformationId, (int x, int y, int z) coord)> TryDetermineScannerLocation(List<(int x, int y, int z)> firstReport, List<(int x, int y, int z)> otherReport)
        {
            for (int j = 0; j < firstReport.Count; j++)
            {
                var referenceBeacon = firstReport[j];
                var list = firstReport.Except(new[] { referenceBeacon }).Select(b => GetRelativeCoordinate(referenceBeacon, b)).OrderBy(c => c.ManhattanDistance()).ToList();

                for (int i = 0; i < otherReport.Count; i++)
                {
                    var candidateSameReferenceBeacon = otherReport[i];
                    var otherBeacons = otherReport.Except(new[] { candidateSameReferenceBeacon }).OrderBy(c => c.ManhattanDistance()).ToArray();

                    int hit = 0;
                    for (int b = 0; b < otherBeacons.Length; b++)
                    {
                        var otherBeacon = otherBeacons[b];
                        var otherReferenceBeacon = GetRelativeCoordinate(candidateSameReferenceBeacon, otherBeacon);

                        var distA = otherReferenceBeacon.ManhattanDistance();
                        foreach(var beacon in list)
                        {
                            var distB = beacon.ManhattanDistance();
                            if(distA == distB)
                            {
                                hit++;
                                if(hit >= 11)
                                {
                                    var possibleMatchingTransformedCoord = GetTransformations(otherReferenceBeacon);
                                    for (int transformationId = 0; transformationId < possibleMatchingTransformedCoord.Length; transformationId++)
                                    {
                                        if (beacon == (possibleMatchingTransformedCoord[transformationId].x, possibleMatchingTransformedCoord[transformationId].y, possibleMatchingTransformedCoord[transformationId].z))
                                        {
                                            return (transformationId, GetRelativeCoordinate(GetTransformations(candidateSameReferenceBeacon)[transformationId], referenceBeacon));
                                        }
                                    }
                                }
                            }
                            else if (distB > distA)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static (int x, int y, int z) GetRelativeCoordinate((int x, int y, int z) firstBeacon, (int x, int y, int z) otherBeacon)
        {
            return (x: otherBeacon.x - firstBeacon.x, y: otherBeacon.y - firstBeacon.y, z: otherBeacon.z - firstBeacon.z);
        }

        private static (int x, int y, int z) GetAbsoluteCoordinate((int x, int y, int z) referenceOffset, (int x, int y, int z) beacon)
        {
            return (x: referenceOffset.x + beacon.x, y: referenceOffset.y + beacon.y, z: referenceOffset.z + beacon.z);
        }

        private static (int x, int y, int z)[] GetTransformations((int x, int y, int z) coord)
        {
            return new (int x, int y, int z)[]
            {
                (x: coord.x, y: coord.y, z: coord.z * -1),
                (x: coord.x, y: coord.z, z: coord.y * -1),
                (x: coord.x, y: coord.y * -1, z: coord.z * -1),
                (x: coord.x, y: coord.z * -1, z: coord.y),
                (x: coord.y, y: coord.x, z: coord.z * -1),
                (x: coord.y, y: coord.z, z: coord.x),
                (x: coord.y, y: coord.x * -1, z: coord.z),
                (x: coord.y, y: coord.z * -1, z: coord.x * -1),
                (x: coord.z, y: coord.x, z: coord.y),
                (x: coord.z, y: coord.y, z: coord.x * -1),
                (x: coord.z, y: coord.x * -1, z: coord.y * -1),
                (x: coord.z, y: coord.y * -1, z: coord.x),
                (x: coord.x * -1, y: coord.y, z: coord.z * -1),
                (x: coord.x * -1, y: coord.z, z: coord.y),
                (x: coord.x * -1, y: coord.y * -1, z: coord.z),
                (x: coord.x * -1, y: coord.z * -1, z: coord.y * -1),
                (x: coord.y * -1, y: coord.x, z: coord.z),
                (x: coord.y * -1, y: coord.z, z: coord.x * -1),
                (x: coord.y * -1, y: coord.x * -1, z: coord.z * -1),
                (x: coord.y * -1, y: coord.z * -1, z: coord.x),
                (x: coord.z * -1, y: coord.x, z: coord.y * -1),
                (x: coord.z * -1, y: coord.y, z: coord.x),
                (x: coord.z * -1, y: coord.x * -1, z: coord.y),
                (x: coord.z * -1, y: coord.y * -1, z: coord.x * -1),

                (x: coord.x, y: coord.y, z: coord.z),
            };
        }

        private static (int x, int y, int z) StringToCoords(string str)
        {
            var coords = str.Split(",").Select(int.Parse).ToArray();
            return (x: coords[0], y: coords[1], z: coords[2]);
        }
    }
}