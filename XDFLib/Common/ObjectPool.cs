using System;
using System.Collections.Concurrent;

namespace XDFLib
{
    public class ObjectPool
    {
        public bool HasCustomCreator => _itemCreator != null;
        Func<object> _itemCreator = null;
        ConcurrentStack<object> _standby = new ConcurrentStack<object>();

        public int Count => _standby.Count;

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
            object result;
            if (!_standby.TryPop(out result))
            {
                result = _itemCreator();
            }

            (result as IPoolOperations)?.OnGetFromPool();
            return result;
        }

        public void Recycle(object obj)
        {
            (obj as IPoolOperations)?.OnRecycleToPool();
            _standby.Push(obj);
        }
    }

    public static class ObjectPool<T> where T : class, new()
    {
        public static int Count => _pool.Count;

        static ObjectPool _poolCache;
        static ObjectPool _pool => _poolCache ??= ObjectRepository.GetPool<T>();

        public static T Get() => _pool.Get() as T;
        public static void Recycle(T obj) => _pool.Recycle(obj);
        public static void Keep(int count) => _pool.Keep(count);
        public static void SetConstructor(Func<T> ctr) => _pool.SetObjectConstructor(ctr);

        //static ConcurrentStack<T> _standby = new ConcurrentStack<T>();
        //public static int Count => _standby.Count;

        //public static void Keep(int count)
        //{
        //    count = count > 1 ? count : 1;
        //    var gap = count - _standby.Count;
        //    if (gap == 0) { return; }
        //    if (gap < 0)
        //    {
        //        for (int i = 0; i < -gap; i++)
        //        {
        //            _standby.TryPop(out var _);
        //        }
        //    }
        //    else
        //    {
        //        var add = new T[gap];
        //        for (int i = 0; i < gap; i++)
        //        {
        //            add[i] = new T();
        //        }
        //        _standby.PushRange(add);
        //    }
        //}

        //public static T Get()
        //{
        //    T result;
        //    if (!_standby.TryPop(out result))
        //    {
        //        result = new T();
        //    }
        //    result.OnGetFromPool();
        //    return result;

        //}

        //public static void Recycle(T obj)
        //{
        //    obj.OnRecycleToPool();
        //    _standby.Push(obj);
        //}
    }
}
