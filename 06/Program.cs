using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _06
{
    class Program
    {
        static void Main(string[] args)
        {
            var lanternFish = File
                .ReadAllLines(args[0])
                .SelectMany(line => line.Split(","))
                .Select(int.Parse)
                .GroupBy(d => d)
                .Select(bucket => new LanternFishBucket(bucket.Key, bucket.Count()));

            School school = new School(lanternFish);

            System.Console.WriteLine("Initial state " + school.ToString());
            foreach (var iteration in Enumerable.Range(1, 256))
            {
                school.Cycle();
                System.Console.WriteLine($"After {iteration} days: {school.ToString()}");
            }
            System.Console.WriteLine($"There are now {school.FishBuckets.Sum(fb => fb.AmountOfFish)} fish");
        }

        private class LanternFishBucket
        {
            public int Timer { get; private set; }
            public long AmountOfFish { get; private set; }
            public LanternFishBucket(int timer, long amountOfFish)
            {
                this.Timer = timer;
                this.AmountOfFish = amountOfFish;
            }
            
            // Returns true when the fish are spawning new fish
            public bool DecreaseTimer()
            {
                if (Timer == 0)
                {
                    Timer = 6;
                    return true;
                }
                Timer--;
                return false;
            }

            public void AddFish(long amount)
            {
                AmountOfFish += amount;
            }
        }

        private class School
        {
            public IList<LanternFishBucket> FishBuckets { get; }
            public School(IEnumerable<LanternFishBucket> initialFishBucket)
            {
                FishBuckets = initialFishBucket.ToList();
            }

            public void SpawnLanternFish(long amount)
            {
                if (!FishBuckets.Any(fb => fb.Timer == 8))
                {
                    FishBuckets.Add(new LanternFishBucket(8, amount));
                }
                else
                {
                    FishBuckets
                        .Where(fb => fb.Timer == 8)
                        .Single()
                        .AddFish(amount);
                }
            }

            public void Cycle()
            {
                long amountToCreate = 0;
                foreach (var fishBucket in FishBuckets)
                {
                    if (fishBucket.DecreaseTimer())
                    {
                        amountToCreate = fishBucket.AmountOfFish;
                    }
                }

                SpawnLanternFish(amountToCreate);

                var toMerge = FishBuckets.GroupBy(fb => fb.Timer).Where(f => f.Count() > 1);
                foreach (var bucketToMerge in toMerge)
                {
                    var duplicates = bucketToMerge.Skip(1);
                    bucketToMerge.First().AddFish(duplicates.Sum(b => b.AmountOfFish));
                    duplicates.ToList().ForEach(fb => FishBuckets.Remove(fb));
                }
            }

            public override String ToString()
            {
                return String.Join(',', FishBuckets.Select(f => f.Timer.ToString() + "x" + f.AmountOfFish));
            }
        }
    }
}