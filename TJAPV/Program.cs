using SeaDrop;
using static TJAPlayerV.Data;

namespace TJAPlayerV
{
    public class Program : Scene
    {
        public static void Main(string[] args)
        {
            DXLib.SetDrop(true);
            DXLib.Init(new Program(), 1280, 720);
        }

        public override void Enable()
        {
            Data.Init();
            Songs.Load();
            base.Enable();
        }

        public override void Draw()
        {
            Drawing.Text(20, 0, DataDir);
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