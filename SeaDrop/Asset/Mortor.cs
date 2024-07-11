namespace SeaDrop
{
    public class Mortor
    {
        private double baseval = 0;
        public double? Min = null, Max = null;
        public bool Loop = true;
        public Counter Counter = new Counter(0, 1000);
        public double? stopvalue = null;
        public List<MAction> Actions = new List<MAction>();


        public void Update()
        {
            Counter.Tick();
            bool stop = false;
            if (stopvalue.HasValue)
            {
                bool minus = Speed < 0 && Value <= stopvalue.Value;
                bool plus = Speed > 0 && Value >= stopvalue.Value;
                if (minus || plus) stop = true;
            }
            if (stop)
            {
                Counter.Stop();
                if (stopvalue.HasValue)
                {
                    Value = stopvalue.Value;
                    if (Max.HasValue && Value > Max)
                        Value = Loop && Min.HasValue ? Min.Value : Max.Value;
                    if (Min.HasValue && Value < Min)
                        Value = Min.Value;
                }
                stopvalue = null;
            }
            else
            {
                if (Counter.Interval > 0 && Counter.Value == Counter.End)
                {
                    baseval++;
                    Counter.Reset();
                    Counter.Start();
                }
                if (Counter.Interval < 0 && Counter.Value == Counter.Begin)
                {
                    baseval--;
                    Counter.Value = Counter.End;
                    Counter.Start();
                }
                if (Counter.Interval > 0 && Max.HasValue && Value >= Max)
                {
                    if (stopvalue.HasValue && stopvalue >= Max)
                    {
                        Value = stopvalue.Value;
                        if (Max.HasValue && Value > Max)
                            Value = Loop && Min.HasValue ? Min.Value : Max.Value;
                        if (Min.HasValue && Value < Min)
                            Value = Min.Value;
                        stopvalue = null;
                        Counter.Stop();
                    }
                    if (Min.HasValue)
                    {
                        Value = Min.Value;
                    }
                    else
                    {
                        Counter.Stop();
                    }
                }
                if (Counter.Interval < 0 && Min.HasValue && Value <= Min)
                {
                    if (stopvalue.HasValue && stopvalue <= Min)
                    {
                        Value = stopvalue.Value;
                        if (Max.HasValue && Value > Max)
                            Value = Loop && Min.HasValue ? Min.Value : Max.Value;
                        if (Min.HasValue && Value < Min)
                            Value = Min.Value;
                        stopvalue = null;
                        Counter.Stop();
                    }
                    if (Max.HasValue)
                    {
                        Value = Max.Value;
                    }
                    else
                    {
                        Counter.Stop();
                    }
                }
            }
            if (Actions.Count > 0)
            {
                var action = Actions[0];
                action.Timer.Tick();
                double start = action.Start == 0 ? 0 : 1000.0 / action.Start;
                double end = action.End == 0 ? 0 : 1000.0 / action.End;
                double value = Easing.Ease(action.Timer, start, end);
                int val = value != 0 ? (int)(1.0 / value * 1000) : 0;
                Counter.ChangeInterval(val != 0 ? val : int.MaxValue);

                if (action.Target.HasValue)
                {
                    Value = Easing.Ease(action.Timer, action.Begin, action.Target.Value, action.Easing, action.Inout);
                }
                if (action.Timer.Value == action.Timer.End)
                {
                    Counter.ChangeInterval((int)action.End != 0 ? (int)action.End : int.MaxValue);
                    Actions.RemoveAt(0);
                    if (Counter.Interval == int.MaxValue)
                    {
                        Counter.Stop();
                    }
                }
                else if (action.Timer.State == 0)
                {
                    action.Begin = Value;
                    if (action.Target.HasValue && action.End != 0)
                    {
                        action.End = value - Easing.Ease(action.Timer.End - 1, action.Timer.End, action.Begin, action.Target.Value, action.Easing, action.Inout);
                        action.End *= action.Timer.End;
                    }
                    action.Timer.Start();
                    if (Counter.State == 0) Counter.Start();
                }
            }
        }

        public void Start(double msec = 1000)
        {
            if (Max.HasValue && Min.HasValue)
            {
                double len = Max.Value - Min.Value;
                double speed = msec / len;
                Counter.ChangeInterval((int)speed);
                Counter.Start();
            }
            else
            {
                Counter.ChangeInterval((int)msec);
                Counter.Start();
            }
        }
        public void Start(int length, double speed)
        {
            double start = Actions.Count > 0 ? Actions[Actions.Count - 1].End : Speed;
            double end = speed;
            Actions.Add(new MAction()
            {
                Timer = new Counter(0, length),
                Start = start,
                End = end
            });
        }

        public void Start(int length, double value, bool stop, EEasing easing = EEasing.Sine)
        {
            double start = Actions.Count > 0 ? Actions[Actions.Count - 1].End : Speed;
            double end = 0;
            double begin = Value;
            EInOut inout;
            if (start == 0)
            {
                inout = stop ? EInOut.InOut : EInOut.In;
                if (!stop)
                {
                    end = value - Easing.Ease(length - 1, length, begin, value, easing, inout);
                    end *= length;
                }
            }
            else
            {
                inout = stop ? EInOut.Out : EInOut.InOut;
            }
            Actions.Add(new MAction()
            {
                Timer = new Counter(0, length),
                Start = start,
                End = end,

                Begin = begin,
                Target = value,
                Easing = easing,
                Inout = inout,
            });
        }

        public void Stop()
        {
            Counter.Stop();
        }
        public void Stop(double value)
        {
            if (Counter.State > 0) stopvalue = value;
        }

        public void Draw(double x, double y)
        {
            Drawing.Circle(x, y, 80, 0xffffff);
            Drawing.Circle(x, y, 80, 0x0000ff, false);
            double rad = 2 * Math.PI * Value / (Max.HasValue ? (double)Max.Value : 1.0);
            var sin = 80.0 * Math.Sin(rad);
            var cos = 80.0 * -Math.Cos(rad);
            Drawing.Line(x, y, sin, cos, 0xff0000, 4);

            Drawing.Text(x + 80, y - 60, $"Value:{Value}", 0x00ff80);
            Drawing.Text(x + 80, y - 40, $"Speed:{Speed}", 0x00ff80);
            if (stopvalue.HasValue) Drawing.Text(x + 80, y - 20, $"Stop:{stopvalue.Value}", 0x00ff80);
            for (int i = 0; i < Actions.Count; i++)
            {
                var action = Actions[i];
                Drawing.Text(x + 80, y + 20 * i, $"{action}", 0x00ff80);
            }
        }

        public double Value
        {
            get
            {
                return Math.Round(baseval + Counter.Value / 1000.0, 4);
            }
            set
            {
                baseval = (int)value;
                double bas = (value - baseval) * 1000;
                double val = Counter.Interval < 0 ? Counter.End - bas : bas;
                if (Counter.Interval < 0) baseval--;
                Counter.Value = (int)val;
            }
        }

        public double Speed
        {
            get
            {
                double speed = Counter.Interval;
                return Counter.State > 0 && speed < int.MaxValue ? speed : 0;
            }
        }
    }

    public class MAction
    {
        public Counter Timer = new Counter();
        public double Start;
        public double End;

        public double Begin;
        public double? Target;
        public EEasing Easing;
        public EInOut Inout;

        public override string ToString()
        {
            string str = $"{Timer.Value}/{Timer.End} {(int)Start}->{(int)End}";
            if (Target.HasValue)
                str += $" ( {Begin}->{Target.Value} {Easing}-{Inout} )";
            return str;
        }
    }
}