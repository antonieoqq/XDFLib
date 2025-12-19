using System;
using System.Buffers;

namespace XDFLib.Collections
{
    /// <summary> A lightweight struct that rents an array from the ArrayPool and returns it on Dispose. </summary>
    public readonly ref struct RentedArray<T>
    {
        public const int MinimumArrayLength = 2;
        public readonly int Length;
        public Span<T> Span => _array.AsSpan(0, Length);

        private readonly ArrayPool<T> _pool;
        private readonly T[] _array;

        public RentedArray(int minLength, ArrayPool<T>? pool = null, bool autoClear = true)
        {
            Length = Math.Max(minLength, MinimumArrayLength);
            _pool = pool ?? ArrayPool<T>.Shared;
            _array = _pool.Rent(Length);

            if (autoClear)
            {
                Clear();
            }
        }

        public T[] GetArray()
        {
            return _array;
        }

        public void CopyTo(T[] destination, int start)
        {
            Array.Copy(_array, 0, destination, start, Length);
        }

        public void CopyTo(T[] destination, int start, int count)
        {
            Array.Copy(_array, 0, destination, start, count);
        }

        public void Clear()
        {
            Array.Clear(_array, 0, _array.Length - 1);
        }

        public void Dispose()
        {
            Clear();
            _pool.Return(_array);
        }
    }
}
