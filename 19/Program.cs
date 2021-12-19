using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _19
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines(args[0]);

            List<List<(int x, int y, int z)>> reports = new List<List<(int x, int y, int z)>>();
            var inputQueue = new Queue<string>(input);
            while (inputQueue.Any())
            {
                var scannerId = int.Parse(inputQueue.Dequeue().Substring(12).Replace("-", string.Empty).Trim());
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

            var beacons = DeduceMap(reports);
            System.Console.WriteLine("Part 1: " + beacons.Count());
        }

        private static IEnumerable<(int x, int y, int z)> DeduceMap(List<List<(int x, int y, int z)>> reports)
        {
            var allReports = reports.ToList();

            var scannerLocations = new Nullable<(int x, int y, int z)>[allReports.Count];
            scannerLocations[0] = (0, 0, 0);
            while (scannerLocations.Any(l => l == null))
            {
                for (int i = 0; i < allReports.Count; i++)
                {
                    for (int j = 0; j < allReports.Count; j++)
                    {
                        if (scannerLocations[i] != null && scannerLocations[j] == null)
                        {
                            var absoluteScannerLocation = DetermineScannerLocation(allReports[i], allReports[j]);
                            if (absoluteScannerLocation.HasValue)
                            {
                                var toSameTransformation = allReports[j].Select(c => GetTransformations(c)[absoluteScannerLocation.Value.transformationId]).ToList();

                                var absoluted = toSameTransformation.Select(c => GetAbsoluteCoordinate(absoluteScannerLocation.Value.coord, c)).ToList();

                                allReports[j] = absoluted;
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
                    var dist = GetRelativeCoordinate(scannerA.Value, scannerB.Value);
                    var sum = Math.Abs(dist.x) + Math.Abs(dist.y) + Math.Abs(dist.z);
                    if(sum > maxDistance)
                    {
                        maxDistance = sum;
                    }
                }
            }
            System.Console.WriteLine("Part 2: " + maxDistance);

            return allReports.SelectMany(ar => ar).Distinct();
        }

        private static Nullable<(int transformationId, (int x, int y, int z) coord)> DetermineScannerLocation(List<(int x, int y, int z)> firstReport, List<(int x, int y, int z)> otherReport)
        {
            var requiredMatches = firstReport.Any(fr => fr.z != 0) ? 11 : 2;
            for (int j = 0; j < firstReport.Count; j++)
            {
                var referenceBeacon = firstReport[j];
                List<(int x, int y, int z)> list = firstReport.Except(new[] { referenceBeacon }).Select(b => GetRelativeCoordinate(referenceBeacon, b)).ToList();

                for (int i = 0; i < otherReport.Count; i++)
                {
                    var fb = otherReport[i];
                    var otherBeacons = otherReport.Except(new[] { fb });

                    List<(int transformationId, int x, int y, int z)> list2 = new List<(int transformationId, int x, int y, int z)>();
                    foreach (var otherBeacon in otherBeacons)
                    {
                        var otherReferenceBeacon = GetRelativeCoordinate(fb, otherBeacon);
                        var possibleCoord = GetTransformations(otherReferenceBeacon);
                        for (int transformationId = 0; transformationId < possibleCoord.Length; transformationId++)
                        {
                            if (list.Contains((possibleCoord[transformationId].x, possibleCoord[transformationId].y, possibleCoord[transformationId].z)))
                            {
                                list2.Add((transformationId, possibleCoord[transformationId].x, possibleCoord[transformationId].y, possibleCoord[transformationId].z));
                                if (list2.Count() >= requiredMatches)
                                {
                                    return (transformationId, GetRelativeCoordinate(GetTransformations(fb)[transformationId], referenceBeacon));
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static void PrintCoord((int x, int y, int z) coord)
        {
            System.Console.WriteLine(coord.x + "," + coord.y + "," + coord.z);
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
            return (x: coords[0], y: coords[1], z: coords.Length > 2 ? coords[2] : 0);
        }
    }
}
