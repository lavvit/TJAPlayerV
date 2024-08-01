using SeaDrop;
using System.Drawing;
using static TJAPlayerV.taiko.Entry;
using static TJAPlayerV.taiko.Sfx;

namespace TJAPlayerV.taiko
{
    public class ModeSelect
    {
        public static bool IsEnable;
        public static int Cursor, MenuCount, Downed;
        private static int[] UsedMenu = [];
        private static int[] MenuPos = [];

        public static int Players = 1;

        public static Counter BarAnimeIn = new(0, 1295, 1000), BarMove = new(0, 250, 1200), CharaIn = new(0, 180, 2000);

        public static void Draw()
        {
            DrawBase();

            BarAnimeIn.Tick();
            BarMove.Tick();
            CharaIn.Tick();

            if (BarAnimeIn.Value >= (int)(16 * 16.6f))
            {
                // act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, BarMove.n現在の値.ToString());

                //for (int i = 0; i < this.nbModes; i++)
                for (int i = 0; i < MenuCount; i++)
                {

                    // Get Menu reference
                    var _menu = Menu.Menus[UsedMenu[i]];
                    if (_menu == null) continue;
                    Texture _bar = _menu.barTex;
                    Texture _chara = _menu.barChara;

                    if (MenuPos[i] == 1 && (BarMove.Value >= 150 || BarAnimeIn.State > 0)) _menu.DrawCenter((int)BarMove.Value, BarAnimeIn.State > 0);
                    else
                    {
                        var point = GetBarPoint(i);
                        _menu.DrawSide(point.X, point.Y);
                    }
                }
            }

            for (int player = 0; player < Players; player++)//ConfigIni.nPlayerCount
            {
                if (player >= 2) continue;

                NamePlate.Draw(Skin.SongSelect_NamePlate_X[player], Skin.SongSelect_NamePlate_Y[player], player, false, 255);
            }
        }

        public static void Update()
        {
            if (Entry_BGM_In.Played)
            {
                Entry_BGM.PlayLoopUp();
            }

            if (Key.IsPushed(EKey.Esc))
            {
                LoadedTimer.Reset();
                Loading = false;
                SaveLoaded = false;
                PlayerEntry = false;
                PlayerDecide = false;
                IsEnable = false;
                Card.Stop();
                Card.Time = 0;
                Entry_Side.Stop();
                Entry_Side.Time = 0;
                CoinIn.Start();
                LoadedTimer.Reset();
                BarSelectTimer.Reset();
                BarAnimeIn.Reset();
                CharaIn.Reset();
                Downed = 0;
            }

            if (Key.IsPushed(EKey.K))
            {
                if (Cursor < MenuCount - 1)
                {
                    Menu.Menus[UsedMenu[Cursor]].barSound.Stop();
                    Change.Play();
                    BarMove.Reset();
                    BarMove.Start();
                    Cursor++;
                    Downed = 1;

                    for (int i = 0; i < MenuCount; i++)
                    {
                        MenuPos[i] = i + 1 - Cursor;
                    }
                    Menu.Menus[UsedMenu[Cursor]].barSound.Play();
                }
            }
            if (Key.IsPushed(EKey.D))
            {
                if (Cursor > 0)
                {
                    Menu.Menus[UsedMenu[Cursor]].barSound.Stop();
                    Change.Play();
                    BarMove.Reset();
                    BarMove.Start();
                    Cursor--;
                    Downed = -1;

                    for (int i = 0; i < MenuCount; i++)
                    {
                        MenuPos[i] = i + 1 - Cursor;
                    }
                    Menu.Menus[UsedMenu[Cursor]].barSound.Play();
                }
            }

            if (Key.IsPushed(EKey.F) || Key.IsPushed(EKey.J))
            {
                bool select = false;
                var menu = Menu.Menus[UsedMenu[Cursor]];
                switch (menu.rp)
                {
                    case EMenu.DANGAMESTART:
                    case EMenu.TAIKOTOWERSSTART:
                        if (Players == 1)// && Songs.DanList.Count > 0
                        {
                            select = true;
                        }
                        break;
                    default:
                        if (menu.implemented && (Players == 1 || !menu._1pRestricted))
                        {
                            select = true;
                        }
                        break;
                }

                if (select)
                {
                    Decide.Play();

                    //Move
                    Thread.Sleep(1000);
                    DXLib.SceneChange(new SongSelect());
                }
                else Error.Play();
            }

            if (BarAnimeIn.Value >= 200 && BarMove.State == 0 && BarMove.Value == BarMove.Begin)
            {
                BarMove.Start();
            }
        }

        public static void Init()
        {
            UsedMenu = [
                    10,
                    0,
                    1,/*
                    2,
                    10,
                    5,
                    3,
                    9,
                    8,
                    6,
                    7,*/

					// -- Debug
					/*
					11,
					12,
					13,
					*/
				];

            MenuCount = UsedMenu.Length;
            Cursor = 1;

            MenuPos = new int[MenuCount];
            for (int i = 0; i < MenuCount; i++)
            {
                MenuPos[i] = i + 1 - Cursor;
            }
        }
        public static void Start()
        {
            Join[PlayerCursor == 2 ? 1 : 0].Play();

            CharaIn.Start();
            BarAnimeIn.Start();
            BarMove.Reset();
            //BarMove.Start();
            IsEnable = true;
        }

        private static PointF GetBarPoint(int n)
        {
            float movevalue = 14 * 16.6f;
            int xoffset = 100, yoffset = 400;

            float BarAnimeX = xoffset;
            float BarAnimeY = yoffset;
            if (BarAnimeIn.Value >= movevalue + 100)
            {
                BarAnimeX = 0;
                BarAnimeY = 0;
                if (BarAnimeIn.Value <= movevalue + 299 && MenuPos[n] != 1)
                {
                    BarAnimeX = xoffset - (BarAnimeIn.Value - (movevalue + 100)) * 0.5f;
                    BarAnimeY = yoffset - (BarAnimeIn.Value - (movevalue + 100)) * 2.0f;
                }
            }
            if (MenuPos[n] < 1)
            {
                BarAnimeX *= -1;
                BarAnimeY *= -1;
            }

            float BarMoveX = 0;
            float BarMoveY = 0;

            #region [Position precalculation]

            //int CurrentPos = this.stModeBar[i].n現在存在している行;
            int CurrentPos = MenuPos[n];
            int Selected = CurrentPos;

            if (Downed == 1)
                Selected = CurrentPos + 1;
            else if (Downed == -1)
                Selected = CurrentPos - 1;

            Point pos = getFixedPositionForBar(CurrentPos);
            Point posSelect = getFixedPositionForBar(Selected);

            #endregion

            int px = pos.X - posSelect.X;
            int py = pos.Y - posSelect.Y;

            BarMoveX = BarMove.Value <= 100 ? px - (BarMove.Value / 100f * px) : 0;
            BarMoveY = BarMove.Value <= 100 ? py - (BarMove.Value / 100f * py) : 0;

            float x = pos.X + BarAnimeX - BarMoveX;
            float y = pos.Y + BarAnimeY - BarMoveY;

            return new PointF(x, y);
        }

        private static Point getFixedPositionForBar(int CurrentPos)
        {
            int posX;
            int posY;

            if (CurrentPos >= 0 && CurrentPos < 3)
            {
                posX = Skin.ModeSelect_Bar_X[CurrentPos];
                posY = Skin.ModeSelect_Bar_Y[CurrentPos];
            }
            else if (CurrentPos < 0)
            {
                posX = Skin.ModeSelect_Bar_X[0] + CurrentPos * Skin.ModeSelect_Bar_Offset[0];
                posY = Skin.ModeSelect_Bar_Y[0] + CurrentPos * Skin.ModeSelect_Bar_Offset[1];
            }
            else
            {
                posX = Skin.ModeSelect_Bar_X[2] + (CurrentPos - 2) * Skin.ModeSelect_Bar_Offset[0];
                posY = Skin.ModeSelect_Bar_Y[2] + (CurrentPos - 2) * Skin.ModeSelect_Bar_Offset[1];
            }

            return new Point(posX, posY);
        }
    }
}
