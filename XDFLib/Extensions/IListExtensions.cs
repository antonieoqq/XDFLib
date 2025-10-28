using System;
using System.Collections.Generic;

namespace XDFLib.Extensions
{
    public static class IListExtensions
    {
        public static T Pop<T>(this IList<T> list)
        {
            if (list.Count > 0)
            {
                var last = list.Count - 1;
                T v = list[last];
                list.RemoveAt(last);
                return v;
            }
            else
                return default;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = XMath.Rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T GetRandomElement<T>(this IList<T> list)
        {
            if (list.Count > 0)
            {
                var currIndex = XMath.RandomRange(0, list.Count);
                return list[currIndex];
            }
            else
            {
                return default;
            }
        }

        public static T GetRandomElement<T>(this IList<T> list, int seed)
        {
            if (list.Count > 0)
            {
                var currIndex = XMath.LCGRandom(seed, 0, list.Count);
                return list[currIndex];
            }
            else
            {
                return default;
            }
        }


        public static void RemoveRandomElement<T>(this IList<T> list)
        {
            if (list.Count > 0)
            {
                var currIndex = XMath.RandomRange(0, list.Count);
                list.RemoveAt(currIndex);
            }
        }

        public static bool IsIndexValid<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        public static T GetByLoopedIndex<T>(this IList<T> list, int index)
        {
            if (list.Count > 0)
            {
                var loopedIndex = XMath.Loop(index, 0, list.Count);
                return list[loopedIndex];
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
