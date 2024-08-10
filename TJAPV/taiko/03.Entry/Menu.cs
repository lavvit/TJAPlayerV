using SeaDrop;
using System.Drawing;
using static TJAPlayerV.taiko.Tx;

namespace TJAPlayerV.taiko
{
    public class Menu
    {
        public static int MenuCount = (int)EMenu.Count - 1; // Number of existing menus
        public static ModeMenu[] Menus = [];
        public static Handle[] MenuHandle = { new(), new() };

        public static string GetBoxName(int boxid)
        {
            switch (boxid)
            {
                case 0:
                default:
                    return "Enso";
                case 1:
                    return "Dan";
                case 2:
                    return "Tower";
                case 3:
                    return "Shop";
                case 4:
                    return "Story";
                case 5:
                    return "Heya";
                case 6:
                    return "Settings";
                case 7:
                    return "Exit";
                case 8:
                    return "Online";
                case 9:
                    return "Document";
                case 10:
                    return "AI";
                case 11:
                    return "Stats";
                case 12:
                    return "Editor";
                case 13:
                    return "Tools";
            }
        }

        public static string GetBoxText(int boxid, bool isTitle = true)
        {
            string append = isTitle ? "" : "_DESC";
            switch (boxid)
            {
                case 0:
                default:
                    return Language.Get($"TITLE_MODE_TAIKO{append}");
                case 1:
                    return Language.Get($"TITLE_MODE_DAN{append}");
                case 2:
                    return Language.Get($"TITLE_MODE_TOWER{append}");
                case 3:
                    return Language.Get($"TITLE_MODE_SHOP{append}");
                case 4:
                    return Language.Get($"TITLE_MODE_STORY{append}");
                case 5:
                    return Language.Get($"TITLE_MODE_HEYA{append}");
                case 6:
                    return Language.Get($"TITLE_MODE_SETTINGS{append}");
                case 7:
                    return Language.Get($"TITLE_MODE_EXIT{append}");
                case 8:
                    return Language.Get($"TITLE_MODE_ONLINE{append}");
                case 9:
                    return Language.Get($"TITLE_MODE_DOCUMENT{append}");
                case 10:
                    return Language.Get($"TITLE_MODE_AI{append}");
                case 11:
                    return Language.Get($"TITLE_MODE_STATS{append}");
                case 12:
                    return Language.Get($"TITLE_MODE_EDITOR{append}");
                case 13:
                    return Language.Get($"TITLE_MODE_TOOLS{append}");
            }
        }

        public static void Init()
        {
            Menus = new ModeMenu[MenuCount];

            #region [Menu Colors]

            Color[] __MenuColors =
            {
                Color.FromArgb(233, 53, 71),
                Color.FromArgb(71, 64, 135),
                Color.FromArgb(255, 180, 42),
                Color.FromArgb(16, 255, 255),
                Color.FromArgb(128, 0, 128),
                Color.FromArgb(24, 128, 24),
                Color.FromArgb(128, 128, 128),
                Color.FromArgb(72, 72, 72),
                Color.FromArgb(199, 8, 119), // Online lounge red/pink
                Color.FromArgb(181, 186, 28),  // Encyclopedia yellow
                Color.FromArgb(78, 166, 171), // AI battle mode blue
                Color.FromArgb(230, 230, 230), // Player stats white
                Color.FromArgb(40, 40, 40), // Chart editor black
                Color.FromArgb(120, 104, 56), // Toolbox brown
            };

            #endregion

            #region [Return points]

            EMenu[] __rps =
            {
                    EMenu.GAMESTART,
                    EMenu.DANGAMESTART,
                    EMenu.AIBATTLEMODE,
                    /*EMenu.TAIKOTOWERSSTART,
                    EMenu.SHOPSTART,
                    EMenu.BOUKENSTART,
                    EMenu.HEYA,
                    EMenu.CONFIG,
                    EMenu.EXIT,
                    EMenu.ONLINELOUNGE,
                    EMenu.ENCYCLOPEDIA,
                    EMenu.AIBATTLEMODE,
                    EMenu.PLAYERSTATS,
                    EMenu.CHARTEDITOR,
                    EMenu.TOOLBOX,*/
                };

            #endregion

            #region [Extra bools]

            bool[] _1PRestricts =
            {
                false,
                true,
                false,
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                true,
                false,
            };

            // To edit while new features are implemented
            bool[] _implemented =
            {
                    true,
                    true,
                    false,
                    false,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    false,
                    false,
                    false,
            };

            #endregion

            string fontpath = $@"{DXLib.AppPath}\Lang\{Language.NowLang}\";
            MenuHandle[0] = new Handle(fontpath + Language.Get("FontName"), Skin.ModeSelect_Title_Scale[0], 1, 6, false, EFontType.AntialiasingEdge);
            MenuHandle[1] = new Handle(fontpath + Language.Get("BoxFontName"), Skin.ModeSelect_Title_Scale[1], 1, 4, false, EFontType.AntialiasingEdge);

            for (int i = 0; i < MenuCount; i++)
            {
                EMenu _rp = (i >= __rps.Length) ? EMenu.GAMESTART : __rps[i];
                Color _mc = (i >= __MenuColors.Length) ? Color.White : __MenuColors[i];
                bool _1pr = (i >= _1PRestricts.Length) ? false : _1PRestricts[i];
                bool _impl = (i >= _implemented.Length) ? false : _implemented[i];

                Menus[i] = new(i, _mc, MenuHandle[0], MenuHandle[1], _rp, _1pr, _impl,
                    ModeSelect_Bar, ModeSelect_Bar_Chara, ModeSelect_Bar_Text, ModeSelect_Bar_EpText, Sfx.Mode);

            }
        }
    }

    public class MenuFont
    {
        public Handle Handle = new();
        public int Col = 0xffffff, ECol = 0, Width;
        public string Text;

        public MenuFont(string str文字, Handle cPrivateFastFont, Color forecolor, Color backcolor, int maxHeight = -1, Color? secondEdge = null)
        {
            Text = str文字;
            Handle = cPrivateFastFont;
            Col = Drawing.Color(forecolor);
            ECol = Drawing.Color(backcolor);
            Width = maxHeight;
            //this.secondEdge = secondEdge;
        }

        public void Draw(double x, double y)
        {
            Drawing.Text(x, y, Text, Col, Handle, ECol);
        }
    }

    public class ModeMenu
    {
        public string Name;
        public string Description;

        public MenuFont ttkTitle;
        public MenuFont ttkBoxText;
        public bool _1pRestricted;
        public bool implemented;
        public Texture barTex;
        public Texture barChara;
        public Texture barText;
        public Texture barEp;
        public Sound barSound;
        public EMenu rp;

        public ModeMenu(int boxId, Color col, Handle tpf, Handle boxpf, EMenu returnPoint, bool _1Ponly, bool impl, Texture[] modeSelect_Bar, Texture[] modeSelect_Bar_Chara, Texture[] modeSelect_Bar_Text, Texture[] modeSelect_Bar_EpText, Sound[] bar_Sound)
        {
            string title = Menu.GetBoxText(boxId);

            Name = title;
            ttkTitle = new MenuFont(title, tpf, Color.White, col, 1280, Color.Black);

            string boxText = Menu.GetBoxText(boxId, false);

            Description = boxText;
            ttkBoxText = new MenuFont(boxText, boxpf, Color.White, Color.Black, 1000);

            rp = returnPoint;

            _1pRestricted = _1Ponly;
            implemented = impl;
            barTex = (modeSelect_Bar.Length > boxId) ? modeSelect_Bar[boxId] : new();
            barChara = (modeSelect_Bar_Chara.Length > boxId) ? modeSelect_Bar_Chara[boxId] : new();
            barText = (modeSelect_Bar_Text.Length > boxId) ? modeSelect_Bar_Text[boxId] : new();
            barEp = (modeSelect_Bar_EpText.Length > boxId) ? modeSelect_Bar_EpText[boxId] : new();
            barSound = (bar_Sound.Length > boxId) ? bar_Sound[boxId] : new();
        }

        public void DrawCenter(int timer, bool start)
        {
            float barAnimef = start ? 1 : (timer / 100.0f) - 1.5f;

            float barAnime = Skin.ModeSelect_Bar_Move[0] +
                (barAnimef * (Skin.ModeSelect_Bar_Move[1] - Skin.ModeSelect_Bar_Move[0]));

            float barAnimeX = Skin.ModeSelect_Bar_Move_X[0] +
                (barAnimef * (Skin.ModeSelect_Bar_Move_X[1] - Skin.ModeSelect_Bar_Move_X[0]));

            float overlayAnime = Skin.ModeSelect_Overlay_Move[0] +
                (barAnimef * (Skin.ModeSelect_Overlay_Move[1] - Skin.ModeSelect_Overlay_Move[0]));

            float overlayAnimeX = Skin.ModeSelect_Overlay_Move_X[0] +
                (barAnimef * (Skin.ModeSelect_Overlay_Move_X[1] - Skin.ModeSelect_Overlay_Move_X[0]));

            float anime = 0;
            float BarAnimeCount = (timer - 150) / 100.0f;

            if (BarAnimeCount <= 0.45)
                anime = BarAnimeCount * 3.333333333f;
            else
                anime = 1.50f - (BarAnimeCount - 0.45f) * 0.61764705f;
            anime *= Skin.ModeSelect_Bar_Chara_Move;
            anime -= 52;

            int opa = (int)(BarAnimeCount * 255f) + (int)(barAnimef * 2.5f);

            if (barTex.Enable)
            {
                barTex.BlendDepth = 255;
                barTex.Scale = 1.0f;
                if (barTex.Height > ModeSelect_Bar[Menu.MenuCount].Height)
                {
                    int width = start ? (int)(barTex.Width / 2 + anime / 2) : barTex.Width / 2;
                    int ani = start ? (int)(anime / 2.0) : 0;
                    int height = barTex.Height / 2;

                    barTex.ReferencePoint = ReferencePoint.TopLeft;
                    barTex.Rectangle = new Rectangle(0, 0, width, height);
                    barTex.Draw(Skin.ModeSelect_Bar_Center_X[0] - (Skin.VerticalBar ? barAnimeX : 0) - ani,
                        Skin.ModeSelect_Bar_Center_Y[0] - (Skin.VerticalBar ? 0 : barAnime));
                    barTex.Rectangle = new Rectangle(0, barTex.Height - height, width, height);
                    barTex.Draw(Skin.ModeSelect_Bar_Center_X[0] - (Skin.VerticalBar ? barAnimeX : 0) - ani,
                        Skin.ModeSelect_Bar_Center_Y[0] - Skin.ModeSelect_Bar_Move[1] * 2.0 + (Skin.VerticalBar ? 0 : barAnime) + height);

                    barTex.Rectangle = new Rectangle(barTex.Width - width, 0, width, height);
                    barTex.Draw(Skin.ModeSelect_Bar_Center_X[0] - (Skin.VerticalBar ? barAnimeX : 0) + width - ani,
                        Skin.ModeSelect_Bar_Center_Y[0] - (Skin.VerticalBar ? 0 : barAnime));
                    barTex.Rectangle = new Rectangle(barTex.Width - width, barTex.Height - height, width, height);
                    barTex.Draw(Skin.ModeSelect_Bar_Center_X[0] - (Skin.VerticalBar ? barAnimeX : 0) + width - ani,
                        Skin.ModeSelect_Bar_Center_Y[0] - Skin.ModeSelect_Bar_Move[1] * 2.0 + (Skin.VerticalBar ? 0 : barAnime) + height);
                }
                else
                {
                    barTex.ReferencePoint = ReferencePoint.TopLeft;
                    barTex.Rectangle = new Rectangle(Skin.ModeSelect_Bar_Center_Rect[0][0],
                        Skin.ModeSelect_Bar_Center_Rect[0][1],
                        Skin.ModeSelect_Bar_Center_Rect[0][2],
                        Skin.ModeSelect_Bar_Center_Rect[0][3]);
                    barTex.XYScale = null;
                    barTex.Draw(Skin.ModeSelect_Bar_Center_X[0] - (Skin.VerticalBar ? barAnimeX : 0),
                        Skin.ModeSelect_Bar_Center_Y[0] - (Skin.VerticalBar ? 0 : barAnime));

                    barTex.Rectangle = new Rectangle(Skin.ModeSelect_Bar_Center_Rect[1][0],
                        Skin.ModeSelect_Bar_Center_Rect[1][1],
                        Skin.ModeSelect_Bar_Center_Rect[1][2],
                        Skin.ModeSelect_Bar_Center_Rect[1][3]);
                    barTex.Draw(Skin.ModeSelect_Bar_Center_X[1] + (Skin.VerticalBar ? barAnimeX : 0),
                        Skin.ModeSelect_Bar_Center_Y[1] + (Skin.VerticalBar ? 0 : barAnime));

                    if (Skin.VerticalBar)
                    {
                        barTex.XYScale = ((barAnimeX / Skin.ModeSelect_Bar_Center_Rect[2][2]) * 2.0f, 1.0);
                    }
                    else
                    {
                        barTex.XYScale = (1.0, (barAnime / Skin.ModeSelect_Bar_Center_Rect[2][3]) * 2.0f);
                    }

                    barTex.Rectangle = new Rectangle(Skin.ModeSelect_Bar_Center_Rect[2][0],
                        Skin.ModeSelect_Bar_Center_Rect[2][1],
                        Skin.ModeSelect_Bar_Center_Rect[2][2],
                        Skin.ModeSelect_Bar_Center_Rect[2][3]);
                    barTex.ReferencePoint = ReferencePoint.Center;
                    barTex.Draw(Skin.ModeSelect_Bar_Center_X[2], Skin.ModeSelect_Bar_Center_Y[2]);
                }
            }


            if (ModeSelect_Bar[Menu.MenuCount].Enable)
            {
                Texture _overlap = ModeSelect_Bar[Menu.MenuCount];

                _overlap.Scale = 1.0f;
                _overlap.Rectangle = new Rectangle(Skin.ModeSelect_Bar_Overlay_Rect[0][0],
                    Skin.ModeSelect_Bar_Overlay_Rect[0][1],
                    Skin.ModeSelect_Bar_Overlay_Rect[0][2],
                    Skin.ModeSelect_Bar_Overlay_Rect[0][3]);
                _overlap.XYScale = null;
                _overlap.Draw(Skin.ModeSelect_Bar_Overlay_X[0], Skin.ModeSelect_Bar_Overlay_Y[0]);

                _overlap.Rectangle = new Rectangle(Skin.ModeSelect_Bar_Overlay_Rect[1][0],
                    Skin.ModeSelect_Bar_Overlay_Rect[1][1],
                    Skin.ModeSelect_Bar_Overlay_Rect[1][2],
                    Skin.ModeSelect_Bar_Overlay_Rect[1][3]);
                _overlap.Draw(Skin.ModeSelect_Bar_Overlay_X[1] + (Skin.VerticalBar ? overlayAnimeX : 0),
                    Skin.ModeSelect_Bar_Overlay_Y[1] + (Skin.VerticalBar ? 0 : overlayAnime));

                if (Skin.VerticalBar)
                {
                    _overlap.XYScale = (overlayAnimeX / Skin.ModeSelect_Bar_Overlay_Rect[2][2], 1.0);
                }
                else
                {
                    _overlap.XYScale = (1.0, overlayAnime / Skin.ModeSelect_Bar_Overlay_Rect[2][3]);
                }

                _overlap.ReferencePoint = ReferencePoint.TopCenter;
                _overlap.Rectangle = new Rectangle(Skin.ModeSelect_Bar_Overlay_Rect[2][0],
                    Skin.ModeSelect_Bar_Overlay_Rect[2][1],
                    Skin.ModeSelect_Bar_Overlay_Rect[2][2],
                    Skin.ModeSelect_Bar_Overlay_Rect[2][3]);
                _overlap.Draw(Skin.ModeSelect_Bar_Overlay_X[2], Skin.ModeSelect_Bar_Overlay_Y[2]);

            }

            if (barChara != null)
            {
                barChara.BlendDepth = opa;
                barChara.ReferencePoint = ReferencePoint.Center;

                double x = DXLib.Width / 2 - barChara.Width / 4;

                barChara.Rectangle = new Rectangle(0, 0, barChara.Width / 2, barChara.Height);
                barChara.Draw(DXLib.Width / 2 - barChara.Width / 4 - anime, DXLib.Height / 2);
                barChara.Rectangle = new Rectangle(barChara.Width / 2, 0, barChara.Width / 2, barChara.Height);
                barChara.Draw(DXLib.Width / 2 + barChara.Width / 4 + anime, DXLib.Height / 2);
            }
            if (barText != null && barText.Enable)
            {
                barText.BlendDepth = !start ? 255 : opa;
                barText.ReferencePoint = ReferencePoint.Center;
                barText.Rectangle = null;
                barText.XYScale = null;
                barText.Draw(DXLib.Width / 2, DXLib.Height / 2 + Skin.ModeSelect_Bar_Center_Move - (Skin.ModeSelect_Bar_Center_Move * BarAnimeCount));
            }
            else Drawing.Text(Skin.ModeSelect_Bar_Center_Title[0] + (Skin.ModeSelect_Bar_Center_Move_X * BarAnimeCount),
                Skin.ModeSelect_Bar_Center_Title[1] - (Skin.ModeSelect_Bar_Center_Move * BarAnimeCount),
                Name, ttkTitle.Col, ttkTitle.Handle, ttkTitle.ECol, ReferencePoint.BottomCenter,
                BarAnimeCount);

            if (barEp != null && barEp.Enable)
            {
                barEp.BlendDepth = opa;
                barEp.ReferencePoint = ReferencePoint.Center;
                barEp.Rectangle = null;
                barEp.XYScale = null;
                barEp.Draw(DXLib.Width / 2, DXLib.Height / 2);
            }
            else if (ttkBoxText != null)
            {
                Drawing.Text(Skin.ModeSelect_Bar_Center_BoxText[0], Skin.ModeSelect_Bar_Center_BoxText[1],
                Description, ttkBoxText.Col, ttkBoxText.Handle, ttkBoxText.ECol, ReferencePoint.Center,
                BarAnimeCount);
            }
        }

        public void DrawSide(double x, double y)
        {
            if (barTex.Enable)
            {
                barTex.BlendDepth = 255;
                barTex.Scale = 1.0f;
                barTex.XYScale = null;
                if (barTex.Height > ModeSelect_Bar[Menu.MenuCount].Height)
                {
                    barTex.ReferencePoint = ReferencePoint.TopLeft;
                    int height = ModeSelect_Bar[Menu.MenuCount].Height / 2;
                    barTex.Rectangle = new Rectangle(0, 0, barTex.Width, height);
                    barTex.Draw(x, y);
                    barTex.Rectangle = new Rectangle(0, barTex.Height - height, barTex.Width, height);
                    barTex.Draw(x, y + height);
                }
                else
                {
                    barTex.Rectangle = null;
                    barTex.ReferencePoint = ReferencePoint.TopLeft;
                    barTex.Draw(x, y);
                }
            }

            if (ModeSelect_Bar[Menu.MenuCount].Enable)
            {
                Texture _overlap = ModeSelect_Bar[Menu.MenuCount];

                _overlap.BlendDepth = 255;
                _overlap.Scale = 1.0f;
                _overlap.Rectangle = null;
                _overlap.XYScale = null;
                _overlap.ReferencePoint = ReferencePoint.TopLeft;
                _overlap.Draw(x, y);
            }

            if (barText != null && barText.Enable)
            {
                barText.BlendDepth = 255;
                barText.Scale = 1.0f;
                barText.Rectangle = null;
                barText.XYScale = null;
                barText.ReferencePoint = ReferencePoint.Center;
                barText.Draw(x + 321, y + 54 + ModeSelect_Bar[Menu.MenuCount].Height / 1.5);
            }
            else Drawing.Text(x + Skin.ModeSelect_Title_Offset[0], y + Skin.ModeSelect_Title_Offset[1],
                Name, ttkTitle.Col, ttkTitle.Handle, ttkTitle.ECol, ReferencePoint.BottomCenter);

            //stageSongSelect.actSongList.ResolveTitleTexture(ttkTitle, Skin.VerticalText)?.t2D中心基準描画(pos.X + BarAnimeX - BarMoveX + Skin.ModeSelect_Offset[0], pos.Y + BarAnimeY - BarMoveY + Skin.ModeSelect_Offset[1]);
        }
    }

    public enum EMenu
    {
        継続 = 0,
        GAMESTART,
        DANGAMESTART, TAIKOTOWERSSTART,
        SHOPSTART,
        BOUKENSTART,
        HEYA,
        CONFIG,
        EXIT,
        ONLINELOUNGE,
        ENCYCLOPEDIA,
        AIBATTLEMODE,
        PLAYERSTATS,
        CHARTEDITOR,
        TOOLBOX,
        Count
    }
}
