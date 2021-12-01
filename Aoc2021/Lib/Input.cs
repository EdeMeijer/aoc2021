using System;
using System.IO;

namespace Aoc2021.Lib
{
    internal static class Input
    {
        internal static string Text(int day) => File.ReadAllText(FileFor(day));

        internal static string[] Lines(int day) => File.ReadAllLines(FileFor(day));

        private static string FileFor(int day)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            
            return $"{dir}/../../../input/day{(day < 10 ? "0" : "")}{day}";
        }
    }
}
