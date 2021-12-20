using Aoc2021.Lib;

namespace Aoc2021;

public class Day20
{
    public static int Part1() => Solve(2);

    public static int Part2() => Solve(50);

    public static int Solve(int rounds)
    {
        var input = Input.Text(20);
        var parts = input.Split("\n\n");
        var algorithm = parts[0];
        var imageInput = parts[1].Split("\n");
        var image = new Matrix<char>(
            imageInput.Length,
            imageInput[0].Length,
            imageInput.SelectMany(line => line)
        );

        var ambient = '.';
        for (var i = 0; i < rounds; i ++)
        {
            (image, ambient) = Enhance(image, ambient, algorithm);
        }

        return image.Values.Count(ch => ch == '#');
    }

    private static readonly (int y, int x)[] offsets =
        { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 0), (0, 1), (1, -1), (1, 0), (1, 1) };

    private static (Matrix<char> image, char ambient) Enhance(Matrix<char> image, char ambient, string algorithm)
    {
        var newImage = new Matrix<char>(image.Height + 2, image.Width + 2, '.');
        foreach (var (y, x) in newImage.Coords)
        {
            var algoInput = offsets.Select(offset =>
            {
                var (dy, dx) = offset;
                var (y2, x2) = (y + dy - 1, x + dx - 1);
                return image.Contains(y2, x2) ? image[y2, x2] : ambient;
            }).ToArray();
            newImage[y, x] = EnhancePixel(algoInput, algorithm);
        }

        var newAmbient = EnhancePixel(Enumerable.Repeat(ambient, 9).ToArray(), algorithm);

        return (newImage, newAmbient);
    }

    private static char EnhancePixel(char[] around, string algorithm)
    {
        var index = around.Reverse().Select((v, i) => (v == '#' ? 1 : 0) << i).Sum();
        return algorithm[index];
    }
}
