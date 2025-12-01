using System;

namespace XDFLib.XRandom
{
    public static class Triangle
    {
        #region float
        public static float Random(float mean, float variance)
        {
            float left = mean - variance;
            float right = mean + variance;
            return Random(left, right, mean);
        }

        public static float Random(int seed, float mean, float variance)
        {
            float left = mean - variance;
            float right = mean + variance;
            return Random(ref seed, left, right, mean);
        }

        public static float Random(ref int seed, float mean, float variance)
        {
            float left = mean - variance;
            float right = mean + variance;
            return Random(ref seed, left, right, mean);
        }

        public static float Random(float left, float right, float mean)
        {
            float x = SplitMix32.Random01();
            return Distribute(x, left, right, mean);
        }

        public static float Random(int seed, float left, float right, float mean)
        {
            float x = SplitMix32.Random01(ref seed);
            return Distribute(x, left, right, mean);
        }

        public static float Random(ref int seed, float left, float right, float mean)
        {
            float x = SplitMix32.Random01(ref seed);
            return Distribute(x, left, right, mean);
        }

        public static float Random(Random random, float mean, float variance)
        {
            float left = mean - variance;
            float right = mean + variance;
            return Random(random, left, right, mean);
        }

        public static float Random(Random random, float left, float right, float mean)
        {
            float x = (float)random.NextDouble();
            return Distribute(x, left, right, mean);
        }

        /// <summary>
        /// 把[0, 1)范围的任意数 x 随机到一个三角分布中
        /// </summary>
        /// <param name="x">[0, 1)</param>
        public static float Distribute(float x, float left, float right, float mean)
        {
            float F = (mean - left) / (right - left);
            if (x > 0 && x < (F))
            {
                return left + MathF.Sqrt(x * (right - left) * (mean - left));
            }
            else
            {
                return right - MathF.Sqrt((1 - x) * (right - left) * (right - mean));
            }
        }
        #endregion

        #region double
        public static double Random(double mean, double variance)
        {
            double left = mean - variance;
            double right = mean + variance;
            return Random(left, right, mean);
        }

        public static double Random(int seed, double mean, double variance)
        {
            double left = mean - variance;
            double right = mean + variance;
            return Random(seed, left, right, mean);
        }

        public static double Random(ref int seed, double mean, double variance)
        {
            double left = mean - variance;
            double right = mean + variance;
            return Random(ref seed, left, right, mean);
        }

        public static double Random(double left, double right, double mean)
        {
            double x = SplitMix32.Random01_Double();
            return Distribute(x, left, right, mean);
        }

        public static double Random(int seed, double left, double right, double mean)
        {
            double x = SplitMix32.Random01_Double(ref seed);
            return Distribute(x, left, right, mean);
        }

        public static double Random(ref int seed, double left, double right, double mean)
        {
            double x = SplitMix32.Random01_Double(ref seed);
            return Distribute(x, left, right, mean);
        }

        public static double Random(Random random, double mean, double variance)
        {
            double left = mean - variance;
            double right = mean + variance;
            return Random(random, left, right, mean);
        }

        public static double Random(Random random, double left, double right, double mean)
        {
            double x = random.NextDouble();
            return Distribute(x, left, right, mean);
        }

        /// <summary>
        /// 把[0, 1)范围的任意数 x 随机到一个三角分布中
        /// </summary>
        /// <param name="x">[0, 1)</param>
        public static double Distribute(double x, double l, double r, double m)
        {
            double F = (m - l) / (r - l);
            if (x > 0 && x < (F))
            {
                return l + Math.Sqrt(x * (r - l) * (m - l));
            }
            else
            {
                return r - Math.Sqrt((1 - x) * (r - l) * (r - m));
            }
        }
        #endregion
    }
}
