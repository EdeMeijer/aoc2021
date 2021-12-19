using System.Diagnostics.CodeAnalysis;
using Aoc2021.Lib;

namespace Aoc2021;

public class Day18
{
    public static int Part1()
    {
        var lines = Input.Lines(18);
        var result = lines.Select(Parse).Aggregate(Add);
        return MagnitudeOf(result);
    }

    public static int Part2()
    {
        var lines = Input.Lines(18);
        var numbers = lines.Select(Parse).ToList();

        var max = 0;
        foreach (var a in numbers)
        {
            foreach (var b in numbers)
            {
                max = Math.Max(max, MagnitudeOf(Add(a, b)));
            }
        }

        return max;
    }

    abstract record Token
    {
        public static implicit operator Token(char val) => new CharToken(val);
        public static implicit operator Token(int val) => new NumToken(val);
    }

    record NumToken(int value) : Token;

    record CharToken(char value) : Token;

    private static List<Token> Parse(string value)
    {
        return value.Select(Token(ch) =>
        {
            if (ch == '[' || ch == ']' || ch == ',')
            {
                return ch;
            }
            else
            {
                return int.Parse(ch.ToString());
            }
        }).ToList();
    }

    private static List<Token> Add(List<Token> left, List<Token> right) => Reduce(
        new Token[] { '[' }
            .Concat(left)
            .Append(',')
            .Concat(right)
            .Append(']')
            .ToList()
    );

    private static List<Token> Reduce(List<Token> tokens)
    {
        for (;;)
        {
            List<Token>? newTokens;
            if (TryExplode(tokens, out newTokens))
            {
                tokens = newTokens;
                continue;
            }

            if (TrySplit(tokens, out newTokens))
            {
                tokens = newTokens;
                continue;
            }

            return tokens;
        }
    }

    private static bool TryExplode(List<Token> tokens, [NotNullWhen(true)] out List<Token>? result)
    {
        result = null;
        var depth = 0;
        for (var i = 0; i < tokens.Count; i ++)
        {
            var token = tokens[i];
            if (token is CharToken { value : '[' })
            {
                depth ++;
                if (depth == 5)
                {
                    var replaceStart = i;
                    var replaceEnd = replaceStart + 5;
                    var leftChunk = tokens.Take(replaceStart).ToList();
                    var rightChunk = tokens.Skip(replaceEnd).ToList();
                    var (numLeft, numRight) = ((NumToken)tokens[i + 1], (NumToken)tokens[i + 3]);

                    // add to first number on the right in left chunk
                    for (var j = leftChunk.Count - 1; j >= 0; j --)
                    {
                        if (leftChunk[j] is not NumToken intVal) continue;
                        leftChunk[j] = intVal.value + numLeft.value;
                        break;
                    }

                    // add to first number on the left in right chunk
                    for (var j = 0; j < rightChunk.Count; j ++)
                    {
                        if (rightChunk[j] is not NumToken intVal) continue;
                        rightChunk[j] = intVal.value + numRight.value;
                        break;
                    }

                    result = leftChunk.Concat(new Token[] { 0 }).Concat(rightChunk).ToList();

                    return true;
                }
            }

            if (token is CharToken { value : ']' })
            {
                depth --;
            }
        }

        return false;
    }

    private static bool TrySplit(List<Token> tokens, [NotNullWhen(true)] out List<Token>? result)
    {
        result = null;
        for (var i = 0; i < tokens.Count; i ++)
        {
            var token = tokens[i];
            if (token is NumToken { value: >= 10 } num)
            {
                var replaceStart = i;
                var replaceEnd = replaceStart + 1;
                var leftChunk = tokens.Take(replaceStart).ToList();
                var rightChunk = tokens.Skip(replaceEnd).ToList();

                var vLeft = num.value / 2;
                var vRight = num.value - vLeft;

                result = leftChunk
                    .Concat(new Token[] { '[', vLeft, ',', vRight, ']' })
                    .Concat(rightChunk).ToList();

                return true;
            }
        }

        return false;
    }

    private static int MagnitudeOf(List<Token> tokens)
    {
        var offset = 0;
        return MagnitudeOf(tokens, ref offset);
    }

    private static int MagnitudeOf(List<Token> tokens, ref int offset)
    {
        if (tokens[offset] is NumToken num)
        {
            // magnitude of number is just the number
            offset ++; // process the number
            return num.value;
        }

        // Dealing with a pair
        offset ++; // skip open brace
        var left = MagnitudeOf(tokens, ref offset);
        offset ++; // skip comma
        var right = MagnitudeOf(tokens, ref offset);
        offset ++; // skip close brace
        return 3 * left + 2 * right;
    }
}
