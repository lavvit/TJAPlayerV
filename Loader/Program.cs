using SeaDrop;
using static Loader.Skin;

namespace Loader
{
    public class Program : Scene
    {

        public static string Song = "Song\\バナナのナナチ";
        public static Lane Lane = new();
        public static Sound Sound = new();

        public static int Size = 2048;

        public static void Main(string[] args)
        {
            var size = Texture.GetSize(@$"{DXLib.AppPath}\System\taiko\Graphics\5_Game\12_Lane\Background_Main.png");
            if (size.Width == 0) size = (512, 96);
            DXLib.SetDrop(true);
            DXLib.VSync = true;
            DXLib.Init(new Program(), size.Width, size.Height, (double)Size / size.Width);//
        }

        public override void Enable()
        {
            LoadSkin();
            taiko.Skin.Load();
            Lane = new(Song);
            Sound = new Sound(Lane.TJA.SoundPath);
            base.Enable();
        }

        public override void Draw()
        {
            Lane.Draw(0, 0);
            Drawing.Text(2, 2, $"{Lane.Timer.Value}" + (Lane.TJA != null ? $"/{Lane.TJA.Length}" : ""));
            Drawing.Text(DXLib.Width - 80, 10, Lane.TJA != null ? Lane.TJA.Header.Title : "", null, 0xffffff, 0, false, ReferencePoint.TopRight);
            Drawing.Text(DXLib.Width - 60, 10, FPS.AverageValue, 0x00ff00);

            //Drawing.Text(20, 20, Lane.TJA);
            /*if (Lane.TJA.Courses[Course] != null)
            {
                int y = 0;
                Drawing.Text(520, 20 + y, Lane.TJA.Courses[Course].Lanes[0]);
                foreach (var line in Lane.TJA.Courses[Course].Lanes[0])
                {
                    Drawing.Text(320, 20 + y, line);
                    y += Drawing.TextSize(line).Height;
                }
            }*/

            base.Draw();
        }

        public override void Drag(string str)
        {
            Song = str;
            Lane = new(Song);
            Sound = new Sound(Lane.TJA.SoundPath);
            Lane.Timer.Reset();
            base.Drag(str);
        }

        public override void Update()
        {
            if (Key.IsPushed(EKey.Esc)) DXLib.End();


            if (Lane.TJA != null)
            {
                if (Key.IsPushed(EKey.Space))
                {
                    if (Lane.Timer.Value > Lane.TJA.Length || Key.IsPushing(EKey.LShift))
                    {
                        Reset();
                    }
                    if (Lane.Timer.State > 0)
                    {
                        if (Sound.Enable) Sound.Stop();
                        Lane.Timer.Stop();
                    }
                    else
                    {
                        if (Sound.Enable && Lane.Timer.Value >= Lane.TJA.Header.Offset * 1000.0)
                        {
                            Sound.Play();
                            Sound.Time = (int)(Lane.Timer.Value - Lane.TJA.Header.Offset * 1000.0);
                        }
                        Lane.Timer.Start();
                    }
                }
                if (Key.IsPushed(EKey.F9))
                {
                    Text.Save(Lane.TJA.Courses[Lane.Course].Texts, $"{DXLib.AppPath}\\{Path.GetFileNameWithoutExtension(Lane.Path)}.txt");
                }

                if (Sound.Enable && Lane.Timer.State > 0 &&
                    Lane.Timer.Value >= Lane.TJA.Header.Offset * 1000.0 && Lane.Timer.Value + 50 < Lane.TJA.Length + Lane.TJA.Header.Offset * 1000.0)
                {
                    Sound.PlayLoopUp();
                }

                foreach (var bar in Lane.TJA.Courses[Lane.Course].Lanes[0])
                {
                    foreach (var chip in bar.Chips)
                    {
                        if (Lane.Timer.Value >= chip.Time - 8 && !chip.Hit)
                        {
                            chip.Hit = true;
                            chip.HitTime = Lane.Timer.Value;

                            switch (chip.Type)
                            {
                                case ENote.Don:
                                    taiko.SFx.Don.Play();
                                    break;
                                case ENote.Ka:
                                    taiko.SFx.Ka.Play();
                                    break;
                                case ENote.DON:
                                    taiko.SFx.Don.Play();
                                    taiko.SFx.Don.Play();
                                    break;
                                case ENote.KA:
                                    taiko.SFx.Ka.Play();
                                    taiko.SFx.Ka.Play();
                                    break;
                            }
                        }
                    }
                }
            }

            base.Update();
        }

        public static void Reset()
        {
            if (Sound.Enable)
            {
                Sound.Stop();
                Sound.Time = 0;
            }
            Lane.Timer.Reset();
            foreach (var bar in Lane.TJA.Courses[Lane.Course].Lanes[0])
            {
                foreach (var chip in bar.Chips)
                {
                    chip.Hit = false;
                    chip.HitTime = 0;
                }
            }
        }
    }
}