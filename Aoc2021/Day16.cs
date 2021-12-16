using Aoc2021.Lib;

namespace Aoc2021;

public class Day16
{
    public static int Part1()
    {
        int SumVersions(AbstractPacket packet) =>
            packet.version + (packet is OperatorPacket Op ? Op.children.Sum(SumVersions) : 0);

        return SumVersions(LoadTransmission());
    }

    public static long Part2()
    {
        long Evaluate(AbstractPacket packet) => packet switch
        {
            LiteralPacket lit => lit.value,
            OperatorPacket op => op.typeId switch
            {
                0 => op.children.Sum(Evaluate),
                1 => op.children.Select(Evaluate).Aggregate(1L, (a, b) => a * b),
                2 => op.children.Min(Evaluate),
                3 => op.children.Max(Evaluate),
                5 => Evaluate(op.children[0]) > Evaluate(op.children[1]) ? 1 : 0,
                6 => Evaluate(op.children[0]) < Evaluate(op.children[1]) ? 1 : 0,
                7 => Evaluate(op.children[0]) == Evaluate(op.children[1]) ? 1 : 0
            }
        };

        return Evaluate(LoadTransmission());
    }

    private static AbstractPacket LoadTransmission()
    {
        var offset = 0;
        return ReadPacket(DecodeHex(Input.Text(16)), ref offset);
    }

    private static byte[] DecodeHex(string hex)
    {
        var result = new byte[hex.Length * 4];
        for (var h = 0; h < hex.Length; h ++)
        {
            var decoded = Convert.ToByte(hex[h].ToString(), 16);
            for (var i = 0; i < 4; i ++)
            {
                result[h * 4 + i] = (byte)((decoded & (1 << (3 - i))) != 0 ? 1 : 0);
            }
        }

        return result;
    }

    private static AbstractPacket ReadPacket(byte[] bits, ref int offset)
    {
        var version = (int)ReadLong(bits, ref offset, 3);
        var typeId = (int)ReadLong(bits, ref offset, 3);
        return typeId == 4 ? ReadLiteral(bits, ref offset, version) : ReadOperator(bits, ref offset, version, typeId);
    }

    private static LiteralPacket ReadLiteral(byte[] bits, ref int offset, int version)
    {
        var valueBits = new List<byte>();
        for (;;)
        {
            var chunk = ReadBits(bits, ref offset, 5);
            valueBits.AddRange(chunk[1..]);
            if (chunk[0] == 0)
            {
                break;
            }
        }

        return new LiteralPacket(version, ParseLong(valueBits));
    }

    private static OperatorPacket ReadOperator(byte[] bits, ref int offset, int version, int typeId)
    {
        var lengthType = ReadLong(bits, ref offset, 1);
        var targetLength = ReadLong(bits, ref offset, lengthType == 0 ? 15 : 11);

        var children = new List<AbstractPacket>();
        var startOffset = offset;

        while ((lengthType == 0 ? (offset - startOffset) : children.Count) < targetLength)
        {
            children.Add(ReadPacket(bits, ref offset));
        }

        return new OperatorPacket(version, typeId, children.ToArray());
    }

    private static byte[] ReadBits(byte[] bits, ref int offset, int length)
    {
        var slice = bits[offset..(offset + length)];
        offset += length;
        return slice;
    }

    private static long ReadLong(byte[] bits, ref int offset, int length) =>
        ParseLong(ReadBits(bits, ref offset, length));

    private static long ParseLong(IEnumerable<byte> bits) => bits.Reverse().Select((bit, i) => bit * 1L << i).Sum();

    private abstract record AbstractPacket(int version, int typeId);

    private record LiteralPacket(int version, long value) : AbstractPacket(version, 4);

    private record OperatorPacket(int version, int typeId, AbstractPacket[] children) : AbstractPacket(version, typeId);
}
