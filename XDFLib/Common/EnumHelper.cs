using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace XDFLib
{
    public static class EnumHelper
    {
        private static unsafe int EnumAsInt32<T>(T item) where T : unmanaged, Enum
        {
            return *(int*)(&item);
        }

        private static unsafe ulong EnumAsULong64<T>(T item) where T : unmanaged, Enum
        {
            ulong x;
            if (sizeof(T) == 1)
                x = *(byte*)(&item);
            else if (sizeof(T) == 2)
                x = *(ushort*)(&item);
            else if (sizeof(T) == 4)
                x = *(uint*)(&item);
            else if (sizeof(T) == 8)
                x = *(ulong*)(&item);
            else
                throw new ArgumentException("Argument is not a usual enum type; it is not 1, 2, 4, or 8 bytes in length.");
            return x;
        }

        // 不要尝试在函数中调用 EnumAsInt32, 会有性能损失
        public static unsafe bool BitwiseAnd<T>(T enum1, T enum2) where T : unmanaged, Enum
        {
            var e1 = *(int*)(&enum1);
            var e2 = *(int*)(&enum2);
            var match = e1 & e2;
            return match != 0;
        }

        public static T GetRandomEnum<T>() where T : struct, Enum
        {
            var values = Enum.GetValues(typeof(T));
            var index = XRandom.LCG.Random(0, values.Length);
            var v = (T)values.GetValue(index);
            return v;
        }

        public static T GetLCGRandomEnum<T>(int seed) where T : struct, Enum
        {
            var values = Enum.GetValues(typeof(T));
            var index = XRandom.LCG.Random(ref seed, 0, values.Length);
            var v = (T)values.GetValue(index);
            return v;
        }

        public static IEnumerable<T> GetEnumValues<T>() where T : struct, Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static void FetchEnumValues<T>(out IEnumerable<T> values, out int length) where T : struct, Enum
        {
            var array = Enum.GetValues(typeof(T));
            length = array.Length;
            values = array.Cast<T>();
        }

        public static T[] BuildEnumValuesArray<T>() where T : struct, Enum
        {
            var array = Enum.GetValues(typeof(T));
            var result = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = (T)array.GetValue(i);
            }
            return result;
        }

        public static Dictionary<T, string> BuildEnumDescriptionDictionary<T>() where T : struct, Enum
        {
            var dict = new Dictionary<T, string>();
            foreach (var e in GetEnumValues<T>())
            {
                var desc = GetEnumDescription(e);
                dict.Add(e, desc);
            }
            return dict;
        }

        public static string GetEnumDescription<T>(T e) where T : struct, Enum
        {
            var memInfo = typeof(T).GetField(Enum.GetName(typeof(T), e));
            var descAttr = Attribute.GetCustomAttribute(memInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return descAttr?.Description ?? string.Empty;
        }
    }
}
