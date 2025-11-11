using System;

//easing functions https://easings.net/zh-cn#

namespace XDFLib
{
    public static class Ease
    {
        const double EaseC1 = 1.70158;
        const double EaseC2 = EaseC1 * 1.525;
        const double EaseC3 = EaseC1 + 1;
        const double EaseC4 = (2 * Math.PI) / 3;
        const double EaseC5 = (2 * Math.PI) / 4.5;

        public enum EEase
        {
            Linear,
            InSine,
            OutSine,
            InOutSine,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InBack,
            OutBack,
            InOutBack,
            InElastic,
            OutElastic,
            InOutElastic,
            InBounce,
            OutBounce,
            InOutBounce
        }

        public static double Easing(double t, EEase ease)
        {
            switch (ease)
            {
                case EEase.InSine:
                    return EaseInSine(t);
                case EEase.OutSine:
                    return EaseOutSine(t);
                case EEase.InOutSine:
                    return EaseInOutSine(t);
                case EEase.InQuad:
                    return EaseInQuad(t);
                case EEase.OutQuad:
                    return EaseOutQuad(t);
                case EEase.InOutQuad:
                    return EaseInOutQuad(t);
                case EEase.InCubic:
                    return EaseInCubic(t);
                case EEase.OutCubic:
                    return EaseOutCubic(t);
                case EEase.InOutCubic:
                    return EaseInOutCubic(t);
                case EEase.InQuart:
                    return EaseInQuart(t);
                case EEase.OutQuart:
                    return EaseOutQuart(t);
                case EEase.InOutQuart:
                    return EaseInOutQuart(t);
                case EEase.InQuint:
                    return EaseInQuint(t);
                case EEase.OutQuint:
                    return EaseOutQuint(t);
                case EEase.InOutQuint:
                    return EaseInOutQuint(t);
                case EEase.InExpo:
                    return EaseInExpo(t);
                case EEase.OutExpo:
                    return EaseOutExpo(t);
                case EEase.InOutExpo:
                    return EaseInOutExpo(t);
                case EEase.InCirc:
                    return EaseInCirc(t);
                case EEase.OutCirc:
                    return EaseOutCirc(t);
                case EEase.InOutCirc:
                    return EaseInOutCirc(t);
                case EEase.InBack:
                    return EaseInBack(t);
                case EEase.OutBack:
                    return EaseOutBack(t);
                case EEase.InOutBack:
                    return EaseInOutBack(t);
                case EEase.InElastic:
                    return EaseInElastic(t);
                case EEase.OutElastic:
                    return EaseOutElastic(t);
                case EEase.InOutElastic:
                    return EaseInOutElastic(t);
                case EEase.InBounce:
                    return EaseInBounce(t);
                case EEase.OutBounce:
                    return EaseOutBounce(t);
                case EEase.InOutBounce:
                    return EaseInOutBounce(t);
                case EEase.Linear:
                default:
                    return t;
            }
        }

        public static double EaseInSine(double t)
        {
            return 1 - Math.Cos((t * Math.PI) / 2);
        }

        public static double EaseOutSine(double t)
        {
            return Math.Sin((t * Math.PI) / 2);
        }

        public static double EaseInOutSine(double t)
        {
            return -(Math.Cos(Math.PI * t) - 1) / 2;
        }

        public static double EaseInQuad(double t)
        {
            return t * t;
        }

        public static double EaseOutQuad(double t)
        {
            t = 1 - t;
            return 1 - t * t;
        }

        public static double EaseInOutQuad(double t)
        {
            var xt = -2 * t + 2;
            return t < 0.5 ? 2 * t * t : 1 - (xt * xt) / 2;
        }

        public static double EaseInCubic(double t)
        {
            return t * t * t;
        }

        public static double EaseOutCubic(double t)
        {
            t = 1 - t;
            return 1 - t * t * t;
        }

        public static double EaseInOutCubic(double t)
        {
            var xt = -2 * t + 2;
            return t < 0.5 ? 4 * t * t * t : 1 - (xt * xt * xt) / 2;
        }

        public static double EaseInQuart(double t)
        {
            t *= t;
            return t * t;
        }

        public static double EaseOutQuart(double t)
        {
            t = 1 - t;
            t *= t;
            return 1 - (t * t);
        }

        public static double EaseInOutQuart(double t)
        {
            var tSqr = t * t;
            var xt = -2 * t + 2;
            xt *= xt;
            return t < 0.5 ? 8 * tSqr * tSqr : 1 - (xt * xt) / 2;
        }

        public static double EaseInQuint(double t)
        {
            var tSqr = t * t;
            return tSqr * tSqr * t;
        }

        public static double EaseOutQuint(double t)
        {
            var tm = 1 - t;
            var tSqr = tm * tm;
            return 1 - (tSqr * tSqr * tm);
        }

        public static double EaseInOutQuint(double t)
        {
            var tSqr = t * t;
            var xt = -2 * t + 2;
            var xtSqr = xt * xt;
            return t < 0.5 ? 16 * tSqr * tSqr * t : 1 - (xtSqr * xtSqr * xt) / 2;
        }

        public static double EaseInExpo(double t)
        {
            return t == 0 ? 0 : Math.Pow(2, 10 * t - 10);
        }

        public static double EaseOutExpo(double t)
        {
            return t == 1 ? 1 : 1 - Math.Pow(2, -10 * t);
        }

        public static double EaseInOutExpo(double t)
        {
            return t == 0 ?
                0 :
                t == 1 ?
                    1 :
                    t < 0.5 ?
                        Math.Pow(2, 20 * t - 10) / 2 :
                        (2 - Math.Pow(2, -20 * t + 10)) / 2;
        }

        public static double EaseInCirc(double t)
        {
            return 1 - Math.Sqrt(1 - t * t);
        }

        public static double EaseOutCirc(double t)
        {
            var xt = t - 1;
            return Math.Sqrt(1 - xt * xt);
        }

        public static double EaseInOutCirc(double t)
        {
            var dt = 2 * t;
            var xt = -2 * t + 2;
            return t < 0.5 ?
                (1 - Math.Sqrt(1 - dt * dt)) / 2 :
                (Math.Sqrt(1 - (xt * xt)) + 1) / 2;
        }

        public static double EaseInBack(double t)
        {
            var tSqr = t * t;
            return EaseC3 * tSqr * t - EaseC1 * tSqr;
        }

        public static double EaseOutBack(double t)
        {
            var tm = t - 1;
            var tmSqr = tm * tm;
            return 1 + EaseC3 * tm * tmSqr + EaseC1 * tmSqr;
        }

        public static double EaseInOutBack(double t)
        {
            var dt = 2 * t;
            var dtm = dt - 2;
            return t < 0.5 ?
                (dt * dt * ((EaseC2 + 1) * dt - EaseC2)) / 2 :
                (dtm * dtm * ((EaseC2 + 1) * dtm + EaseC2) + 2) / 2;
        }

        public static double EaseInElastic(double t)
        {
            return t == 0 ?
                0 :
                t == 1 ?
                    1 :
                    -Math.Pow(2, 10 * t - 10) * Math.Sin((t * 10 - 10.75) * EaseC4);
        }

        public static double EaseOutElastic(double t)
        {
            return t == 0 ?
                0 :
                t == 1 ?
                    1 :
                    Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * EaseC4) + 1;
        }

        public static double EaseInOutElastic(double t)
        {
            return t == 0 ?
                0 :
                t == 1 ?
                    1 :
                    t < 0.5 ?
                        -(Math.Pow(2, 20 * t - 10) * Math.Sin((20 * t - 11.125) * EaseC5)) / 2 :
                        (Math.Pow(2, -20 * t + 10) * Math.Sin((20 * t - 11.125) * EaseC5)) / 2 + 1;
        }

        public static double EaseInBounce(double t)
        {
            return 1 - EaseOutBounce(1 - t);
        }

        public static double EaseOutBounce(double t)
        {
            const double N1 = 7.5625;
            const double D1 = 2.75;

            if (t < 1 / D1)
            {
                return N1 * t * t;
            }
            else if (t < 2 / D1)
            {
                return N1 * (t -= 1.5 / D1) * t + 0.75;
            }
            else if (t < 2.5 / D1)
            {
                return N1 * (t -= 2.25 / D1) * t + 0.9375;
            }
            else
            {
                return N1 * (t -= 2.625 / D1) * t + 0.984375;
            }
        }

        public static double EaseInOutBounce(double t)
        {
            return t < 0.5 ?
                (1 - EaseOutBounce(1 - 2 * t)) / 2 :
                (1 + EaseOutBounce(2 * t - 1)) / 2;
        }
    }
}
