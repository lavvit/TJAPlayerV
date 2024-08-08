using SeaDrop;
using System.Drawing;
using System.Runtime.Versioning;
using static SeaDrop.Drawing;

namespace TJAPlayerV.taiko
{
    public class Startup : Scene
    {
        public static Handle Handle = new();

        [SupportedOSPlatform("windows")]
        public override void Enable()
        {
            Handle = new(SystemFonts.MenuFont != null ? SystemFonts.MenuFont.Name : "");
            Task.Run(Skin.Load);
            Language.Load();
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
            Text(20, 0, Data.DataDir, Handle);
            var songs = Songs.SongList;
            Text(20, 20, $"Skin Load : {Skin.LoadStatus}", Handle);
            Text(20, 40, $"Songs : {songs.Count}   {Songs.Loading}", Handle);

            int w = DXLib.Width / 480;
            int h = (DXLib.Height - 60) / 20;
            for (int i = 0; i < w * h; i++)
            {
                if (i >= songs.Count) break;
                Song song = songs[i];
                Text(20 + 480 * (i / h), 80 + 20 * (i % h), song, Handle);
            }
            base.Draw();
        }

        public override void Update()
        {
            if (Songs.LoadStatus == ELoadState.Success && Skin.LoadStatus == ELoadState.Success)
            {
                Thread.Sleep(2000);
                DXLib.SceneChange(new SongSelect());//Entry
                Songs.LoadStatus = ELoadState.None;
                Skin.LoadStatus = ELoadState.None;
            }
            base.Update();
        }
    }
}