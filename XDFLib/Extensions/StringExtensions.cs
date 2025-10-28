namespace XDFLib.Extensions
{
    public static class StringExtensions
    {
        public const string IntegerRegex = @"^-?\d*$";
        public const string ScalarRegex = @"^-?\d*\.?\d*$";

        public static string RemoveFromEnd(this string str, int count)
        {
            if (count < 0)
            {
                return str;
            }
            else if (str.Length <= count)
            {
                return "";
            }
            else
            {
                var removeFromIndex = str.Length - count;
                var result = str.Remove(removeFromIndex);
                return result;
            }
        }

        public static bool IsInteger(this string str)
        {
            int result;
            return int.TryParse(str, out result);
        }

        /// <summary>
        /// 尝试把 string 解析成 int
        /// </summary>
        /// <param name="str">需要解析的 string</param>
        /// <returns>返回 int? 作为结果，如果解析失败的话会返回 null</returns>
        public static int? ToInteger(this string str)
        {
            int? result = null;
            int parseOut;
            var success = int.TryParse(str, out parseOut);
            if (success)
            {
                result = parseOut;
            }
            return result;
        }

        public static bool IsFloat(this string str)
        {
            float result;
            return float.TryParse(str, out result);
        }

        /// <summary>
        /// 尝试把 string 解析成 float
        /// </summary>
        /// <param name="str">需要解析的 string</param>
        /// <returns>返回 float? 作为结果，如果解析失败的话会返回 null</returns>
        public static float? ToFloat(this string str)
        {
            float? result = null;
            float parseOut;
            var success = float.TryParse(str, out parseOut);
            if (success)
            {
                result = parseOut;
            }
            return result;
        }

        public static bool IsDouble(this string str)
        {
            double result;
            return double.TryParse(str, out result);
        }

        /// <summary>
        /// 尝试把 string 解析成 double
        /// </summary>
        /// <param name="str">需要解析的 string</param>
        /// <returns>返回 double? 作为结果，如果解析失败的话会返回 null</returns>
        public static double? ToDouble(this string str)
        {
            double? result = null;
            double parseOut;
            var success = double.TryParse(str, out parseOut);
            if (success)
            {
                result = parseOut;
            }
            return result;
        }
    }
}
