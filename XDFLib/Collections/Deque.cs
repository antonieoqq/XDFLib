using System;
using System.Collections;
using System.Collections.Generic;
using XDFLib.Extensions;

namespace XDFLib.Collections
{
    public class Deque<T> : IList<T>
    {
        public struct Enumerator : IEnumerator<T>
        {
            private readonly Deque<T> _parent;
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

            internal Enumerator(Deque<T> parent)
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
        public int IndexOffset => _indexOffset;

        T[] _array;
        int _count = 0;
        int _indexOffset = 0;

        public int Capacity
        {
            get { return _array.Length; }
            set
            {
                var newCap = value > _count ? value : _count;
                if (_array.Length < newCap) { MoveToNewArray(newCap); }
            }
        }

        public bool IsReadOnly => false;

        public Deque()
        {
            _array = new T[DefaultCapacity];
        }

        public Deque(int capacity)
        {
            capacity = Math.Max(2, capacity);
            _array = new T[capacity];
        }

        public T this[int index]
        {
            get
            {
                var looped = GetLoopedIndex(index);
                var accessableIndex = GetAccessIndex(looped);
                return _array[accessableIndex];
            }
            set
            {
                var looped = GetLoopedIndex(index);
                var accessableIndex = GetAccessIndex(looped);
                _array[accessableIndex] = value;
            }
        }

        public ref T GetRef(int index)
        {
            var looped = GetLoopedIndex(index);
            var accessableIndex = GetAccessIndex(looped);
            return ref _array[accessableIndex];
        }

        public void AddFirst(in T e)
        {
            ExpandIfArrayIsFull();

            _indexOffset--;
            _count++;
            this[0] = e;
        }

        public void AddRangeFirst(ICollection<T> es)
        {
            var newCount = _count + es.Count;
            if (newCount > Capacity)
            {
                // 为了避免重新分配后再添加引起的多余操作，这里直接新分配并拷贝数据
                var newLen = (int)MathF.Ceiling(newCount * 1.5f);
                var newArray = new T[newLen];
                var newArrayIndex = 0;
                foreach (var e in es)
                {
                    newArray[newArrayIndex] = e;
                    newArrayIndex++;
                }
                for (int i = 0; i < Count; i++)
                {
                    newArray[newArrayIndex] = this[i];
                    newArrayIndex++;
                }
                _array = newArray;
                _indexOffset = 0;
                _count = newCount;
            }
            else
            {
                // 添加在队列头部导致了_indexOffset前移
                _indexOffset -= es.Count;
                var index = 0;
                // 注意，要在循环添加之前先改变_count，否则this[index]会抛出index超出范围的异常
                _count = newCount;
                foreach (var e in es)
                {
                    this[index] = e;
                    index++;
                }
            }
        }

        public void AddLast(in T e)
        {
            ExpandIfArrayIsFull();

            _count++;
            this[_count - 1] = e;
        }

        public void AddRangeLast(ICollection<T> es)
        {
            var newCount = _count + es.Count;
            if (newCount > Capacity)
            {
                // 为了避免重新分配后再添加引起的多余操作，这里直接新分配并拷贝数据
                var newLen = (int)MathF.Ceiling(newCount * 1.5f);
                var newArray = new T[newLen];
                var newArrayIndex = 0;
                for (int i = 0; i < Count; i++)
                {
                    newArray[newArrayIndex] = this[i];
                    newArrayIndex++;
                }
                foreach (var e in es)
                {
                    newArray[newArrayIndex] = e;
                    newArrayIndex++;
                }
                _array = newArray;
                _indexOffset = 0;
                _count = newCount;
            }
            else
            {
                // 注意，这里的index要在_count发生变化之前赋值，才能从当前队列的末尾开始添加成员
                var index = _count;
                // 注意，要在循环添加之前先改变_count，否则this[index]会抛出index超出范围的异常
                _count = newCount;
                foreach (var e in es)
                {
                    this[index] = e;
                    index++;
                }
            }
        }

        public void RemoveFirst()
        {
            this[0] = default(T);
            _indexOffset++;
            _count--;
        }

        public void RemoveLast()
        {
            this[_count - 1] = default(T);
            _count--;
        }

        public void RemoveFirst(int count)
        {
            RemoveToFirst(count - 1);
        }

        public void RemoveLast(int count)
        {
            RemoveToLast(_count - count);
        }

        /// <summary> 包含fromIndex </summary>
        /// <param name="fromIndex"></param>
        public void RemoveToFirst(int fromIndex)
        {
            //var validIndex = GetValidIndex(fromIndex);
            if (fromIndex >= _count - 1)
            {
                Clear();
            }
            else
            {
                var removeCount = 0;
                for (int i = 0; i <= fromIndex; i++)
                {
                    this[i] = default(T);
                    removeCount++;
                }
                _indexOffset += removeCount;
                _count -= removeCount;
            }
        }

        /// <summary> 包含fromIndex </summary>
        /// <param name="fromIndex"></param>
        public void RemoveToLast(int fromIndex)
        {
            //var validIndex = GetValidIndex(fromIndex);
            if (fromIndex <= 0)
            {
                Clear();
            }
            else
            {
                var removeCount = 0;
                for (int i = fromIndex; i < _count; i++)
                {
                    this[i] = default(T);
                    removeCount++;
                }
                _count -= removeCount;
            }
        }

        public void Insert(int insertBefore, T value)
        {
            if (_count == 0)
            {
                AddLast(value);
                return;
            }

            //var validIndex = GetValidIndex(insertBefore);
            /// array已满时，insert发生队列膨胀，此时不使用Expand，因为Expand函数会直接分配并移动数据到新的Array
            /// 而插入数据的操作还要重新位移成员，为了避免性能浪费，新array分配后，拷贝数据的操作此处单独处理，以直接包含需要插入的数据
            if (_count == _array.Length)
            {
                int newLen = (int)(_array.Length * ExpandFactor);
                var newArray = new T[newLen];
                int writingIndex = 0;
                for (int i = 0; i < insertBefore; i++)
                {
                    newArray[writingIndex] = this[i];
                    writingIndex++;
                }
                newArray[writingIndex] = value;
                writingIndex++;
                for (int i = insertBefore; i < _count; i++)
                {
                    newArray[writingIndex] = this[i];
                    writingIndex++;
                }
                _array = newArray;
                _indexOffset = 0;
                _count++;
            }
            /// 如果array未满，插入数据时检查索引距离两端的长度，如果首端方向更近，则前移之前的项并让索引偏移-1
            else
            {
                var midIndex = _count / 2;
                if (insertBefore < midIndex)
                {
                    AddFirst(this[0]); // 此时头部索引已前移，Count已增加
                    for (int i = 1; i < insertBefore; i++)
                    {
                        this[i] = this[i + 1];
                    }
                    this[insertBefore] = value;
                }
                else
                {
                    AddLast(this[_count - 1]); // 此时尾部已延长，Count已增加，注意循环是从尾端向前，i--
                    for (int i = _count - 2; i > insertBefore; i--)
                    {
                        this[i] = this[i - 1];
                    }
                    this[insertBefore] = value;
                }
            }
        }

        public T GetFirst()
        {
            return this[0];
        }

        public ref T GetFirstRef()
        {
            return ref GetRef(0);
        }


        public T GetLast()
        {
            return this[_count - 1];
        }

        public ref T GetLastRef()
        {
            return ref GetRef(-1);
        }

        public T PopFirst()
        {
            var first = this[0];
            RemoveFirst();
            return first;
        }

        public T PopLast()
        {
            var last = this[_count - 1];
            RemoveLast();
            return last;
        }

        public void Clear()
        {
            Array.Clear(_array, 0, _array.Length);
            _indexOffset = 0;
            _count = 0;
        }

        public int GetLoopedIndex(int index)
        {
            return XMath.Loop(index, _count);
        }

        public int ClampToValidIndex(int index)
        {
            return XMath.Clamp(index, 0, _count - 1);
        }

        public void Resize(int newSize)
        {
            MoveToNewArray(newSize);
        }

        public T[] ToArray()
        {
            var array = new T[_count];
            for (int i = 0; i < _count; i++)
            {
                array[i] = this[i];
            }
            return array;
        }

        public void Sort(Comparison<T> comparison)
        {
            if (_indexOffset < 0 || _indexOffset + Count > _array.Length)
            {
                MoveToNewArray(_array.Length);
            }
            var right = _indexOffset + Count - 1;
            _array.QuickSort(_indexOffset, right, comparison);
        }

        public Span<T> AsSpan()
        {
            if (_indexOffset < 0)
            {
                MoveToNewArray(_array.Length);
            }
            return new Span<T>(_array, _indexOffset, Count);
        }

        public ReadOnlySpan<T> AsReadOnlySpan()
        {
            if (_indexOffset < 0)
            {
                MoveToNewArray(_array.Length);
            }
            return new ReadOnlySpan<T>(_array, _indexOffset, Count);
        }

        public void Add(T item)
        {
            AddLast(item);
        }

        public void Add(ICollection<T> items)
        {
            var newCount = _count + items.Count;
            if (newCount > _array.Length)
            {
                MoveToNewArray(newCount);
            }
            var index = _count;
            foreach (var item in items)
            {
                _array[index] = item;
                index++;
            }
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
            for (int i = 0; i < _count; i++)
            {
                array[arrayIndex++] = this[i];
            }
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

        public void RemoveAt(int index)
        {
            /// 删除数据时检查索引距离两端的长度，如果首端方向更近，则后移前段队列
            var midIndex = _count / 2;
            if (index < midIndex)
            {
                for (int i = index; i > 0; i--)
                {
                    this[i] = this[i - 1];
                }
                this[0] = default(T);
                _indexOffset++;
                _count--;
            }
            // 反之前移后段队列
            else
            {
                for (int i = index; i < _count - 1; i++)
                {
                    this[i] = this[i + 1];
                }
                this[_count - 1] = default(T);
                _count--;
            }
        }

        public Enumerator GetEnumerator()
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

        int GetAccessIndex(int index)
        {
            return XMath.Loop(index + _indexOffset, _array.Length);
        }

        void ExpandIfArrayIsFull()
        {
            if (_count == _array.Length)
            {
                Expand();
            }
        }

        /// <summary> 默认的膨胀系数为1.5 </summary>
        void Expand()
        {
            int newLen = (int)(_array.Length * ExpandFactor);
            MoveToNewArray(newLen);
        }

        void MoveToNewArray(int newLen)
        {
            var newArray = new T[newLen];
            var countToCopy = Math.Min(newLen, _count);
            for (int i = 0; i < countToCopy; i++)
            {
                newArray[i] = this[i];
            }
            _array = newArray;
            _indexOffset = 0;
            _count = countToCopy;
        }

    }
}
