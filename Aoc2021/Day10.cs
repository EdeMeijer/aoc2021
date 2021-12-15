using Aoc2021.Lib;

namespace Aoc2021;

public class Day10
{
    public static long Part1()
    {
        var scores = new long[] { 3, 57, 1197, 25137 };
        var score = 0L;
        Evaluate(i => score += scores[i]);
        return score;
    }

    public static long Part2()
    {
        var scores = new List<long>();
        Evaluate(null, stack => scores.Add(stack.Aggregate(0L, (cur, i) => cur * 5 + i + 1)));
        return scores.OrderBy(s => s).Skip(scores.Count / 2).First();
    }

    public static void Evaluate(Action<int>? onInvalid = null, Action<Stack<int>>? onIncomplete = null)
    {
        const string open = "([{<";
        const string close = ")]}>";

        foreach (var line in Input.Lines(10))
        {
            var stack = new Stack<int>();
            var invalid = false;
            foreach (var ch in line)
            {
                var openI = open.IndexOf(ch);
                if (openI > -1)
                {
                    stack.Push(openI);
                }
                else
                {
                    var closeI = close.IndexOf(ch);
                    if (closeI == stack.Peek())
                    {
                        stack.Pop();
                    }
                    else
                    {
                        onInvalid?.Invoke(closeI);
                        invalid = true;
                        break;
                    }
                }
            }

            if (!invalid && stack.Any())
            {
                onIncomplete?.Invoke(stack);
            }
        }
    }
}