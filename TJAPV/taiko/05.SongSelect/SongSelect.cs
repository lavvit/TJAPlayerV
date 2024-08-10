using SeaDrop;

namespace TJAPlayerV.taiko
{
    public class SongSelect : Scene
    {
        public static Sound BGMIn
        {
            get
            {
                return Sfx.SongSelect_BGM_In;
            }
        }
        public static Sound BGM
        {
            get
            {
                return Sfx.SongSelect_BGM;
            }
        }
        public static Texture NowBG = new(), PrevBG = new();

        public override void Enable()
        {
            SongBars.Load();
            BGMIn.Play();
            NowBG = Tx.SongSelect_BG;
            base.Enable();
        }

        public override void Draw()
        {
            if (PrevBG.Enable)
            {
                PrevBG.Draw(0, 0);
            }
            if (NowBG.Enable)
            {
                NowBG.Draw(0, 0);
            }
            SongBars.Draw();
            base.Draw();
        }
        public override void Debug()
        {
            base.Debug();
        }

        public override void Drag(string str)
        {
            var song = SongBars.NowSong;
            base.Drag(str);
        }

        public override void Update()
        {
            if (BGMIn.Played)
            {
                BGM.PlayLoopUp();
            }
            if (Key.IsPushed(EKey.D))
            {
                SongBars.Prev();
                Change();
            }
            if (Key.IsPushed(EKey.K))
            {
                SongBars.Next();
                Change();
            }
            if (Key.IsPushed(EKey.F) || Key.IsPushed(EKey.J))
            {
                var song = SongBars.NowSong;
                if (song.Type == ESongType.Folder)
                {
                    SongBars.Load(song.Path);
                    Sfx.Decide.Play();
                    SongBars.BoxOpen.Reset();
                    SongBars.BoxOpen.Start();
                }
                if (song.Type == ESongType.Back)
                {
                    SongBars.Back();
                    Sfx.Decide.Play();
                }
            }
            if (Key.IsPushed(EKey.Esc))
            {
                if (!SongBars.Back()) DXLib.End();
                Sfx.Change.Play();
            }
            if (Key.IsPushed(EKey.F5))
            {
                Songs.Load();
                SongBars.Load();
            }
            base.Update();
        }

        public static void Change()
        {
            Sfx.Change.Play();
        }
    }
}