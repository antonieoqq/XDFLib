using System;

namespace XDFLib
{
    public class XEvent
    {
        Action _act;

        public void AddListener(Action listoner, bool allowDuplicateListeners = false)
        {
            if (!allowDuplicateListeners)
            {
                _act -= listoner;
            }
            _act += listoner;
        }

        public void RemoveListener(Action listoner)
        {
            _act -= listoner;
        }

        public void ClearListener()
        {
            _act = null;
        }

        public void Invoke()
        {
            _act?.Invoke();
        }
    }

    public class XEvent<T>
    {
        Action<T> _act;

        public void AddListener(Action<T> listoner, bool allowDuplicateListeners = false)
        {
            if (!allowDuplicateListeners)
            {
                _act -= listoner;
            }
            _act += listoner;
        }

        public void RemoveListener(Action<T> listoner)
        {
            _act -= listoner;
        }

        public void ClearListener()
        {
            _act = null;
        }

        public void Invoke(T val)
        {
            _act?.Invoke(val);
        }
    }

    public class XEvent<T1, T2>
    {
        Action<T1, T2> _act;

        public void AddListener(Action<T1, T2> listoner, bool allowDuplicateListeners = false)
        {
            if (!allowDuplicateListeners)
            {
                _act -= listoner;
            }
            _act += listoner;
        }

        public void RemoveListener(Action<T1, T2> listoner)
        {
            _act -= listoner;
        }

        public void ClearListener()
        {
            _act = null;
        }

        public void Invoke(T1 val1, T2 val2)
        {
            _act?.Invoke(val1, val2);
        }
    }

    public class XEvent<T1, T2, T3>
    {
        Action<T1, T2, T3> _act;

        public void AddListener(Action<T1, T2, T3> listoner, bool allowDuplicateListeners = false)
        {
            if (!allowDuplicateListeners)
            {
                _act -= listoner;
            }
            _act += listoner;
        }

        public void RemoveListener(Action<T1, T2, T3> listoner)
        {
            _act -= listoner;
        }

        public void ClearListener()
        {
            _act = null;
        }

        public void Invoke(T1 val1, T2 val2, T3 val3)
        {
            _act?.Invoke(val1, val2, val3);
        }
    }

    public class XEvent<T1, T2, T3, T4>
    {
        Action<T1, T2, T3, T4> _act;

        public void AddListener(Action<T1, T2, T3, T4> listoner, bool allowDuplicateListeners = false)
        {
            if (!allowDuplicateListeners)
            {
                _act -= listoner;
            }
            _act += listoner;
        }

        public void RemoveListener(Action<T1, T2, T3, T4> listoner)
        {
            _act -= listoner;
        }

        public void ClearListener()
        {
            _act = null;
        }

        public void Invoke(T1 val1, T2 val2, T3 val3, T4 val4)
        {
            _act?.Invoke(val1, val2, val3, val4);
        }
    }

    public class XEvent<T1, T2, T3, T4, T5>
    {
        Action<T1, T2, T3, T4, T5> _act;

        public void AddListener(Action<T1, T2, T3, T4, T5> listoner, bool allowDuplicateListeners = false)
        {
            if (!allowDuplicateListeners)
            {
                _act -= listoner;
            }
            _act += listoner;
        }

        public void RemoveListener(Action<T1, T2, T3, T4, T5> listoner)
        {
            _act -= listoner;
        }

        public void ClearListener()
        {
            _act = null;
        }

        public void Invoke(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5)
        {
            _act?.Invoke(val1, val2, val3, val4, val5);
        }
    }

    public class XEvent<T1, T2, T3, T4, T5, T6>
    {
        Action<T1, T2, T3, T4, T5, T6> _act;

        public void AddListener(Action<T1, T2, T3, T4, T5, T6> listoner, bool allowDuplicateListeners = false)
        {
            if (!allowDuplicateListeners)
            {
                _act -= listoner;
            }
            _act += listoner;
        }

        public void RemoveListener(Action<T1, T2, T3, T4, T5, T6> listoner)
        {
            _act -= listoner;
        }

        public void ClearListener()
        {
            _act = null;
        }

        public void Invoke(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6)
        {
            _act?.Invoke(val1, val2, val3, val4, val5, val6);
        }
    }

    public class XEvent<T1, T2, T3, T4, T5, T6, T7>
    {
        Action<T1, T2, T3, T4, T5, T6, T7> _act;

        public void AddListener(Action<T1, T2, T3, T4, T5, T6, T7> listoner, bool allowDuplicateListeners = false)
        {
            if (!allowDuplicateListeners)
            {
                _act -= listoner;
            }
            _act += listoner;
        }

        public void RemoveListener(Action<T1, T2, T3, T4, T5, T6, T7> listoner)
        {
            _act -= listoner;
        }

        public void ClearListener()
        {
            _act = null;
        }

        public void Invoke(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6, T7 val7)
        {
            _act?.Invoke(val1, val2, val3, val4, val5, val6, val7);
        }
    }

    public class XEvent<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        Action<T1, T2, T3, T4, T5, T6, T7, T8> _act;

        public void AddListener(Action<T1, T2, T3, T4, T5, T6, T7, T8> listoner, bool allowDuplicateListeners = false)
        {
            if (!allowDuplicateListeners)
            {
                _act -= listoner;
            }
            _act += listoner;
        }

        public void RemoveListener(Action<T1, T2, T3, T4, T5, T6, T7, T8> listoner)
        {
            _act -= listoner;
        }

        public void ClearListener()
        {
            _act = null;
        }

        public void Invoke(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6, T7 val7, T8 val8)
        {
            _act?.Invoke(val1, val2, val3, val4, val5, val6, val7, val8);
        }
    }

}
