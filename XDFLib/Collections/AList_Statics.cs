using System;

namespace XDFLib.Collections
{
    public static class AList
    {
        public static AList<T> CreateFrom<T>(ReadOnlySpan<T> array, bool filterNull = true)
        {
            var result = new AList<T>(array.Length);
            foreach (var item in array)
            {
                if (filterNull && item == null) continue;
                result.Add(item);
            }
            return result;
        }
    }
}
