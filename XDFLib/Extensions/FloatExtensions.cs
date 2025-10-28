namespace XDFLib.Extensions
{
    public static class FloatExtensions
    {
        const string _digit0 = "{0:F0}";
        const string _digit1 = "{0:F1}";
        const string _digit2 = "{0:F2}";
        const string _digit3 = "{0:F3}";
        const string _digit4 = "{0:F4}";

        public static float Epsilon = 0.0001f;

        public static string ToStringDigit(this float value, uint digitCount)
        {
            switch (digitCount)
            {
                case 0: return string.Format(_digit0, value);
                case 1: return string.Format(_digit1, value);
                case 2: return string.Format(_digit2, value);
                case 3: return string.Format(_digit3, value);
                case 4: return string.Format(_digit4, value);
                default:
                    var f = _digit1.Replace("1", digitCount.ToString());
                    return string.Format(f, value);
            }
        }

        public static bool IsNearZero(this float value)
        {
            return value < Epsilon;
        }
    }
}
