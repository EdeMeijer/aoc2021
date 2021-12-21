namespace Aoc2021;

public class Day21
{
    public static int Part1()
    {
        var die = new Die();
        var players = new[] { new Player(4), new Player(8) };

        for (;;)
        {
            for (var i = 0; i < players.Length; i ++)
            {
                var player = players[i];
                player.Forward(die.Roll(3));
                if (player.Score >= 1000)
                {
                    return players[1 - i].Score * die.TotalRolls;
                }
            }
        }
    }

    public static long Part2()
    {
        var history = new Stack<int>();

        var players = new[] { new Player(8), new Player(4) };

        var universes = 1L;
        var wins = new[] { 0L, 0L };

        var didWin = false;

        var multiplier = new Dictionary<int, int>
        {
            [3] = 1,
            [4] = 3,
            [5] = 6,
            [6] = 7,
            [7] = 6,
            [8] = 3,
            [9] = 1,
        };

        for (var i = 0L;; i ++)
        {
            var nextThrow = 3;
            int? revert = null;
            if (didWin)
            {
                if (history.Count == 0)
                {
                    return wins.Max();
                }

                var lastMove = history.Pop();
                nextThrow = lastMove + 1;
                revert = lastMove;
            }

            var playerIndex = history.Count % 2;
            var player = players[playerIndex];

            if (revert != null)
            {
                player.Backward(revert.Value);
                universes /= multiplier[revert.Value];
            }

            if (nextThrow == 10)
            {
                continue;
            }

            universes *= multiplier[nextThrow];

            player.Forward(nextThrow);
            didWin = player.Score >= 21;
            if (didWin)
            {
                wins[playerIndex] += universes;
            }

            history.Push(nextThrow);
        }
    }

    private class Player
    {
        public int Position { get; private set; }
        public int Score { get; private set; } = 0;

        public Player(int position)
        {
            Position = position;
        }

        public void Forward(int rolled)
        {
            Position += rolled;
            if (Position > 10)
            {
                Position -= 10;
            }

            Score += Position;
        }

        public void Backward(int rolled)
        {
            Score -= Position;
            Position -= rolled;
            if (Position < 1)
            {
                Position += 10;
            }
        }
    }

    private class Die
    {
        public int TotalRolls { get; private set; } = 0;

        public int Roll(int n)
        {
            return Enumerable.Range(1, n).Sum(_ => Roll());
        }

        public int Roll()
        {
            TotalRolls ++;
            return (TotalRolls - 1) % 100 + 1;
        }
    }
}
