using System;
using System.Runtime.CompilerServices;

namespace XDFLib
{

    public static class XMath
    {
        public const float Sqrt2 = 1.41421356237f;

        public static float QuickPow(float x, int exp)
        {
            if (exp == 0) return 1.0f;
            if (exp < 0)
            {
                x = 1 / x;
                exp = -exp;
            }
            float result = 1;
            float currentProduct = x;
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                {
                    result *= currentProduct;
                }
                currentProduct *= currentProduct;
                exp >>= 1;
            }
            return result;
        }

        public static double QuickPow(double x, int exp)
        {
            if (exp == 0) return 1.0;
            if (exp < 0)
            {
                x = 1 / x;
                exp = -exp;
            }
            double result = 1;
            double currentProduct = x;
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                {
                    result *= currentProduct;
                }
                currentProduct *= currentProduct;
                exp >>= 1;
            }
            return result;
        }

        public static float Lerp(float from, float to, float t)
        {
            if (from == to) return from;

            float step = to - from;
            float move = step * t;
            float result = from + move;
            return result;
        }

        public static double Lerp(double from, double to, double t)
        {
            if (from == to) return from;

            double step = to - from;
            double move = step * t;
            double result = from + move;
            return result;
        }

        public static float Lerp(float from, float to, float t, bool clamp)
        {
            if (clamp)
            {
                if (t <= 0)
                    return from;
                else if (t >= 1)
                    return to;
            }
            return Lerp(from, to, t);
        }

        public static double Lerp(double from, double to, double t, bool clamp)
        {
            if (clamp)
            {
                if (t <= 0)
                    return from;
                else if (t >= 1)
                    return to;
            }
            return Lerp(from, to, t);
        }

        public static float LerpByAmount(float from, float to, float amount, bool clamp)
        {
            if (from == to) return from;

            return from < to ?
                (clamp ? MathF.Min(from + amount, to) : from + amount) :
                (clamp ? MathF.Max(from - amount, to) : from - amount);
        }

        public static double LerpByAmount(double from, double to, double amount, bool clamp)
        {
            if (from == to) return from;

            return from < to ?
                (clamp ? Math.Min(from + amount, to) : from + amount) :
                (clamp ? Math.Max(from - amount, to) : from - amount);
        }

        public static float Rescale(float value, float start, float end, float newStart, float newEnd, bool isClamp = false)
        {
            if (start == end || value == start)
                return newStart;
            else if (value == end)
                return newEnd;
            else
            {
                float result = newStart + (((value - start) / (end - start)) * (newEnd - newStart));
                if (isClamp)
                    if (newStart < newEnd)
                    {
                        result = Clamp(result, newStart, newEnd);
                    }
                    else
                    {
                        result = Clamp(result, newEnd, newStart);
                    }
                return result;
            }
        }

        public static double Rescale(double value, double start, double end, double newStart, double newEnd, bool isClamp = false)
        {
            if (start == end || value == start)
                return newStart;
            else if (value == end)
                return newEnd;
            else
            {
                double result = newStart + (((value - start) / (end - start)) * (newEnd - newStart));
                if (isClamp)
                    if (newStart < newEnd)
                    {
                        result = Clamp(result, newStart, newEnd);
                    }
                    else
                    {
                        result = Clamp(result, newEnd, newStart);
                    }
                return result;
            }
        }

        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>[min, max]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int v, int min, int max)
        {
            return (min < max) ?
                ((v < min) ? min : (v > max) ? max : v) :
                ((v > min) ? min : (v < max) ? max : v);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>[min, max]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float v, float min, float max)
        {
            return (min < max) ?
                ((v < min) ? min : (v > max) ? max : v) :
                ((v > min) ? min : (v < max) ? max : v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double v, double min, double max)
        {
            return (min < max) ?
                ((v < min) ? min : (v > max) ? max : v) :
                ((v > min) ? min : (v < max) ? max : v);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>[0, 1]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float v)
        {
            return v < 0 ? 0 : v > 1 ? 1 : v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>[0, 1]</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp01(double v)
        {
            return v < 0 ? 0 : v > 1 ? 1 : v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Loop(int v, int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive.");

            var m = v % count;
            var r = m >= 0 ? m : m + count;
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Loop(float v, float count)
        {
            var m = v % count;
            var r = m >= 0 ? m : m + count;
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Loop(double v, double count)
        {
            var m = v % count;
            var r = m >= 0 ? m : m + count;
            return r;
        }

        /// <summary>
        /// 不包含end [ )
        /// </summary>
        /// <param name="v"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Loop(int v, int start, int end)
        {
            var length = end - start;
            if (length == 0) return start; // 防止除以 0

            // 使用数学公式：Result = start + ((v - start) % length + length) % length
            // 这个公式可以同时处理 v < start 或 v > end 的情况，且不依赖 start 和 end 的大小关系
            return start + ((v - start) % length + length) % length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Loop(float v, float start, float end)
        {
            var length = end - start;
            if (length == 0) return start; // 防止除以 0

            // 使用数学公式：Result = start + ((v - start) % length + length) % length
            // 这个公式可以同时处理 v < start 或 v > end 的情况，且不依赖 start 和 end 的大小关系
            return start + ((v - start) % length + length) % length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Loop(double v, double start, double end)
        {
            var length = end - start;
            if (length == 0) return start; // 防止除以 0

            // 使用数学公式：Result = start + ((v - start) % length + length) % length
            // 这个公式可以同时处理 v < start 或 v > end 的情况，且不依赖 start 和 end 的大小关系
            return start + ((v - start) % length + length) % length;
        }

        public static bool Compare(int source, int compareTo, ECompMode compMode)
        {
            switch (compMode)
            {
                case ECompMode.Less: return source < compareTo;
                case ECompMode.LessOrEqual: return source <= compareTo;
                case ECompMode.Equal: return source == compareTo;
                case ECompMode.BiggerOrEqual: return source >= compareTo;
                case ECompMode.Bigger: return source > compareTo;
                case ECompMode.NotEqual: return source != compareTo;
                default: return false;
            }
        }

        public static bool Compare(float source, float compareTo, ECompMode compMode)
        {
            switch (compMode)
            {
                case ECompMode.Less: return source < compareTo;
                case ECompMode.LessOrEqual: return source <= compareTo;
                case ECompMode.Equal: return source == compareTo;
                case ECompMode.BiggerOrEqual: return source >= compareTo;
                case ECompMode.Bigger: return source > compareTo;
                case ECompMode.NotEqual: return source != compareTo;
                default: return false;
            }
        }

        public static bool Compare(double source, double compareTo, ECompMode compMode)
        {
            switch (compMode)
            {
                case ECompMode.Less: return source < compareTo;
                case ECompMode.LessOrEqual: return source <= compareTo;
                case ECompMode.Equal: return source == compareTo;
                case ECompMode.BiggerOrEqual: return source >= compareTo;
                case ECompMode.Bigger: return source > compareTo;
                case ECompMode.NotEqual: return source != compareTo;
                default: return false;
            }
        }

        internal static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }
    }
}
