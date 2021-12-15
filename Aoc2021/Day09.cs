using System.Collections.Generic;
using System.Linq;
using Aoc2021.Lib;

namespace Aoc2021;

public class Day09
{
    private static readonly (int y, int x)[] offsets = { (-1, 0), (1, 0), (0, -1), (0, 1) };

    public static int Part1()
    {
        var grid = LoadGrid();
        return CalcLowPoints(grid).Sum(coord => grid[coord.y, coord.x] + 1);
    }

    public static int Part2()
    {
        var grid = LoadGrid();
        return CalcLowPoints(grid)
            .Select(lp => CalcBasin(grid, lp))
            .OrderByDescending(b => b.Count)
            .Take(3)
            .Aggregate(1, (acc, b) => acc * b.Count);
    }

    private static Matrix<int> LoadGrid()
    {
        var lines = Input.Lines(9);
        return new Matrix<int>(
            lines.Length,
            lines[0].Length,
            lines.SelectMany(line => line.Select(c => int.Parse(c.ToString())))
        );
    }

    private static List<(int y, int x)> CalcLowPoints(Matrix<int> grid)
    {
        var lowPoints = new List<(int y, int x)>();

        for (var y = 0; y < grid.Height; y ++)
        {
            for (var x = 0; x < grid.Width; x ++)
            {
                var h = grid[y, x];
                var isLow = !offsets
                    .Select(offset => (y: y + offset.y, x: x + offset.x))
                    .Any(c => grid.Contains(c.y, c.x) && grid[c.y, c.x] <= h);

                if (isLow)
                {
                    lowPoints.Add((y, x));
                }
            }
        }

        return lowPoints;
    }

    private static HashSet<(int y, int x)> CalcBasin(Matrix<int> grid, (int, int) lowPoint)
    {
        var result = new HashSet<(int y, int x)> { lowPoint };
        var todo = new Queue<(int y, int x)>(result);

        while (todo.Any())
        {
            var (y, x) = todo.Dequeue();
            foreach (var (dy, dx) in offsets)
            {
                var (y2, x2) = (y + dy, x + dx);
                if (grid.Contains(y2, x2) && grid[y2, x2] < 9 && result.Add((y2, x2)))
                {
                    todo.Enqueue((y2, x2));
                }
            }
        }

        return result;
    }
}