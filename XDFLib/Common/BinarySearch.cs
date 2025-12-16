using System;
using System.Collections.Generic;

namespace XDFLib
{
    public static class BinarySearch
    {
        public static int IndexOfMinGreaterThan<T>(ReadOnlySpan<T> array, T value, bool ascending = true)
            where T : IComparable<T>
        {
            int low = 0;
            int high = array.Length - 1;
            int resultIndex = -1;
            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                var compare = array[mid].CompareTo(value);
                var precede = ascending ? compare >= 0 : compare <= 0;
                if (precede)
                {
                    // 可能是候选解，继续往左边找有没有更小的符合条件的
                    resultIndex = mid;
                    high = mid - 1;
                }
                else
                {
                    // 不满足条件，往右边缩小范围
                    low = mid + 1;
                }
            }
            return resultIndex;
        }

        public static int IndexOfMinGreaterThan<T>(IList<T> array, T value, bool ascending = true)
            where T : IComparable<T>
        {
            int low = 0;
            int high = array.Count - 1;
            int resultIndex = -1;
            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                var compare = array[mid].CompareTo(value);
                var precede = ascending ? compare >= 0 : compare <= 0;
                if (precede)
                {
                    // 可能是候选解，继续往左边找有没有更小的符合条件的
                    resultIndex = mid;
                    high = mid - 1;
                }
                else
                {
                    // 不满足条件，往右边缩小范围
                    low = mid + 1;
                }
            }
            return resultIndex;
        }

        public static int IndexOfMaxLessThan<T>(ReadOnlySpan<T> array, T value, bool ascending = true)
            where T : IComparable<T>
        {
            int low = 0;
            int high = array.Length - 1;
            int resultIndex = -1;

            while (low <= high)
            {
                int mid = low + (high - low) / 2;

                var compare = array[mid].CompareTo(value);
                var precede = ascending ? compare <= 0 : compare >= 0;
                if (precede)
                {
                    // 可能是候选解，继续往右边找有没有更大的符合条件的
                    resultIndex = mid;
                    low = mid + 1;
                }
                else
                {
                    // 不满足条件，往左边缩小范围
                    high = mid - 1;
                }
            }

            return resultIndex;
        }

        public static int IndexOfMaxLessThan<T>(IList<T> array, T value, bool ascending = true)
            where T : IComparable<T>
        {
            int low = 0;
            int high = array.Count - 1;
            int resultIndex = -1;

            while (low <= high)
            {
                int mid = low + (high - low) / 2;

                var compare = array[mid].CompareTo(value);
                var precede = ascending ? compare <= 0 : compare >= 0;
                if (precede)
                {
                    // 可能是候选解，继续往右边找有没有更大的符合条件的
                    resultIndex = mid;
                    low = mid + 1;
                }
                else
                {
                    // 不满足条件，往左边缩小范围
                    high = mid - 1;
                }
            }

            return resultIndex;
        }


        public static int IndexOfClosest(ReadOnlySpan<int> array, int x)
        {
            if (array == null || array.Length == 0) return -1;

            int low = 0;
            int high = array.Length - 1;

            // 用二分查找找到插入点
            while (low < high)
            {
                int mid = low + (high - low) / 2;

                if (array[mid] < x)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }

            // 此时 low 是第一个 >= x 的位置
            int closestIndex = low;

            // 如果不是第一个元素，比较前一个元素是否更接近
            if (closestIndex > 0 && Math.Abs(array[closestIndex - 1] - x) <= Math.Abs(array[closestIndex] - x))
            {
                closestIndex--;
            }

            return closestIndex;
        }

        public static int IndexOfClosest(IList<int> array, int x)
        {
            if (array == null || array.Count == 0) return -1;

            int low = 0;
            int high = array.Count - 1;

            // 用二分查找找到插入点
            while (low < high)
            {
                int mid = low + (high - low) / 2;

                if (array[mid] < x)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }

            // 此时 low 是第一个 >= x 的位置
            int closestIndex = low;

            // 如果不是第一个元素，比较前一个元素是否更接近
            if (closestIndex > 0 && Math.Abs(array[closestIndex - 1] - x) <= Math.Abs(array[closestIndex] - x))
            {
                closestIndex--;
            }

            return closestIndex;
        }


        public static int IndexOfClosest(ReadOnlySpan<float> array, float x)
        {
            if (array == null || array.Length == 0) return -1;

            int low = 0;
            int high = array.Length - 1;

            // 用二分查找找到插入点
            while (low < high)
            {
                int mid = low + (high - low) / 2;

                if (array[mid] < x)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }

            // 此时 low 是第一个 >= x 的位置
            int closestIndex = low;

            // 如果不是第一个元素，比较前一个元素是否更接近
            if (closestIndex > 0 && Math.Abs(array[closestIndex - 1] - x) <= Math.Abs(array[closestIndex] - x))
            {
                closestIndex--;
            }

            return closestIndex;
        }

        public static int IndexOfClosest(IList<float> array, float x)
        {
            if (array == null || array.Count == 0) return -1;

            int low = 0;
            int high = array.Count - 1;

            // 用二分查找找到插入点
            while (low < high)
            {
                int mid = low + (high - low) / 2;

                if (array[mid] < x)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }

            // 此时 low 是第一个 >= x 的位置
            int closestIndex = low;

            // 如果不是第一个元素，比较前一个元素是否更接近
            if (closestIndex > 0 && Math.Abs(array[closestIndex - 1] - x) <= Math.Abs(array[closestIndex] - x))
            {
                closestIndex--;
            }

            return closestIndex;
        }

        public static (int? leftIndex, int? rightIndex, float lerpT) Neighbours(ReadOnlySpan<int> array, int value)
        {
            int left = 0;
            int right = array.Length - 1;

            if (value <= array[left]) { return (null, left, 1); }
            else if (value >= array[right]) { return (right, null, 0); }

            int mid;
            while (left <= right)
            {
                if (left == right)
                {
                    var midV = array[left];
                    if (value >= midV)
                    {
                        return (left, left + 1, 0);
                    }
                    else
                    {
                        return (left - 1, left, 1);
                    }
                }
                else if (left + 1 == right)
                {
                    var leftV = array[left];
                    var rightV = array[right];
                    var t = (value - leftV) / (float)(rightV - leftV);
                    return (left, right, t);
                }

                mid = (left + right) / 2;
                var midValue = array[mid];
                if (value == midValue)
                {
                    return (mid, mid + 1, 0);
                }
                else if (value > midValue)
                {
                    left = mid;
                }
                else if (value < midValue)
                {
                    right = mid;
                }
            }
            return (null, null, 0);
        }

        public static (int? leftIndex, int? rightIndex, float lerpT) Neighbours(IList<int> array, int value)
        {
            int left = 0;
            int right = array.Count - 1;

            if (value <= array[left]) { return (null, left, 1); }
            else if (value >= array[right]) { return (right, null, 0); }

            int mid;
            while (left <= right)
            {
                if (left == right)
                {
                    var midV = array[left];
                    if (value >= midV)
                    {
                        return (left, left + 1, 0);
                    }
                    else
                    {
                        return (left - 1, left, 1);
                    }
                }
                else if (left + 1 == right)
                {
                    var leftV = array[left];
                    var rightV = array[right];
                    var t = (value - leftV) / (float)(rightV - leftV);
                    return (left, right, t);
                }

                mid = (left + right) / 2;
                var midValue = array[mid];
                if (value == midValue)
                {
                    return (mid, mid + 1, 0);
                }
                else if (value > midValue)
                {
                    left = mid;
                }
                else if (value < midValue)
                {
                    right = mid;
                }
            }
            return (null, null, 0);
        }


        public static (int? leftIndex, int? rightIndex, float lerpT) Neighbours(ReadOnlySpan<float> array, float value)
        {
            int left = 0;
            int right = array.Length - 1;

            if (value <= array[left]) { return (null, left, 1); }
            else if (value >= array[right]) { return (right, null, 0); }

            int mid;
            while (left <= right)
            {
                if (left == right)
                {
                    var midV = array[left];
                    if (value >= midV)
                    {
                        return (left, left + 1, 0);
                    }
                    else
                    {
                        return (left - 1, left, 1);
                    }
                }
                else if (left + 1 == right)
                {
                    var leftV = array[left];
                    var rightV = array[right];
                    var t = (value - leftV) / (rightV - leftV);
                    return (left, right, t);
                }

                mid = (left + right) / 2;
                var midValue = array[mid];
                if (value == midValue)
                {
                    return (mid, mid + 1, 0);
                }
                else if (value > midValue)
                {
                    left = mid;
                }
                else if (value < midValue)
                {
                    right = mid;
                }
            }
            return (null, null, 0);
        }

        public static (int? leftIndex, int? rightIndex, float lerpT) Neighbours(IList<float> array, float value)
        {
            int left = 0;
            int right = array.Count - 1;

            if (value <= array[left]) { return (null, left, 1); }
            else if (value >= array[right]) { return (right, null, 0); }

            int mid;
            while (left <= right)
            {
                if (left == right)
                {
                    var midV = array[left];
                    if (value >= midV)
                    {
                        return (left, left + 1, 0);
                    }
                    else
                    {
                        return (left - 1, left, 1);
                    }
                }
                else if (left + 1 == right)
                {
                    var leftV = array[left];
                    var rightV = array[right];
                    var t = (value - leftV) / (rightV - leftV);
                    return (left, right, t);
                }

                mid = (left + right) / 2;
                var midValue = array[mid];
                if (value == midValue)
                {
                    return (mid, mid + 1, 0);
                }
                else if (value > midValue)
                {
                    left = mid;
                }
                else if (value < midValue)
                {
                    right = mid;
                }
            }
            return (null, null, 0);
        }

    }
}
