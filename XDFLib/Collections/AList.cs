using System;
using System.Collections;
using System.Collections.Generic;
using XDFLib.Extensions;

namespace XDFLib.Collections
{
    public class AList<T> : IList<T>
    {
        public const int DefaultCapacity = 4;
        public const float ExpandFactor = 1.5f;

        T[] _array;
        int _count = 0;

        public int Count => _count;

        public int Capacity
        {
            get { return _array.Length; }
            set
            {
                var newCap = value > _count ? value : _count;
                if (_array.Length < newCap) { MoveToNewArray(newCap); }
            }
        }

        public bool IsReadOnly => throw new NotImplementedException();

        public struct Enumerator : IEnumerator<T>
        {
            private readonly AList<T> _parent;
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

            internal Enumerator(AList<T> parent)
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


        public AList()
        {
            _array = new T[DefaultCapacity];
        }

        public AList(int capacity = DefaultCapacity)
        {
            _array = new T[capacity];
        }

        public T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        public ref T GetRef(int index)
        {
            return ref _array[index];
        }

        public void Sort(Comparison<T> comparison)
        {
            _array.QuickSort(0, Count - 1, comparison);
        }

        public Span<T> AsSpan()
        {
            return new Span<T>(_array, 0, Count);
        }

        public ReadOnlySpan<T> AsReadOnlySpan()
        {
            return new ReadOnlySpan<T>(_array, 0, Count);
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < _count; i++)
            {
                var e = this[i];
                if (Utilities.AreEqual(e, item))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException();
            }

            /// array已满时，insert发生队列膨胀，此时不使用Expand，因为Expand函数会直接分配并移动数据到新的Array
            /// 而插入数据的操作还要重新位移成员，为了避免性能浪费，新array分配后，拷贝数据的操作此处单独处理，以直接包含需要插入的数据
            if (_count == _array.Length)
            {
                int newLen = (int)(_array.Length * ExpandFactor);
                var newArray = new T[newLen];
                int writingIndex = 0;
                for (int i = 0; i < index; i++)
                {
                    newArray[writingIndex] = _array[i];
                    writingIndex++;
                }
                newArray[writingIndex] = item;
                writingIndex++;
                for (int i = index; i < _count; i++)
                {
                    newArray[writingIndex] = _array[i];
                    writingIndex++;
                }
                _array = newArray;
                _count++;
            }
            else
            {
                // 注意循环是从尾端向前，i--
                for (int i = _count; i > index; i--)
                {
                    _array[i] = _array[i - 1];
                }
                _array[index] = item;
                _count++;
            }
        }

        public void RemoveAt(int index)
        {
            for (int i = index; i < _count - 1; i++)
            {
                _array[i] = _array[i + 1];
            }
            _array[_count - 1] = default;
            _count--;
        }

        public void Add(T item)
        {
            if (Count == _array.Length)
            {
                Expand();
            }
            _array[Count] = item;
            _count++;
        }

        public void AddRange(ICollection<T> items)
        {
            var newCount = Count + items.Count;
            if (newCount > _array.Length)
            {
                MoveToNewArray(newCount);
            }

            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void AddRange(ReadOnlySpan<T> items)
        {
            var newCount = Count + items.Length;
            if (newCount > _array.Length)
            {
                MoveToNewArray(newCount);
            }

            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void AddRange(T[] items)
        {
            var newCount = Count + items.Length;
            if (newCount > _array.Length)
            {
                MoveToNewArray(newCount);
            }

            Array.Copy(items, 0, _array, Count, items.Length);
        }

        public void Clear()
        {
            Array.Clear(_array, 0, Count);
            _count = 0;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _count; i++)
            {
                var e = this[i];
                if (Utilities.AreEqual(e, item))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _array.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var removeIndex = IndexOf(item);
            if (removeIndex != -1)
            {
                RemoveAt(removeIndex);
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        void Expand()
        {
            int newLen = (int)(_array.Length * ExpandFactor);
            MoveToNewArray(newLen);
        }

        void MoveToNewArray(int newLen)
        {
            var newArray = new T[newLen];
            var countToCopy = Math.Min(newLen, _count);
            Array.Copy(_array, newArray, countToCopy);
            _array = newArray;
            _count = countToCopy;
        }

    }
}
