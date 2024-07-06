using SeaDrop;

namespace TJAPlayerV
{
    public class Program : Scene
    {
        public static string val = "";
        public static void Main(string[] args)
        {
            DXLib.MultiThreading = true;
            DXLib.SetDrop(true);
            DXLib.Init(new Program(), 1920, 1080);
        }

        public override void Enable()
        {
            Songs.Load();
            base.Enable();
        }

        public override void Draw()
        {
            Drawing.Text(20, 20, val);
            var songs = Songs.SongList;
            Drawing.Text(20, 20, $"Songs : {songs.Count}   {Songs.Loading}");

            for (int i = 0; i < 200; i++)
            {
                if (i >= songs.Count) break;
                Song song = songs[i];
                Drawing.Text(20 + 400 * (i / 50), 60 + 20 * (i % 50), song);
            }


            base.Draw();
        }

        public override void Drag(string str)
        {
            base.Drag(str);
        }
    }
}