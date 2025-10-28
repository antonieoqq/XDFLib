using System.Collections;
using System.Collections.Generic;

namespace XDFLib.Collections
{
    public class BiDictionary<Ta, Tb> : IEnumerable
    {
        Dictionary<Ta, HashSet<Tb>> _a2b = new Dictionary<Ta, HashSet<Tb>>();
        Dictionary<Tb, HashSet<Ta>> _b2a = new Dictionary<Tb, HashSet<Ta>>();

        public int Count { get { return _a2b.Count; } }

        public int EnsureCapacity(int capacity)
        {
            _a2b.EnsureCapacity(capacity);
            return _b2a.EnsureCapacity(capacity);
        }

        public bool ContainsBind(Ta ta, Tb tb)
        {
            HashSet<Tb> r;
            if (TryGetByTypeA(ta, out r))
            {
                return r.Contains(tb);
            }
            else
            {
                return false;
            }
        }

        public bool ContainsKey(Ta key)
        {
            return ContainsKeyByTypeA(key);
        }

        public bool ContainsKeyByTypeA(Ta key)
        {
            HashSet<Tb> r;
            return TryGetByTypeA(key, out r);
        }

        public bool TryGet(Ta key, out HashSet<Tb> binds)
        {
            return TryGetByTypeA(key, out binds);
        }

        public bool TryGetByTypeA(Ta key, out HashSet<Tb> binds)
        {
            return _a2b.TryGetValue(key, out binds);
        }

        public bool ContainsKey(Tb key)
        {
            return ContainsKeyByTypeB(key);
        }

        public bool ContainsKeyByTypeB(Tb key)
        {
            HashSet<Ta> r;
            return TryGetByTypeB(key, out r);
        }

        public bool TryGet(Tb key, out HashSet<Ta> binds)
        {
            return TryGetByTypeB(key, out binds);
        }

        public bool TryGetByTypeB(Tb key, out HashSet<Ta> binds)
        {
            return _b2a.TryGetValue(key, out binds);
        }

        public void Add(Ta a, Tb b)
        {
            HashSet<Tb> bindsB;
            var a2b_HasKey = TryGetByTypeA(a, out bindsB);
            HashSet<Ta> bindsA;
            var b2a_HasKey = TryGetByTypeB(b, out bindsA);

            if (!a2b_HasKey)
            {
                bindsB = new HashSet<Tb>();
                _a2b.Add(a, bindsB);
            }
            bindsB.Add(b);

            if (!b2a_HasKey)
            {
                bindsA = new HashSet<Ta>();
                _b2a.Add(b, bindsA);
            }
            bindsA.Add(a);
        }

        public bool Remove(Ta a, Tb b)
        {
            if (!ContainsBind(a, b))
            {
                return false;
            }

            HashSet<Tb> bindsB;
            var a2b_HasKey = TryGetByTypeA(a, out bindsB);
            var a2b_hasBind = false;
            if (a2b_HasKey)
            {
                a2b_hasBind = bindsB.Contains(b);
            }
            HashSet<Ta> bindsA;
            var b2a_HasKey = TryGetByTypeB(b, out bindsA);
            var b2a_hasBind = false;
            if (b2a_HasKey)
            {
                b2a_hasBind = bindsA.Contains(a);
            }

            if (a2b_HasKey && a2b_hasBind && b2a_HasKey && b2a_hasBind)
            {
                bindsB.Remove(b);
                if (bindsB.Count == 0)
                {
                    _a2b.Remove(a);
                }

                bindsA.Remove(a);
                if (bindsA.Count == 0)
                {
                    _b2a.Remove(b);
                }

                return true;
            }

            return false;
        }

        public bool Remove(Ta a)
        {
            return RemoveByTypeA(a);
        }

        public bool RemoveByTypeA(Ta a)
        {
            HashSet<Tb> bBinds;
            if (TryGetByTypeA(a, out bBinds))
            {
                _a2b.Remove(a);
                foreach (var b in bBinds)
                {
                    HashSet<Ta> aBinds;
                    if (TryGetByTypeB(b, out aBinds))
                    {
                        aBinds.Remove(a);
                        if (aBinds.Count == 0)
                        {
                            _b2a.Remove(b);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public bool Remove(Tb b)
        {
            return RemoveByTypeB(b);
        }

        public bool RemoveByTypeB(Tb b)
        {
            HashSet<Ta> aBinds;
            if (TryGetByTypeB(b, out aBinds))
            {
                _b2a.Remove(b);
                foreach (var a in aBinds)
                {
                    HashSet<Tb> bBinds;
                    if (TryGetByTypeA(a, out bBinds))
                    {
                        bBinds.Remove(b);
                        if (aBinds.Count == 0)
                        {
                            _a2b.Remove(a);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public void Clear()
        {
            _a2b.Clear();
            _b2a.Clear();
        }


        public IEnumerator<KeyValuePair<Ta, HashSet<Tb>>> GetEnumerator()
        {
            return _a2b.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<Ta, HashSet<Tb>>> Get_A2B_Enumerator()
        {
            return _a2b.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<Tb, HashSet<Ta>>> Get_B2A_Enumerator()
        {
            return _b2a.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _a2b.GetEnumerator();
        }

    }
}
