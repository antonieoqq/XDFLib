using System;
using XDFLib.XRandom;

namespace XDFLib.Extensions
{
    public static class ArrayExtentions
    {
        public static void Shuffle<T>(this T[] values)
        {
            int n = values.Length;
            while (n > 1)
            {
                n--;
                int k = SplitMix32.Random(0, n + 1);
                T value = values[k];
                values[k] = values[n];
                values[n] = value;
            }
        }

        public static T GetRandomElement<T>(this T[] values)
        {
            if (values.Length > 0)
            {
                var currIndex = SplitMix32.Random(0, values.Length);
                return values[currIndex];
            }
            else
            {
                return default;
            }
        }

        public static void QuickSort<T>(this T[] array, Comparison<T> comparison)
        {
            QuickSort(array, 0, array.Length - 1, comparison);
        }

        public static void QuickSort<T>(this T[] array, int left, int right, Comparison<T> comparison)
        {
            if (left < right)
            {
                int pivotIndex = Partition(array, left, right, comparison);
                QuickSort(array, left, pivotIndex - 1, comparison);
                QuickSort(array, pivotIndex + 1, right, comparison);
            }
        }

        private static int Partition<T>(T[] array, int left, int right, Comparison<T> comparison)
        {
            T pivot = array[right];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                if (comparison(array[j], pivot) <= 0)
                {
                    i++;
                    Swap(array, i, j);
                }
            }

            Swap(array, i + 1, right);
            return i + 1;
        }

        private static void Swap<T>(T[] array, int i, int j)
        {
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
