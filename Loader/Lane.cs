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
                        Drawing.Circle(x + NoteX(note), y, 32, 0xc0c0c0, false, 2);
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
    }
}
