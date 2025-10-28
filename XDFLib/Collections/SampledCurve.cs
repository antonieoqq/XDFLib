using System;

namespace XDFLib.Collections
{
    /// <summary>
    /// 这个类用于预先采样并记录某个曲线的数值，并支持二分查找取值
    /// </summary>
    public class SampledCurve
    {
        public float StartX { get; private set; }
        public float EndX { get; private set; }

        public float RangeX => EndX - StartX;

        float[] _samples;
        public Span<float> Samples => _samples;
        public int SampleCount => _samples.Length;

        public SampledCurve(int sampleCount, float startX, float endX)
        {
            _samples = new float[sampleCount];
            StartX = MathF.Min(startX, endX);
            EndX = MathF.Max(startX, endX);
        }

        public void Resample(Func<float, float> sampleFunc, float startX, float endX)
        {
            StartX = startX;
            EndX = endX;
            Resample(sampleFunc);
        }

        public void Resample(Func<float, float> sampleFunc)
        {
            for (int i = 0; i < SampleCount; i++)
            {
                var percent = (float)i / (SampleCount - 1);
                var x = StartX + (percent * RangeX);
                _samples[i] = sampleFunc(x);
            }
        }

        public float GetValueAtX(float x)
        {
            var percent = (x - StartX) / (EndX - StartX);
            var t = XMath.Clamp(percent, 0, 1);
            return GetValueAtT(t);
        }

        public float GetValueAtPercent(float percent)
        {
            var t = XMath.Clamp(percent, 0, 1);
            return GetValueAtT(t);
        }

        float GetValueAtT(float t)
        {
            var floatIndex = t * (Samples.Length - 1);
            var minIndex = (int)floatIndex;
            var subT = floatIndex - minIndex;
            var maxIndex = (int)MathF.Ceiling(floatIndex);
            var result = XMath.Lerp(_samples[minIndex], _samples[maxIndex], subT);
            return result;
        }
    }
}
