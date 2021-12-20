using System.Diagnostics.CodeAnalysis;
using Aoc2021.Lib;

namespace Aoc2021;

public class Day19
{
    private static readonly List<Rotation[]> rotations;

    static Day19() => rotations = GetAllRotations3D();

    public static int Part1()
    {
        var input = LoadInput();

        var includedScanners = input.Take(1).ToList();
        var pendingScanners = input.Skip(1).ToList();

        // Keep searching a pending scanner that can be matched to any of the included scanners
        while (pendingScanners.Any())
        {
            var matchedAny = false;
            for (var i = 0; i < includedScanners.Count; i ++)
            {
                var included = includedScanners[i];
                for (var p = 0; p < pendingScanners.Count; p ++)
                {
                    var pending = pendingScanners[p];

                    if (MatchScannerPair(included, pending, out var result, out var offset))
                    {
                        pendingScanners.RemoveAt(p);
                        includedScanners.Add(result);
                        matchedAny = true;
                    }
                }
            }

            if (!matchedAny)
            {
                throw new Exception("Could not match any scanner");
            }
        }

        return includedScanners.SelectMany(sc => sc).ToHashSet().Count;
    }

    public static int Part2()
    {
        var input = LoadInput();

        var scannerPositions = new List<Point> { new(new[] { 0, 0, 0 }) };
        var includedScanners = input.Take(1).ToList();
        var pendingScanners = input.Skip(1).ToList();

        // Keep searching a pending scanner that can be matched to any of the included scanners
        while (pendingScanners.Any())
        {
            var matchedAny = false;
            for (var i = 0; i < includedScanners.Count; i ++)
            {
                var included = includedScanners[i];
                for (var p = 0; p < pendingScanners.Count; p ++)
                {
                    var pending = pendingScanners[p];

                    if (MatchScannerPair(included, pending, out var result, out var offset))
                    {
                        pendingScanners.RemoveAt(p);
                        includedScanners.Add(result);
                        scannerPositions.Add(offset);
                        matchedAny = true;
                    }
                }
            }

            if (!matchedAny)
            {
                throw new Exception("Could not match any scanner");
            }
        }

        var max = 0;
        foreach (var sa in scannerPositions)
        {
            foreach (var sb in scannerPositions)
            {
                var dist = (sa - sb).coordinates.Select(Math.Abs).Sum();
                max = Math.Max(max, dist);
            }
        }

        return max;
    }

    private static bool MatchScannerPair(
        List<Point> scannerA,
        List<Point> scannerB,
        [NotNullWhen(true)] out List<Point>? result,
        [NotNullWhen(true)] out Point? offset
    )
    {
        result = null;
        offset = null;
        foreach (var rots in rotations)
        {
            var rotatedB = scannerB.Select(point => Rotate(point, rots)).ToList();

            // Find an offset between the scanners that results in both scanners having 12 points in common
            if (TryFindOverlapOffset(scannerA, rotatedB, out result, out offset))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryFindOverlapOffset(
        List<Point> scannerA,
        List<Point> scannerB,
        [NotNullWhen(true)] out List<Point>? result,
        [NotNullWhen(true)] out Point? offset
    )
    {
        result = null;
        offset = null;

        // Find candidate offsets by considering all possible pairings of points of the scanners
        for (var ia = 0; ia < scannerA.Count; ia ++)
        {
            var pointA = scannerA[ia];
            for (var ib = 0; ib < scannerB.Count; ib ++)
            {
                var pointB = scannerB[ib];
                offset = pointA - pointB;
                if (IsScannerMatch(scannerA, scannerB, offset, out result))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsScannerMatch(List<Point> scannerA, List<Point> scannerB, Point offset, out List<Point> result)
    {
        result = scannerB.Select(point => point + offset).ToList();
        return scannerA.Intersect(result).Count() >= 12;
    }

    private sealed record Point(int[] coordinates)
    {
        private int? _hashCode;

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.coordinates.Zip(b.coordinates).Select(tup => tup.First + tup.Second).ToArray());
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.coordinates.Zip(b.coordinates).Select(tup => tup.First - tup.Second).ToArray());
        }

        public bool Equals(Point? other)
        {
            return coordinates.SequenceEqual(other.coordinates);
        }

        public override int GetHashCode() => _hashCode ??= CalcHashCode();

        private int CalcHashCode()
        {
            var hc = new HashCode();
            foreach (var v in coordinates)
            {
                hc.Add(v);
            }

            return hc.ToHashCode();
        }
    }

    private static List<List<Point>> LoadInput()
    {
        var input = Input.Text(19);
        var scanners = input.Split("\n\n");

        return scanners
            .Select(scannerInput =>
                scannerInput
                    .Split("\n")
                    .Skip(1)
                    .Select(line => new Point(line.Split(',').Select(int.Parse).ToArray()))
                    .ToList()
            )
            .ToList();
    }

    private static List<Rotation[]> GetAllRotations2D()
    {
        return GetAllRotationsInPlane(0, 1)
            .Select(rotation => new[] { rotation }.Where(rot => rot.degrees != 0).ToArray())
            .ToList();
    }

    private static List<Rotation[]> GetAllRotations3D()
    {
        var result = new List<Rotation[]>();

        void addRotations(int axisA1, int axisB1, int deg1, int axisA2, int axisB2)
        {
            var rot1 = new Rotation(axisA1, axisB1, deg1);
            result.AddRange(
                GetAllRotationsInPlane(axisA2, axisB2)
                    .Select(rot2 => new[] { rot1, rot2 }.Where(rot => rot.degrees != 0).ToArray())
            );
        }

        addRotations(0, 1, 0, 1, 2);
        addRotations(0, 1, 90, 0, 2);
        addRotations(0, 1, 180, 1, 2);
        addRotations(0, 1, 270, 0, 2);
        addRotations(0, 2, 90, 0, 1);
        addRotations(0, 2, 270, 0, 1);

        return result;
    }

    private static IEnumerable<Rotation> GetAllRotationsInPlane(int axisA, int axisB) =>
        new[] { 0, 90, 180, 270 }.Select(deg => new Rotation(axisA, axisB, deg));

    private static Point Rotate(Point point, IEnumerable<Rotation> rotations) => rotations.Aggregate(point, Rotate);

    private static Point Rotate(Point point, Rotation rot)
    {
        var coords = point.coordinates;
        var result = coords.ToArray();

        (result[rot.axisA], result[rot.axisB]) = rot.degrees switch
        {
            90 => (-coords[rot.axisB], coords[rot.axisA]),
            180 => (-coords[rot.axisA], -coords[rot.axisB]),
            270 => (coords[rot.axisB], -coords[rot.axisA]),
            _ => throw new ArgumentException()
        };

        return new Point(result);
    }

    private sealed record Rotation(int axisA, int axisB, int degrees);
}
