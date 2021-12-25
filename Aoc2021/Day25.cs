using Aoc2021.Lib;

namespace Aoc2021;

using State = Dictionary<(int y, int x), char>;

public class Day25
{
    public static int Part1()
    {
        var input = Input.Lines(25);

        var state = input
            .SelectMany((line, y) => line.Select((ch, x) => (y, x, ch)))
            .Where(tup => tup.ch != '.')
            .ToDictionary(tup => (tup.y, tup.x), tup => tup.ch);

        var (height, width) = (input.Length, input[0].Length);

        for (var step = 1;; step ++)
        {
            var nextState = Simulate(state, height, width);
            if (StateEquals(nextState, state))
            {
                return step;
            }

            state = nextState;
        }
    }

    public static int Part2() => 0; // No part 2 today :)

    private static State Simulate(State state, int height, int width)
    {
        state = SubSimulate(state, '>', 0, 1, height, width);
        state = SubSimulate(state, 'v', 1, 0, height, width);
        return state;
    }

    private static State SubSimulate(State state, char filter, int moveY, int moveX, int height, int width)
    {
        var result = new State();

        foreach (var (pos, ch) in state)
        {
            var target = pos;
            if (ch == filter)
            {
                var moveTargetY = pos.y + moveY;
                var moveTargetX = pos.x + moveX;
                moveTargetY = moveTargetY == height ? 0 : moveTargetY;
                moveTargetX = moveTargetX == width ? 0 : moveTargetX;
                var moveTarget = (moveTargetY, moveTargetX);
                
                if (!state.ContainsKey(moveTarget))
                {
                    target = moveTarget;
                }
            }
            
            result[target] = ch;
        }

        return result;
    }

    private static bool StateEquals(State a, State b) => 
        a.Keys.ToHashSet().SetEquals(b.Keys) && a.All(kvp => kvp.Value == b[kvp.Key]);
}
