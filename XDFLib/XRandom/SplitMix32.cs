using System;
using System.Runtime.CompilerServices;

namespace XDFLib.XRandom
{
    public class SplitMix32
    {
        // 预计算倒数, 1 / 2^32，用于将 uint 映射到 [0, 1)
        private const float INV_UINT_MAX = 1f / 4294967296f;
        private const double INV_UINT_MAX_D = 1.0 / 4294967296.0;

        static int _randomSeed = Guid.NewGuid().GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Random(int seed)
        {
            unchecked // 显式声明允许溢出，防止编译选项导致运行时异常
            {
                uint z = (uint)(seed + 0x9E3779B9); // 黄金比例常数（32位版）
                z ^= z >> 16;
                z *= 0x85EBCA6B;
                z ^= z >> 13;
                z *= 0xC2B2AE35;
                z ^= z >> 16;
                return (int)z;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Random()
        {
            _randomSeed = Random(_randomSeed++);
            return _randomSeed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SeedTo01(int seed)
        {
            return (uint)seed * INV_UINT_MAX;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SeedTo01_Double(int seed)
        {
            return (uint)seed * INV_UINT_MAX_D;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Random01()
        {
            _randomSeed = Random(_randomSeed++);
            return SeedTo01(_randomSeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Random01(int seed)
        {
            seed = Random(seed);
            return SeedTo01(seed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Random01(ref int seed)
        {
            seed = Random(seed);
            return SeedTo01(seed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random01_Double()
        {
            _randomSeed = Random(_randomSeed++);
            return SeedTo01_Double(_randomSeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random01_Double(int seed)
        {
            seed = Random(seed);
            return SeedTo01_Double(seed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random01_Double(ref int seed)
        {
            seed = Random(seed);
            return SeedTo01_Double(seed);
        }

        #region Random in range

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SeedToRange(int seed, int min, int max)
        {
            var len = max - min;
            var result = min + (int)(len * SeedTo01(seed));
            return result;
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Random(int seed, int min, int max)
        {
            if (min == max) { return min; }
            seed = Random(seed);
            return SeedToRange(seed, min, max);
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Random(ref int seed, int min, int max)
        {
            if (min == max) { return min; }
            seed = Random(seed);
            return SeedToRange(seed, min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SeedToRange(int seed, float min, float max)
        {
            var len = max - min;
            var result = min + (len * SeedTo01(seed));
            return result;
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Random(int seed, float min, float max)
        {
            if (min == max) { return min; }
            seed = Random(seed);
            return SeedToRange(seed, min, max);
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Random(ref int seed, float min, float max)
        {
            if (min == max) { return min; }
            seed = Random(seed);
            return SeedToRange(seed, min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SeedToRange(int seed, double min, double max)
        {
            var len = max - min;
            var result = min + (len * SeedTo01_Double(seed));
            return result;
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random(int seed, double min, double max)
        {
            if (min == max) { return min; }
            seed = Random(seed);
            return SeedToRange(seed, min, max);
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random(ref int seed, double min, double max)
        {
            if (min == max) { return min; }
            seed = Random(seed);
            return SeedToRange(seed, min, max);
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Random(int min, int max)
        {
            _randomSeed = Random(_randomSeed++);
            return SeedToRange(_randomSeed, min, max);
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Random(float min, float max)
        {
            _randomSeed = Random(_randomSeed++);
            return SeedToRange(_randomSeed, min, max);
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random(double min, double max)
        {
            _randomSeed = Random(_randomSeed++);
            return SeedToRange(_randomSeed, min, max);
        }
        #endregion
    }
}
