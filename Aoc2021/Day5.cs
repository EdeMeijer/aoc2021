using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Aoc2021.Lib;

namespace Aoc2021
{
    public class Day5
    {
        public static int Part1() => Solve(false);

        public static int Part2() => Solve(true);
        
        private static int Solve(bool includeDiag)
        {
            var input = Load();
            var counts = new ConcurrentDictionary<(int, int), int>();
            foreach (var line in input)
            {
                var dx = Math.Clamp(line.to.x - line.from.x, -1, 1);
                var dy = Math.Clamp(line.to.y - line.from.y, -1, 1);
                if (dx == 0 || dy == 0 || includeDiag)
                {
                    var cur = line.from;
                    counts.AddOrUpdate(cur, _ => 1, (_, c) => c + 1);
                    while (cur != line.to)
                    {
                        cur = (cur.x + dx, cur.y + dy);
                        counts.AddOrUpdate(cur, _ => 1, (_, c) => c + 1);
                    }
                }
            }
            
            return counts.Values.Count(c => c > 1);
        }

        private static List<((int x, int y) from, (int x, int y) to)> Load() => 
            Input.Lines(5)
                .Select(line => ToTuple(line.Split(" -> ").Select(ParseCoord).ToArray()))
                .ToList();

        private static (int x, int y) ParseCoord(string Coord) =>
            ToTuple(Coord.Split(',').Select(int.Parse).ToArray());

        private static (T, T) ToTuple<T>(IReadOnlyList<T> values) => (values[0], values[1]);
    }
}
