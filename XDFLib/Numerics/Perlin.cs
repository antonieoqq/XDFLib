using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace XDFLib.Numerics
{
    public sealed class Perlin
    {
        #region Static
        // 线程安全的静态实例
        public static readonly Perlin Instance = new Perlin(851128);

        // 修复后的二维梯度向量（16个元素）
        private static readonly Vector2[] Gradients2D = {
            new Vector2( 1f, 1f), new Vector2(-1f, 1f), new Vector2( 1f,-1f), new Vector2(-1f,-1f),
            new Vector2( 1f, 0f), new Vector2(-1f, 0f), new Vector2( 0f, 1f), new Vector2( 0f,-1f),
            // 补充8个梯度保持均匀分布
            new Vector2( 1f, 1f).Normalized(), new Vector2(-1f, 1f).Normalized(),
            new Vector2( 1f,-1f).Normalized(), new Vector2(-1f,-1f).Normalized(),
            new Vector2( 1f, 0.5f).Normalized(), new Vector2(-1f, 0.5f).Normalized(),
            new Vector2( 0.5f, 1f).Normalized(), new Vector2( 0.5f,-1f).Normalized()
        };


        // 三维梯度向量
        private static readonly Vector3[] Gradients3D = {
            new Vector3( 1f, 1f, 0f), new Vector3(-1f, 1f, 0f), new Vector3( 1f,-1f, 0f), new Vector3(-1f,-1f, 0f),
            new Vector3( 1f, 0f, 1f), new Vector3(-1f, 0f, 1f), new Vector3( 1f, 0f,-1f), new Vector3(-1f, 0f,-1f),
            new Vector3( 0f, 1f, 1f), new Vector3( 0f,-1f, 1f), new Vector3( 0f, 1f,-1f), new Vector3( 0f,-1f,-1f),
            // 补充4个梯度保持均匀分布
            new Vector3( 1f, 1f, 1f).Normalized(), new Vector3(-1f, 1f, 1f).Normalized(),
            new Vector3( 1f,-1f, 1f).Normalized(), new Vector3(-1f,-1f, 1f).Normalized()
        };
        #endregion

        public readonly int Seed;
        // 排列表（512大小用于快速索引）
        private readonly int[] _permutationTable;

        public Perlin(int seed = 0)
        {
            Seed = seed;
            // 初始化排列表
            _permutationTable = new int[512];

            // 填充0-255的随机排列
            for (int i = 0; i < 256; i++)
                _permutationTable[i] = i;

            // Fisher-Yates洗牌算法
            for (int i = 0; i < 256; i++)
            {
                int j = XRandom.SplitMix32.Random(ref seed, 1, 256);
                (_permutationTable[i], _permutationTable[j]) =
                    (_permutationTable[j], _permutationTable[i]);
            }

            // 复制到后半部分（避免索引时取模）
            for (int i = 256; i < 512; i++)
                _permutationTable[i] = _permutationTable[i - 256];
        }

        /// <summary>
        /// 二维Perlin噪声
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>理论上返回(-1, 1)的结果，但只有采样规模足够大时，才会出现极值</returns>
        public float Noise(float x, float y)
        {
            // 确定网格单元
            int xi = (int)Math.Floor(x) & 255;
            int yi = (int)Math.Floor(y) & 255;

            // 计算相对位置
            float xf = x - (int)Math.Floor(x);
            float yf = y - (int)Math.Floor(y);

            // 计算四个角点的哈希值
            int aa = _permutationTable[_permutationTable[xi] + yi];
            int ab = _permutationTable[_permutationTable[xi] + yi + 1];
            int ba = _permutationTable[_permutationTable[xi + 1] + yi];
            int bb = _permutationTable[_permutationTable[xi + 1] + yi + 1];

            // 计算四个角点的梯度贡献
            float g1 = GradDot(aa, xf, yf);
            float g2 = GradDot(ab, xf, yf - 1);
            float g3 = GradDot(ba, xf - 1, yf);
            float g4 = GradDot(bb, xf - 1, yf - 1);

            // 使用五次多项式平滑插值
            float sx = SmoothStep(xf);
            float sy = SmoothStep(yf);

            // 双线性插值
            float a = XMath.Lerp(g1, g3, sx);
            float b = XMath.Lerp(g2, g4, sx);
            return XMath.Lerp(a, b, sy);
        }

        // 三维Perlin噪声
        public float Noise(float x, float y, float z)
        {
            // 确定网格单元
            int xi = (int)Math.Floor(x) & 255;
            int yi = (int)Math.Floor(y) & 255;
            int zi = (int)Math.Floor(z) & 255;

            // 计算相对位置
            float xf = x - (int)Math.Floor(x);
            float yf = y - (int)Math.Floor(y);
            float zf = z - (int)Math.Floor(z);

            // 计算八个角点的哈希值
            int aaa = _permutationTable[_permutationTable[_permutationTable[xi] + yi] + zi];
            int aab = _permutationTable[_permutationTable[_permutationTable[xi] + yi] + zi + 1];
            int aba = _permutationTable[_permutationTable[_permutationTable[xi] + yi + 1] + zi];
            int abb = _permutationTable[_permutationTable[_permutationTable[xi] + yi + 1] + zi + 1];
            int baa = _permutationTable[_permutationTable[_permutationTable[xi + 1] + yi] + zi];
            int bab = _permutationTable[_permutationTable[_permutationTable[xi + 1] + yi] + zi + 1];
            int bba = _permutationTable[_permutationTable[_permutationTable[xi + 1] + yi + 1] + zi];
            int bbb = _permutationTable[_permutationTable[_permutationTable[xi + 1] + yi + 1] + zi + 1];

            // 计算八个角点的梯度贡献（使用三维梯度）
            float g1 = GradDot3D(aaa, xf, yf, zf);
            float g2 = GradDot3D(aab, xf, yf, zf - 1);
            float g3 = GradDot3D(aba, xf, yf - 1, zf);
            float g4 = GradDot3D(abb, xf, yf - 1, zf - 1);
            float g5 = GradDot3D(baa, xf - 1, yf, zf);
            float g6 = GradDot3D(bab, xf - 1, yf, zf - 1);
            float g7 = GradDot3D(bba, xf - 1, yf - 1, zf);
            float g8 = GradDot3D(bbb, xf - 1, yf - 1, zf - 1);

            // 使用五次多项式平滑插值
            float sx = SmoothStep(xf);
            float sy = SmoothStep(yf);
            float sz = SmoothStep(zf);

            // 三线性插值
            float a = XMath.Lerp(g1, g2, sz);
            float b = XMath.Lerp(g3, g4, sz);
            float c = XMath.Lerp(g5, g6, sz);
            float d = XMath.Lerp(g7, g8, sz);

            float ab = XMath.Lerp(a, b, sy);
            float cd = XMath.Lerp(c, d, sy);

            return XMath.Lerp(ab, cd, sx);
        }

        // 平滑插值函数 (6t^5 - 15t^4 + 10t^3)
        private float SmoothStep(float t)
        {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        // 二维梯度点积（修复索引问题）
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GradDot(int hash, float x, float y)
        {
            // 安全索引：hash & 15 (0-15)
            Vector2 grad = Gradients2D[hash & 15];
            return grad.X * x + grad.Y * y;
        }

        // 三维梯度点积
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GradDot3D(int hash, float x, float y, float z)
        {
            // 安全索引：hash & 15 (0-15)
            Vector3 grad = Gradients3D[hash & 15];
            return grad.X * x + grad.Y * y + grad.Z * z;
        }
    }
}
