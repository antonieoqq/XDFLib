using System;
using System.Runtime.CompilerServices;

namespace XDFLib
{

    public static partial class XMath
    {
        #region Basic Random
        /// <summary> 经实测，当一个Random对象在生成了大约8000个随机数后，会失去随机性，可能这和伪随机的算法有关 </summary>
        public static int MaxGenerationForEachRandom = 8000;
        /// <summary> 这个0.25请参考GaussianDistributeRandom的函数说明 </summary>
        public const float GaussianDistributeScale = 0.25f;

        static Random _rnd;
        static int _randNumGenerated = 0;
        public static Random Rnd
        {
            get
            {
                _randNumGenerated++;
                if (_rnd == null || _randNumGenerated > MaxGenerationForEachRandom)
                {
                    _rnd = new Random(Guid.NewGuid().GetHashCode());
                    _randNumGenerated = 0;
                }
                return _rnd;
            }
        }

        /// <summary>
        /// 不包含max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomRange(int min, int max)
        {
            return Rnd.Next(min, max);
        }

        /// <summary>
        /// 不包含max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float RandomRange(float min, float max)
        {
            float r = 1f - (float)Rnd.NextDouble();
            float range = max - min;
            float result = min + r * range;

            return result;
        }

        /// <summary>
        /// 不包含max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double RandomRange(double min, double max)
        {
            double r = 1.0 - Rnd.NextDouble();
            double range = max - min;
            double result = min + r * range;
            return result;
        }

        /// <summary>
        /// 不包含max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Random01()
        {
            return (float)Rnd.NextDouble();
        }

        #endregion

        #region linear congruential generator
        private const int LCGmod = 2147483647; // 2^31 - 1
        private const int LCGa = 48271;

        static int _LCGRandomSeed = Guid.NewGuid().GetHashCode();

        /// <summary>
        /// 基于LCG（linear congruential generator）线性同余算法的随机数实现
        /// 公式：X(n+1) = (a * X（n） + c) mod m;
        /// 常量采用的是C++11标准库的版本：其中 mod = 2147483647, a = 48271, c = 0
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>Random in [-2147483648, 2147483647)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LCGRandom(int seed)
        {
            //int mod = 2147483647; // 2^31 - 1
            seed = (LCGa * seed) % LCGmod;
            return seed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SeedTo01(int seed)
        {
            return MathF.Abs(seed * (1.0f / 2147483647.0f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SeedTo01_Double(int seed)
        {
            var result = seed * (1.0 / 2147483647.0);
            return result < 0 ? -result : result;
        }

        [Obsolete("Use LCGRandom01(ref int seed) instead.")]
        /// <returns>Random in [0, 1)</returns>
        public static float LCGRandom01(int seed)
        {
            seed = LCGRandom(seed);
            return SeedTo01(seed);
        }

        public static float LCGRandom01(ref int seed)
        {
            seed = LCGRandom(seed);
            return SeedTo01(seed);
        }


        [Obsolete("Use LCGRandom01_Double(ref int seed) instead.")]
        public static double LCGRandom01_Double(int seed)
        {
            seed = LCGRandom(seed);
            return SeedTo01_Double(seed);
        }

        public static double LCGRandom01_Double(ref int seed)
        {
            seed = LCGRandom(seed);
            return SeedTo01_Double(seed);
        }

        public static int SeedToRange(int seed, int min, int max)
        {
            int len = max - min;
            int result = min + (int)(len * SeedTo01(seed));
            return result;
        }

        [Obsolete("Use LCGRandom(ref int seed, int min, int max) instead.")]
        /// <returns>Random in [min, max)</returns>
        public static int LCGRandom(int seed, int min, int max)
        {
            if (min == max) { return min; }
            else
            {
                seed = LCGRandom(seed);
                return SeedToRange(seed, min, max);
            }
        }

        public static int LCGRandom(ref int seed, int min, int max)
        {
            if (min == max) { return min; }
            else
            {
                seed = LCGRandom(seed);
                return SeedToRange(seed, min, max);
            }
        }

        public static float SeedToRange(int seed, float min, float max)
        {
            float len = max - min;
            float result = min + len * SeedTo01(seed);
            return result;

        }

        [Obsolete("Use LCGRandom(ref int seed, float min, float max) instead.")]
        /// <returns>Random in [min, max)</returns>
        public static float LCGRandom(int seed, float min, float max)
        {
            if (min == max) { return min; }
            else
            {
                seed = LCGRandom(seed);
                return SeedToRange(seed, min, max);
            }
        }

        public static float LCGRandom(ref int seed, float min, float max)
        {
            if (min == max) { return min; }
            else
            {
                seed = LCGRandom(seed);
                return SeedToRange(seed, min, max);
            }
        }

        public static double SeedToRange(int seed, double min, double max)
        {
            double len = max - min;
            double result = min + len * SeedTo01_Double(seed);
            return result;
        }

        [Obsolete("Use LCGRandom(ref int seed, double min, double max) instead.")]
        public static double LCGRandom(int seed, double min, double max)
        {
            if (min == max) { return min; }
            else
            {
                seed = LCGRandom(seed);
                return SeedToRange(seed, min, max);
            }
        }

        public static double LCGRandom(ref int seed, double min, double max)
        {
            if (min == max) { return min; }
            else
            {
                seed = LCGRandom(seed);
                return SeedToRange(seed, min, max);
            }
        }

        public static int LCGRandom()
        {
            _LCGRandomSeed = LCGRandom(_LCGRandomSeed++);
            return _LCGRandomSeed;
        }

        public static float LCGRandom01()
        {
            _LCGRandomSeed = LCGRandom(_LCGRandomSeed++);
            return SeedTo01(_LCGRandomSeed);
        }

        public static double LCGRandom01_Double()
        {
            _LCGRandomSeed = LCGRandom(_LCGRandomSeed++);
            return SeedTo01_Double(_LCGRandomSeed);
        }

        public static int LCGRandom(int min, int max)
        {
            _LCGRandomSeed = LCGRandom(_LCGRandomSeed++);
            return SeedToRange(_LCGRandomSeed, min, max);
        }

        public static float LCGRandom(float min, float max)
        {
            _LCGRandomSeed = LCGRandom(_LCGRandomSeed++);
            return SeedToRange(_LCGRandomSeed, min, max);
        }

        public static double LCGRandom(double min, double max)
        {
            _LCGRandomSeed = LCGRandom(_LCGRandomSeed++);
            return SeedToRange(_LCGRandomSeed, min, max);
        }
        #endregion

        #region gaussian random
        /// <summary>
        /// 通过任意数 u1，u2 随机到一个高斯分布中
        /// </summary>
        /// <param name="u1">[0, 1)</param>
        /// <param name="u2">[0, 1)</param>
        public static float GaussianDistribute(float u1, float u2, float mean, float variance)
        {
            float dev = variance * GaussianDistributeScale;
            return GaussianDeviationDistribute(u1, u2, mean, dev);
        }

        /// <summary>
        /// 基于内部的LCG随机数生成器生成高斯分布的随机数
        /// </summary>
        /// <param name="mean">数学期望</param>
        /// <param name="variance">散布半径</param>
        /// <param name="forceResultInRange">有大约0.005%~0.007%的概率出现散布在范围外的结果，设置此参数为true可以保证结果在范围内</param>
        /// <param name="maxTriesWhenOutOfRange">如果超出范围，会尝试重新随机，为防止意外情况下无限尝试导致死循环，这个参数用于限制尝试次数</param>
        /// <returns></returns>
        public static float GaussianRandom(float mean, float variance, bool forceResultInRange = true, int maxTriesWhenOutOfRange = 4)
        {
            float dev = variance * GaussianDistributeScale;
            float result = GaussianDeviationRandom(mean, dev);
            if (forceResultInRange)
            {
                int triedTimes = 0;
                float min = mean - variance;
                float max = mean + variance;
                while (triedTimes < maxTriesWhenOutOfRange && (result < min || result > max))
                {
                    triedTimes++;
                    result = GaussianDeviationRandom(mean, dev);
                }
            }
            return result;
        }

        private static float GaussianDeviationRandom(float mean, float deviation)
        {
            if (deviation <= 0) { return mean; }
            float u1 = LCGRandom01();
            float u2 = LCGRandom01();
            return GaussianDeviationDistribute(u1, u2, mean, deviation);
        }

        /// <summary>
        /// 基于传入的Seed，使用内部LCG随机数生成器生成高斯分布的随机数
        /// </summary>
        /// <param name="seed">种子</param>
        /// <param name="mean">数学期望</param>
        /// <param name="variance">散布半径</param>
        /// <param name="forceResultInRange">有大约0.005%~0.007%的概率出现散布在范围外的结果，设置此参数为true可以保证结果在范围内</param>
        /// <param name="maxTriesWhenOutOfRange">如果超出范围，会尝试重新随机，为防止意外情况下无限尝试导致死循环，这个参数用于限制尝试次数</param>
        /// <returns></returns>
        public static float GaussianRandom(int seed, float mean, float variance, bool forceResultInRange = true, int maxTriesWhenOutOfRange = 4)
        {
            float dev = variance * GaussianDistributeScale;
            float result = GaussianDeviationRandom(seed, mean, dev);
            if (forceResultInRange)
            {
                int triedTimes = 0;
                float min = mean - variance;
                float max = mean + variance;
                while (triedTimes < maxTriesWhenOutOfRange && (result < min || result > max))
                {
                    triedTimes++;
                    result = GaussianDeviationRandom(seed, mean, dev);
                }
            }
            return result;
        }

        private static float GaussianDeviationRandom(int seed, float mean, float deviation)
        {
            if (deviation <= 0)
            {
                return mean;
            }
            float u1 = LCGRandom01(seed);
            float u2 = LCGRandom01(seed);
            return GaussianDeviationDistribute(u1, u2, mean, deviation);
        }

        /// <summary>
        /// 基于传入的随机数生成器生成高斯分布的随机数
        /// </summary>
        /// <param name="random">Random对象</param>
        /// <param name="mean">数学期望</param>
        /// <param name="variance">散布半径</param>
        /// <param name="forceResultInRange">有大约0.005%~0.007%的概率出现散布在范围外的结果，设置此参数为true可以保证结果在范围内</param>
        /// <param name="maxTriesWhenOutOfRange">如果超出范围，会尝试重新随机，为防止意外情况下无限尝试导致死循环，这个参数用于限制尝试次数</param>
        /// <returns></returns>
        public static float GaussianRandom(Random random, float mean, float variance, bool forceResultInRange = true, int maxTriesWhenOutOfRange = 4)
        {
            float dev = variance * GaussianDistributeScale;
            float result = GaussianDeviationRandom(random, mean, dev);
            if (forceResultInRange)
            {
                int triedTimes = 0;
                float min = mean - variance;
                float max = mean + variance;
                while (triedTimes < maxTriesWhenOutOfRange && (result < min || result > max))
                {
                    triedTimes++;
                    result = GaussianDeviationRandom(random, mean, dev);
                }
            }
            return result;
        }

        /// <summary>
        /// 此函数采用的近似算法存在一些误差
        /// 标准函数的结果分布一般在数学期望左右两侧3倍sigma范围内
        /// 此函数的分布结果则大约为两侧的4倍deviation范围，并且有大约0.005%~0.007%的概率出现4倍deviation范围外的结果
        /// 所以如果按照mean和variance设定值域，deviation 应该为 variance * 0.25
        /// 建议直接用GaussianRandom方法
        /// </summary>
        private static float GaussianDeviationRandom(Random random, float mean, float deviation)
        {
            if (deviation <= 0) { return mean; }
            float u1 = (float)random.NextDouble();
            float u2 = (float)random.NextDouble();
            return GaussianDeviationDistribute(u1, u2, mean, deviation);
        }

        /// <summary>
        /// 高斯偏离分布，散布范围在mean两侧各4倍deviation的范围内
        /// </summary>
        /// <param name="u1">[0, 1)</param>
        /// <param name="u2">[0, 1)</param>
        private static float GaussianDeviationDistribute(float u1, float u2, float mean, float deviation)
        {
            float randStdNormal = MathF.Sqrt(-2.0f * MathF.Log(u1)) *
                MathF.Sin(2.0f * MathF.PI * u2); //random normal(0,1)
            float randNormal =
                         mean + deviation * randStdNormal;
            return randNormal;
        }

        /// <summary>
        /// 通过任意数 u1，u2 随机到一个高斯分布中
        /// </summary>
        /// <param name="u1">[0, 1)</param>
        /// <param name="u2">[0, 1)</param>
        public static double GaussianDistribute(double u1, double u2, double mean, double variance)
        {
            double dev = variance * GaussianDistributeScale;
            return GaussianDeviationDistribute(u1, u2, mean, dev);
        }

        /// <summary>
        /// 基于内部的LCG随机数生成器生成高斯分布的随机数
        /// </summary>
        /// <param name="mean">数学期望</param>
        /// <param name="variance">散布半径</param>
        /// <param name="forceResultInRange">有大约0.005%~0.007%的概率出现散布在范围外的结果，设置此参数为true可以保证结果在范围内</param>
        /// <param name="maxTriesWhenOutOfRange">如果超出范围，会尝试重新随机，为防止意外情况下无限尝试导致死循环，这个参数用于限制尝试次数</param>
        /// <returns></returns>
        public static double GaussianRandom(double mean, double variance, bool forceResultInRange = true, int maxTriesWhenOutOfRange = 4)
        {
            double dev = variance * GaussianDistributeScale;
            double result = GaussianDeviationRandom(mean, dev);
            if (forceResultInRange)
            {
                int triedTimes = 0;
                double min = mean - variance;
                double max = mean + variance;
                while (triedTimes < maxTriesWhenOutOfRange && (result < min || result > max))
                {
                    triedTimes++;
                    result = GaussianDeviationRandom(mean, dev);
                }
            }
            return result;
        }

        /// <summary>
        /// 基于传入的Seed，使用内部LCG随机数生成器生成高斯分布的随机数
        /// </summary>
        /// <param name="seed">种子</param>
        /// <param name="mean">数学期望</param>
        /// <param name="variance">散布半径</param>
        /// <param name="forceResultInRange">有大约0.005%~0.007%的概率出现散布在范围外的结果，设置此参数为true可以保证结果在范围内</param>
        /// <param name="maxTriesWhenOutOfRange">如果超出范围，会尝试重新随机，为防止意外情况下无限尝试导致死循环，这个参数用于限制尝试次数</param>
        /// <returns></returns>
        public static double GaussianRandom(int seed, double mean, double variance, bool forceResultInRange = true, int maxTriesWhenOutOfRange = 4)
        {
            double dev = variance * GaussianDistributeScale;
            double result = GaussianDeviationRandom(seed, mean, dev);
            if (forceResultInRange)
            {
                int triedTimes = 0;
                double min = mean - variance;
                double max = mean + variance;
                while (triedTimes < maxTriesWhenOutOfRange && (result < min || result > max))
                {
                    triedTimes++;
                    result = GaussianDeviationRandom(seed, mean, dev);
                }
            }
            return result;
        }

        private static double GaussianDeviationRandom(int seed, double mean, double deviation)
        {
            if (deviation <= 0)
            {
                return mean;
            }
            double u1 = LCGRandom01_Double(seed);
            double u2 = LCGRandom01_Double(seed);
            return GaussianDeviationDistribute(u1, u2, mean, deviation);
        }


        /// <summary>
        /// 此函数采用的近似算法存在一些误差
        /// 标准函数的结果分布一般在数学期望左右两侧3倍sigma范围内
        /// 此函数的分布结果则大约为两侧的4倍deviation范围，并且有大约0.005%~0.007%的概率出现4倍deviation范围外的结果
        /// 所以如果按照mean和variance设定值域，deviation 应该为 variance * 0.25
        /// 建议直接用GaussianRandom方法
        /// </summary>
        private static double GaussianDeviationRandom(double mean, double deviation)
        {
            if (deviation <= 0)
            {
                return mean;
            }

            double u1 = LCGRandom01();
            double u2 = LCGRandom01();
            return GaussianDeviationDistribute(u1, u2, mean, deviation);
        }

        /// <summary>
        /// 基于传入的随机数生成器生成高斯分布的随机数
        /// </summary>
        /// <param name="random"></param>
        /// <param name="mean">数学期望</param>
        /// <param name="variance">散布半径</param>
        /// <param name="forceResultInRange">有大约0.005%~0.007%的概率出现散布在范围外的结果，设置此参数为true可以保证结果在范围内</param>
        /// <param name="maxTriesWhenOutOfRange">如果超出范围，会尝试重新随机，为防止意外情况下无限尝试导致死循环，这个参数用于限制尝试次数</param>
        /// <returns></returns>
        public static double GaussianRandom(Random random, double mean, double variance, bool forceResultInRange = true, int maxTriesWhenOutOfRange = 4)
        {
            double dev = variance * GaussianDistributeScale;
            double result = GaussianDeviationRandom(random, mean, dev);
            if (forceResultInRange)
            {
                int triedTimes = 0;
                double min = mean - variance;
                double max = mean + variance;
                while (triedTimes < maxTriesWhenOutOfRange && (result < min || result > max))
                {
                    triedTimes++;
                    result = GaussianDeviationRandom(random, mean, dev);
                }
            }
            return result;
        }

        /// <summary>
        /// 此函数采用的近似算法存在一些误差
        /// 标准函数的结果分布一般在数学期望左右两侧3倍sigma范围内
        /// 此函数的分布结果则大约为两侧的4倍deviation范围，并且有大约0.005%~0.007%的概率出现4倍deviation范围外的结果
        /// 所以如果按照mean和variance设定值域，deviation 应该为 variance * 0.25
        /// 建议直接用GaussianRandom方法
        /// </summary>
        private static double GaussianDeviationRandom(Random random, double mean, double deviation)
        {
            if (deviation <= 0) { return mean; }

            double u1 = random.NextDouble();
            double u2 = random.NextDouble();
            return GaussianDeviationDistribute(u1, u2, mean, deviation);
        }

        /// <summary>
        /// 高斯偏离分布，散布范围在mean两侧各4倍deviation的范围内
        /// </summary>
        /// <param name="u1">[0, 1)</param>
        /// <param name="u2">[0, 1)</param>
        private static double GaussianDeviationDistribute(double u1, double u2, double mean, double deviation)
        {
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + deviation * randStdNormal;
            return randNormal;
        }
        #endregion

        #region triangle random
        public static float TriangleRandom(float mean, float variance)
        {
            float left = mean - variance;
            float right = mean + variance;
            return TriangleRandom(left, right, mean);
        }

        public static float TriangleRandom(float left, float right, float mean)
        {
            float x = LCGRandom01();
            return TriangleDistribute(x, left, right, mean);
        }

        public static float TriangleRandom(int seed, float mean, float variance)
        {
            float left = mean - variance;
            float right = mean + variance;
            return TriangleRandom(seed, left, right, mean);
        }

        public static float TriangleRandom(int seed, float left, float right, float mean)
        {
            float x = LCGRandom01(seed);
            return TriangleDistribute(x, left, right, mean);
        }


        public static float TriangleRandom(Random random, float mean, float variance)
        {
            float left = mean - variance;
            float right = mean + variance;
            return TriangleRandom(random, left, right, mean);
        }

        public static float TriangleRandom(Random random, float left, float right, float mean)
        {
            float x = (float)random.NextDouble();
            return TriangleDistribute(x, left, right, mean);
        }

        /// <summary>
        /// 把[0, 1)范围的任意数 x 随机到一个三角分布中
        /// </summary>
        /// <param name="x">[0, 1)</param>
        public static float TriangleDistribute(float x, float left, float right, float mean)
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

        public static double TriangleRandom(double mean, double variance)
        {
            double left = mean - variance;
            double right = mean + variance;
            return TriangleRandom(left, right, mean);

        }

        public static double TriangleRandom(double left, double right, double mean)
        {
            double x = LCGRandom01_Double();
            return TriangleDistribute(x, left, right, mean);
        }

        public static double TriangleRandom(Random random, double mean, double variance)
        {
            double left = mean - variance;
            double right = mean + variance;
            return TriangleRandom(random, left, right, mean);
        }

        public static double TriangleRandom(Random random, double left, double right, double mean)
        {
            double x = random.NextDouble();
            return TriangleDistribute(x, left, right, mean);
        }

        /// <summary>
        /// 把[0, 1)范围的任意数 x 随机到一个三角分布中
        /// </summary>
        /// <param name="x">[0, 1)</param>
        public static double TriangleDistribute(double x, double l, double r, double m)
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
