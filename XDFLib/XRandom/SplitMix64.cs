using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace XDFLib.XRandom
{
    public class SplitMix64
    {
        // 预计算倒数, 1 / 2^32，用于将 uint 映射到 [0, 1)
        private const double INV_UINT_MAX = 1.0 / 4294967296.0;

        static long _randomSeed = NextInt64();

        static long NextInt64()
        {
            byte[] buffer = new byte[8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(buffer);
            }
            return BitConverter.ToInt64(buffer, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Random(long seed)
        {
            // 使用 ulong 进行中间计算（避免符号扩展和溢出问题）
            ulong z = (ulong)seed + 0x9E3779B97F4A7C15L;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            z = z ^ (z >> 31);

            // 转为 long 并清除符号位（确保结果 ≥ 0）
            return (long)z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Random()
        {
            _randomSeed = Random(_randomSeed++);
            return _randomSeed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SeedTo01(long seed)
        {
            return (ulong)seed * INV_UINT_MAX;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random01()
        {
            _randomSeed = Random(_randomSeed++);
            return SeedTo01(_randomSeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random01(long seed)
        {
            seed = Random(seed);
            return SeedTo01(seed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random01(ref long seed)
        {
            seed = Random(seed);
            return SeedTo01(seed);
        }

        #region Random in range
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SeedToRange(long seed, long min, long max)
        {
            var len = max - min;
            var result = min + (long)(len * SeedTo01(seed));
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SeedToRange(long seed, double min, double max)
        {
            var len = max - min;
            var result = min + (len * SeedTo01(seed));
            return result;
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Random(long min, long max)
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

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Random(long seed, long min, long max)
        {
            if (min == max) { return min; }
            seed = Random(seed);
            return SeedToRange(seed, min, max);
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Random(ref long seed, long min, long max)
        {
            if (min == max) { return min; }
            seed = Random(seed);
            return SeedToRange(seed, min, max);
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random(long seed, double min, double max)
        {
            if (min == max) { return min; }
            seed = Random(seed);
            return SeedToRange(seed, min, max);
        }

        /// <returns>Random in [min, max)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Random(ref long seed, double min, double max)
        {
            if (min == max) { return min; }
            seed = Random(seed);
            return SeedToRange(seed, min, max);
        }
        #endregion
    }
}
