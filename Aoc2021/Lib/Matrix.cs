using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoc2021.Lib
{
    public interface IMatrix<T> : IEquatable<IMatrix<T>> where T : IEquatable<T>
    {
        int Height { get; }

        int Width { get; }

        IEnumerable<T> Values { get; }

        T this[int y, int x] { get; set; }

        IEnumerable<T> Row(int y);

        IEnumerable<T> Col(int x);

        IEnumerable<(int y, int x)> Coords { get; }

        IMatrix<T> Clone();

        IMatrix<T> With(int y, int x, T value);

        bool Contains(int y, int x);

        IMatrix<T> RotateCw();

        IMatrix<T> FlipHorizontal();

        IMatrix<T> Slice(int y, int height, int x, int width);
    }

    public abstract class AbstractMatrix<T> : IMatrix<T> where T : IEquatable<T>
    {
        public abstract int Height { get; }
        public abstract int Width { get; }

        public virtual IEnumerable<T> Values
        {
            get
            {
                for (var y = 0; y < Height; y ++)
                {
                    for (var x = 0; x < Width; x ++)
                    {
                        yield return this[y, x];
                    }
                }
            }
        }

        public abstract T this[int y, int x] { get; set; }

        public IEnumerable<T> Row(int y)
        {
            for (var x = 0; x < Width; x ++)
            {
                yield return this[y, x];
            }
        }

        public IEnumerable<T> Col(int x)
        {
            for (var y = 0; y < Height; y ++)
            {
                yield return this[y, x];
            }
        }

        public IEnumerable<(int y, int x)> Coords
        {
            get
            {
                for (var y = 0; y < Height; y ++)
                {
                    for (var x = 0; x < Width; x ++)
                    {
                        yield return (y, x);
                    }
                }
            }
        }

        public IMatrix<T> Clone() => new Matrix<T>(Height, Width, Values.ToList());

        public IMatrix<T> With(int y, int x, T value)
        {
            var copy = Clone();
            copy[y, x] = value;
            return copy;
        }

        public bool Contains(int y, int x) => y >= 0 && x >= 0 && y < Height && x < Width;

        public IMatrix<T> RotateCw() => new RotatedMatrix<T>(this);

        public IMatrix<T> FlipHorizontal() => new FlippedMatrix<T>(this);

        public IMatrix<T> Slice(int y, int height, int x, int width) => new MatrixSlice<T>(this, y, height, x, width);

        public override string ToString()
        {
            var result = new StringBuilder();
            for (var y = 0; y < Height; y ++)
            {
                for (var x = 0; x < Width; x ++)
                {
                    result.Append(this[y, x]);
                }

                result.Append('\n');
            }

            return result.ToString();
        }

        public bool Equals(IMatrix<T>? other)
        {
            return other != null &&
                   Height == other.Height &&
                   Width == other.Width &&
                   ValuesEqual(other);
        }

        private bool ValuesEqual(IMatrix<T> other)
        {
            for (var y = 0; y < Height; y ++)
            {
                for (var x = 0; x < Width; x ++)
                {
                    if (!Equals(this[y, x], other[y, x]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is IMatrix<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Height, Width, Values);
        }
    }

    public sealed class RotatedMatrix<T> : AbstractMatrix<T> where T : IEquatable<T>
    {
        private readonly IMatrix<T> _inner;

        public override int Height => _inner.Width;
        public override int Width => _inner.Height;

        public RotatedMatrix(IMatrix<T> inner)
        {
            _inner = inner;
        }

        public override T this[int y, int x]
        {
            get => _inner[_inner.Height - x - 1, y];
            set => _inner[_inner.Height - x - 1, y] = value;
        }
    }

    public sealed class FlippedMatrix<T> : AbstractMatrix<T> where T : IEquatable<T>
    {
        private readonly IMatrix<T> _inner;

        public override int Height => _inner.Height;
        public override int Width => _inner.Width;

        public FlippedMatrix(IMatrix<T> inner)
        {
            _inner = inner;
        }

        public override T this[int y, int x]
        {
            get => _inner[y, _inner.Width - x - 1];
            set => _inner[y, _inner.Width - x - 1] = value;
        }
    }

    public sealed class MatrixSlice<T> : AbstractMatrix<T> where T : IEquatable<T>
    {
        private readonly IMatrix<T> _inner;
        private readonly int _y;
        private readonly int _x;

        public override int Height { get; }
        public override int Width { get; }

        public MatrixSlice(IMatrix<T> inner, int y, int height, int x, int width)
        {
            _inner = inner;
            _y = y;
            _x = x;
            Height = height;
            Width = width;
        }

        public override T this[int y, int x]
        {
            get => _inner[_y + y, _x + x];
            set => _inner[_y + y, _x + x] = value;
        }
    }

    public sealed class Matrix<T> : AbstractMatrix<T> where T : IEquatable<T>
    {
        private readonly List<T> _values;

        public override int Height { get; }
        public override int Width { get; }
        public override IEnumerable<T> Values => _values;

        public Matrix(int height, int width, T defaultValue) :
            this(height, width, Enumerable.Repeat(defaultValue, height * width))
        {
        }

        public Matrix(int height, int width, IEnumerable<T> values)
        {
            Height = height;
            Width = width;
            _values = values.ToList();
            if (_values.Count != height * width)
            {
                throw new ArgumentException("Invalid number of values");
            }
        }

        public override T this[int y, int x]
        {
            get => _values[IndexOf(y, x)];
            set => _values[IndexOf(y, x)] = value;
        }

        private int IndexOf(int y, int x)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                throw new ArgumentException($"Invalid coordinate ({y},{x}) (height={Height},width={Width})");
            }

            return y * Width + x;
        }
    }
}
