using SeaDrop;
using static SeaDrop.Drawing;

namespace TJAPlayerV.taiko
{
    public class Startup : Scene
    {
        public override void Enable()
        {
            Data.Init();
            Songs.Load();
            base.Enable();
        }
        public override void Disable()
        {
            base.Disable();
        }

        public override void Draw()
        {
            Text(20, 0, Data.DataDir);
            var songs = Songs.SongList;
            Text(20, 20, $"Songs : {songs.Count}   {Songs.Loading}");

            int w = DXLib.Width / 480;
            int h = (DXLib.Height - 60) / 20;
            for (int i = 0; i < w * h; i++)
            {
                if (i >= songs.Count) break;
                Song song = songs[i];
                Text(20 + 480 * (i / h), 60 + 20 * (i % h), song);
            }
            base.Draw();
        }

        public override void Update()
        {
            if (Songs.LoadStatus == ELoadState.Success)
            {
                Thread.Sleep(2000);
                DXLib.SceneChange(new Entry());
                Songs.LoadStatus = ELoadState.None;
            }
            base.Update();
        }
    }
}