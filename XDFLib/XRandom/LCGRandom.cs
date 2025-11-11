using System;
using System.Runtime.CompilerServices;

namespace XDFLib.XRandom
{
    /// <summary>
    /// 基于LCG（linear congruential generator）线性同余算法的随机数实现
    /// 公式：X(n+1) = (a * X（n） + c) mod m;
    /// 常量采用的是C++11标准库的版本：其中 mod = 2147483647, a = 48271, c = 0
    /// </summary>
    public static class LCG
    {
        public const int LCGmod = 2147483647; // 2^31 - 1
        public const int LCGa = 48271;

        static int _randomSeed = Guid.NewGuid().GetHashCode();

        /// <returns>Random in [-2147483648, 2147483647)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Random(int seed)
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

        public static float Random01(int seed)
        {
            seed = Random(seed);
            return SeedTo01(seed);
        }

        /// <returns>Random in [0, 1)</returns>
        public static float Random01(ref int seed)
        {
            seed = Random(seed);
            return SeedTo01(seed);
        }

        public static double Random01_Double(int seed)
        {
            seed = Random(seed);
            return SeedTo01_Double(seed);
        }

        public static double Random01_Double(ref int seed)
        {
            seed = Random(seed);
            return SeedTo01_Double(seed);
        }

        public static int SeedToRange(int seed, int min, int max)
        {
            int len = max - min;
            int result = min + (int)(len * SeedTo01(seed));
            return result;
        }

        /// <returns>Random in [min, max)</returns>
        public static int Random(int seed, int min, int max)
        {
            if (min == max) { return min; }
            else
            {
                seed = Random(seed);
                return SeedToRange(seed, min, max);
            }
        }

        /// <returns>Random in [min, max)</returns>
        public static int Random(ref int seed, int min, int max)
        {
            if (min == max) { return min; }
            else
            {
                seed = Random(seed);
                return SeedToRange(seed, min, max);
            }
        }

        public static float SeedToRange(int seed, float min, float max)
        {
            float len = max - min;
            float result = min + len * SeedTo01(seed);
            return result;

        }

        /// <returns>Random in [min, max)</returns>
        public static float Random(int seed, float min, float max)
        {
            if (min == max) { return min; }
            else
            {
                seed = Random(seed);
                return SeedToRange(seed, min, max);
            }
        }

        /// <returns>Random in [min, max)</returns>
        public static float Random(ref int seed, float min, float max)
        {
            if (min == max) { return min; }
            else
            {
                seed = Random(seed);
                return SeedToRange(seed, min, max);
            }
        }

        public static double SeedToRange(int seed, double min, double max)
        {
            double len = max - min;
            double result = min + len * SeedTo01_Double(seed);
            return result;
        }

        public static double Random(int seed, double min, double max)
        {
            if (min == max) { return min; }
            else
            {
                seed = Random(seed);
                return SeedToRange(seed, min, max);
            }
        }

        public static double Random(ref int seed, double min, double max)
        {
            if (min == max) { return min; }
            else
            {
                seed = Random(seed);
                return SeedToRange(seed, min, max);
            }
        }

        public static int Random()
        {
            _randomSeed = Random(_randomSeed++);
            return _randomSeed;
        }

        public static float Random01()
        {
            _randomSeed = Random(_randomSeed++);
            return SeedTo01(_randomSeed);
        }

        public static double Random01_Double()
        {
            _randomSeed = Random(_randomSeed++);
            return SeedTo01_Double(_randomSeed);
        }

        /// <returns>Random in [min, max)</returns>
        public static int Random(int min, int max)
        {
            _randomSeed = Random(_randomSeed++);
            return SeedToRange(_randomSeed, min, max);
        }

        /// <returns>Random in [min, max)</returns>
        public static float Random(float min, float max)
        {
            _randomSeed = Random(_randomSeed++);
            return SeedToRange(_randomSeed, min, max);
        }

        /// <returns>Random in [min, max)</returns>
        public static double Random(double min, double max)
        {
            _randomSeed = Random(_randomSeed++);
            return SeedToRange(_randomSeed, min, max);
        }
    }
}
