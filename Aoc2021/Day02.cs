using Aoc2021.Lib;

namespace Aoc2021;

public class Day02
{
    private static readonly Dictionary<string, (int x, int d)> vectors = new()
    {
        ["forward"] = (1, 0),
        ["down"] = (0, 1),
        ["up"] = (0, -1)
    };
            
    public static int Part1()
    {
        var (d, x) = (0, 0);
        foreach (var Line in Input.Lines(2))
        {
            var parts = Line.Split(' ');
            var (cmd, value) = (parts[0], int.Parse(parts[1]));
            var (vx, vd) = vectors[cmd];
            d += vd * value;
            x += vx * value;
        }

        return d * x;
    }

    public static int Part2()
    {
        var (d, x, aim) = (0, 0, 0);
        foreach (var Line in Input.Lines(2))
        {
            var parts = Line.Split(' ');
            var (cmd, value) = (parts[0], int.Parse(parts[1]));
            if (cmd == "forward")
            {
                x += value;
                d += aim * value;
            }
            else
            {
                aim += value * (cmd == "down" ? 1 : -1);
            }
        }

        return d * x;
    }
}