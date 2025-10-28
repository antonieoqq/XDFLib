using System;
using System.Collections;
using System.Collections.Generic;

namespace XDFLib.Collections
{
    /// <summary>
    /// 这是一个同时将正向表与反向表整合在一张_bindTable的数据结构，表示存在互相关联的一个成员集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BiSet<T> : IEnumerable
    {
        Dictionary<T, HashSet<T>> _binds;

        public BiSet()
        {
            _binds = new Dictionary<T, HashSet<T>>();
        }

        public BiSet(Dictionary<T, HashSet<T>> binds)
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

        public HashSet<T> this[T key]
        {
            get
            {
                HashSet<T> r;
                if (_binds.TryGetValue(key, out r))
                {
                    return r;
                }
                throw new ArgumentOutOfRangeException("SameTypeBindMap");
            }
            set
            {
                Remove(key);

                if (value != null && value.Count > 0)
                {
                    _binds.Add(key, value);
                    foreach (var dest in value)
                    {
                        HashSet<T> currDestBind;
                        if (TryGetBinds(dest, out currDestBind))
                        {
                            currDestBind.Add(key);
                        }
                        else
                        {
                            currDestBind = new HashSet<T>();
                            currDestBind.Add(key);
                            _binds.Add(dest, currDestBind);
                        }
                    }
                }
            }
        }

        public bool ContainsKey(T key)
        {
            HashSet<T> binds;
            return TryGetBinds(key, out binds);
        }

        public bool TryGetBinds(T key, out HashSet<T> binds)
        {
            return _binds.TryGetValue(key, out binds);
        }

        public void Add(T key, T dest)
        {
            HashSet<T> binds;
            if (TryGetBinds(key, out binds))
            {
                binds.Add(dest);
            }
            else
            {
                binds = new HashSet<T>();
                binds.Add(dest);
                _binds.Add(key, binds);
            }

            if (TryGetBinds(dest, out binds))
            {
                binds.Add(key);
            }
            else
            {
                binds = new HashSet<T>();
                binds.Add(key);
                _binds.Add(dest, binds);
            }
        }

        public bool Remove(T key)
        {
            HashSet<T> binded;
            if (TryGetBinds(key, out binded))
            {
                foreach (var destKey in binded)
                {
                    HashSet<T> destBindSet;
                    if (TryGetBinds(destKey, out destBindSet))
                    {
                        destBindSet.Remove(key);
                        if (destBindSet.Count == 0)
                        {
                            _binds.Remove(destKey);
                        }
                    }
                }
                _binds.Remove(key);
                return true;
            }
            return false;
        }

        public bool Remove(T key, T dest)
        {
            HashSet<T> bindSet, bindSetInverse;
            if (TryGetBinds(key, out bindSet))
            {
                if (bindSet.Contains(dest))
                {
                    bindSet.Remove(dest);
                    if (bindSet.Count == 0)
                    {
                        _binds.Remove(key);
                    }
                    if (TryGetBinds(dest, out bindSetInverse))
                    {
                        if (bindSetInverse.Contains(key))
                        {
                            bindSetInverse.Remove(key);
                            if (bindSetInverse.Count == 0)
                            {
                                _binds.Remove(dest);
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            _binds.Clear();
        }

        public IEnumerator<KeyValuePair<T, HashSet<T>>> GetEnumerator()
        {
            return _binds.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _binds.GetEnumerator();
        }
    }
}
