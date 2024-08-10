using SeaDrop;
using static TJAPlayerV.taiko.Skin;

namespace TJAPlayerV.taiko
{
    public class SongBars
    {
        public static SongBar[] NowList = [];

        public static List<SongBar> SongList = [];
        public static int Cursor;
        public static string NowFolder = "";
        public static Counter BoxOpen = new();
        public static Counter BarOpen = new();
        public static Handle BarHandle = new();
        public static Handle BoxHandle = new();

        public static Song NowSong
        {
            get
            {
                if (Cursor >= 0 && Cursor < SongList.Count)
                    return SongList[Cursor].Song;
                else return new Song();
            }
        }
        public static string? BackFolder
        {
            get
            {
                if (SongList.Count > 0 && SongList[0].Song.Type == ESongType.Back)
                {
                    return SongList[0].Song.Directory;
                }
                return null;
            }
        }


        public static void Draw()
        {
            BoxOpen.Tick();
            for (int i = 0; i < NowList.Length; i++)
            {
                var song = NowList[i];

                float BarAnimeCount = BarOpen.Value <= 200 ? 0 : (float)Math.Sin((BarOpen.Value - 200) * 1.5f * (Math.PI / 180));
                int centerMove = (int)(BarAnimeCount * SongSelect_Bar_Center_Move);
                int centerMoveX = (int)(BarAnimeCount * SongSelect_Bar_Center_Move_X);
                song?.Draw(SongSelect_Bar_X[i], SongSelect_Bar_Y[i], 0, 0, i == NowList.Length / 2, i == Cursor);//
            }
        }

        public static void Load(string root = "", string cursor = "")
        {
            Cursor = 0;
            NowList = new SongBar[SongSelect_Bar_Count];
            SongList = [];

            //SongDic = Songs.ToDictionary(song => song.Path);
            string fontpath = $@"{DXLib.AppPath}\Lang\{Language.NowLang}\" + Language.Get("FontName");
            BarHandle = new Handle(fontpath, ModeSelect_Title_Scale[0], 1, 6, false, EFontType.AntialiasingEdge);
            BoxHandle = new Handle(fontpath, ModeSelect_Title_Scale[1], 1, 4, false, EFontType.AntialiasingEdge);

            if (!string.IsNullOrEmpty(root))
            {
                var rsong = Songs.SongList.Find(s => s.Path == root);
                if (rsong != null)
                {
                    Song song = new()
                    {
                        Path = rsong.Path,
                        Name = rsong.Name,
                        Directory = rsong.Directory,
                        Type = ESongType.Back,
                    };
                    SongBar bar = new()
                    {
                        Song = song,
                        BarTexture = Tx.SongSelect_Bar_Back,
                        Title = song.Name,
                        Description = $"Back to u{song.Directory}v",
                        Handle = BarHandle,
                        DescHandle = BoxHandle,
                        Overlay = false,
                    };
                    SongList.Add(bar);
                }
            }

            foreach (var song in Songs.SongList)
            {
                if (song.Directory == root)
                {
                    if (song.Type == ESongType.Song)
                    {
                        SongBar bar = new()
                        {
                            Song = song,
                            BarTexture = Tx.SongSelect_GenreBar[Songs.GenreNum(Songs.GenreName(song.Genre))],
                            Title = song.TJA.Header.Title,
                            Description = "",
                            Handle = BarHandle,
                            DescHandle = BoxHandle,
                            Overlay = true,
                        };
                        SongList.Add(bar);
                    }
                    else if (song.Type == ESongType.Folder)
                    {
                        SongBar bar = new()
                        {
                            Song = song,
                            BarTexture = Tx.SongSelect_GenreBar[Songs.GenreNum(Songs.GenreName(song.Genre))],
                            Title = song.Name,
                            Description = $"{song.Folder.Count} Files",
                            Handle = BarHandle,
                            DescHandle = BoxHandle,
                            Overlay = true,
                        };
                        if (cursor != "" && song.Path == cursor) Cursor = SongList.Count;
                        SongList.Add(bar);
                    }
                }
            }
            SongList.Sort((a, b) =>
            {
                int r = Songs.RootDirNum(a.Song.Path) - Songs.RootDirNum(b.Song.Path);
                int t = r != 0 ? r : a.Song.Type - b.Song.Type;
                return t != 0 ? t : new NaturalComparer().Compare(a.Song.Path, b.Song.Path);
            });

            Set();

            NowFolder = root;
        }

        public static void Set()
        {
            if (SongList.Count == 0) return;

            int c = NowList.Length / 2;
            NowList[c] = SongList[Cursor];
            var csong = NowList[c];
            for (int i = c + 1; i < NowList.Length; i++)
            {
                NowList[i] = NextSong(NowList[i - 1]);
            }
            for (int i = c - 1; i >= 0; i--)
            {
                NowList[i] = PrevSong(NowList[i + 1]);
            }
        }

        public static void Next()
        {
            if (Cursor++ >= SongList.Count - 1) Cursor = 0;
            Set();
        }

        public static void Prev()
        {
            if (Cursor-- <= 0) Cursor = SongList.Count - 1;
            Set();
        }

        public static SongBar NextSong(SongBar song)
        {
            var list = SongList;

            int index = list.IndexOf(song);

            if (index < 0 || list.Count == 0)
                return new SongBar();

            if (index == (list.Count - 1))
                return list[0];

            return list[index + 1];
        }
        public static SongBar PrevSong(SongBar song)
        {
            var list = SongList;

            int index = list.IndexOf(song);

            if (index < 0 || list.Count == 0)
                return new SongBar();

            if (index == 0)
                return list[list.Count - 1];

            return list[index - 1];
        }

        public static bool Back()
        {
            string from = NowFolder;
            var back = BackFolder;
            if (back != null)
            {
                Load(back, from);
                return true;
            }
            else return false;
        }
    }

    public class SongBar
    {
        public string Title = "";
        public string Description = "";
        public Handle Handle = new();
        public Handle DescHandle = new();

        public Song Song = new();
        public int GenreNumber = 0;
        public bool Coloring = false, Overlay = false, FullOverlay = false;
        public Texture BarTexture = new();

        public void Draw(double x, double y, double movex, double movey, bool center = false, bool cursor = false)
        {
            var texture = BarTexture;
            var overlay = Tx.SongSelect_Bar_Overlay;
            double barx = x, bary = y;

            var point = ReferencePoint.Center;
            texture.ReferencePoint = point;
            overlay.ReferencePoint = point;

            double anime = 1;


            if (center)
            {
                int width = overlay.Width / 3;
                int height = overlay.Height / 3;
                double moveoffset = SongSelect_Bar_Center_Move_X * (1.0 - anime);

                if (SongBars.BoxOpen.Value >= 1300 && SongBars.BoxOpen.Value <= 1940)
                {
                    anime -= Math.Sin((SongBars.BoxOpen.Value - 1300) * 0.28125f * (Math.PI / 180));
                }

                if (texture.Enable)
                {
                    double offset = (texture.Width / 3.0) * (1.0 - anime);

                    barx = x + (offset * 1.5) + moveoffset - movex;
                    bary = y - movey;

                    texture.XYScale = (1.0 * anime, 1.0);
                    texture.SetRectangle(0, 0, width, height);
                    texture.Draw(barx, bary);

                    texture.XYScale = (1.0 * anime, 1.0 + ((movex / (double)height) * 2.0));
                    texture.SetRectangle(0, height, width, height);
                    texture.Draw(barx, bary + height * 1);

                    texture.XYScale = (1.0 * anime, 1.0);
                    texture.SetRectangle(0, height * 2, width, height);
                    texture.Draw(barx, bary + height * 2);

                    barx = x + (offset / 2) + moveoffset - movex + width;

                    texture.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0);
                    texture.SetRectangle(width, 0, width, height);
                    texture.Draw(barx, bary);

                    texture.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0 + ((movey / (double)height) * 2.0));
                    texture.SetRectangle(width, height, width, height);
                    texture.Draw(barx, bary + height * 1);

                    texture.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0);
                    texture.SetRectangle(width, height * 2, width, height);
                    texture.Draw(barx, bary + height * 2);

                    barx = x + (offset / 2) + moveoffset - movex + (width * 2);

                    texture.XYScale = (1.0 * anime, 1.0);
                    texture.SetRectangle(width * 2, 0, width, height);
                    texture.Draw(barx, bary);

                    texture.XYScale = (1.0 * anime, 1.0 + ((movey / (double)height) * 2.0));
                    texture.SetRectangle(width * 2, height, width, height);
                    texture.Draw(barx, bary + height * 1);

                    texture.XYScale = (1.0 * anime, 1.0);
                    texture.SetRectangle(width * 2, height * 2, width, height);
                    texture.Draw(barx, bary + height * 2);

                    texture.XYScale = null;
                    texture.Rectangle = null;
                }

                if (Overlay && overlay.Enable)
                {
                    double offset = (overlay.Width / 3.0) * (1.0 - anime);
                    double size = FullOverlay ? 2.0 : 1.0;

                    barx = x + (offset * 1.5) + moveoffset - movex;
                    bary = FullOverlay ? y - movey : y;

                    overlay.XYScale = (1.0 * anime, 1.0);
                    overlay.SetRectangle(0, 0, width, height);
                    overlay.Draw(barx, bary);

                    overlay.XYScale = (1.0 * anime, 1.0 + ((movey / (double)height) * size));
                    overlay.SetRectangle(0, height, width, height);
                    overlay.Draw(barx, bary + height * 1);

                    overlay.XYScale = (1.0 * anime, 1.0);
                    overlay.SetRectangle(0, height * 2, width, height);
                    overlay.Draw(barx, bary + height * 2);

                    barx = x + (offset / 2) + moveoffset - movex + width;

                    overlay.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0);
                    overlay.SetRectangle(width, 0, width, height);
                    overlay.Draw(barx, bary);

                    overlay.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0 + ((movey / (double)height) * size));
                    overlay.SetRectangle(width, height, width, height);
                    overlay.Draw(barx, bary + height * 1);

                    overlay.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0);
                    overlay.SetRectangle(width, height * 2, width, height);
                    overlay.Draw(barx, bary + height * 2);

                    barx = x + (offset / 2) + moveoffset - movex + (width * 2);

                    overlay.XYScale = (1.0 * anime, 1.0);
                    overlay.SetRectangle(width * 2, 0, width, height);
                    overlay.Draw(barx, bary);

                    overlay.XYScale = (1.0 * anime, 1.0 + ((movey / (double)height) * size));
                    overlay.SetRectangle(width * 2, height, width, height);
                    overlay.Draw(barx, bary + height * 1);

                    overlay.XYScale = (1.0 * anime, 1.0);
                    overlay.SetRectangle(width * 2, height * 2, width, height);
                    overlay.Draw(barx, bary + height * 2);

                    overlay.XYScale = null;
                    overlay.Rectangle = null;
                }

                double toffset = (texture.Width / 3.0) * (1.0 - anime);
                barx = x + (toffset / 2) + moveoffset - movex + width;
                bary = y - movey + height;
                Drawing.Text(barx, bary, Title, Handle, 0xffffff, 0, false, point);
                Drawing.Text(barx, bary + toffset, Description, DescHandle, 0xffffff, 0, false, point);
                /*if (cursor)
                {
                    var curtex = Tx.SongSelect_Bar*Ov;
                    double offset = (texture.Width / 3.0) * (1.0 - anime);

                    barx = x + (offset * 1.5) + moveoffset - movex;
                    bary = y - movey;

                    curtex.XYScale = (1.0 * anime, 1.0);
                    curtex.SetRectangle(0, 0, width, height);
                    curtex.Draw(barx, bary);

                    curtex.XYScale = (1.0 * anime, 1.0 + ((movex / (double)height) * 2.0));
                    curtex.SetRectangle(0, height, width, height);
                    curtex.Draw(barx, bary + height * 1);

                    curtex.XYScale = (1.0 * anime, 1.0);
                    curtex.SetRectangle(0, height * 2, width, height);
                    curtex.Draw(barx, bary + height * 2);

                    barx = x + (offset / 2) + moveoffset - movex + width;

                    curtex.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0);
                    curtex.SetRectangle(width, 0, width, height);
                    curtex.Draw(barx, bary);

                    curtex.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0 + ((movex / (double)height) * 2.0));
                    curtex.SetRectangle(width, height, width, height);
                    curtex.Draw(barx, bary + height * 1);

                    curtex.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0);
                    curtex.SetRectangle(width, height * 2, width, height);
                    curtex.Draw(barx, bary + height * 2);

                    barx = x + (offset / 2) + moveoffset - movex + (width * 2);

                    curtex.XYScale = (1.0 * anime, 1.0);
                    curtex.SetRectangle(width * 2, 0, width, height);
                    curtex.Draw(barx, bary);

                    curtex.XYScale = (1.0 * anime, 1.0 + ((movex / (double)height) * 2.0));
                    curtex.SetRectangle(width * 2, height, width, height);
                    curtex.Draw(barx, bary + height * 1);

                    curtex.XYScale = (1.0 * anime, 1.0);
                    curtex.SetRectangle(width * 2, height * 2, width, height);
                    curtex.Draw(barx, bary + height * 2);

                    curtex.XYScale = null;
                    curtex.Rectangle = null;
                }*/
            }
            else
            {
                int width = overlay.Width / 3;
                int height = overlay.Height / 3;
                double moveoffset = SongSelect_Bar_Center_Move_X * (1.0 - anime);

                if (SongBars.BoxOpen.Value >= 1300 && SongBars.BoxOpen.Value <= 1940)
                {
                    anime -= Math.Sin((SongBars.BoxOpen.Value - 1300) * 0.28125f * (Math.PI / 180));
                }

                if (texture.Enable)
                {
                    double offset = (texture.Width / 3.0) * (1.0 - anime);

                    barx = x + (offset * 1.5) + moveoffset - movex;
                    bary = y - movey;

                    texture.XYScale = (1.0 * anime, 1.0);
                    texture.SetRectangle(0, 0, width, height);
                    texture.Draw(barx, bary);

                    texture.XYScale = (1.0 * anime, 1.0 + ((movex / (double)height) * 2.0));
                    texture.SetRectangle(0, height, width, height);
                    texture.Draw(barx, bary + height * 1);

                    texture.XYScale = (1.0 * anime, 1.0);
                    texture.SetRectangle(0, height * 2, width, height);
                    texture.Draw(barx, bary + height * 2);

                    barx = x + (offset / 2) + moveoffset - movex + width;

                    texture.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0);
                    texture.SetRectangle(width, 0, width, height);
                    texture.Draw(barx, bary);

                    texture.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0 + ((movex / (double)height) * 2.0));
                    texture.SetRectangle(width, height, width, height);
                    texture.Draw(barx, bary + height * 1);

                    texture.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0);
                    texture.SetRectangle(width, height * 2, width, height);
                    texture.Draw(barx, bary + height * 2);

                    barx = x + (offset / 2) + moveoffset - movex + (width * 2);

                    texture.XYScale = (1.0 * anime, 1.0);
                    texture.SetRectangle(width * 2, 0, width, height);
                    texture.Draw(barx, bary);

                    texture.XYScale = (1.0 * anime, 1.0 + ((movex / (double)height) * 2.0));
                    texture.SetRectangle(width * 2, height, width, height);
                    texture.Draw(barx, bary + height * 1);

                    texture.XYScale = (1.0 * anime, 1.0);
                    texture.SetRectangle(width * 2, height * 2, width, height);
                    texture.Draw(barx, bary + height * 2);

                    texture.XYScale = null;
                    texture.Rectangle = null;
                }

                if (Overlay && overlay.Enable)
                {
                    double offset = (overlay.Width / 3.0) * (1.0 - anime);
                    double size = FullOverlay ? 2.0 : 1.0;

                    barx = x + (offset * 1.5) + moveoffset - movex;
                    bary = FullOverlay ? y - movey : y;

                    overlay.XYScale = (1.0 * anime, 1.0);
                    overlay.SetRectangle(0, 0, width, height);
                    overlay.Draw(barx, bary);

                    overlay.XYScale = (1.0 * anime, 1.0 + ((movex / (double)height) * size));
                    overlay.SetRectangle(0, height, width, height);
                    overlay.Draw(barx, bary + height * 1);

                    overlay.XYScale = (1.0 * anime, 1.0);
                    overlay.SetRectangle(0, height * 2, width, height);
                    overlay.Draw(barx, bary + height * 2);

                    barx = x + (offset / 2) + moveoffset - movex + width;

                    overlay.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0);
                    overlay.SetRectangle(width, 0, width, height);
                    overlay.Draw(barx, bary);

                    overlay.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0 + ((movex / (double)height) * size));
                    overlay.SetRectangle(width, height, width, height);
                    overlay.Draw(barx, bary + height * 1);

                    overlay.XYScale = (1.0 + ((movex / (double)width) * 2.0) * anime, 1.0);
                    overlay.SetRectangle(width, height * 2, width, height);
                    overlay.Draw(barx, bary + height * 2);

                    barx = x + (offset / 2) + moveoffset - movex + (width * 2);

                    overlay.XYScale = (1.0 * anime, 1.0);
                    overlay.SetRectangle(width * 2, 0, width, height);
                    overlay.Draw(barx, bary);

                    overlay.XYScale = (1.0 * anime, 1.0 + ((movex / (double)height) * size));
                    overlay.SetRectangle(width * 2, height, width, height);
                    overlay.Draw(barx, bary + height * 1);

                    overlay.XYScale = (1.0 * anime, 1.0);
                    overlay.SetRectangle(width * 2, height * 2, width, height);
                    overlay.Draw(barx, bary + height * 2);

                    overlay.XYScale = null;
                    overlay.Rectangle = null;
                }
                double toffset = (texture.Width / 3.0) * (1.0 - anime);
                barx = x + (toffset / 2) + moveoffset - movex + width;
                bary = y - movey + height;
                Drawing.Text(barx, bary, Title, Handle, 0xffffff, 0, false, point);
            }
        }
    }
}
