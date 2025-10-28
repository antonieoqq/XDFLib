using System.Runtime.CompilerServices;

namespace XDFLib
{
    public static class Global<T> where T : new()
    {
        static T _instance;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get(bool createIfNotExist = false)
        {
            if (_instance == null && createIfNotExist)
            {
                _instance = new T();
            }
            return _instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Set(T instance)
        {
            _instance = instance;
            return _instance;
        }
    }
}
