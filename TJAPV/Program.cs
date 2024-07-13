using SeaDrop;
using static SeaDrop.Drawing;
using static SeaDrop.DXLib;

namespace TJAPlayerV
{
    public class Program : Scene
    {
        public static void Main(string[] args)
        {
            SetDrop(true);
            Init(new taiko.Entry(), 1280, 720);
            //Init(new taiko.Startup());
            //Init(new Program(), 3840, 2160, 0.5);
        }

        public static string Version = "0.1.0";

        public override void Enable()
        {
            Data.Init();
            Songs.Load();
            base.Enable();
        }

        public override void Draw()
        {
            Text(20, 0, Data.DataDir);
            var songs = Songs.SongList;
            Text(20, 20, $"Songs : {songs.Count}   {Songs.Loading}");

            int w = Width / 480;
            int h = (Height - 60) / 20;
            for (int i = 0; i < w * h; i++)
            {
                if (i >= songs.Count) break;
                Song song = songs[i];
                Text(20 + 480 * (i / h), 60 + 20 * (i % h), song);
            }


            base.Draw();
        }

        public override void Drag(string str)
        {
            base.Drag(str);
        }
    }
}