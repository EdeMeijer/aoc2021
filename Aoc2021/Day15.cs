using Aoc2021.Lib;

namespace Aoc2021;

public class Day15
{
    private static readonly (int y, int x)[] offsets = { (0, -1), (0, 1), (-1, 0), (1, 0) };

    public static int Part1() => Solve(LoadBaseMap());

    public static int Part2() => Solve(ExtendMap(LoadBaseMap(), 5));

    private static Matrix<int> LoadBaseMap()
    {
        var lines = Input.Lines(15);
        var height = lines.Length;
        var width = lines[0].Length;
        return new Matrix<int>(
            height,
            width,
            lines.SelectMany(line => line.Select(ch => int.Parse(ch.ToString())))
        );
    }

    private static Matrix<int> ExtendMap(Matrix<int> baseMap, int scale)
    {
        var extendedMap = new Matrix<int>(baseMap.Height * scale, baseMap.Width * scale, 0);
        for (var ry = 0; ry < scale; ry ++)
        {
            var offset = ry * baseMap.Height;
            foreach (var (y, x) in baseMap.Coords)
            {
                extendedMap[offset + y, x] = (baseMap[y, x] + ry - 1) % 9 + 1;
            }
        }

        for (var rx = 1; rx < scale; rx ++)
        {
            var offset = rx * baseMap.Width;
            for (var y = 0; y < extendedMap.Height; y ++)
            {
                for (var x = 0; x < baseMap.Width; x ++)
                {
                    extendedMap[y, offset + x] = (extendedMap[y, x] + rx - 1) % 9 + 1;
                }
            }
        }

        return extendedMap;
    }

    private static int Solve(Matrix<int> map)
    {
        var (height, width) = (map.Height, map.Width);
        var target = (y: height - 1, x: width - 1);

        var dist = new Matrix<int>(height, width, int.MaxValue)
        {
            [0, 0] = 0
        };
        var done = new Matrix<bool>(height, width, false);
        var todo = new PriorityQueue<(int y, int x), int>();
        todo.Enqueue((0, 0), 0);

        while (todo.Count > 0)
        {
            var (y, x) = todo.Dequeue();
            if ((y, x) == target)
            {
                return dist[target.y, target.x];
            }

            foreach (var (dy, dx) in offsets)
            {
                var (y2, x2) = (y + dy, x + dx);
                if (!done.Contains(y2, x2) || done[y2, x2]) continue;
                var altDist = dist[y, x] + map[y2, x2];
                if (altDist >= dist[y2, x2]) continue;
                dist[y2, x2] = altDist;
                todo.Enqueue((y2, x2), altDist);
            }

            done[y, x] = true;
        }

        throw new Exception();
    }
}
