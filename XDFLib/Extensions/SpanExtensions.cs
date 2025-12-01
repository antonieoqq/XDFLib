using System;
using System.Collections.Generic;
using XDFLib.XRandom;

namespace XDFLib.Extensions
{
    public static class SpanExtensions
    {
        public static T GetRandomElement<T>(this ReadOnlySpan<T> roSpan)
        {
            if (roSpan.Length == 0) { return default; }

            var currIndex = SplitMix32.Random(0, roSpan.Length);
            return roSpan[currIndex];
        }

        public static void Shuffle<T>(this Span<T> span)
        {
            int n = span.Length;
            while (n > 1)
            {
                n--;
                int k = SplitMix32.Random(n + 1);
                T value = span[k];
                span[k] = span[n];
                span[n] = value;
            }
        }

        public static int Count<T>(this Span<T> span, Predicate<T> predicate)
        {
            int count = 0;
            for (int i = 0; i < span.Length; i++)
            {
                if (predicate(span[i]))
                {
                    count++;
                }
            }
            return count;
        }

        public static void FilterTo<T>(this Span<T> span, ref ICollection<T> filterTo, Predicate<T> predicate)
        {
            for (int i = 0; i < span.Length; i++)
            {
                var v = span[i];
                if (predicate(v))
                {
                    filterTo.Add(v);
                }
            }
        }

        public static void QuickSort<T>(this Span<T> span, Comparison<T> comparison)
        {
            QuickSort(span, 0, span.Length - 1, comparison);
        }

        public static void QuickSort<T>(this Span<T> span, int left, int right, Comparison<T> comparison)
        {
            if (left < right)
            {
                int pivotIndex = Partition(span, left, right, comparison);
                QuickSort(span, left, pivotIndex - 1, comparison);
                QuickSort(span, pivotIndex + 1, right, comparison);
            }
        }

        private static int Partition<T>(Span<T> span, int left, int right, Comparison<T> comparison)
        {
            T pivot = span[right];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                if (comparison(span[j], pivot) <= 0)
                {
                    i++;
                    Swap(span, i, j);
                }
            }

            Swap(span, i + 1, right);
            return i + 1;
        }

        private static void Swap<T>(Span<T> span, int i, int j)
        {
            T temp = span[i];
            span[i] = span[j];
            span[j] = temp;
        }
    }
}
