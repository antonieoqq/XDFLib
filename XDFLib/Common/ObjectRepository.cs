using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace XDFLib
{
    /// <summary>
    /// 这个类的存在，本质上是为了解决 Recycle 一个 object，但无法在编译时确定 T 的情况
    /// </summary>
    public static class ObjectRepository
    {
        static ConcurrentDictionary<Type, ObjectPool> _pools = new ConcurrentDictionary<Type, ObjectPool>();

        internal static ObjectPool GetPool<T>() where T : class, new()
        {
            if (!_pools.TryGetValue(typeof(T), out var pool))
            {
                pool = new ObjectPool(() => { return new T(); });
                _pools[typeof(T)] = pool;
            }
            return pool;
        }

        public static bool SetPoolConstructor<T>(Func<T> constructor) where T : class, new()
        {
            if (constructor == null) return false;

            var pool = GetPool<T>();
            if (pool == null) return false;
            pool.SetObjectConstructor(constructor);
            return true;
        }

        //public static bool SetPoolConstructor(Func<IPoolable> constructor, Type type)
        //{
        //    if (constructor == null) return false;

        //    var pool = GetPool(type);
        //    if (pool == null) return false;
        //    pool.SetObjectConstructor(constructor);
        //    return true;
        //}

        public static void Keep<T>(int count) where T : class, new()
        {
            var pool = GetPool<T>();
            pool?.Keep(count);
        }

        //public static void Keep(Type type, int count)
        //{
        //    var pool = GetPool(type);
        //    pool?.Keep(count);
        //}

        public static bool RemovePool<T>() where T : class, new()
        {
            return _pools.Remove(typeof(T), out var _);
        }

        //public static bool RemovePool(Type type)
        //{
        //    return _pools.Remove(type, out var _);
        //}

        public static T GetObject<T>() where T : class, new()
        {
            var pool = GetPool<T>();
            return pool?.Get() as T;
        }

        //public static object GetObject(Type type)
        //{
        //    var pool = GetPool(type);
        //    return pool?.Get();
        //}

        public static void RecycleObject<T>(T obj) where T : class, new()
        {
            var pool = GetPool<T>();
            pool?.Recycle(obj);
        }

        public static void RecycleObject(object obj)
        {
            var type = obj.GetType();
            var pool = GetPool(type);
            pool?.Recycle(obj);
        }

        public static void Clear()
        {
            _pools.Clear();
        }

        static ObjectPool GetPool(Type type)
        {
            _pools.TryGetValue(type, out var pool);
            return pool;
            //if (!_pools.TryGetValue(type, out var pool))
            //{
            //    pool = new ObjectPool(() => { return Activator.CreateInstance(type) as IPoolable; });
            //    _pools[type] = pool;
            //}
            //return pool;
        }
    }
}
