using Aoc2021.Lib;

namespace Aoc2021;

public class Day12
{
    public static int Part1() => Solve(false);

    public static int Part2() => Solve(true);

    public static int Solve(bool allowVisitingOneSmallCaveTwice)
    {
        var lines = Input.Lines(12);

        // Parse input
        var nodes = new Dictionary<string, Node>();
        foreach (var line in lines)
        {
            var parts = line.Split('-');
            for (var i = 0; i < 2; i ++)
            {
                var label = parts[i];
                if (!nodes.ContainsKey(label))
                {
                    nodes[label] = new Node(label == label.ToUpperInvariant(), new List<string>());
                }

                nodes[label].edges.Add(parts[1 - i]);
            }
        }

        // Solve
        var foundPaths = 0;
        var stack = new Stack<(string node, int siblingIndex)>();
        var visitedSmallCaves = new Dictionary<string, int>();
        var visitedAnyCaveTwice = false;
        int? nextStartIndex = null;

        for (;;)
        {
            var prev = stack.Any() ? stack.Peek().node : "start";
            var edges = nodes[prev].edges;

            var nextIndex = edges.FindIndex(nextStartIndex ?? 0, nextNode =>
                (
                    // Always allow large cave
                    nodes[nextNode].large ||
                    // Also allow non-visited small caves
                    !visitedSmallCaves.ContainsKey(nextNode) ||
                    // Allow re-visiting a single small cave, if enabled
                    (allowVisitingOneSmallCaveTwice && visitedSmallCaves[nextNode] == 1 && !visitedAnyCaveTwice)
                )
                && nextNode != "start"
            );
            nextStartIndex = null;

            if (nextIndex >= 0)
            {
                // Found a next node to visit
                var nextNodeLabel = edges[nextIndex];

                if (nextNodeLabel == "end")
                {
                    // Found a path to the end.
                    foundPaths ++;

                    // Visit the next sibling node in the next iteration
                    nextStartIndex = nextIndex + 1;
                }
                else
                {
                    // Add to the current path
                    stack.Push((nextNodeLabel, nextIndex));

                    if (!nodes[nextNodeLabel].large)
                    {
                        // Visiting a small cave
                        if (!visitedSmallCaves.ContainsKey(nextNodeLabel))
                        {
                            visitedSmallCaves[nextNodeLabel] = 0;
                        }
                        else
                        {
                            // Already visited, cache the fact we are revisiting a small cave
                            visitedAnyCaveTwice = true;
                        }

                        visitedSmallCaves[nextNodeLabel] ++;
                    }
                }
            }
            else
            {
                // There is nowhere we can go. 
                if (!stack.Any())
                {
                    // Done
                    break;
                }

                // Pop the current cave from the path
                var leaving = stack.Pop();

                if (!nodes[leaving.node].large)
                {
                    // Left a small cave
                    visitedSmallCaves[leaving.node] --;
                    if (visitedSmallCaves[leaving.node] == 1)
                    {
                        // This was the twice-visited cave in the path, so now we may again visit one twice
                        visitedAnyCaveTwice = false;
                    }
                    else
                    {
                        visitedSmallCaves.Remove(leaving.node);
                    }
                }

                // Visit the next sibling node in the next iteration
                nextStartIndex = leaving.siblingIndex + 1;
            }
        }

        return foundPaths;
    }

    private record Node(bool large, List<string> edges);
}