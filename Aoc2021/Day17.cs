using System.Text.RegularExpressions;
using Aoc2021.Lib;

namespace Aoc2021;

public class Day17
{
    private static readonly Regex inputPattern = new Regex(@": x=([\d-]+)\.\.([\d-]+), y=([\d-]+)\.\.([\d-]+)");

    public static int Part1()
    {
        var bounds = LoadInput();

        // We know that when launching upwards, the positions of the downward trajectory are a mirror image of the
        // positions of the upward trajectory. Hence, the best we can do is go in 1 step down into the lowest row of
        // the target area. So, at the second last step, the y velocity should equal yMin. That means that the launch
        // velocity in the y direction should be 1 point less in the other direction.
        var launchY = -bounds.yMin - 1;

        // The max position will be in the form of 1+2+3+...+launchY
        return launchY * (launchY + 1) / 2;
    }

    public static int Part2()
    {
        var bounds = LoadInput();

        // We know the highest y velocity that gets it in the target area. The other extreme is shooting it downwards
        // with a velocity of yMin and get it into the target in 1 step with a high enough x velocity.
        var vyMax = -bounds.yMin - 1;
        var vyMin = bounds.yMin;

        // For x, we can calculate a global minimum and maximum velocity. The minimum is where vx * (vx + 1) / 2 >= xMin
        // and the max is shooting it to the maximum target x value in one step.
        var vxMin = CalcMinVx(bounds.xMin);
        var vxMax = bounds.xMax;

        // Now just simulate all possible combinations in the search grid
        var result = 0;
        for (var vy = vyMin; vy <= vyMax; vy ++)
        {
            for (var vx = vxMin; vx <= vxMax; vx ++)
            {
                if (HitsTarget(vy, vx, bounds))
                {
                    result ++;
                }
            }
        }

        return result;
    }

    private static (int xMin, int xMax, int yMin, int yMax) LoadInput()
    {
        var input = Input.Text(17);
        var bounds = inputPattern.Match(input).Groups.Values.Skip(1).Select(group => int.Parse(group.Value)).ToArray();
        return (bounds[0], bounds[1], bounds[2], bounds[3]);
    }

    private static int CalcMinVx(int xMin)
    {
        for (var vx = 1;; vx ++)
        {
            if (vx * (vx + 1) / 2 >= xMin)
            {
                return vx;
            }
        }
    }

    private static bool HitsTarget(int vy, int vx, (int xMin, int xMax, int yMin, int yMax) target)
    {
        var (x, y) = (0, 0);

        // Simulate until it either hits the box, or passes it
        while (x <= target.xMax && y >= target.yMin)
        {
            (y, x) = (y + vy, x + vx);
            if (y >= target.yMin && y <= target.yMax && x >= target.xMin && x <= target.xMax)
            {
                return true;
            }

            (vy, vx) = (vy - 1, Math.Max(vx - 1, 0));
        }

        return false;
    }
}
