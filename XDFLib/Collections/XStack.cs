using System;
using System.Collections;
using System.Collections.Generic;

namespace XDFLib.Collections
{
    public class XStack<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
    {
        public struct Enumerator : IEnumerator<T>
        {
            private readonly XStack<T> _parent;
            private int _index;

            public T Current
            {
                get
                {
                    if (_index < 0 || _index >= _parent._count)
                    {
                        throw new InvalidOperationException();
                    }
                    return _parent[_index];
                }
            }

            object IEnumerator.Current { get { return Current; } }

            internal Enumerator(XStack<T> parent)
            {
                _parent = parent;
                _index = -1;
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                if (_index < _parent._count)
                {
                    _index++;
                }
                return _index >= 0 && _index < _parent._count;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        public const int DefaultCapacity = 4;
        public const float ExpandFactor = 1.5f;

        public int Count => _count;
        public int Capacity
        {
            get { return _array.Length; }
            set
            {
                var newCap = value > _count ? value : _count;
                newCap = newCap > 2 ? newCap : 2;
                if (_array.Length != newCap) { MoveToNewArray(newCap); }
            }
        }

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        object _syncRoot;
        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                }
                return _syncRoot;
            }
        }

        T[] _array;
        int _count = 0;

        public T this[int index]
        {
            get { return _array[index]; }
            set { _array[index] = value; }
        }

        public XStack() { _array = new T[DefaultCapacity]; }

        public XStack(int capacity)
        {
            capacity = Math.Max(2, capacity);
            _array = new T[capacity];
        }

        void ExpandIfArrayIsFull()
        {
            if (_count == _array.Length)
            {
                Expand();
            }
        }

        void Expand()
        {
            int newLen = (int)(_array.Length * ExpandFactor + 1);
            MoveToNewArray(newLen);
        }

        void MoveToNewArray(int newLen)
        {
            var newArray = new T[newLen];
            var countToCopy = Math.Min(newLen, _count);
            for (int i = 0; i < countToCopy; i++)
            {
                newArray[i] = _array[i];
            }
            _array = newArray;
            _count = countToCopy;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            _array.CopyTo(array, index);
        }

        public void Push(T item)
        {
            ExpandIfArrayIsFull();
            _array[_count++] = item;
        }

        public void Push(ICollection<T> collection)
        {
            var newSize = collection.Count + _count;
            if (newSize > _array.Length)
            {
                var newLen = (int)(newSize * 1.5f) + 1;
                MoveToNewArray(newLen);
            }
            foreach (var item in collection)
            {
                _array[_count++] = item;
            }
        }

        public void Push(ReadOnlySpan<T> span)
        {
            var newSize = span.Length + _count;
            if (newSize > _array.Length)
            {
                var newLen = (int)(newSize * 1.5f) + 1;
                MoveToNewArray(newLen);
            }
            foreach (var item in span)
            {
                _array[_count++] = item;
            }
        }

        public bool Pop(out T item)
        {
            if (_count > 0)
            {
                item = _array[_count - 1];
                _array[_count - 1] = default;
                _count--;
                return true;
            }
            item = default;
            return false;
        }

        public bool Pop(T[] popTo, int count)
        {
            if (count > 0 && count <= _count)
            {
                var startIndex = _count - count;
                for (int i = 0; i < count; i++)
                {
                    popTo[i] = _array[startIndex + i];
                    _array[startIndex + i] = default;
                }
                _count = startIndex;
            }
            return false;
        }

        public bool Pop(int count)
        {
            if (count > 0 && count <= _count)
            {
                var startIndex = _count - count;
                for (int i = 0; i < count; i++)
                {
                    _array[startIndex + i] = default;
                }
                _count = startIndex;
            }
            return false;
        }

        public void Clear()
        {
            for (int i = 0; i < _count; i++)
            {
                _array[i] = default;
            }
            _count = 0;
        }

        public T Peek()
        {
            if (_count > 0)
            {
                return _array[_count - 1];
            }
            return default;
        }

        public Span<T> AsSpan()
        {
            return new Span<T>(_array, 0, _count);
        }

        public ReadOnlySpan<T> AsReadOnlySpan()
        {
            return new ReadOnlySpan<T>(_array, 0, _count);
        }
    }
}
