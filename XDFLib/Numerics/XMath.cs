using System;

namespace XDFLib
{

    public static partial class XMath
    {
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
        public static float Clamp(float v, float min, float max)
        {
            return (min < max) ?
                ((v < min) ? min : (v > max) ? max : v) :
                ((v > min) ? min : (v < max) ? max : v);
        }

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
        public static float Clamp01(float v)
        {
            return v < 0 ? 0 : v > 1 ? 1 : v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>[0, 1]</returns>
        public static double Clamp01(double v)
        {
            return v < 0 ? 0 : v > 1 ? 1 : v;
        }

        public static int Loop(int v, int count)
        {
            var m = v % count;
            var r = m >= 0 ? m : m + count;
            return r;
        }

        public static float Loop(float v, float count)
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
        public static int LoopOld(int v, int start, int end)
        {
            if (start == end) return start;

            if (start > end)
                Utilities.Swap(ref start, ref end);

            int distance = v - start;
            int repeatLength = end - start;
            int firstMod = distance % repeatLength;
            return firstMod + start + (firstMod < 0 ? repeatLength : 0);
        }

        public static float LoopOld(float v, float start, float end)
        {
            if (start == end) return start;

            if (start > end)
                Utilities.Swap(ref start, ref end);

            float distance = v - start;
            float repeatLength = end - start;
            float firstMod = distance % repeatLength;
            return firstMod + start + (firstMod < 0 ? repeatLength : 0);
        }

        /// <summary>
        /// 不包含end [ )
        /// </summary>
        /// <param name="v"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int Loop(int v, int start, int end)
        {
            if (start == end) return start;

            if (start < end)
            {
                var len = end - start;
                var loop = Loop(v - start, len);
                return loop + start;
            }
            else
            {
                var len = start - end;
                var loop = Loop(v - end, len);
                return loop + end;
            }
        }

        public static float Loop(float v, float start, float end)
        {
            if (start == end) return start;

            if (start < end)
            {
                var len = end - start;
                var loop = Loop(v - start, len);
                return loop + start;
            }
            else
            {
                var len = start - end;
                var loop = Loop(v - end, len);
                return loop + end;
            }
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
