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

        public Lane() { }
        public Lane(string path, int course = 3)
        {
            if (string.IsNullOrEmpty(path)) return;
            Path = path;
            TJA = new(Path);
            TJA.SetLen();
        }

        #region Draw
        public void Draw(double x, double y)
        {
            Timer.Tick();
            if (taiko.Tx.Lane.Enable) taiko.Tx.Lane.Draw(x, y);
            else Drawing.Blackout(1, 0x303030);

            int size = taiko.Tx.Notes.Enable ? taiko.Tx.Notes.Height / 4 : DXLib.Height;
            double defx = x + size / 2, defy = y + size / 2;
            if (taiko.Tx.Notes.Enable)
            {
                taiko.Tx.Notes.SetRectangle(0, 0, size, size);
                taiko.Tx.Notes.ReferencePoint = ReferencePoint.Center;
                taiko.Tx.Notes.Draw(defx, defy);
            }
            else
            {
                Drawing.Circle(defx, defy, 32, 0xc0c0c0, false, 2);
                Drawing.Circle(defx, defy, 44, 0xc0c0c0, false, 2);
            }
            if (TJA == null || TJA.Courses[Course] == null) return;
            for (int i = TJA.Courses[Course].Lanes[0].Length - 1; i >= 0; i--)
            {
                var bar = TJA.Courses[Course].Lanes[0][i];
                if (taiko.Tx.Bar.Enable)
                {
                    taiko.Tx.Bar.ReferencePoint = ReferencePoint.Center;
                    taiko.Tx.Bar.Draw(defx + NoteX(bar), defy);
                }
                else
                {
                    Drawing.Box(defx + NoteX(bar) - 1, defy - size / 2, 3, size);
                }
                for (int j = bar.Chips.Count - 1; j >= 0; j--)
                {
                    var chip = bar.Chips[j];
                    if (!chip.Hit && chip.Type > ENote.None)
                        DrawNote(defx, defy, chip);
                }
            }
        }

        public void DrawNote(double x, double y, Chip note)
        {
            int size = taiko.Tx.Notes.Enable ? taiko.Tx.Notes.Height / 4 : DXLib.Height;
            int[] defsize = [32, 44];
            int[] color = [0xe7372a, 0x4ecdbe, 0xecb907, 0xff5000, 0xcc245e];

            double notex = x + NoteX(note);
            double notey = y;
            if (notex > DXLib.Width + size || notex < -size)
            {
                if (note.LongEnd != null)
                {
                    double endx = x + NoteX(note.LongEnd);
                    double endy = y;
                    if ((notex > DXLib.Width + size && endx > DXLib.Width + size) || (notey < -size && endy < -size))
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
                    if (taiko.Tx.Notes.Enable)
                    {
                        taiko.Tx.Notes.SetRectangle(size * (int)note.Type, 0, size, size);
                        taiko.Tx.Notes.ReferencePoint = ReferencePoint.Center;
                        taiko.Tx.Notes.Draw(notex, notey);
                    }
                    else
                    {
                        int ed = note.Type >= ENote.DON ? 8 : 6;
                        int sz = defsize[note.Type >= ENote.DON ? 1 : 0];
                        Drawing.Circle(notex, notey, sz);
                        Drawing.Circle(notex, notey, sz, 0, false, 2);
                        Drawing.Circle(notex, notey, sz - ed, (int)note.Type % 2 == (int)ENote.Don ? color[0] : color[1]);
                    }
                    break;
                case ENote.Roll:
                case ENote.ROLL:
                    if (taiko.Tx.Notes.Enable)
                    {
                        int rec = note.Type == ENote.ROLL ? 8 : 5;
                        if (note.LongEnd != null)
                        {
                            double endx = x + NoteX(note.LongEnd);
                            double endy = y;

                            taiko.Tx.Notes.SetRectangle(size * (rec + 1), 0, size, size);
                            taiko.Tx.Notes.XYScale = ((endx - (notex)) / size, 1.0);
                            taiko.Tx.Notes.ReferencePoint = ReferencePoint.CenterLeft;
                            taiko.Tx.Notes.Draw(notex, notey);
                            taiko.Tx.Notes.XYScale = null;

                            taiko.Tx.Notes.SetRectangle(size * (rec + 2), 0, size, size);
                            taiko.Tx.Notes.ReferencePoint = ReferencePoint.CenterLeft;
                            taiko.Tx.Notes.Draw(endx, endy);
                        }
                        taiko.Tx.Notes.SetRectangle(size * rec, 0, size, size);
                        taiko.Tx.Notes.ReferencePoint = ReferencePoint.Center;
                        taiko.Tx.Notes.Draw(notex, notey);
                    }
                    else
                    {
                        int sz = defsize[note.Type >= ENote.ROLL ? 1 : 0];
                        int ed = note.Type >= ENote.ROLL ? 8 : 6;
                        if (note.LongEnd != null)
                        {
                            double endx = x + NoteX(note.LongEnd);
                            double endy = y;

                            Drawing.Circle(endx, endy, sz);
                            Drawing.Circle(endx, endy, sz, 0, false, 2);
                            Drawing.Circle(endx, endy, sz - ed, color[2]);
                            double x1 = notex;
                            double y1 = y - sz;
                            double x2 = endx;
                            double y2 = endy + sz;
                            Drawing.BoxZ(x1, y1 - 1, x2, y2 + 2, 0);
                            Drawing.BoxZ(x1, y1 + 1, x2, y2 - 0);
                            Drawing.BoxZ(x1, y1 + ed, x2, y2 - ed + 1, color[2]);
                        }
                        Drawing.Circle(notex, notey, sz);
                        Drawing.Circle(notex, notey, sz, 0, false, 2);
                        Drawing.Circle(notex, notey, sz - ed, color[2]);
                    }
                    break;
                case ENote.Balloon:
                case ENote.Potato:
                    if (taiko.Tx.Notes.Enable)
                    {
                        int rec = note.Type == ENote.Potato ? 13 : 11;
                        double balloonx = notex;
                        double balloony = notey;
                        if (note.LongEnd != null)
                        {
                            double endx = x + NoteX(note.LongEnd);
                            double endy = y;
                            if (Timer.Value >= note.LongEnd.Time)
                            {
                                balloonx = endx;
                                balloony = endy;
                            }
                            else if (Timer.Value >= note.Time)
                            {
                                balloonx = x;
                                balloony = y;
                            }
                        }
                        taiko.Tx.Notes.SetRectangle(size * rec, 0, size * 2, size);
                        taiko.Tx.Notes.SetCenter(size / 2.0, size / 2.0);
                        taiko.Tx.Notes.ReferencePoint = ReferencePoint.Center;
                        taiko.Tx.Notes.Draw(balloonx, balloony);
                        taiko.Tx.Notes.Center = null;
                    }
                    else
                    {
                        int sz = defsize[note.Type >= ENote.Potato ? 1 : 0];
                        int ed = note.Type >= ENote.Potato ? 8 : 6;
                        double balloonx = notex;
                        double balloony = notey;
                        if (note.LongEnd != null)
                        {
                            double endx = x + NoteX(note.LongEnd);
                            double endy = y;
                            if (Timer.Value >= note.LongEnd.Time)
                            {
                                balloonx = endx;
                                balloony = endy;
                            }
                            else if (Timer.Value >= note.Time)
                            {
                                balloonx = x;
                                balloony = y;
                            }
                            Drawing.Circle(endx, endy, sz, color[3], false, 2, 0.5);
                            double x1 = notex;
                            double y1 = y - sz;
                            double x2 = endx;
                            double y2 = endy + sz;
                            Drawing.BoxZ(x1, y1, x2, y2 + 1, color[3], false, 2, 0.5);
                        }

                        double objx = balloonx + sz * 2;
                        double objy = balloony;

                        Drawing.Circle(objx, objy, sz / 2);
                        Drawing.Circle(objx, objy, sz / 2, 0, false, 2);
                        Drawing.Circle(objx, objy, sz / 2 - ed / 2, color[2]);
                        double ox1 = balloonx;
                        double oy1 = balloony - sz / 2;
                        double ox2 = objx;
                        double oy2 = objy + sz / 2;
                        Drawing.BoxZ(ox1, oy1 - 1, ox2, oy2 + 2, 0);
                        Drawing.BoxZ(ox1, oy1 + 1, ox2, oy2 - 0);
                        Drawing.BoxZ(ox1, oy1 + ed, ox2, oy2 - ed / 2 + 1, color[2]);

                        Drawing.Circle(balloonx, balloony, sz);
                        Drawing.Circle(balloonx, balloony, sz, 0, false, 2);
                        Drawing.Circle(balloonx, balloony, sz - ed, note.Type >= ENote.Potato ? color[4] : color[3]);
                    }
                    break;
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

        public void Auto()
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
        public void Hit(bool[] hits)
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
