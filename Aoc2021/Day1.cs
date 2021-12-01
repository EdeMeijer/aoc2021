using System.Linq;
using Aoc2021.Lib;

namespace Aoc2021
{
    public class Day1
    {
        public static int Part1() => CountIncrements(LoadInput());

        public static int Part2()
        {
            var depths = LoadInput();
            var Windows = Enumerable.Range(0, depths.Length - 2).Select(offset => depths[offset..(offset + 3)].Sum());
            return CountIncrements(Windows.ToArray());
        }

        private static int[] LoadInput() => Input.Lines(1).Select(int.Parse).ToArray();

        private static int CountIncrements(int[] depths) =>
            depths.SkipLast(1).Zip(depths.Skip(1)).Count(tup => tup.Second > tup.First);
    }
}
