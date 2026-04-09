using System;
using System.Collections.Generic;
using XDFLib.XRandom;

namespace XDFLib.Extensions
{
    public static class SpanExtensions
    {
        public static T GetRandomElement<T>(this Span<T> roSpan)
        {
            if (roSpan.Length == 0) { return default; }

            var currIndex = SplitMix32.Random(0, roSpan.Length);
            return roSpan[currIndex];
        }

        public static T GetRandomElement<T>(this ReadOnlySpan<T> roSpan)
        {
            if (roSpan.Length == 0) { return default; }

            var currIndex = SplitMix32.Random(0, roSpan.Length);
            return roSpan[currIndex];
        }

        public static T GetRandomElement<T>(this Span<T> roSpan, ref int seed)
        {
            if (roSpan.Length == 0) { return default; }

            var currIndex = SplitMix32.Random(ref seed, 0, roSpan.Length);
            return roSpan[currIndex];
        }


        public static T GetRandomElement<T>(this ReadOnlySpan<T> roSpan, ref int seed)
        {
            if (roSpan.Length == 0) { return default; }

            var currIndex = SplitMix32.Random(ref seed, 0, roSpan.Length);
            return roSpan[currIndex];
        }

        public static void Shuffle<T>(this Span<T> span)
        {
            int n = span.Length;
            while (n > 1)
            {
                n--;
                int k = SplitMix32.Random(0, n + 1);
                T value = span[k];
                span[k] = span[n];
                span[n] = value;
            }
        }

        public static void Shuffle<T>(this Span<T> span, ref int seed)
        {
            int n = span.Length;
            while (n > 1)
            {
                n--;
                int k = SplitMix32.Random(ref seed, 0, n + 1);
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

        #region Sort
        private const int InsertionSortThreshold = 16;
        private static Stack<(int left, int right)> _tempStack = new Stack<(int left, int right)>(64);

        public static void QuickSort<T>(this Span<T> span, Comparison<T> comparison)
        {
            if (span.Length < 2) return;

            //// 手动维护一个栈来存储待处理的区间范围
            //// 对于 2^31 大小的数组，栈深度最大约 31 (Log N)，所以预设容量 64 足够了
            //var stack = new Stack<(int left, int right)>(64);
            _tempStack.Clear();
            _tempStack.Push((0, span.Length - 1));

            while (_tempStack.Count > 0)
            {
                var (left, right) = _tempStack.Pop();

                // 优化 1：小区间使用插入排序，效率更高
                if (right - left + 1 < InsertionSortThreshold)
                {
                    InsertionSort(span.Slice(left, right - left + 1), comparison);
                    continue;
                }

                // 优化 2：执行分区（内部包含三数取中）
                int pivotIndex = Partition(span, left, right, comparison);

                // 优化 3：先处理较大的区间，将较小的区间推入栈中
                // 这能确保栈的深度最小（保持在 Log N 级别）
                int leftSize = pivotIndex - left;
                int rightSize = right - pivotIndex;

                if (leftSize > rightSize)
                {
                    if (leftSize > 1) _tempStack.Push((left, pivotIndex - 1));
                    if (rightSize > 1) _tempStack.Push((pivotIndex + 1, right));
                }
                else
                {
                    if (rightSize > 1) _tempStack.Push((pivotIndex + 1, right));
                    if (leftSize > 1) _tempStack.Push((left, pivotIndex - 1));
                }
            }
        }

        private static int Partition<T>(Span<T> span, int left, int right, Comparison<T> comparison)
        {
            // 优化 4：三数取中，避免最坏情况 O(n^2)
            int mid = left + (right - left) / 2;
            SortThree(span, left, mid, right, comparison);

            // 将 Pivot 放到 right - 1 的位置（因为 span[right] 已经比 pivot 大了）
            T pivot = span[mid];
            Swap(span, mid, right - 1);

            int i = left;
            int j = right - 1;

            // Hoare 分区方案：比 Lomuto 交换次数更少
            while (true)
            {
                while (comparison(span[++i], pivot) < 0) ;
                while (comparison(span[--j], pivot) > 0) ;

                if (i >= j) break;
                Swap(span, i, j);
            }

            // 还原基准值到正确位置
            Swap(span, i, right - 1);
            return i;
        }

        private static void InsertionSort<T>(Span<T> span, Comparison<T> comparison)
        {
            for (int i = 1; i < span.Length; i++)
            {
                T key = span[i];
                int j = i - 1;
                while (j >= 0 && comparison(span[j], key) > 0)
                {
                    span[j + 1] = span[j];
                    j--;
                }
                span[j + 1] = key;
            }
        }

        private static void SortThree<T>(Span<T> span, int a, int b, int c, Comparison<T> comparison)
        {
            if (comparison(span[a], span[b]) > 0) Swap(span, a, b);
            if (comparison(span[a], span[c]) > 0) Swap(span, a, c);
            if (comparison(span[b], span[c]) > 0) Swap(span, b, c);
        }

        private static void Swap<T>(Span<T> span, int i, int j)
        {
            T temp = span[i];
            span[i] = span[j];
            span[j] = temp;
        }
        #endregion
    }
}
