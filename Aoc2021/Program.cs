using System;
using System.Diagnostics;
using Aoc2021;

Console.WriteLine("======== PART 1 ========");
var sw = Stopwatch.StartNew();
Console.WriteLine($"Answer = {Day9.Part1()}");
Console.WriteLine($"Runtime = {sw.ElapsedMilliseconds}ms");

Console.WriteLine("======== PART 2 ========");
sw.Restart();
Console.WriteLine($"Answer = {Day9.Part2()}");
Console.WriteLine($"Runtime = {sw.ElapsedMilliseconds}ms");