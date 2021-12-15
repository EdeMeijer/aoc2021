using Aoc2021.Lib;

namespace Aoc2021;

public class Day03
{
    public static int Part1()
    {
        var lines = Input.Lines(3);
        var report = lines.Select(line => line.Select(c => c == '1' ? 1 : 0).ToArray()).ToArray();
        var nBits = report[0].Length;
        var nLines = report.Length;

        var onesCount = new int[nBits];
        foreach (var line in report)
        {
            for (var i = 0; i < nBits; i ++)
            {
                onesCount[i] += line[i];
            }
        }

        var gammaBits = onesCount.Select(n => n > nLines / 2 ? 1 : 0).ToArray();
        var epsilonBits = onesCount.Select(n => n < nLines / 2 ? 1 : 0).ToArray();
            
        var gamma = gammaBits.Reverse().Select((bit, pos) => bit * (1 << pos)).Sum();
        var epsilon = epsilonBits.Reverse().Select((bit, pos) => bit * (1 << pos)).Sum();

        return gamma * epsilon;
    }
        
    public static int Part2()
    {
        var lines = Input.Lines(3);
        var report = lines.Select(line => line.Select(c => c == '1' ? 1 : 0).ToArray()).ToArray();
        var nBits = report[0].Length;

        int filter(int target)
        {
            var subset = report;
            for (var position = 0; position < nBits; position ++)
            {
                var onesCount = subset.Sum(line => line[position]);
                var zerosCount = subset.Length - onesCount;
                var keep = onesCount == zerosCount ? target : onesCount > zerosCount ? target : 1 - target;
                subset = subset.Where(line => line[position] == keep).ToArray();
                if (subset.Length == 1)
                {
                    break;
                }
            }
            return subset[0].Reverse().Select((bit, pos) => bit * (1 << pos)).Sum();
        }

        return filter(1) * filter(0);
    }
}