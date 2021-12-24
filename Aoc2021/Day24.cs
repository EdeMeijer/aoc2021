namespace Aoc2021;

public class Day24
{
    public static long Part1()
    {
        return result.Value.biggest;
    }

    public static long Part2()
    {
        return result.Value.smallest;
    }

    private static readonly Lazy<(long biggest, long smallest)> result;

    static Day24()
    {
        result = new Lazy<(long biggest, long smallest)>(Solve);
    }
    
    private static (long biggest, long smallest) Solve()
    {
        var (biggest, smallest) = (0L, long.MaxValue);
        Solve(13, 0, 0, ref smallest, ref biggest);
        return (biggest, smallest);
    }

    private static readonly int[] modX = { 10, 11, 14, 13, -6, -14, 14, 13, -8, -15, 10, -11, -13, -4 };
    private static readonly int[] modY = { 1, 9, 12, 6, 9, 15, 7, 12, 15, 3, 6, 2, 10, 12 };
    private static readonly int[] modZ = { 1, 1, 1, 1, 26, 26, 1, 1, 26, 26, 1, 26, 26, 26 };

    private static readonly Dictionary<(int step, int zOut), List<(int digit, List<int> zIn)>> cache = new();

    private static void Solve(int step, int zOut, long sequence, ref long resultMin, ref long resultMax)
    {
        var inputs = GetValidInputs(step, zOut);

        foreach (var (digit, zIns) in inputs)
        {
            var tmpSeq = sequence + digit * PowerOf10(13 - step);

            foreach (var zIn in zIns)
            {
                if (step == 0)
                {
                    if (zIn == 0)
                    {
                        // Found a valid sequence
                        if (tmpSeq > resultMax)
                        {
                            resultMax = tmpSeq;
                        }
                        if (tmpSeq < resultMin)
                        {
                            resultMin = tmpSeq;
                        }
                    }
                    continue;
                }
                
                Solve(step - 1, zIn, tmpSeq, ref resultMin, ref resultMax);
            }
        }
    }

    private static long PowerOf10(int power)
    {
        long result = 1;
        for (int i = 0; i < power; i ++)
        {
            result *= 10;
        }

        return result;
    }

    private static List<(int digit, List<int> zIns)> GetValidInputs(int step, int zOut)
    {
        var key = (step, zOut);
        if (cache.TryGetValue(key, out var result))
        {
            return result;
        }

        result = new List<(int digit, List<int> zIns)>();
        var zInCandidates = GenerateZInCandidates(zOut);
        
        for (var digit = 1; digit <= 9; digit ++)
        {
            var zIns = new List<int>();
            
            foreach (var zIn in zInCandidates)
            {
                var z = EvalBlock(step, digit, zIn);
                if (z == zOut)
                {
                    zIns.Add(zIn);
                }
            }

            if (zIns.Any())
            {
                result.Add((digit, zIns));
            }
        }
        
        cache.Add(key, result);

        return result;
    }

    private static HashSet<int> GenerateZInCandidates(int zOut)
    {
        var result = new HashSet<int>();
        int[] points = { zOut / 26, zOut, zOut * 26 };
        foreach (var point in points)
        {
            for (var zIn = point - 25; zIn <= point + 25; zIn ++)
            {
                result.Add(zIn);
            }
        }

        return result;
    }

    private static int EvalBlock(int step, int digit, int zPrev)
    {
        var (w, x, y2, z) = (digit, zPrev, 25, zPrev);
        x %= 26;
        x += modX[step];
        var mask = w == x ? 0 : 1;

        var y1 = digit + modY[step];
        y1 *= mask;

        y2 *= mask;
        y2 ++;
        
        z /= modZ[step];

        z *= y2;
        z += y1;

        return z;
    }
}
