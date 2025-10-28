using System;
using System.Collections.Concurrent;

namespace XDFLib
{
    public class ObjectPool
    {
        public bool HasCustomCreator => _itemCreator != null;
        Func<object> _itemCreator = null;
        ConcurrentStack<object> _standby = new ConcurrentStack<object>();

        public ObjectPool(Func<object> createFunc)
        {
            _itemCreator = createFunc;
        }

        public void SetObjectConstructor(Func<object> createFunc)
        {
            _itemCreator = createFunc;
        }

        public void Keep(int count)
        {
            count = count > 1 ? count : 1;
            var gap = count - _standby.Count;
            if (gap == 0) { return; }
            if (gap < 0)
            {
                for (int i = 0; i < -gap; i++)
                {
                    _standby.TryPop(out var _);
                }
            }
            else
            {
                var add = new object[gap];
                for (int i = 0; i < gap; i++)
                {
                    add[i] = _itemCreator();
                }
                _standby.PushRange(add);
            }
        }


        public object Get()
        {
            if (_standby.TryPop(out var result)) { return result; }
            else { return _itemCreator(); }
        }

        public void Recycle(object obj)
        {
            _standby.Push(obj);
        }
    }

    public static class ObjectPool<T> where T : class, new()
    {
        static ConcurrentStack<T> _standby = new ConcurrentStack<T>();
        public static int Count => _standby.Count;

        public static void Keep(int count)
        {
            count = count > 1 ? count : 1;
            var gap = count - _standby.Count;
            if (gap == 0) { return; }
            if (gap < 0)
            {
                for (int i = 0; i < -gap; i++)
                {
                    _standby.TryPop(out var _);
                }
            }
            else
            {
                var add = new T[gap];
                for (int i = 0; i < gap; i++)
                {
                    add[i] = new T();
                }
                _standby.PushRange(add);
            }
        }

        public static T Get()
        {
            if (_standby.TryPop(out var result))
            {
                return result;
            }
            else
            {
                return new T();
            }
        }

        public static void Recycle(T obj)
        {
            _standby.Push(obj);
        }
    }
}
