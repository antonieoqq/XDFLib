using System.Collections;
using System.Collections.Generic;

namespace XDFLib.Collections
{
    public class BiDictionary_1v1<Ta, Tb> : IEnumerable
    {
        Dictionary<Ta, Tb> _a2b = new Dictionary<Ta, Tb>();
        Dictionary<Tb, Ta> _b2a = new Dictionary<Tb, Ta>();

        public int Count { get { return _a2b.Count; } }

        public int EnsureCapacity(int capacity)
        {
            _a2b.EnsureCapacity(capacity);
            return _b2a.EnsureCapacity(capacity);
        }

        public bool ContainsKeyA(Ta a)
        {
            return _a2b.TryGetValue(a, out Tb b);
        }

        public bool ContainsKeyB(Tb b)
        {
            return _b2a.TryGetValue(b, out Ta a);
        }

        public bool TryGetByKeyA(Ta a, out Tb b)
        {
            return _a2b.TryGetValue(a, out b);
        }

        public bool TryGetByKeyB(Tb b, out Ta a)
        {
            return _b2a.TryGetValue(b, out a);
        }

        public bool Add(Ta a, Tb b)
        {
            var a2b_hasKey = ContainsKeyA(a);
            var b2a_haskey = ContainsKeyB(b);
            if (!a2b_hasKey && !b2a_haskey)
            {
                _a2b.Add(a, b);
                _b2a.Add(b, a);
                return true;
            }
            return false;
        }

        public bool RemoveByKeyA(Ta a)
        {
            var hasKey = TryGetByKeyA(a, out Tb b);
            if (hasKey)
            {
                _a2b.Remove(a);
                _b2a.Remove(b);
                return true;
            }
            return false;
        }

        public bool RemoveByKeyB(Tb b)
        {
            var hasKey = TryGetByKeyB(b, out Ta a);
            if (hasKey)
            {
                _a2b.Remove(a);
                _b2a.Remove(b);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            _a2b.Clear();
            _b2a.Clear();
        }

        public void SetValueBToKeyA(Ta keyA, Tb valueB)
        {
            RemoveByKeyA(keyA);
            Add(keyA, valueB);
        }

        public void SetValueAToKeyB(Tb keyB, Ta valueA)
        {
            RemoveByKeyB(keyB);
            Add(valueA, keyB);
        }

        public IEnumerator<KeyValuePair<Ta, Tb>> GetEnumerator()
        {
            return _a2b.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<Ta, Tb>> Get_A2B_Enumerator()
        {
            return _a2b.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<Tb, Ta>> Get_B2A_Enumerator()
        {
            return _b2a.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _a2b.GetEnumerator();
        }
    }
}
