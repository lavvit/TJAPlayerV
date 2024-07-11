using static DxLibDLL.DX;

namespace SeaDrop
{
    /// <summary>
    /// FPSを計測するクラス。
    /// </summary>
    public class FPS
    {
        /// <summary>
        /// FPSを更新します。
        /// </summary>
        public static void Update(bool process = false)
        {
            var fps = process ? ProcessData : Data;
            DateTime time = DateTime.Now;
            TimeSpan span = time - fps.Time;
            double value = span.TotalMilliseconds > 0.0 ? 1000.0 / span.TotalMilliseconds : 10000000;
            if (process) Process = value;
            else Value = value;

            //if (Value > 0) AverageList.Add(Value);
            //if (AverageList.Count > ListMax) AverageList.RemoveAt(0);
            fps.Time = time;
            int rl = GetNowCount();
            fps.Total += span.TotalMilliseconds;
            fps.Count++;
            if (rl - fps.ReloadTime > 0)// && fpsc % 10 == 0
            {
                double average = span.TotalMilliseconds > 0.0 ? 1000.0 / span.TotalMilliseconds : 10000000;
                if (process) AverageProcess = average; else AverageValue = average;
                //AverageValue = AverageList.Count > 0 ? AverageList.Average() : (int)Value;
                //AverageList.Clear();
                fps.Total = 0;
                fps.Count = 0;
                fps.ReloadTime = rl + fps.ReloadInterval;
            }
        }

        /// <summary>
        /// 現在の描画FPS。
        /// </summary>
        public static double Value { get; private set; }
        /// <summary>
        /// 現在の処理FPS。
        /// </summary>
        public static double Process { get; private set; }
        /// <summary>
        /// 現在の平均描画FPS。
        /// </summary>
        public static double AverageValue { get; private set; }
        /// <summary>
        /// 現在の平均処理FPS。
        /// </summary>
        public static double AverageProcess { get; private set; }

        private static FPSCount Data = new FPSCount(), ProcessData = new FPSCount();
    }

    public class FPSCount
    {
        public double Total = 0;
        public int Count = 0;
        public int ListMax = 100, ReloadTime, ReloadInterval = 200;
        public List<double> AverageList = new List<double>();
        public DateTime Time = DateTime.Now;
    }
}
