using System.Linq;
using Aoc2021.Lib;

namespace Aoc2021
{
    public class Day6
    {
        public static long Part1() => Solve(80);

        public static long Part2() => Solve(256);

        private static long Solve(int days)
        {
            var buckets = Input.Text(6).Trim().Split(',').Select(int.Parse).GroupBy(c => c)
                .ToDictionary(group => group.Key, group => (long)group.Count());

            for (var i = 0; i < days; i ++)
            {
                var newBuckets = Enumerable.Range(0, 9).ToDictionary(c => c, _ => 0L);
                foreach (var kvp in buckets)
                {
                    var newCounter = kvp.Key - 1;
                    if (newCounter == -1)
                    {
                        newBuckets[8] += kvp.Value;
                        newCounter = 6;
                    }

                    newBuckets[newCounter] += kvp.Value;
                }

                buckets = newBuckets;
            }

            return buckets.Sum(kvp => kvp.Value);
        }
    }
}
