using System.Collections.Generic;
using System.Linq;
using Aoc2021.Lib;

namespace Aoc2021
{
    public class Day8
    {
        public static int Part1()
        {
            var uniqueLengths = new[] { 2, 4, 3, 7 };

            return Input.Lines(8).Select(line =>
            {
                var parts = line.Split(" | ");
                var digits = parts[1].Split(' ');
                return digits.Count(digit => uniqueLengths.Contains(digit.Length));
            }).Sum();
        }

        public static int Part2() => Input.Lines(8).Sum(SolveLine);

        private static int SolveLine(string line)
        {
            var parts = line.Split(" | ");
            var brokenDigits = parts[1].Split(' ').Select(parseDigit).ToArray();
            var examples = parts[0].Split(' ').Select(parseDigit).ToArray();

            var schema = new List<int>();
            var valid = true;

            for (;;)
            {
                var segment = 0;
                if (!valid)
                {
                    // Try a different wiring for the last segment
                    segment = schema.Last() + 1;
                    schema.RemoveAt(schema.Count - 1);
                }

                while (schema.Contains(segment))
                {
                    segment ++;
                }

                if (segment == 7)
                {
                    // No valid segments left, go back one level in invalid state
                    valid = false;
                    continue;
                }

                schema.Add(segment);
                valid = validateSchema(schema, examples);

                if (valid && schema.Count == 7)
                {
                    break;
                }
            }

            // Now we have a valid mapping
            var result = brokenDigits
                .Select(brokenDigit => brokenDigit.Select(i => schema[i]).ToArray())
                .Select(fixedDigit => digits.FindIndex(d => d.SetEquals(fixedDigit)))
                .Aggregate("", (current, digit) => current + digit);

            return int.Parse(result);
        }

        private static int[] parseDigit(string digit) => digit.Select(c => c - 97).ToArray();

        private static readonly List<HashSet<int>> digits = new()
        {
            new[] { 0, 1, 2, 4, 5, 6 }.ToHashSet(), // 0
            new[] { 2, 5 }.ToHashSet(), // 1
            new[] { 0, 2, 3, 4, 6 }.ToHashSet(), // 2
            new[] { 0, 2, 3, 5, 6 }.ToHashSet(), // 3
            new[] { 1, 2, 3, 5 }.ToHashSet(), // 4
            new[] { 0, 1, 3, 5, 6 }.ToHashSet(), // 5
            new[] { 0, 1, 3, 4, 5, 6 }.ToHashSet(), // 6
            new[] { 0, 2, 5 }.ToHashSet(), // 7
            new[] { 0, 1, 2, 3, 4, 5, 6 }.ToHashSet(), // 8
            new[] { 0, 1, 2, 3, 5, 6 }.ToHashSet(), // 9
        };

        private static bool validateSchema(List<int> schema, int[][] examples) => examples.All(example =>
        {
            var mapped = example.Where(i => i < schema.Count).Select(i => schema[i]).ToArray();
            return digits.Any(digit => digit.IsSupersetOf(mapped) && digit.Count == example.Length);
        });
    }
}
