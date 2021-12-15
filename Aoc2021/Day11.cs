using Aoc2021.Lib;

namespace Aoc2021;

public class Day11
{
    public static int Part1()
    {
        var grid = LoadGrid();
        return Enumerable.Range(0, 100).Sum(_ => Step(grid));
    }

    public static int Part2()
    {
        var grid = LoadGrid();
        for (var step = 1;; step ++)
        {
            if (Step(grid) == grid.Height * grid.Width)
            {
                return step;
            }
        }
    }

    private static int Step(IMatrix<int> grid)
    {
        var flashes = 0;
        var flashQueue = new Queue<(int y, int x)>();

        void BumpEnergy(int y, int x)
        {
            grid[y, x] ++;
            if (grid[y, x] == 10)
            {
                flashQueue.Enqueue((y, x));
                flashes ++;
            }
        }
                
        foreach (var (y, x) in grid.Coords)
        {
            BumpEnergy(y, x);
        }

        while (flashQueue.Any())
        {
            var (y, x) = flashQueue.Dequeue();
            for (var dy = -1; dy <= 1; dy ++)
            {
                for (var dx = -1; dx <= 1; dx ++)
                {
                    var (y2, x2) = (y + dy, x + dx);
                    if (grid.Contains(y2, x2))
                    {
                        BumpEnergy(y2, x2);
                    }
                }
            }
        }

        foreach (var (y, x) in grid.Coords)
        {
            if (grid[y, x] > 9)
            {
                grid[y, x] = 0;
            }
        }

        return flashes;
    }

    private static Matrix<int> LoadGrid()
    {
        var lines = Input.Lines(11);
        return new Matrix<int>(lines.Length, lines[0].Length,
            lines.SelectMany(line => line.Select(ch => int.Parse(ch.ToString()))));
    }
}