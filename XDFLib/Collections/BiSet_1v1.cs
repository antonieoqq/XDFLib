using System;
using System.Collections;
using System.Collections.Generic;

namespace XDFLib.Collections
{
    /// <summary>
    /// 这是一个同时将正向表与反向表整合在一张_bindTable的数据结构，表示存在互相关联的一个成员集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BiSet_1v1<T> : IEnumerable
    {
        Dictionary<T, T> _binds;

        public BiSet_1v1()
        {
            _binds = new Dictionary<T, T>();
        }

        public BiSet_1v1(Dictionary<T, T> binds)
        {
            _binds = binds;
            if (_binds == null)
            {
                throw new Exception("Initiating BiSet with invalid Data");
            }
        }

        public int Count { get { return _binds.Count; } }

        public int EnsureCapacity(int capacity)
        {
            return _binds.EnsureCapacity(capacity);
        }

        public T this[T key]
        {
            get
            {
                T r;
                if (_binds.TryGetValue(key, out r))
                {
                    return r;
                }
                throw new ArgumentOutOfRangeException("SameTypeBindMap");
            }
            set
            {
                if (value != null)
                {
                    TryAdd(key, value, true);
                }
            }
        }

        public bool ContainsKey(T key)
        {
            T r;
            return TryGetBind(key, out r);
        }

        public bool TryGetBind(T key, out T bind)
        {
            return _binds.TryGetValue(key, out bind);
        }

        public bool TryAdd(T key, T bind, bool overwrite = false)
        {
            if (key.Equals(bind))
            {
                return false;
            }

            if (overwrite)
            {
                Remove(key);
                Remove(bind);
                _binds.Add(key, bind);
                _binds.Add(bind, key);
                return true;
            }
            else if (!ContainsKey(key) && !ContainsKey(bind))
            {
                _binds.Add(key, bind);
                _binds.Add(bind, key);
                return true;
            }
            return false;
        }

        public bool Remove(T key)
        {
            T r;
            if (TryGetBind(key, out r))
            {
                _binds.Remove(key);
                _binds.Remove(r);
            }
            return false;
        }

        public bool Remove(T key, T bind)
        {
            T r;
            if (TryGetBind(key, out r))
            {
                if (r.Equals(bind))
                {
                    _binds.Remove(key);
                    _binds.Remove(bind);
                    return true;
                }
                return false;
            }
            return false;
        }

        public void Clear()
        {
            _binds.Clear();
        }

        public IEnumerator<KeyValuePair<T, T>> GetEnumerator()
        {
            return _binds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _binds.GetEnumerator();
        }
    }
}
