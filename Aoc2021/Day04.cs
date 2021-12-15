using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Aoc2021.Lib;

namespace Aoc2021;

public class Day04
{
    private static readonly Regex Whitespace = new("\\s+");

    public static int Part1()
    {
        var (numbers, boards) = LoadInput();

        for (var i = 0; i < numbers.Length; i ++)
        {
            var drawn = numbers[..(i + 1)].ToHashSet();
            foreach (var board in boards)
            {
                if (BoardWins(board, drawn))
                {
                    return CalcBoardScore(board, drawn);
                }
            }
        }

        throw new Exception("No board won");
    }

    public static int Part2()
    {
        var (numbers, boards) = LoadInput();

        for (var i = 0; i < numbers.Length; i ++)
        {
            var drawn = numbers[..(i + 1)].ToHashSet();
            for (var b = 0; b < boards.Count; b ++)
            {
                if (BoardWins(boards[b], drawn))
                {
                    if (boards.Count == 1)
                    {
                        return CalcBoardScore(boards[b], drawn);
                    }

                    boards.RemoveAt(b);
                }
            }
        }

        throw new Exception("No board won");
    }

    private static (int[] numbers, List<int[][]> boards) LoadInput()
    {
        var data = Input.Text(4);
        var chunks = data.Split("\n\n");

        var numbers = chunks[0].Split(',').Select(int.Parse).ToArray();
        var boards = chunks[1..].Select(ParseBoard).ToList();

        return (numbers, boards);
    }

    private static int[][] ParseBoard(string input)
    {
        return input.Split("\n")
            .Select(line => Whitespace.Split(line.Trim()).Select(int.Parse).ToArray())
            .ToArray();
    }

    private static bool BoardWins(int[][] board, HashSet<int> drawn)
    {
        for (var r = 0; r < board.Length; r ++)
        {
            if (board[r].All(drawn.Contains))
            {
                return true;
            }
        }

        for (var c = 0; c < board[0].Length; c ++)
        {
            if (board.Select(row => row[c]).All(drawn.Contains))
            {
                return true;
            }
        }

        return false;
    }

    private static int CalcBoardScore(int[][] board, HashSet<int> drawn)
    {
        var unmarkedSum = board.SelectMany(row => row).Where(val => !drawn.Contains(val)).Sum();
        return unmarkedSum * drawn.Last();
    }
}