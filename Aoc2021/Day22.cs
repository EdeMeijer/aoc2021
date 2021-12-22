using Aoc2021.Lib;

namespace Aoc2021;

public class Day22
{
    public static long Part1() => Solve(instr =>
        instr.block.from.Concat(instr.block.to).All(v => v is >= -50 and <= 50)
    );

    public static long Part2() => Solve(_ => true);

    private static long Solve(Func<Instruction, bool> instructionFilter)
    {
        var instructions = Input.Lines(22).Select(ParseInstruction).ToList();

        var enabled = new List<Block>();

        foreach (var (block, flag) in instructions.Where(instructionFilter))
        {
            enabled = CarveBlock(enabled, block);
            if (flag)
            {
                enabled.Add(block);
            }
        }

        return enabled.Sum(VolumeOf);
    }

    private static Instruction ParseInstruction(string line)
    {
        var parts = line.Split(' ');
        var flag = parts[0] == "on";
        var values = parts[1].Split(',')
            .Select(part => part[2..].Split("..").Select(int.Parse).ToArray())
            .ToArray();

        var from = values.Select(tup => tup[0]).ToArray();
        var to = values.Select(tup => tup[1] + 1).ToArray();
        return new Instruction(new Block(from, to), flag);
    }

    private static List<Block> CarveBlock(List<Block> blocks, Block toCarve)
    {
        var result = new List<Block>();
        foreach (var block in blocks)
        {
            if (Overlaps(block, toCarve))
            {
                result.AddRange(CarveBlock(block, toCarve));
            }
            else
            {
                result.Add(block);
            }
        }

        return result;
    }

    private static List<Block> CarveBlock(Block block, Block toCarve)
    {
        var result = new List<Block>();

        for (var dim = 0; dim < 3; dim ++)
        {
            var chunks = CarveBlock(block, toCarve, dim).ToList();
            block = chunks.Last();
            result.AddRange(chunks.SkipLast(1));
        }

        return result;
    }

    private static IEnumerable<Block> CarveBlock(Block block, Block toCarve, int dimension)
    {
        var carveFrom = toCarve.from[dimension];
        var carveTo = toCarve.to[dimension];

        var blockFrom = block.from[dimension];
        var blockTo = block.to[dimension];

        if (carveFrom > blockFrom)
        {
            // Split left chunk
            (var left, block) = SplitBlock(block, carveFrom, dimension);
            yield return left;
        }

        if (carveTo < blockTo)
        {
            // Split right chunk
            (block, var right) = SplitBlock(block, carveTo, dimension);
            yield return right;
        }

        yield return block;
    }

    private static (Block left, Block right) SplitBlock(Block block, int point, int dimension)
    {
        var leftTo = block.to.ToArray();
        leftTo[dimension] = point;
        var rightFrom = block.from.ToArray();
        rightFrom[dimension] = point;

        var left = new Block(block.from, leftTo);
        var right = new Block(rightFrom, block.to);
        return (left, right);
    }

    private static bool Overlaps(Block blockA, Block blockB) => Enumerable.Range(0, 3).All(dim =>
        blockA.@from[dim] < blockB.to[dim] &&
        blockB.@from[dim] < blockA.to[dim]
    );

    private static long VolumeOf(Block block) =>
        block.@from.Zip(block.to).Aggregate(1L, (res, tup) => res * (tup.Second - tup.First));

    private record Block(int[] from, int[] to);

    private record Instruction(Block block, bool flag);
}
