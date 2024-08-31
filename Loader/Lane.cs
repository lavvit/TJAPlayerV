using SeaDrop;

namespace Loader
{
    public class Lane
    {
        public string Path = "";
        public TJA TJA = new("");
        public int Course = 3;
        public bool IsAuto = true;
        public int StartMeasure = 0;
        public double StartTime = -1000;
        public static Counter Timer = new(-1000, int.MaxValue);
        public int Width = DXLib.Width, Height = DXLib.Height;
        public int[] NoteSize = [32, 44], NoteFrame = [6, 8], NoteColor = [0xe7372a, 0x4ecdbe, 0xecb907, 0xff5000, 0xcc245e];

        public Lane() { }
        public Lane(string path, int course = 3)
        {
            if (string.IsNullOrEmpty(path)) return;
            Path = path;
            Course = course;
            TJA = new(Path);
            TJA.SetLen();
        }

        #region Draw
        public void Draw(double x, double y)
        {
            Timer.Tick();
            DrawBack(x, y);

            DrawCenter(x, y);
            if (TJA == null || TJA.Courses[Course] == null) return;
            for (int i = TJA.Courses[Course].Lanes[0].Length - 1; i >= 0; i--)
            {
                var bar = TJA.Courses[Course].Lanes[0][i];
                DrawBar(x + NoteX(bar), y);
                for (int j = bar.Chips.Count - 1; j >= 0; j--)
                {
                    var chip = bar.Chips[j];
                    if (!chip.Hit && chip.Type > ENote.None)
                        DrawChip(x, y, chip);
                }
            }
        }

        public void DrawChip(double x, double y, Chip note)
        {
            int[] defsize = NoteSize;
            int[] color = NoteColor;
            int[] edge = NoteFrame;

            int size = taiko.Tx.Notes.Enable ? taiko.Tx.Notes.Height / 4 : Height;
            double defx = x + size / 2, defy = y + size / 2;
            double notex = defx + NoteX(note);
            double notey = defy;
            double endx = note.LongEnd != null ? defx + NoteX(note.LongEnd) : notex;
            double endy = notey;
            if (notex > DXLib.Width + size || notex < -size)
            {
                if (note.LongEnd != null)
                {
                    if ((notex > DXLib.Width && endx > DXLib.Width) || (notex < -size && endx < -size))
                        return;
                }
                else return;
            }

            switch (note.Type)
            {
                case ENote.Don:
                case ENote.Ka:
                case ENote.DON:
                case ENote.KA:
                    DrawNote(notex, notey, note.Type);
                    break;
                case ENote.Roll:
                case ENote.ROLL:
                    if (note.LongEnd != null) DrawLongNote(notex, notey, endx, endy, note.Type);
                    else DrawNote(notex, notey, note.Type);
                    break;
                case ENote.Balloon:
                case ENote.Potato:
                    if (note.LongEnd != null)
                    {
                        if (Timer.Value >= note.LongEnd.Time)
                        {
                            notex = endx;
                            notey = endy;
                        }
                        else if (Timer.Value >= note.Time)
                        {
                            notex = defx;
                            notey = defy;
                        }
                    }
                    DrawBalloon(notex, notey, note.Type);
                    break;
            }
        }

        public virtual void DrawBack(double x, double y)
        {
            if (taiko.Tx.Lane.Enable)
            {
                taiko.Tx.Lane.Draw(x, y);
            }
            else
            {
                Drawing.Box(x, y, Width, Height, 0x202020);
            }
        }
        public virtual void DrawCenter(double x, double y)
        {
            int size = taiko.Tx.Notes.Enable ? taiko.Tx.Notes.Height / 4 : Height;
            double defx = x + size / 2, defy = y + size / 2;
            if (taiko.Tx.Notes.Enable)
            {
                taiko.Tx.Notes.SetRectangle(0, 0, size, size);
                taiko.Tx.Notes.ReferencePoint = ReferencePoint.Center;
                taiko.Tx.Notes.Draw(defx, defy);
            }
            else
            {
                Drawing.Circle(defx, defy, NoteSize[0], 0xc0c0c0, false, 2);
                Drawing.Circle(defx, defy, NoteSize[1], 0xc0c0c0, false, 2);
            }
        }
        public virtual void DrawBar(double x, double y)
        {
            int size = taiko.Tx.Notes.Enable ? taiko.Tx.Notes.Height / 4 : Height;
            double defx = x + size / 2, defy = y + size / 2;
            if (taiko.Tx.Bar.Enable)
            {
                taiko.Tx.Bar.ReferencePoint = ReferencePoint.Center;
                taiko.Tx.Bar.Draw(defx, defy);
            }
            else
            {
                Drawing.Box(defx - 1, y, 3, size);
            }
        }
        public virtual void DrawNote(double x, double y, ENote type = 0)
        {
            int size = taiko.Tx.Notes.Enable ? taiko.Tx.Notes.Height / 4 : Height;
            int rec = GetRec(type);
            if (taiko.Tx.Notes.Enable)
            {
                taiko.Tx.Notes.SetRectangle(size * rec, 0, size, size);
                taiko.Tx.Notes.ReferencePoint = ReferencePoint.Center;
                taiko.Tx.Notes.Draw(x, y);
            }
            else
            {
                int sz = NoteSize[GetSize(type)];
                int ed = NoteFrame[GetSize(type)];
                int col = NoteColor[GetColor(type)];
                Drawing.Circle(x, y, sz);
                Drawing.Circle(x, y, sz, 0, false, 2);
                Drawing.Circle(x, y, sz - ed, col);
            }
        }
        public virtual void DrawLongNote(double x, double y, double endx, double endy, ENote type = 0)
        {
            int size = taiko.Tx.Notes.Enable ? taiko.Tx.Notes.Height / 4 : Height;
            int rec = GetRec(type);
            if (taiko.Tx.Notes.Enable)
            {
                taiko.Tx.Notes.SetRectangle(size * (rec + 1), 0, size, size);
                taiko.Tx.Notes.XYScale = ((endx - x) / size, 1.0);
                taiko.Tx.Notes.ReferencePoint = ReferencePoint.CenterLeft;
                taiko.Tx.Notes.Draw(x, y);
                taiko.Tx.Notes.XYScale = null;

                taiko.Tx.Notes.SetRectangle(size * (rec + 2), 0, size, size);
                taiko.Tx.Notes.ReferencePoint = ReferencePoint.CenterLeft;
                taiko.Tx.Notes.Draw(endx, endy);
            }
            else
            {
                int sz = NoteSize[GetSize(type)];
                int ed = NoteFrame[GetSize(type)];
                int col = NoteColor[GetColor(type)];
                Drawing.Circle(endx, endy, sz);
                Drawing.Circle(endx, endy, sz, 0, false, 2);
                Drawing.Circle(endx, endy, sz - ed, col);
                double x1 = x;
                double y1 = y - sz;
                double x2 = endx;
                double y2 = endy + sz;
                Drawing.BoxZ(x1, y1 - 1, x2, y2 + 2, 0);
                Drawing.BoxZ(x1, y1 + 1, x2, y2 - 0);
                Drawing.BoxZ(x1, y1 + ed, x2, y2 - ed + 1, col);
            }
            DrawNote(x, y, type);
        }
        public virtual void DrawBalloon(double x, double y, ENote type = 0)
        {
            int size = taiko.Tx.Notes.Enable ? taiko.Tx.Notes.Height / 4 : Height;
            int rec = GetRec(type);
            if (taiko.Tx.Notes.Enable)
            {
                taiko.Tx.Notes.SetRectangle(size * rec, 0, size * 2, size);
                taiko.Tx.Notes.SetCenter(size / 2.0, size / 2.0);
                taiko.Tx.Notes.ReferencePoint = ReferencePoint.Center;
                taiko.Tx.Notes.Draw(x, y);
                taiko.Tx.Notes.Center = null;
            }
            else
            {
                int sz = NoteSize[GetSize(type)];
                int ed = NoteFrame[GetSize(type)];
                int col = NoteColor[GetColor(type)];
                Drawing.Circle(x, y, sz);
                Drawing.Circle(x, y, sz, 0, false, 2);
                Drawing.Circle(x, y, sz - ed, col);
            }
        }
        private static int GetRec(ENote type)
        {
            switch (type)
            {
                default:
                case ENote.Don:
                case ENote.Ka:
                case ENote.DON:
                case ENote.KA:
                case ENote.Roll:
                    return (int)type;
                case ENote.ROLL:
                    return (int)type + 2;
                case ENote.Balloon:
                    return (int)type + 4;
                case ENote.Potato:
                    return (int)type + 5;
            }
        }
        public static int GetSize(ENote type)
        {
            switch (type)
            {
                default:
                case ENote.Don:
                case ENote.Ka:
                case ENote.Roll:
                case ENote.Balloon:
                    return 0;
                case ENote.DON:
                case ENote.KA:
                case ENote.ROLL:
                case ENote.Potato:
                    return 1;
            }
        }
        private static int GetColor(ENote type)
        {
            switch (type)
            {
                default:
                case ENote.Don:
                case ENote.DON:
                    return 0;
                case ENote.Ka:
                case ENote.KA:
                    return 1;
                case ENote.Roll:
                case ENote.ROLL:
                    return 2;
                case ENote.Balloon:
                    return 3;
                case ENote.Potato:
                    return 4;
            }
        }

        public double NoteX(double ctime, double bpm, double scroll)
        {
            double time = ctime - Timer.Value;
            return time / 1000.0 * (bpm / 240.0) * DXLib.Width * scroll;
        }
        public double NoteX(Chip chip)
        {
            return NoteX(chip.Time, chip.BPM, chip.Scroll);
        }
        public double NoteX(Bar bar)
        {
            return NoteX(bar.Time, bar.BPM, bar.Scroll);
        }
        #endregion

        #region Process

        public virtual void Auto()
        {
            if (TJA.Courses[Course] == null) return;
            foreach (var bar in TJA.Courses[Course].Lanes[0])
            {
                foreach (var chip in bar.Chips)
                {
                    if (Timer.Value >= chip.Time - 8 && !chip.Hit)
                    {
                        switch (chip.Type)
                        {
                            case ENote.Don:
                                taiko.SFx.Don.Play();
                                chip.Hit = true;
                                break;
                            case ENote.Ka:
                                taiko.SFx.Ka.Play();
                                chip.Hit = true;
                                break;
                            case ENote.DON:
                                taiko.SFx.Don.Play();
                                taiko.SFx.Don.Play();
                                chip.Hit = true;
                                break;
                            case ENote.KA:
                                taiko.SFx.Ka.Play();
                                taiko.SFx.Ka.Play();
                                chip.Hit = true;
                                break;
                        }
                    }
                }
            }
        }
        public virtual void Hit(bool[] hits, bool auto = false)
        {
            if (TJA == null || TJA.Courses[Course] == null) return;
            //ld,rd,lk,rk
            bool isdon = hits[0] || hits[1];
            bool iska = hits[2] || hits[3];

            if (isdon)
                taiko.SFx.Don.Play();
            if (iska)
                taiko.SFx.Ka.Play();

            if (!isdon && !iska) return;

            int[] judge = [16, 25, 75, 108];
            var chips = NowChip(judge[3]);
            bool hitted = false;
            foreach (var loc in chips)
            {
                var chip = TJA.Courses[Course].Lanes[0][loc.Lane].Chips[loc.Pos];
                switch (chip.Type)
                {
                    case ENote.Don:
                    case ENote.DON:
                        if (isdon && !hitted)
                        {
                            chip.Hit = true;
                            chip.HitTime = Timer.Value;
                            hitted = true;
                        }
                        break;
                    case ENote.Ka:
                    case ENote.KA:
                        if (iska && !hitted)
                        {
                            chip.Hit = true;
                            chip.HitTime = Timer.Value;
                            hitted = true;
                        }
                        break;
                }
            }
        }

        public (int Lane, int Pos)[] NowChip(int range = 120)
        {
            if (TJA == null || TJA.Courses[Course] == null) return [];
            List<(int, int)> chips = [];
            foreach (var bar in TJA.Courses[Course].Lanes[0])
            {
                foreach (var chip in bar.Chips)
                {
                    if (!chip.Hit)
                    {
                        bool add = Math.Abs(Timer.Value - chip.Time) <= range;
                        if (chip.LongEnd != null)
                            add &= Math.Abs(Timer.Value - chip.LongEnd.Time) <= range;
                        if (add)
                            chips.Add((chip.Bar - 1, chip.Position - 1));
                    }
                }
            }
            return chips.ToArray();
        }

        #endregion


        public int NowMeasure()
        {
            if (TJA == null || TJA.Courses[Course] == null) return 0;
            var course = TJA.Courses[Course];
            int n = 0;
            for (int i = 0; i < course.Lanes[0].Length; i++)
            {
                var bar = course.Lanes[0][i];
                if (Timer.Value < bar.Time) return n;
                n++;
            }
            return n;
        }
        public string NowMeasureText()
        {
            if (TJA == null || TJA.Courses[Course] == null) return "";
            int num = NowMeasure();
            if (num == 0) return "";
            return TJA.Courses[Course].Lanes[0][NowMeasure() - 1].ToString();
        }
        public int AllMeasure()
        {
            if (TJA == null || TJA.Courses[Course] == null) return 0;
            var course = TJA.Courses[Course];
            return course.Lanes[0].Length;
        }
    }
}
