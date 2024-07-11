namespace SeaDrop
{
    public class Easing
    {
        public static double Ease(Counter counter, double min = 0, double max = 1,
            EEasing type = 0, EInOut inout = 0, double backrate = 4)
        {
            return Ease(counter, (int)counter.End, min, max, type, inout, backrate);
        }
        public static double Ease(Counter counter, int end, double min = 0, double max = 1,
            EEasing type = 0, EInOut inout = 0, double backrate = 4)
        {
            return EaseL(counter, 0, end, min, max, type, inout, backrate);
        }
        public static double EaseL(Counter counter, int start, int end, double min = 0, double max = 1,
            EEasing type = 0, EInOut inout = 0, double backrate = 4)
        {
            if (counter.Value < start) return min;
            if (counter.Value > end) return max;
            return Ease(counter.Value - start, end - start, min, max, type, inout, backrate);
        }
        public static double Ease(double t, double totaltime, double min = 0, double max = 1,
            EEasing type = 0, EInOut inout = 0, double backrate = 4)
        {
            if (min > max) return min - Ease(t, totaltime, 0, min - max, type, inout, backrate);
            int io = (int)inout;
            switch (type)
            {
                case EEasing.Linear:
                default:
                    return Linear(t, totaltime, min, max);
                case EEasing.Sine:
                    return Sine(t, totaltime, min, max, io);
                case EEasing.Quad:
                    return Quad(t, totaltime, min, max, io);
                case EEasing.Cubic:
                    return Cubic(t, totaltime, min, max, io);
                case EEasing.Quart:
                    return Quart(t, totaltime, min, max, io);
                case EEasing.Quint:
                    return Quint(t, totaltime, min, max, io);
                case EEasing.Exp:
                    return Exp(t, totaltime, min, max, io);
                case EEasing.Circ:
                    return Circ(t, totaltime, min, max, io);
                case EEasing.Back:
                    return Back(t, totaltime, min, max, io, backrate);
                case EEasing.Elastic:
                    return Elastic(t, totaltime, min, max, io);
                case EEasing.Bounce:
                    return Bounce(t, totaltime, min, max, io);
            }
        }

        // type 0:In 1:Out 2:InOut
        public static double Quad(double t, double totaltime, double min, double max, int type)
        {
            max -= min;
            if (max == 0) return min;
            switch (type)
            {
                case 0:
                    t /= totaltime;
                    return max * t * t + min;
                case 1:
                    t /= totaltime;
                    return -max * t * (t - 2) + min;
                case 2:
                default:
                    t /= totaltime / 2;
                    if (t < 1) return max / 2 * t * t + min;

                    t = t - 1;
                    return -max / 2 * (t * (t - 2) - 1) + min;
            }
        }

        public static double Cubic(double t, double totaltime, double min, double max, int type)
        {
            max -= min;
            if (max == 0) return min;
            switch (type)
            {
                case 0:
                    t /= totaltime;
                    return max * t * t * t + min;
                case 1:
                    t = t / totaltime - 1;
                    return max * (t * t * t + 1) + min;
                case 2:
                default:
                    t /= totaltime / 2;
                    if (t < 1) return max / 2 * t * t * t + min;

                    t = t - 2;
                    return max / 2 * (t * t * t + 2) + min;
            }
        }

        public static double Quart(double t, double totaltime, double min, double max, int type)
        {
            max -= min;
            if (max == 0) return min;
            switch (type)
            {
                case 0:
                    t /= totaltime;
                    return max * t * t * t * t + min;
                case 1:
                    t = t / totaltime - 1;
                    return -max * (t * t * t * t - 1) + min;
                case 2:
                default:
                    t /= totaltime / 2;
                    if (t < 1) return max / 2 * t * t * t * t + min;

                    t = t - 2;
                    return -max / 2 * (t * t * t * t - 2) + min;
            }
        }

        public static double Quint(double t, double totaltime, double min, double max, int type)
        {
            max -= min;
            if (max == 0) return min;
            switch (type)
            {
                case 0:
                    t /= totaltime;
                    return max * t * t * t * t * t + min;
                case 1:
                    t = t / totaltime - 1;
                    return max * (t * t * t * t * t + 1) + min;
                case 2:
                default:
                    t /= totaltime / 2;
                    if (t < 1) return max / 2 * t * t * t * t * t + min;

                    t = t - 2;
                    return max / 2 * (t * t * t * t * t + 2) + min;
            }
        }

        public static double Sine(double t, double totaltime, double min, double max, int type)
        {
            max -= min;
            if (max == 0) return min;
            switch (type)
            {
                case 0:
                    return -max * Math.Cos(t * (Math.PI * 90 / 180) / totaltime) + max + min;
                case 1:
                    return max * Math.Sin(t * (Math.PI * 90 / 180) / totaltime) + min;
                case 2:
                default:
                    return -max / 2 * (Math.Cos(t * Math.PI / totaltime) - 1) + min;
            }
        }

        public static double Exp(double t, double totaltime, double min, double max, int type)
        {
            switch (type)
            {
                case 0:
                    max -= min;
                    if (max == 0) return min;
                    return t == 0.0 ? min : max * Math.Pow(2, 10 * (t / totaltime - 1)) + min;
                case 1:
                    max -= min;
                    if (max == 0) return min;
                    return t == totaltime ? max + min : max * (-Math.Pow(2, -10 * t / totaltime) + 1) + min;
                case 2:
                default:
                    if (t == 0.0f) return min;
                    if (t == totaltime) return max;
                    max -= min;
                    if (max == 0) return min;
                    t /= totaltime / 2;

                    if (t < 1) return max / 2 * Math.Pow(2, 10 * (t - 1)) + min;

                    t = t - 1;
                    return max / 2 * (-Math.Pow(2, -10 * t) + 2) + min;
            }
        }

        public static double Circ(double t, double totaltime, double min, double max, int type)
        {
            max -= min;
            if (max == 0) return min;
            switch (type)
            {
                case 0:
                    t /= totaltime;
                    return -max * (Math.Sqrt(1 - t * t) - 1) + min;
                case 1:
                    t = t / totaltime - 1;
                    return max * Math.Sqrt(1 - t * t) + min;
                case 2:
                default:
                    t /= totaltime / 2;
                    if (t < 1) return -max / 2 * (Math.Sqrt(1 - t * t) - 1) + min;

                    t = t - 2;
                    return max / 2 * (Math.Sqrt(1 - t * t) + 1) + min;
            }
        }

        public static double Elastic(double t, double totaltime, double min, double max, int type)
        {
            max -= min;
            if (max == 0) return min;
            double s = 1.70158f;
            double p = totaltime * 0.3f;
            double a = max;
            switch (type)
            {
                case 0:
                    t /= totaltime;
                    if (t == 0) return min;
                    if (t == 1) return min + max;

                    if (a < Math.Abs(max))
                    {
                        a = max;
                        s = p / 4;
                    }
                    else
                    {
                        s = p / (2 * Math.PI) * Math.Asin(max / a);
                    }

                    t = t - 1;
                    return -(a * Math.Pow(2, 10 * t) * Math.Sin((t * totaltime - s) * (2 * Math.PI) / p)) + min;
                case 1:
                    t /= totaltime;
                    if (t == 0) return min;
                    if (t == 1) return min + max;

                    if (a < Math.Abs(max))
                    {
                        a = max;
                        s = p / 4;
                    }
                    else
                    {
                        s = p / (2 * Math.PI) * Math.Asin(max / a);
                    }

                    double n = a * Math.Pow(2, -10 * t) * Math.Sin((t * totaltime - s) * (2 * Math.PI) / p) + max + min;
                    return !double.IsNaN(n) ? n : 0;
                case 2:
                default:
                    t /= totaltime / 2;
                    p *= 1.5f;

                    if (t == 0) return min;
                    if (t == 2) return min + max;

                    if (a < Math.Abs(max))
                    {
                        a = max;
                        s = p / 4;
                    }
                    else
                    {
                        s = p / (2 * Math.PI) * Math.Asin(max / a);
                    }

                    if (t < 1)
                    {
                        return -0.5f * (a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * totaltime - s) * (2 * Math.PI) / p)) + min;
                    }

                    t = t - 1;
                    return a * Math.Pow(2, -10 * t) * Math.Sin((t * totaltime - s) * (2 * Math.PI) / p) * 0.5f + max + min;
            }
        }

        public static double Back(double t, double totaltime, double min, double max, int type, double s = 1.0)
        {
            max -= min;
            if (max == 0) return min;
            switch (type)
            {
                case 0:
                    t /= totaltime;
                    double m = max * t * t * ((s + 1) * t - s) + min;
                    return max * t * t * ((s + 1) * t - s) + min;
                case 1:
                    t = t / totaltime - 1;
                    return max * (t * t * ((s + 1) * t + s) + 1) + min;
                case 2:
                default:
                    s *= 1.525f;
                    t /= totaltime / 2;
                    if (t < 1) return max / 2 * (t * t * ((s + 1) * t - s)) + min;

                    t = t - 2;
                    return max / 2 * (t * t * ((s + 1) * t + s) + 2) + min;
            }
        }

        public static double Bounce(double t, double totaltime, double min, double max, int type)
        {
            switch (type)
            {
                case 0:
                    return BounceIn(t, totaltime, min, max);
                case 1:
                    return BounceOut(t, totaltime, min, max);
                case 2:
                default:
                    if (t < totaltime / 2)
                    {
                        return BounceIn(t * 2, totaltime, 0, max - min) * 0.5f + min;
                    }
                    else
                    {
                        return BounceOut(t * 2 - totaltime, totaltime, 0, max - min) * 0.5f + min + (max - min) * 0.5f;
                    }
            }
        }
        public static double BounceIn(double t, double totaltime, double min, double max)
        {
            max -= min;
            if (max == 0) return min;
            return max - BounceOut(totaltime - t, totaltime, 0, max) + min;
        }

        public static double BounceOut(double t, double totaltime, double min, double max)
        {
            max -= min;
            if (max == 0) return min;
            t /= totaltime;

            if (t < 1.0f / 2.75f)
            {
                return max * (7.5625f * t * t) + min;
            }
            else if (t < 2.0f / 2.75f)
            {
                t -= 1.5f / 2.75f;
                return max * (7.5625f * t * t + 0.75f) + min;
            }
            else if (t < 2.5f / 2.75f)
            {
                t -= 2.25f / 2.75f;
                return max * (7.5625f * t * t + 0.9375f) + min;
            }
            else
            {
                t -= 2.625f / 2.75f;
                return max * (7.5625f * t * t + 0.984375f) + min;
            }
        }

        public static double Linear(double t, double totaltime, double min, double max)
        {
            return (max - min) * t / totaltime + min;
        }
    }

    public enum EEasing
    {
        Linear,
        Sine,
        Quad,
        Cubic,
        Quart,
        Quint,
        Exp,
        Circ,
        Back,
        Elastic,
        Bounce,
    }

    public enum EInOut
    {
        In,
        Out,
        InOut,
    }
}