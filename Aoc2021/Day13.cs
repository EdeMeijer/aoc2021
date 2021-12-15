using System.Collections.Generic;
using System.Linq;
using Aoc2021.Lib;

namespace Aoc2021;

public class Day13
{
    public static int Part1() => Solve(1).Count;

    public static string Part2()
    {
        var dots = Solve(null);

        IMatrix<char> grid = new Matrix<char>(
            dots.Max(dot => dot.y) + 1,
            dots.Max(dot => dot.x) + 1,
            '.'
        );
        foreach (var (y, x) in dots)
        {
            grid[y, x] = '#';
        }

        return grid.ToString();
    }

    private static HashSet<(int y, int x)> Solve(int? maxInstructions)
    {
        var lines = Input.Text(13);
        var parts = lines.Split("\n\n");
        var dots = parts[0].Split("\n").Select(ParseDot).ToHashSet();
        var instructions = parts[1].Split("\n").Select(ParseInstruction).ToList();

        if (maxInstructions != null)
        {
            instructions = instructions.Take(maxInstructions.Value).ToList();
        }

        return instructions.Aggregate(
            dots,
            (current, instruction) => Fold(current, instruction.axis, instruction.value)
        );
    }

    private static HashSet<(int y, int x)> Fold(HashSet<(int y, int x)> dots, string axis, int position)
    {
        return dots.Select(dot =>
        {
            if (axis == "x" && dot.x > position)
            {
                return (dot.y, position - (dot.x - position));
            }

            if (axis == "y" && dot.y > position)
            {
                return (position - (dot.y - position), dot.x);
            }

            return dot;
        }).ToHashSet();
    }

    private static (int y, int x) ParseDot(string dot)
    {
        var parts = dot.Split(',').Select(int.Parse).ToArray();
        return (parts[1], parts[0]);
    }

    private static (string axis, int value) ParseInstruction(string instruction)
    {
        var relevant = instruction.Split(' ')[2];
        var parts = relevant.Split('=');
        return (parts[0], int.Parse(parts[1]));
    }
}