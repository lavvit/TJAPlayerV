using SeaDrop;
using System.Drawing;

namespace TJAPlayerV.taiko
{
    public class Menu
    {
        public static int MenuCount = (int)EMenu.Count - 1; // Number of existing menus
        public static ModeMenu[] Menus = [];
        public static Handle[] MenuHandle = { new(), new() };

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

                Menus[i] = new(i, _mc, MenuHandle[0], MenuHandle[1], _rp, _1pr, _impl, Tx.ModeSelect_Bar, Tx.ModeSelect_Bar_Chara);

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
        public EMenu rp;

        public ModeMenu(int boxId, Color col, Handle tpf, Handle boxpf, EMenu returnPoint, bool _1Ponly, bool impl, Texture[] modeSelect_Bar, Texture[] modeSelect_Bar_Chara)
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
            barTex = (modeSelect_Bar.Length > boxId) ? modeSelect_Bar[boxId] : null;
            barChara = (modeSelect_Bar_Chara.Length > boxId) ? modeSelect_Bar_Chara[boxId] : null;
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
