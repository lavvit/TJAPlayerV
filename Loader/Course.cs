using SeaDrop;

namespace Loader
{
    public class Course
    {
        public Header Head;
        public List<string> Texts = [];
        public Bar[][] Lanes = [];
        public List<(int line, int pos)>[] LongList = [[]];
        //dong
        public Course(Header header, List<string> text)
        {
            Head = header;
            Texts = text;
            Lanes = [ new Bar[BarCount()] ];

            Read();
            Set();
        }

        public int BarCount()
        {
            int n = 0;
            foreach (string line in Texts)
            {
                if (!line.StartsWith("#"))
                {
                    foreach (char note in line)
                    {
                        if (note == ',')
                        {
                            n++;
                        }
                    }
                }
            }
            return n;
        }

        public void Read()
        {
            var lane = Lanes[0];
            int n = 0, b = 0;
            LongList = [[]];
            List<(int line, int pos)> longs = [];
            foreach (string line in Texts)
            {
                if (!line.StartsWith("#"))
                {
                    foreach (char note in line)
                    {
                        if (note >= '0' && note <= '9')
                        {
                            n++;
                            Chip chip = new Chip()
                            {
                                Type = (ENote)int.Parse(note.ToString()),
                                Position = n,
                            };
                            if (lane[b] == null) lane[b] = new Bar();
                            lane[b].Chips.Add(chip);
                            if (note == '8')
                            {
                                if (longs.Count > 0)
                                {
                                    var end = longs[longs.Count - 1];
                                    lane[end.line].Chips[end.pos - 1].LongEnd = new Chip()
                                    {
                                        Type = lane[end.line].Chips[end.pos - 1].Type,
                                        Position = n,
                                        Bar = b + 1,
                                    };
                                }
                            }
                            else
                            {
                                if (note >= '5')
                                {
                                    longs.Add((b, n));
                                    LongList[0].Add((b, n));
                                }
                            }

                        }
                        if (note == ',')
                        {
                            if (lane[b] == null)
                            {
                                lane[b] = new Bar();
                            }
                            lane[b].NoteCount = n;
                            if (lane[b].NoteCount == 0)
                            {
                                n++;
                                Chip chip = new Chip()
                                {
                                    Type = ENote.None,
                                    Position = n
                                };
                                lane[b].Chips.Add(chip);
                                lane[b].NoteCount = n;
                            }
                            n = 0;
                            b++;
                        }
                    }
                }
                else
                {
                    Command command = new Command()
                    {
                        Position = n + 1,
                        Name = line,
                    };
                    if (lane[b] == null) lane[b] = new Bar();
                    lane[b].Commands.Add(command);
                }
            }
        }

        public void Set()
        {
            var lane = Lanes[0];
            double t = -Head.Offset;

            double bpm = Head.BPM;
            double scroll = 1;
            double measure = 1;
            double measuremom = 1;

            for (int i = 0; i < lane.Length; i++)
            {
                var bar = lane[i];
                bar.Time = t;
                bar.BPM = bpm;
                bar.Measure = measure;
                bar.Scroll = scroll;

                for (int j = 0; j < bar.Chips.Count; j++)
                {
                    #region Command
                    foreach (var comm in bar.Commands)
                    {
                        if (comm.Position == j + 1)
                        {
                            string[] split = comm.Name.Substring(1).Split(' ');
                            var spls = split.ToList();
                            spls.RemoveAt(0);
                            string value = string.Join(" ", spls).Trim();
                            switch (split[0].ToLower())
                            {
                                case "scroll":
                                    if (float.TryParse(value, out float fval)) scroll = fval;
                                    else double.TryParse(value, out scroll);
                                    if (j == 0) bar.Scroll = scroll;
                                    break;
                                case "bpmchange":
                                    if (float.TryParse(value, out  fval)) bpm = fval;
                                    else double.TryParse(value, out bpm);
                                    if (j == 0) bar.BPM = bpm;
                                    break;
                                case "measure":
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        if (!value.Contains('/')) value += $"/{measuremom}";
                                        string[] maj = value.Split('/');
                                        measure = double.Parse(maj[1]) / double.Parse(maj[0]);
                                        double.TryParse(maj[1], out measuremom);
                                    }
                                    else measure = 1;
                                    bar.Measure = measure;
                                    break;
                            }
                        }
                    }
                    #endregion
                    var chip = bar.Chips[j];
                    chip.BPM = bpm;
                    chip.Scroll = scroll;
                }
                double length = GetTime(bar, bar.Chips.Count);
                for (int j = 0; j < bar.Chips.Count; j++)
                {
                    bar.Chips[j].Time = t + GetTime(bar, j);
                }
                if (bar.Measure * bar.BPM > 0)t += length;
            }
            var longs = LongList[0];
            for (int i = 0; i < longs.Count; i++)
            {
                var longend = longs[i];
                var chip = lane[longend.line].Chips[longend.pos - 1].LongEnd;
                if (chip != null)
                {
                    var lbar = lane[chip.Bar - 1];
                    var endchip = lane[chip.Bar - 1].Chips[chip.Position - 1];
                    chip.Time = endchip.Time;
                    chip.BPM = endchip.BPM;
                    chip.Scroll = endchip.Scroll;
                }
            }
        }

        public double GetTime(Bar bar, int current)
        {
            double t = 0;
            double measurelength = 240000.0 / bar.BPM / bar.Measure;
            if (bar.Chips.Count == 0) return measurelength;
            for (int i = 0; i < current; i++)
            {
                #region Command
                foreach (var comm in bar.Commands)
                {
                    if (comm.Position == i + 1)
                    {
                        string[] split = comm.Name.Substring(1).Split(' ');
                        var spls = split.ToList();
                        spls.RemoveAt(0);
                        string value = string.Join(" ", spls).Trim();
                        switch (split[0].ToLower())
                        {
                            case "delay":
                                if (float.TryParse(value, out float fdel)) t += fdel * 1000.0;
                                else if (double.TryParse(value, out double ddel)) t += ddel * 1000.0;
                                break;
                        }
                    }
                }
                #endregion
                var chip = bar.Chips[i];
                t += 240000.0 / bar.BPM / bar.Measure / bar.NoteCount;
            }
            return t;
        }

        public static int GetCourse(string str)
        {
            var ret = int.TryParse(str, out int nCourse);
            if (ret) return nCourse;
            switch (str.ToLower())
            {
                case "easy": return 0;
                case "normal": return 1;
                case "hard": return 2;
                case "oni": return 3;
                case "edit": return 4;
                case "tower": return 5;
                case "dan": return 6;
                default: return 3;
            }
        }

        public static int GetCourseColor(ECourse course, bool enable = true)
        {
            int[] col = [ 0xff4000, 0x80ff40, 0x00c0ff, 0xff00c0, 0xc000ff, 0x804000, 0x4000c0 ];
            if (!enable) col = [ 0x802000, 0x408020, 0x006080, 0x800060, 0x600080, 0x402000, 0x200080 ];
            switch (course)
            {
                case ECourse.Easy:
                    return col[0];
                case ECourse.Normal:
                    return col[1];
                case ECourse.Hard:
                    return col[2];
                case ECourse.Oni:
                    return col[3];
                case ECourse.Edit:
                    return col[4];
                case ECourse.Tower:
                    return col[5];
                case ECourse.Dan:
                    return col[6];
                default:
                    return 0xffffff;
            }
        }
    }

    public enum ECourse
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
        Oni = 3,
        Edit = 4,
        Tower = 5,
        Dan = 6,
    }
}