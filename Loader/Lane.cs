using SeaDrop;
using System.Drawing;

namespace Loader
{
    public class Lane
    {
        public string Path = "";
        public TJA TJA;
        public int Course = 3;
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
            int[] color = [ 0xe7372a, 0x4ecdbe, 0xecb907, 0xff5000, 0xcc245e ];
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
        #endregion
    }
}
