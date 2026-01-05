using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace XDFLib.Collections
{
    /// <summary> A lightweight struct that rents an array from the ArrayPool and returns it on Dispose. </summary>
    public readonly ref struct RentedArray<T>
    {
        public const int MinimumArrayLength = 2;
        public readonly int Length;
        private readonly ArrayPool<T> _pool;
        private readonly T[] _array;

        public Span<T> Span => _array.AsSpan(0, Length);
        public T[] InternalArray => _array;

        public RentedArray(int minLength, ArrayPool<T>? pool = null)
        {
            Length = Math.Max(minLength, MinimumArrayLength);
            _pool = pool ?? ArrayPool<T>.Shared;
            _array = _pool.Rent(Length);
        }


        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)index >= (uint)Length) ThrowIndexOutOfRange();
                return ref _array[index];
            }
        }

        public void CopyTo(Span<T> destination)
        {
            Span.CopyTo(destination);
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
            _pool.Return(_array, clearArray: true);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowIndexOutOfRange() => throw new IndexOutOfRangeException();
    }
}
