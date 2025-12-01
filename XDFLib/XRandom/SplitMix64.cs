using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XDFLib.XRandom
{
    public class SplitMix64
    {
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
