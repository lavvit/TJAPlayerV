using Loader;
using SeaDrop;
using System.Collections.Concurrent;
using TJAPlayerV;

namespace TJAPlayerV.taiko
{
    public class SongBars
    {
        public static List<SongBar> SongList = [];
        public static int Cursor;
        public static Counter BoxOpen = new();

        public static Song NowSong
        {
            get
            {
                if (Cursor > 0 && Cursor < SongList.Count)
                    return SongList[Cursor].Song;
                else return new Song();
            }
        }


        public static void Load()
        {
            //SongDic = Songs.ToDictionary(song => song.Path);
            foreach (var song in Songs.SongList)
            {
                SongBar bar = new()
                {
                    Song = song,
                    BarTexture = Tx.SongSelect_GenreBar[0],
                    Title = song.TJA.Header.Title,
                    Description = "",
                    Overlay = true
                };
                SongList.Add(bar);
            }
        }

        public static void Draw()
        {
            BoxOpen.Tick();
            for (int i = 0; i < SongList.Count; i++)
            {
                var song = SongList[i];
                song.Draw(200, 100 + 40 * i, 0, 0, true);
            }
        }
    }

    public class SongBar
    {
        public string Title = "";
        public string Description = "";
        public Handle Handle = new Handle();

        public Song Song = new();
        public int GenreNumber = 0;
        public bool Coloring = false, Overlay = false, FullOverlay = false;
        public Texture BarTexture = new();

        public void Draw(double x, double y, double movex, double movey, bool center = false)
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
                double moveoffset = Skin.SongSelect_Bar_Center_Move_X * (1.0 - anime);

                if (SongBars.BoxOpen.Value >= 1300 && SongBars.BoxOpen.Value <= 1940)
                {
                    anime -= Math.Sin(((SongBars.BoxOpen.Value - 1300) * 0.28125f) * (Math.PI / 180));
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
            }
            else
            {

            }

            Drawing.Text(x, y, Title, Handle);
        }
    }
}
