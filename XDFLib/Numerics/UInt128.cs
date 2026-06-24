using System;
using System.Runtime.CompilerServices;

namespace XDFLib.Numerics
{
    /// <summary>
    /// 高性能 128-bit 无符号整数，兼容 .NET Standard 2.1。
    /// 内存布局：_lo (低64位) + _hi (高64位)，小端语义。
    /// </summary>
    public readonly struct UInt128 : IEquatable<UInt128>, IComparable<UInt128>
    {
        private readonly ulong _lo;
        private readonly ulong _hi;

        // ──────────────────────── 构造 & 常量 ────────────────────────

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt128(ulong hi, ulong lo)
        {
            _hi = hi;
            _lo = lo;
        }

        public static readonly UInt128 Zero = new UInt128(0, 0);
        public static readonly UInt128 One = new UInt128(0, 1);
        public static readonly UInt128 MaxValue = new UInt128(ulong.MaxValue, ulong.MaxValue);

        // ──────────────────────── 隐式/显式转换 ────────────────────────

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UInt128(ulong value) => new UInt128(0, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UInt128(uint value) => new UInt128(0, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator ulong(UInt128 value) => value._lo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator uint(UInt128 value) => (uint)value._lo;

        // ──────────────────────── 位运算 ────────────────────────

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128 operator &(UInt128 a, UInt128 b) =>
            new UInt128(a._hi & b._hi, a._lo & b._lo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128 operator |(UInt128 a, UInt128 b) =>
            new UInt128(a._hi | b._hi, a._lo | b._lo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128 operator ^(UInt128 a, UInt128 b) =>
            new UInt128(a._hi ^ b._hi, a._lo ^ b._lo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128 operator ~(UInt128 a) =>
            new UInt128(~a._hi, ~a._lo);

        // ──────────────────────── 移位运算 ────────────────────────

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128 operator <<(UInt128 value, int shift)
        {
            // C# 对 ulong 的 << 自动取 mod 64，这里需要手动处理跨段
            shift &= 127; // 规范化到 [0, 127]
            if (shift == 0) return value;

            if (shift < 64)
            {
                ulong newHi = (value._hi << shift) | (value._lo >> (64 - shift));
                ulong newLo = value._lo << shift;
                return new UInt128(newHi, newLo);
            }
            else // shift >= 64
            {
                // 低64位全部移入高位，低位补零
                ulong newHi = value._lo << (shift - 64);
                return new UInt128(newHi, 0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128 operator >>(UInt128 value, int shift)
        {
            shift &= 127;
            if (shift == 0) return value;

            if (shift < 64)
            {
                ulong newLo = (value._lo >> shift) | (value._hi << (64 - shift));
                ulong newHi = value._hi >> shift;
                return new UInt128(newHi, newLo);
            }
            else // shift >= 64
            {
                ulong newLo = value._hi >> (shift - 64);
                return new UInt128(0, newLo);
            }
        }

        // ──────────────────────── 比较 & 相等 ────────────────────────

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(UInt128 a, UInt128 b) =>
            a._hi == b._hi && a._lo == b._lo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(UInt128 a, UInt128 b) => !(a == b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(UInt128 a, UInt128 b) =>
            a._hi < b._hi || (a._hi == b._hi && a._lo < b._lo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(UInt128 a, UInt128 b) => b < a;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(UInt128 a, UInt128 b) => !(b < a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(UInt128 a, UInt128 b) => !(a < b);

        public bool Equals(UInt128 other) => this == other;
        public override bool Equals(object obj) => obj is UInt128 other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                return (_hi.GetHashCode() * 397) ^ _lo.GetHashCode();
            }
        }

        public int CompareTo(UInt128 other)
        {
            int hiCmp = _hi.CompareTo(other._hi);
            return hiCmp != 0 ? hiCmp : _lo.CompareTo(other._lo);
        }

        // ──────────────────────── 算术运算（基础） ────────────────────────

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128 operator +(UInt128 a, UInt128 b)
        {
            ulong lo = a._lo + b._lo;
            // 进位检测：如果结果小于任一加数，则发生了溢出
            ulong carry = lo < a._lo ? 1UL : 0UL;
            ulong hi = a._hi + b._hi + carry;
            return new UInt128(hi, lo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt128 operator -(UInt128 a, UInt128 b)
        {
            ulong lo = a._lo - b._lo;
            // 借位检测：如果被减数小于减数，则需要从高位借位
            ulong borrow = a._lo < b._lo ? 1UL : 0UL;
            ulong hi = a._hi - b._hi - borrow;
            return new UInt128(hi, lo);
        }

        // ──────────────────────── 辅助方法 ────────────────────────

        /// <summary>获取高64位</summary>
        public ulong High => _hi;

        /// <summary>获取低64位</summary>
        public ulong Low => _lo;

        /// <summary>是否为0</summary>
        public bool IsZero => _hi == 0 && _lo == 0;

        /// <summary>计算前导零位数 (0~128)</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LeadingZeroCount()
        {
            if (_hi != 0)
                return LeadingZeroCount64(_hi);
            return 64 + LeadingZeroCount64(_lo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LeadingZeroCount64(ulong value)
        {
            if (value == 0) return 64;
            // De Bruijn 算法或二分法；这里用简单高效的二分
            int n = 0;
            if ((value & 0xFFFFFFFF00000000UL) == 0) { n += 32; value <<= 32; }
            if ((value & 0xFFFF000000000000UL) == 0) { n += 16; value <<= 16; }
            if ((value & 0xFF00000000000000UL) == 0) { n += 8; value <<= 8; }
            if ((value & 0xF000000000000000UL) == 0) { n += 4; value <<= 4; }
            if ((value & 0xC000000000000000UL) == 0) { n += 2; value <<= 2; }
            if ((value & 0x8000000000000000UL) == 0) { n += 1; }
            return n;
        }

        public override string ToString()
        {
            if (_hi == 0) return _lo.ToString();
            // 简易十六进制表示；如需十进制可另行实现
            return $"0x{_hi:X16}{_lo:X16}";
        }
    }
}
