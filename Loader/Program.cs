using SeaDrop;
using System.Reflection;
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
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;

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
            Drawing.Text(160, 2, $"{Lane.NowMeasure()}/{Lane.AllMeasure()} {Lane.NowMeasureText()}");
            Drawing.Text(DXLib.Width - 130, 2, Lane.TJA != null ? Lane.TJA.Header.Title : "", null, 0xffffff, 0, false, ReferencePoint.TopRight);
            Drawing.Text(DXLib.Width - 120, 2, Lane.TJA != null ? (ECourse)Lane.Course : "", Course.GetCourseColor((ECourse)Lane.Course));
            Drawing.Text(DXLib.Width - 60, 2, FPS.AverageValue, 0x00ff00);
            //Drawing.Text(20, 20, Lane.TJA);
            if (Lane.TJA != null && Lane.TJA.Courses[Lane.Course] != null)
            {
                int y = 0;
                var c = Lane.NowChip(108);
                foreach (var line in c)
                {
                    Drawing.Text(320, 20 + 20 + y, line);
                    y += Drawing.TextSize(line).Height;
                }
                /*int y = 0;
                Drawing.Text(520, 20 + y, Lane.TJA.Courses[Lane.Course].Lanes[0]);
                foreach (var line in Lane.TJA.Courses[Lane.Course].Lanes[0])
                {
                    Drawing.Text(320, 20 + y, line);
                    y += Drawing.TextSize(line).Height;
                }*/
            }

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
                if (Key.IsPushed(EKey.Up))
                {
                    if (Lane.Course++ >= (int)ECourse.Edit) Lane.Course = (int)ECourse.Edit;
                    NoteSet();
                }
                if (Key.IsPushed(EKey.Down))
                {
                    if (Lane.Course-- <= (int)ECourse.Easy) Lane.Course = (int)ECourse.Easy;
                    NoteSet();
                }
                if (Key.IsPushed(EKey.F1))
                {
                    Lane.IsAuto = !Lane.IsAuto;
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

                if (Lane.IsAuto) Lane.Auto();
                else
                {
                    bool[] hit = [Key.IsPushed(EKey.F), Key.IsPushed(EKey.J), Key.IsPushed(EKey.D), Key.IsPushed(EKey.K)];
                    Lane.Hit(hit);
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
            NoteSet();
        }
        public static void NoteSet()
        {
            if (Lane.TJA != null && Lane.TJA.Courses[Lane.Course] != null)
            {
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

        private static Assembly? Resolver(object? sender, ResolveEventArgs args)
        {
            string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
            string? appbase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string assemblyPath = Path.Combine(appbase != null ? appbase : "",
                                                       "dll",
                                                       assemblyName);

            return File.Exists(assemblyPath) ? Assembly.LoadFile(assemblyPath) : null;
        }
    }
}