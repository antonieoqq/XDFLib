using System;
using System.Buffers;

namespace XDFLib.Collections
{
    /// <summary> A lightweight struct that rents an array from the ArrayPool and returns it on Dispose. </summary>
    public readonly ref struct RentedArray<T>
    {
        public readonly int Length;
        public Span<T> Span => _array.AsSpan(0, Length);

        private readonly ArrayPool<T> _pool;
        private readonly T[] _array;

        public RentedArray(int minLength, ArrayPool<T>? pool = null)
        {
            _pool = pool ?? ArrayPool<T>.Shared;
            _array = _pool.Rent(minLength);
            Length = minLength;
            Array.Clear(_array, 0, minLength);
        }

        public void CopyTo(T[] destination, int start)
        {
            Array.Copy(_array, 0, destination, start, Length);
        }

        public void CopyTo(T[] destination, int start, int count)
        {
            Array.Copy(_array, 0, destination, start, count);
        }

        public void Dispose()
        {
            Array.Clear(_array, 0, Length);
            _pool.Return(_array);
        }
    }
}
