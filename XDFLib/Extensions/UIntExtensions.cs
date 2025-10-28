using System.Runtime.CompilerServices;

namespace XDFLib.Extensions
{
    public static class UIntExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BitIntersect(this uint va1, uint va2)
        {
            return (va1 & va2) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckBit(this uint value, int bitIndex)
        {
            uint bit = (uint)(1 << bitIndex);
            return BitIntersect(value, bit);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this uint value, int bitIndex)
        {
            uint bit = (uint)(1 << bitIndex);
            value |= bit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetBit(ref this uint value, int bitIndex)
        {
            uint bit = (uint)(1 << bitIndex);
            value &= ~bit;
        }
    }
}
