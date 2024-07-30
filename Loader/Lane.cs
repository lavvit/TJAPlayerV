﻿using SeaDrop;

namespace Loader
{
    public class Lane
    {
        public string Path = "";
        public TJA TJA = new("");
        public int Course = 3;
        public bool IsAuto = true;
        public int StartMeasure = 0;
        public double StartTime = -2000;
        public static Counter Timer = new(-2000, int.MaxValue);

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
            else Drawing.Circle(defx, defy, 32, 0xc0c0c0, false, 2);
            if (TJA == null || TJA.Courses[Course] == null) return;
            for (int i = TJA.Courses[Course].Lanes[0].Length - 1; i >= 0; i--)
            {
                var bar = TJA.Courses[Course].Lanes[0][i];
                if (taiko.Tx.Bar.Enable)
                {
                    taiko.Tx.Bar.ReferencePoint = ReferencePoint.Center;
                    taiko.Tx.Bar.Draw(defx + NoteX(bar), defy);
                }
                for (int j = bar.Chips.Count - 1; j >= 0; j--)
                {
                    var chip = bar.Chips[j];
                    if (!chip.Hit)
                        DrawNote(defx, defy, chip);
                }
            }
        }

        public void DrawNote(double x, double y, Chip note)
        {
            int size = taiko.Tx.Notes.Enable ? taiko.Tx.Notes.Height / 4 : DXLib.Height;
            int[] color = [0xe7372a, 0x4ecdbe, 0xecb907, 0xff5000, 0xcc245e];
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
                        taiko.Tx.Notes.Draw(x + NoteX(note), y);
                    }
                    else
                    {
                        Drawing.Circle(x + NoteX(note), y, note.Type >= ENote.DON ? 44 : 32, (int)note.Type % 2 == (int)ENote.Don ? color[0] : color[1], false, 2);
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
                            taiko.Tx.Notes.XYScale = ((endx - (x + NoteX(note))) / size, 1.0);
                            taiko.Tx.Notes.ReferencePoint = ReferencePoint.CenterLeft;
                            taiko.Tx.Notes.Draw(x + NoteX(note), y);
                            taiko.Tx.Notes.XYScale = null;

                            taiko.Tx.Notes.SetRectangle(size * (rec + 2), 0, size, size);
                            taiko.Tx.Notes.ReferencePoint = ReferencePoint.CenterLeft;
                            taiko.Tx.Notes.Draw(endx, endy);
                        }
                        taiko.Tx.Notes.SetRectangle(size * rec, 0, size, size);
                        taiko.Tx.Notes.ReferencePoint = ReferencePoint.Center;
                        taiko.Tx.Notes.Draw(x + NoteX(note), y);
                    }
                    else
                    {
                        if (note.LongEnd != null)
                        {
                            double endx = x + NoteX(note.LongEnd);
                            double endy = y;

                            Drawing.BoxZ(x, y - size / 2, endx, endy + size / 2, 0xc0c0c0);
                            Drawing.Circle(x + NoteX(note), y, 32, 0xc0c0c0, false, 2);
                        }
                        Drawing.Circle(x + NoteX(note), y, note.Type >= ENote.ROLL ? 44 : 32, color[2], false, 2);
                    }
                    break;
                case ENote.Balloon:
                case ENote.Potato:
                    if (taiko.Tx.Notes.Enable)
                    {
                        int rec = note.Type == ENote.Potato ? 13 : 11;
                        double balloonx = x + NoteX(note);
                        double balloony = y;
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
                        double balloonx = x + NoteX(note);
                        double balloony = y;
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
                        Drawing.Circle(balloonx, balloony, note.Type >= ENote.Potato ? 44 : 32, note.Type >= ENote.Potato ? color[4] : color[3], false, 2);
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
