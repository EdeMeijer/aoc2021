using System;
using System.Diagnostics;

namespace Aoc2021.Lib
{
    public static class Runner
    {
        public static void Run<R1, R2>(Func<R1> part1, Func<R2> part2, bool benchmark = true)
        {
            if (benchmark)
            {
                // Warm up for better benchmarking
                for (var i = 0; i < 10; i ++)
                {
                    part1();
                    part2();
                }
            }

            Console.WriteLine("======== PART 1 ========");
            var sw = Stopwatch.StartNew();
            Console.WriteLine($"Answer = {part1()}");

            if (benchmark)
            {
                Console.WriteLine($"Runtime = {sw.Elapsed}");
            }

            Console.WriteLine("======== PART 2 ========");
            sw.Restart();
            Console.WriteLine($"Answer = {part2()}");
            if (benchmark)
            {
                Console.WriteLine($"Runtime = {sw.Elapsed}");
            }
        }
    }
}
