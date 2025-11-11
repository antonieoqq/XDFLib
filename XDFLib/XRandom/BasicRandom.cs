using System;

namespace XDFLib.XRandom
{
    public static class Basic
    {
        /// <summary> 经实测，当一个Random对象在生成了大约8000个随机数后，会失去随机性，可能这和伪随机的算法有关 </summary>
        public const int MaxGenerationForEachRandom = 8000;

        static Random _rnd;
        static int _randNumGenerated = 0;
        public static Random Rnd
        {
            get
            {
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
            _randNumGenerated++;
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
            _randNumGenerated++;

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
            _randNumGenerated++;

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
            _randNumGenerated++;

            return (float)Rnd.NextDouble();
        }

        public static double Random01_Double()
        {
            _randNumGenerated++;

            return Rnd.NextDouble();
        }
    }
}
