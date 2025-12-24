using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XDFLib.XRandom;

namespace XDFLib
{
    public static class EnumHelper
    {
        public static T GetRandomEnum<T>() where T : struct, Enum
        {
            var values = Enum.GetValues(typeof(T));
            var index = SplitMix32.Random(0, values.Length);
            var v = (T)values.GetValue(index);
            return v;
        }

        public static T GetRandomEnum<T>(ref int seed) where T : struct, Enum
        {
            var values = Enum.GetValues(typeof(T));
            var index = SplitMix32.Random(ref seed, 0, values.Length);
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
