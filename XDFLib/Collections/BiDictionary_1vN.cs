using System;
using System.Collections;
using System.Collections.Generic;

namespace XDFLib.Collections
{
    public class BiDictionary_1vN<Tk, Tv> : IEnumerable
    {
        Dictionary<Tk, Deque<Tv>> _k2vs;
        Dictionary<Tv, Tk> _v2k;

        public BiDictionary_1vN()
        {
            _k2vs = ObjectPool<Dictionary<Tk, Deque<Tv>>>.Get();
            _v2k = ObjectPool<Dictionary<Tv, Tk>>.Get();
        }

        ~BiDictionary_1vN()
        {
            Clear();
            ObjectPool<Dictionary<Tk, Deque<Tv>>>.Recycle(_k2vs);
            ObjectPool<Dictionary<Tv, Tk>>.Recycle(_v2k);
        }

        public int KeyCount => _k2vs.Count;
        public int ValueCount => _v2k.Count;

        public bool Add(Tk key, Tv value)
        {
            if (_v2k.TryGetValue(value, out var k))
            {
                return false;
            }
            if (!_k2vs.TryGetValue(key, out var vs))
            {
                vs = ObjectPool<Deque<Tv>>.Get();
                _k2vs.Add(key, vs);
            }
            if (vs.Contains(value))
            {
                return false;
            }

            vs.Add(value);
            _v2k.Add(value, key);
            return true;
        }

        public void Clear()
        {
            foreach (var kv in _k2vs)
            {
                kv.Value.Clear();
                ObjectPool<Deque<Tv>>.Recycle(kv.Value);
            }
            _k2vs.Clear();
            _v2k.Clear();
        }

        public bool Remove(Tk key, Tv value)
        {
            if (_k2vs.TryGetValue(key, out var vs) && vs.Contains(value)
                && _v2k.TryGetValue(value, out var k) && k.Equals(key))
            {
                RemoveKV(key, value, vs);
                return true;
            }
            return false;
        }

        public bool RemoveByKey(Tk key)
        {
            if (_k2vs.TryGetValue(key, out var vs))
            {
                foreach (var v in vs) { _v2k.Remove(v); }

                vs.Clear();
                ObjectPool<Deque<Tv>>.Recycle(vs);
                _k2vs.Remove(key);
            }
            return false;
        }

        public bool RemoveByValue(Tv value)
        {
            if (_v2k.TryGetValue(value, out var key) && _k2vs.TryGetValue(key, out var vs))
            {
                RemoveKV(key, value, vs);
                return true;
            }
            return false;
        }

        private void RemoveKV(Tk key, Tv value, Deque<Tv> vs)
        {
            vs.Remove(value);
            if (vs.Count == 0)
            {
                ObjectPool<Deque<Tv>>.Recycle(vs);
                _k2vs.Remove(key);
            }
            _v2k.Remove(value);
        }

        public ReadOnlySpan<Tv> GetValues(Tk key)
        {
            if (_k2vs.TryGetValue(key, out var values))
            {
                return values.AsSpan();
            }
            return ReadOnlySpan<Tv>.Empty;
        }

        public Tk GetKey(Tv value)
        {
            if (_v2k.TryGetValue(value, out var key))
            {
                return key;
            }
            return default;
        }

        public IEnumerator GetEnumerator()
        {
            return _k2vs.GetEnumerator();
        }
    }
}
