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

            List<(int scanner, List<(int x, int y, int z)> coords)> reports = new List<(int scanner, List<(int x, int y, int z)> coords)>();
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
                reports.Add((scannerId, list));
            }

            DeduceMap(reports);
        }

        private static void DeduceMap(List<(int scanner, List<(int x, int y, int z)> coords)> reports)
        {
            var locations = new Nullable<(int x, int y, int z)>[reports.Count];
            var baseReport = reports[0];
            locations[0] = (0, 0, 0);
            for (int i = 1; i < reports.Count; i++)
            {
                var scannerLocation = MakeRelativeToSameBeacon(baseReport, reports[i]);
                locations[i] = scannerLocation;
                if (scannerLocation.HasValue)
                {
                    System.Console.WriteLine("Found " + i + " at ");
                    PrintCoord(scannerLocation.Value);
                }
            }
        }

        private static Nullable<(int x, int y, int z)> MakeRelativeToSameBeacon((int scanner, List<(int x, int y, int z)> coords) firstReport, (int scanner, List<(int x, int y, int z)> coords) otherReport)
        {
            var requiredMatches = firstReport.coords.Any(fr => fr.z != 0) ? 11 : 2;
            for (int j = 0; j < firstReport.coords.Count; j++)
            {
                List<(int x, int y, int z)> list = new List<(int x, int y, int z)>();
                var referenceBeacon = firstReport.coords[j];
                foreach (var otherBeacon in firstReport.coords.Except(new [] { referenceBeacon }))
                {
                    var coord = GetRelativeCoordinate(referenceBeacon, otherBeacon);
                    list.Add(coord);
                }

                if(j ==0)
                {
                    list.ForEach(PrintCoord);
                }
                System.Console.WriteLine("-");

                // All beacons in the first report are now relative to a beacon. Let's see if we can find the same beacon in another report
                for (int i = 0; i < otherReport.coords.Count; i++)
                {
                    var fb = otherReport.coords[i];
                    var otherCoords = otherReport.coords.Except(new[] { fb });

                    List<(int perspectiveId, int x, int y, int z)> list2 = new List<(int perspectiveId, int x, int y, int z)>();
                    foreach (var otherBeacon in otherCoords)
                    {
                        var relCoord = GetRelativeCoordinate(fb, otherBeacon);
                        var possibleCoord = GeneratePerspectives(relCoord);
                        for(int perspectiveId = 0; perspectiveId < possibleCoord.Length; perspectiveId++)
                        {
                            list2.Add((perspectiveId, possibleCoord[perspectiveId].x, possibleCoord[perspectiveId].y, possibleCoord[perspectiveId].z));
                        }
                    }

                    var matches = Intersect(list, list2);
                    
                    if (matches.Count() >= requiredMatches)
                    {
                        var perspectiveId = matches.First().perspectiveId;
                        var possibleCoord = GeneratePerspectives(fb)[perspectiveId];
                        return GetRelativeCoordinate(possibleCoord, referenceBeacon);
                    }
                }
            }
            return null;
        }
        
        private static IEnumerable<(int perspectiveId, int x, int y, int z)> Intersect(List<(int x, int y, int z)> list, List<(int perspectiveId, int x, int y, int z)> list2)
        {
            foreach(var i in list2)
            {
                foreach(var j in list)
                {
                    if(j.x == i.x && j.y == i.y && j.z == i.z)
                    {
                        yield return i;
                    }
                }
            }

        }

        private static void PrintCoord((int x, int y, int z) coord)
        {
            System.Console.WriteLine("X: " + coord.x + " Y: " + coord.y + " Z: " + coord.z);
        }

        private static (int x, int y, int z) GetRelativeCoordinate((int x, int y, int z) firstBeacon, (int x, int y, int z) otherBeacon)
        {
            return (x: otherBeacon.x - firstBeacon.x, y: otherBeacon.y - firstBeacon.y, z: otherBeacon.z - firstBeacon.z);
        }

        private static (int x, int y, int z)[] GeneratePerspectives((int x, int y, int z) coord)
        {
            return new (int x, int y, int z)[]
            {
                (x: coord.x * -1, y: coord.y, z: coord.z * -1),
                (x: coord.x * -1, y: coord.y, z: coord.z),

                (x: coord.x, y: coord.y, z: coord.z),
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
                (x: coord.x* -1, y: coord.z, z: coord.y * -1),
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
