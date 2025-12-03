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

    }
}
