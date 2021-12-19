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

            DeduceMap(reports);
        }

        private static void DeduceMap(List<List<(int x, int y, int z)>> reports)
        {
            var locations = new Nullable<(int x, int y, int z)>[reports.Count];
            var baseReport = reports[0];
            locations[0] = (0, 0, 0);
            while (locations.Any(l => l == null))
            {
                for (int i = 0; i < reports.Count; i++)
                {
                    for (int j = 0; j < reports.Count; j++)
                    {
                        if (locations[j] == null && locations[i] != null)
                        {
                            var scannerLocation = MakeRelativeToSameBeacon(reports[i], reports[j]);
                            if (scannerLocation.HasValue)
                            {
                                System.Console.WriteLine("Found " + j + " by comparing to " + i + " at ");
                                PrintCoord(scannerLocation.Value.coord);

                                var reverseTransformation = MakeRelativeToSameBeacon(reports[j], reports[i]).Value.perspectiveId;
                                System.Console.WriteLine("Reverse = " + reverseTransformation);
                                var reversed = reports[j].Select(c => GetAbsoluteCoordinate(scannerLocation.Value.coord, GetPerspectives(c)[reverseTransformation])).ToList();

                                if (reports[i].Intersect(reversed).Count() >= 12)
                                {
                                    reports[j] = reversed;
                                    locations[j] = scannerLocation.Value.coord;
                                }
                                else
                                {
                                    System.Console.WriteLine("NOPE");
                                }
                            }
                        }
                    }
                }
            }
        }

        private static Nullable<(int perspectiveId, (int x, int y, int z) coord)> MakeRelativeToSameBeacon(List<(int x, int y, int z)> firstReport, List<(int x, int y, int z)> otherReport)
        {
            var requiredMatches = firstReport.Any(fr => fr.z != 0) ? 11 : 2;
            for (int j = 0; j < firstReport.Count; j++)
            {
                List<(int x, int y, int z)> list = new List<(int x, int y, int z)>();
                var referenceBeacon = firstReport[j];
                foreach (var otherBeacon in firstReport.Except(new[] { referenceBeacon }))
                {
                    var coord = GetRelativeCoordinate(referenceBeacon, otherBeacon);
                    list.Add(coord);
                }

                // All beacons in the first report are now relative to a beacon. Let's see if we can find the same beacon in another report
                for (int i = 0; i < otherReport.Count; i++)
                {
                    var fb = otherReport[i];
                    var otherCoords = otherReport.Except(new[] { fb });

                    List<(int perspectiveId, int x, int y, int z)> list2 = new List<(int perspectiveId, int x, int y, int z)>();
                    foreach (var otherBeacon in otherCoords)
                    {
                        var relCoord = GetRelativeCoordinate(fb, otherBeacon);
                        var possibleCoord = GetPerspectives(relCoord);
                        for (int perspectiveId = 0; perspectiveId < possibleCoord.Length; perspectiveId++)
                        {
                            if (list.Contains((possibleCoord[perspectiveId].x, possibleCoord[perspectiveId].y, possibleCoord[perspectiveId].z)))
                            {
                                list2.Add((perspectiveId, possibleCoord[perspectiveId].x, possibleCoord[perspectiveId].y, possibleCoord[perspectiveId].z));

                                if (list2.Count() >= requiredMatches)
                                {
                                    return (perspectiveId, GetRelativeCoordinate(GetPerspectives(fb)[perspectiveId], referenceBeacon));
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
            System.Console.WriteLine("X: " + coord.x + " Y: " + coord.y + " Z: " + coord.z);
        }

        private static (int x, int y, int z) GetRelativeCoordinate((int x, int y, int z) firstBeacon, (int x, int y, int z) otherBeacon)
        {
            return (x: otherBeacon.x - firstBeacon.x, y: otherBeacon.y - firstBeacon.y, z: otherBeacon.z - firstBeacon.z);
        }

        private static (int x, int y, int z) GetAbsoluteCoordinate((int x, int y, int z) referenceOffset, (int x, int y, int z) beacon)
        {
            return (x: referenceOffset.x + beacon.x, y: referenceOffset.y + beacon.y, z: referenceOffset.z + beacon.z);
        }

        private static (int x, int y, int z)[] GetPerspectives((int x, int y, int z) coord)
        {
            return new (int x, int y, int z)[]
            {
                (x: coord.x * -1, y: coord.y, z: coord.z * -1),
                (x: coord.x, y: coord.y, z: coord.z),
                (x: coord.x * -1, y: coord.y, z: coord.z),
                (x: coord.x, y: coord.y * -1, z: coord.z),
                (x: coord.x, y: coord.y, z: coord.z * -1),
                (x: coord.x * -1, y: coord.y * -1, z: coord.z),
                (x: coord.x, y: coord.y * -1, z: coord.z * -1),
                (x: coord.x * -1, y: coord.y * -1, z: coord.z * -1),
                (x: coord.x, y: coord.z, z: coord.y),
                (x: coord.x * -1, y: coord.z, z: coord.y),
                (x: coord.x, y: coord.z * -1, z: coord.y),
                (x: coord.x, y: coord.z, z: coord.y * -1),
                (x: coord.x * -1, y: coord.z * -1, z: coord.y),
                (x: coord.x * -1, y: coord.z, z: coord.y * -1),
                (x: coord.x, y: coord.z * -1, z: coord.y * -1),
                (x: coord.x * -1, y: coord.z * -1, z: coord.y * -1),
                (x: coord.z, y: coord.x, z: coord.y),
                (x: coord.z * -1, y: coord.x, z: coord.y),
                (x: coord.z, y: coord.x * -1, z: coord.y),
                (x: coord.z, y: coord.x, z: coord.y * -1),
                (x: coord.z * -1, y: coord.x * -1, z: coord.y),
                (x: coord.z * -1, y: coord.x, z: coord.y * -1),
                (x: coord.z, y: coord.x * -1, z: coord.y * -1),
                (x: coord.z * -1, y: coord.x * -1, z: coord.y * -1),
                (x: coord.z, y: coord.y, z: coord.x),
                (x: coord.z * -1, y: coord.y, z: coord.x),
                (x: coord.z, y: coord.y * -1, z: coord.x),
                (x: coord.z, y: coord.y, z: coord.x * -1),
                (x: coord.z * -1, y: coord.y * -1, z: coord.x),
                (x: coord.z * -1, y: coord.y, z: coord.x * -1),
                (x: coord.z, y: coord.y * -1, z: coord.x * -1),
                (x: coord.z * -1, y: coord.y * -1, z: coord.x * -1),
                (x: coord.y, y: coord.z, z: coord.x),
                (x: coord.y * -1, y: coord.z, z: coord.x),
                (x: coord.y, y: coord.z * -1, z: coord.x),
                (x: coord.y, y: coord.z, z: coord.x * -1),
                (x: coord.y * -1, y: coord.z * -1, z: coord.x),
                (x: coord.y, y: coord.z * -1, z: coord.x * -1),
                (x: coord.y * -1, y: coord.z, z: coord.x * -1),
                (x: coord.y * -1, y: coord.z * -1, z: coord.x * -1),
                (x: coord.y, y: coord.x, z: coord.z),
                (x: coord.y * -1, y: coord.x, z: coord.z),
                (x: coord.y, y: coord.x * -1, z: coord.z),
                (x: coord.y, y: coord.x, z: coord.z * -1),
                (x: coord.y * -1, y: coord.x * -1, z: coord.z),
                (x: coord.y, y: coord.x * -1, z: coord.z * -1),
                (x: coord.y * -1, y: coord.x, z: coord.z * -1),
                (x: coord.y * -1, y: coord.x * -1, z: coord.z * -1),
            };
        }

        private static (int x, int y, int z) StringToCoords(string str)
        {
            var coords = str.Split(",").Select(int.Parse).ToArray();
            return (x: coords[0], y: coords[1], z: coords.Length > 2 ? coords[2] : 0);
        }
    }
}
