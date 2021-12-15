using Aoc2021.Lib;

namespace Aoc2021;

public class Day07
{
    public static long Part1() => Solve(d => d);

    public static long Part2() => Solve(d => d * (d + 1) / 2);

    private static long Solve(Func<long, long> calcFuel)
    {
        var crabs = Input.Text(7).Trim().Split(',').Select(int.Parse).ToArray();
        return Enumerable.Range(crabs.Min(), crabs.Max() - crabs.Min() + 1)
            .Min(pos => crabs.Select(c => calcFuel(Math.Abs(pos - c))).Sum());
    }
}