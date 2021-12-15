using Aoc2021.Lib;

namespace Aoc2021;

public class Day14
{
    public static long Part1() => Solve(10);

    public static long Part2() => Solve(40);

    private static long Solve(int depth)
    {
        var parts = Input.Text(14).Split("\n\n");

        var template = parts[0];

        var rules = parts[1].Split("\n")
            .Select(line => line.Split(" -> "))
            .ToDictionary(tup => tup[0], tup => tup[1][0]);


        var cache = new Dictionary<(string pattern, int level), Dictionary<char, long>>();
        foreach (var tup in rules)
        {
            var value = new Dictionary<char, long>();
            addCount(value, tup.Key[0]);
            addCount(value, tup.Key[1]);
            addCount(value, tup.Value);
            cache[(tup.Key, 1)] = value;
        }

        Dictionary<char, long> getProduct(string pair, int level)
        {
            if (cache.TryGetValue((pair, level), out var result))
            {
                return result;
            }

            var insertion = rules[pair];
            var left = getProduct($"{pair[0]}{insertion}", level - 1);
            var right = getProduct($"{insertion}{pair[1]}", level - 1);

            result = new Dictionary<char, long>();
            addCount(result, left);
            addCount(result, right);
            // subtract the inserted character once to prevent counting it double
            addCount(result, insertion, -1);

            cache[(pair, level)] = result;
            return result;
        }

        var totalCounts = new Dictionary<char, long>();
        for (var i = 1; i < template.Length; i ++)
        {
            var pair = template[(i - 1)..(i + 1)];
            addCount(totalCounts, getProduct(pair, depth));
            if (i < template.Length - 1)
            {
                // All middle atoms are counted double, so account for that
                addCount(totalCounts, template[i], -1);
            }
        }

        return totalCounts.Values.Max() - totalCounts.Values.Min();
    }

    private static void addCount(Dictionary<char, long> counts, char ch, long num = 1)
    {
        if (!counts.ContainsKey(ch))
        {
            counts[ch] = num;
        }
        else
        {
            counts[ch] += num;
        }
    }

    private static void addCount(Dictionary<char, long> counts, Dictionary<char, long> values)
    {
        foreach (var kvp in values)
        {
            addCount(counts, kvp.Key, kvp.Value);
        }
    }
}