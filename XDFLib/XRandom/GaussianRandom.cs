using System;

namespace XDFLib.XRandom
{
    public static class Gaussian
    {
        // 当随机数结果被截断到范围内时，超越上界的数会被截断到上界，使值域为[min, max]
        // 但理想的随机结果的值域应该是[min, max)，右半应为开区间（不能取到max）
        // 所以Torlerence的作用是对结果进行一次修正
        public const double DoubleTorlerenceFactor = 0.999999999999f;
        public const float FloatTorlerenceFactor = 0.999999f;
        public const float GaussianDistributeScale = 0.25f;

        #region gaussian random
        /// <summary>
        /// 通过任意数 u1，u2 随机到一个高斯分布中
        /// </summary>
        /// <param name="u1">[0, 1)</param>
        /// <param name="u2">[0, 1)</param>
        public static float Distribute(float u1, float u2, float mean, float variance)
        {
            float dev = variance * GaussianDistributeScale;
            return DeviationDistribute(u1, u2, mean, dev);
        }

        /// <summary>
        /// 基于内部的LCG随机数生成器生成高斯分布的随机数
        /// </summary>
        /// <param name="mean">数学期望</param>
        /// <param name="variance">散布半径</param>
        /// <param name="forceResultInRange">有大约0.005%~0.007%的概率出现散布在范围外的结果，设置此参数为true可以保证结果在范围内</param>
        /// <param name="maxTriesWhenOutOfRange">如果超出范围，会尝试重新随机，为防止意外情况下无限尝试导致死循环，这个参数用于限制尝试次数</param>
        /// <returns></returns>
        public static float Random(float mean, float variance, bool forceResultInRange = true)
        {
            float dev = variance * GaussianDistributeScale;
            float result = DeviationRandom(mean, dev);
            if (forceResultInRange)
            {
                float min = mean - variance;
                float max = mean + variance;
                return XMath.Clamp(result, min, max * FloatTorlerenceFactor);
            }
            return result;
        }

        private static float DeviationRandom(float mean, float deviation)
        {
            if (deviation <= 0) { return mean; }
            float u1 = SplitMix32.Random01();
            float u2 = SplitMix32.Random01();
            return DeviationDistribute(u1, u2, mean, deviation);
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
        public static float Random(ref int seed, float mean, float variance, bool forceResultInRange = true)
        {
            float dev = variance * GaussianDistributeScale;
            float result = DeviationRandom(ref seed, mean, dev);
            if (forceResultInRange)
            {
                float min = mean - variance;
                float max = mean + variance;
                return XMath.Clamp(result, min, max * FloatTorlerenceFactor);
            }
            return result;
        }

        private static float DeviationRandom(ref int seed, float mean, float deviation)
        {
            if (deviation <= 0)
            {
                return mean;
            }
            float u1 = SplitMix32.Random01(ref seed);
            float u2 = SplitMix32.Random01(ref seed);
            return DeviationDistribute(u1, u2, mean, deviation);
        }

        public static float Random(int seed, float mean, float variance, bool forceResultInRange = true)
        {
            float dev = variance * GaussianDistributeScale;
            float result = DeviationRandom(seed, mean, dev);
            if (forceResultInRange)
            {
                float min = mean - variance;
                float max = mean + variance;
                return XMath.Clamp(result, min, max * FloatTorlerenceFactor);
            }
            return result;
        }

        private static float DeviationRandom(int seed, float mean, float deviation)
        {
            if (deviation <= 0)
            {
                return mean;
            }
            float u1 = SplitMix32.Random01(ref seed);
            float u2 = SplitMix32.Random01(seed);
            return DeviationDistribute(u1, u2, mean, deviation);
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
        public static float Random(Random random, float mean, float variance, bool forceResultInRange = true)
        {
            float dev = variance * GaussianDistributeScale;
            float result = DeviationRandom(random, mean, dev);
            if (forceResultInRange)
            {
                float min = mean - variance;
                float max = mean + variance;
                return XMath.Clamp(result, min, max * FloatTorlerenceFactor);
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
        private static float DeviationRandom(Random random, float mean, float deviation)
        {
            if (deviation <= 0) { return mean; }
            float u1 = (float)random.NextDouble();
            float u2 = (float)random.NextDouble();
            return DeviationDistribute(u1, u2, mean, deviation);
        }

        /// <summary>
        /// 高斯偏离分布，散布范围在mean两侧各4倍deviation的范围内
        /// </summary>
        /// <param name="u1">[0, 1)</param>
        /// <param name="u2">[0, 1)</param>
        private static float DeviationDistribute(float u1, float u2, float mean, float deviation)
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
        public static double Distribute(double u1, double u2, double mean, double variance)
        {
            double dev = variance * GaussianDistributeScale;
            return DeviationDistribute(u1, u2, mean, dev);
        }

        /// <summary>
        /// 基于内部的LCG随机数生成器生成高斯分布的随机数
        /// </summary>
        /// <param name="mean">数学期望</param>
        /// <param name="variance">散布半径</param>
        /// <param name="forceResultInRange">有大约0.005%~0.007%的概率出现散布在范围外的结果，设置此参数为true可以保证结果在范围内</param>
        /// <param name="maxTriesWhenOutOfRange">如果超出范围，会尝试重新随机，为防止意外情况下无限尝试导致死循环，这个参数用于限制尝试次数</param>
        /// <returns></returns>
        public static double Random(double mean, double variance, bool forceResultInRange = true)
        {
            double dev = variance * GaussianDistributeScale;
            double result = DeviationRandom(mean, dev);
            if (forceResultInRange)
            {
                double min = mean - variance;
                double max = mean + variance;
                return XMath.Clamp(result, min, max * DoubleTorlerenceFactor);
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
        public static double Random(ref int seed, double mean, double variance, bool forceResultInRange = true)
        {
            double dev = variance * GaussianDistributeScale;
            double result = DeviationRandom(ref seed, mean, dev);
            if (forceResultInRange)
            {
                double min = mean - variance;
                double max = mean + variance;
                return XMath.Clamp(result, min, max * DoubleTorlerenceFactor);
            }
            return result;
        }

        private static double DeviationRandom(ref int seed, double mean, double deviation)
        {
            if (deviation <= 0)
            {
                return mean;
            }
            double u1 = SplitMix32.Random01_Double(ref seed);
            double u2 = SplitMix32.Random01_Double(ref seed);
            return DeviationDistribute(u1, u2, mean, deviation);
        }

        public static double Random(int seed, double mean, double variance, bool forceResultInRange = true)
        {
            double dev = variance * GaussianDistributeScale;
            double result = DeviationRandom(seed, mean, dev);
            if (forceResultInRange)
            {
                double min = mean - variance;
                double max = mean + variance;
                return XMath.Clamp(result, min, max * DoubleTorlerenceFactor);
            }
            return result;
        }

        private static double DeviationRandom(int seed, double mean, double deviation)
        {
            if (deviation <= 0)
            {
                return mean;
            }
            double u1 = SplitMix32.Random01_Double(ref seed);
            double u2 = SplitMix32.Random01_Double(seed);
            return DeviationDistribute(u1, u2, mean, deviation);
        }

        /// <summary>
        /// 此函数采用的近似算法存在一些误差
        /// 标准函数的结果分布一般在数学期望左右两侧3倍sigma范围内
        /// 此函数的分布结果则大约为两侧的4倍deviation范围，并且有大约0.005%~0.007%的概率出现4倍deviation范围外的结果
        /// 所以如果按照mean和variance设定值域，deviation 应该为 variance * 0.25
        /// 建议直接用GaussianRandom方法
        /// </summary>
        private static double DeviationRandom(double mean, double deviation)
        {
            if (deviation <= 0)
            {
                return mean;
            }

            double u1 = SplitMix32.Random01();
            double u2 = SplitMix32.Random01();
            return DeviationDistribute(u1, u2, mean, deviation);
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
        public static double Random(Random random, double mean, double variance, bool forceResultInRange = true)
        {
            double dev = variance * GaussianDistributeScale;
            double result = DeviationRandom(random, mean, dev);
            if (forceResultInRange)
            {
                double min = mean - variance;
                double max = mean + variance;
                return XMath.Clamp(result, min, max * DoubleTorlerenceFactor);
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
        private static double DeviationRandom(Random random, double mean, double deviation)
        {
            if (deviation <= 0) { return mean; }

            double u1 = random.NextDouble();
            double u2 = random.NextDouble();
            return DeviationDistribute(u1, u2, mean, deviation);
        }

        /// <summary>
        /// 高斯偏离分布，散布范围在mean两侧各4倍deviation的范围内
        /// </summary>
        /// <param name="u1">[0, 1)</param>
        /// <param name="u2">[0, 1)</param>
        private static double DeviationDistribute(double u1, double u2, double mean, double deviation)
        {
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + deviation * randStdNormal;
            return randNormal;
        }
        #endregion

    }
}
