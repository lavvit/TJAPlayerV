using SeaDrop;
using System.Drawing;
using static TJAPlayerV.taiko.Entry;
using static TJAPlayerV.taiko.Sfx;
using static TJAPlayerV.taiko.Tx;

namespace TJAPlayerV.taiko
{
    public class ModeSelect
    {
        public static bool IsEnable;
        public static int Cursor, MenuCount, Downed;
        private static int[] UsedMenu = [];
        private static int[] MenuPos = [];

        public static Counter BarAnimeIn = new(0, 1295), BarMove = new(0, 250, 1200), CharaIn = new(0, 180, 2000);

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
                    Texture _bar = _menu.barTex;
                    Texture _chara = _menu.barChara;

                    #region [Disable visualy 1p specific buttons if 2p]

                    /*if ((_menu._1pRestricted == true && ConfigIni.nPlayerCount > 1)
                        || _menu.implemented == false)
                    {
                        if (_bar != null)
                            _bar.color4 = CConversion.ColorToColor4(Color.DarkGray);
                        if (_chara != null)
                            _chara.color4 = CConversion.ColorToColor4(Color.DarkGray);
                        stageSongSelect.actSongList.ResolveTitleTexture(_menu.ttkBoxText, Skin.VerticalText, true).color4 = CConversion.ColorToColor4(Color.DarkGray);
                        stageSongSelect.actSongList.ResolveTitleTexture(_menu.ttkTitle, Skin.VerticalText).color4 = CConversion.ColorToColor4(Color.DarkGray);
                    }
                    else
                    {
                        if (_bar != null)
                            _bar.color4 = CConversion.ColorToColor4(Color.White);
                        if (_chara != null)
                            _chara.color4 = CConversion.ColorToColor4(Color.White);
                        stageSongSelect.actSongList.ResolveTitleTexture(_menu.ttkBoxText, Skin.VerticalText, true).color4 = CConversion.ColorToColor4(Color.White);
                        stageSongSelect.actSongList.ResolveTitleTexture(_menu.ttkTitle, Skin.VerticalText).color4 = CConversion.ColorToColor4(Color.White);
                    }*/

                    #endregion

                    // if (this.stModeBar[i].n現在存在している行 == 1 && BarMove.n現在の値 >= 150)
                    if (MenuPos[i] == 1 && BarMove.Value >= 150)
                    {
                        float barAnimef = (BarMove.Value / 100.0f) - 1.5f;

                        float barAnime = Skin.ModeSelect_Bar_Move[0] +
                            (barAnimef * (Skin.ModeSelect_Bar_Move[1] - Skin.ModeSelect_Bar_Move[0]));

                        float barAnimeX = Skin.ModeSelect_Bar_Move_X[0] +
                            (barAnimef * (Skin.ModeSelect_Bar_Move_X[1] - Skin.ModeSelect_Bar_Move_X[0]));

                        float overlayAnime = Skin.ModeSelect_Overlay_Move[0] +
                            (barAnimef * (Skin.ModeSelect_Overlay_Move[1] - Skin.ModeSelect_Overlay_Move[0]));

                        float overlayAnimeX = Skin.ModeSelect_Overlay_Move_X[0] +
                            (barAnimef * (Skin.ModeSelect_Overlay_Move_X[1] - Skin.ModeSelect_Overlay_Move_X[0]));



                        //int BarAnime = BarAnimeIn.n現在の値 >= (int)(26 * 16.6f) + 100 ? 0 : BarAnimeIn.n現在の値 >= (int)(26 * 16.6f) && BarAnimeIn.n現在の値 <= (int)(26 * 16.6f) + 100 ? 40 + (int)((BarAnimeIn.n現在の値 - (26 * 16.6)) / 100f * 71f) : BarAnimeIn.n現在の値 < (int)(26 * 16.6f) ? 40 : 111;
                        //int BarAnime1 = BarAnime == 0 ? BarMove.n現在の値 >= 150 ? 40 + (int)((BarMove.n現在の値 - 150) / 100f * 71f) : BarMove.n現在の値 < 150 ? 40 : 111 : 0;

                        if (_bar != null)
                        {
                            _bar.BlendDepth = 255;
                            _bar.Scale = 1.0f;
                            _bar.ReferencePoint = ReferencePoint.TopLeft;
                            _bar.Rectangle = new Rectangle(Skin.ModeSelect_Bar_Center_Rect[0][0],
                                Skin.ModeSelect_Bar_Center_Rect[0][1],
                                Skin.ModeSelect_Bar_Center_Rect[0][2],
                                Skin.ModeSelect_Bar_Center_Rect[0][3]);
                            _bar.Draw(Skin.ModeSelect_Bar_Center_X[0] - (Skin.VerticalBar ? barAnimeX : 0),
                                Skin.ModeSelect_Bar_Center_Y[0] - (Skin.VerticalBar ? 0 : barAnime));

                            _bar.Rectangle = new Rectangle(Skin.ModeSelect_Bar_Center_Rect[1][0],
                                Skin.ModeSelect_Bar_Center_Rect[1][1],
                                Skin.ModeSelect_Bar_Center_Rect[1][2],
                                Skin.ModeSelect_Bar_Center_Rect[1][3]);
                            _bar.Draw(Skin.ModeSelect_Bar_Center_X[1] + (Skin.VerticalBar ? barAnimeX : 0),
                                Skin.ModeSelect_Bar_Center_Y[1] + (Skin.VerticalBar ? 0 : barAnime));

                            if (Skin.VerticalBar)
                            {
                                _bar.XYScale = ((barAnimeX / Skin.ModeSelect_Bar_Center_Rect[2][2]) * 2.0f, 1.0);
                            }
                            else
                            {
                                _bar.XYScale = (1.0, (barAnime / Skin.ModeSelect_Bar_Center_Rect[2][3]) * 2.0f);
                            }

                            _bar.Rectangle = new Rectangle(Skin.ModeSelect_Bar_Center_Rect[2][0],
                                Skin.ModeSelect_Bar_Center_Rect[2][1],
                                Skin.ModeSelect_Bar_Center_Rect[2][2],
                                Skin.ModeSelect_Bar_Center_Rect[2][3]);
                            _bar.ReferencePoint = ReferencePoint.Center;
                            _bar.Draw(Skin.ModeSelect_Bar_Center_X[2], Skin.ModeSelect_Bar_Center_Y[2]);
                        }


                        if (ModeSelect_Bar[Menu.MenuCount] != null)
                        {
                            Texture _overlap = ModeSelect_Bar[Menu.MenuCount];

                            _overlap.Scale = 1.0f;
                            _overlap.Rectangle = new Rectangle(Skin.ModeSelect_Bar_Overlay_Rect[0][0],
                                Skin.ModeSelect_Bar_Overlay_Rect[0][1],
                                Skin.ModeSelect_Bar_Overlay_Rect[0][2],
                                Skin.ModeSelect_Bar_Overlay_Rect[0][3]);
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


                        float anime = 0;
                        float BarAnimeCount = (BarMove.Value - 150) / 100.0f;

                        if (BarAnimeCount <= 0.45)
                            anime = BarAnimeCount * 3.333333333f;
                        else
                            anime = 1.50f - (BarAnimeCount - 0.45f) * 0.61764705f;
                        anime *= Skin.ModeSelect_Bar_Chara_Move;

                        if (_chara != null)
                        {
                            _chara.BlendDepth = (int)(BarAnimeCount * 255f) + (int)(barAnimef * 2.5f);
                            _chara.ReferencePoint = ReferencePoint.Center;
                            _chara.Rectangle = new Rectangle(0, 0, _chara.Width / 2, _chara.Height);
                            _chara.Draw(Skin.ModeSelect_Bar_Chara_X[0] - anime, Skin.ModeSelect_Bar_Chara_Y[0]);
                            _chara.Rectangle = new Rectangle(_chara.Width / 2, 0, _chara.Width / 2, _chara.Height);
                            _chara.Draw(Skin.ModeSelect_Bar_Chara_X[1] + anime, Skin.ModeSelect_Bar_Chara_Y[1]);
                        }
                        Drawing.Text(Skin.ModeSelect_Bar_Center_Title[0] + (Skin.ModeSelect_Bar_Center_Move_X * BarAnimeCount),
                            Skin.ModeSelect_Bar_Center_Title[1] - (Skin.ModeSelect_Bar_Center_Move * BarAnimeCount),
                            _menu.Name, _menu.ttkTitle.Col, _menu.ttkTitle.Handle, _menu.ttkTitle.ECol, false, ReferencePoint.Center,
                            BarAnimeCount);

                        if (_menu.ttkBoxText != null)
                        {
                            Drawing.Text(Skin.ModeSelect_Bar_Center_BoxText[0], Skin.ModeSelect_Bar_Center_BoxText[1],
                            _menu.Description, _menu.ttkBoxText.Col, _menu.ttkBoxText.Handle, _menu.ttkBoxText.ECol, false, ReferencePoint.Center,
                            BarAnimeCount);
                        }

                        /*stageSongSelect.actSongList.ResolveTitleTexture(_menu.ttkTitle, Skin.VerticalText)?.t2D中心基準描画(
                            Skin.ModeSelect_Bar_Center_Title[0] + (Skin.ModeSelect_Bar_Center_Move_X * BarAnimeCount),
                            Skin.ModeSelect_Bar_Center_Title[1] - (Skin.ModeSelect_Bar_Center_Move * BarAnimeCount));

                        Texture currentText = stageSongSelect.actSongList.ResolveTitleTexture(_menu.ttkBoxText, Skin.VerticalText, true);
                        if (currentText != null)
                        {
                            currentText.Opacity = (int)(BarAnimeCount * 255f);
                            currentText?.t2D中心基準描画(Skin.ModeSelect_Bar_Center_BoxText[0], Skin.ModeSelect_Bar_Center_BoxText[1]);
                        }*/
                    }
                    else
                    {
                        int BarAnimeY = (int)(BarAnimeIn.Value >= (int)(26 * 16.6f) + 100 && BarAnimeIn.Value <= (int)(26 * 16.6f) + 299 ? 600 - (BarAnimeIn.Value - (int)(26 * 16.6f + 100)) * 3 : BarAnimeIn.Value >= (int)(26 * 16.6f) + 100 ? 0 : 600);
                        int BarAnimeX = BarAnimeIn.Value >= (int)(26 * 16.6f) + 100 && BarAnimeIn.Value <= (int)(26 * 16.6f) + 299 ? 100 - (int)((BarAnimeIn.Value - (int)(26 * 16.6f + 100)) * 0.5f) : BarAnimeIn.Value >= (int)(26 * 16.6f) + 100 ? 0 : 100;

                        int BarMoveX = 0;
                        int BarMoveY = 0;

                        #region [Position precalculation]

                        //int CurrentPos = this.stModeBar[i].n現在存在している行;
                        int CurrentPos = MenuPos[i];
                        int Selected = CurrentPos;

                        if (Downed == 1)
                            Selected = CurrentPos + 1;
                        else if (Downed == -1)
                            Selected = CurrentPos - 1;

                        Point pos = getFixedPositionForBar(CurrentPos);
                        Point posSelect = getFixedPositionForBar(Selected);

                        #endregion

                        BarMoveX = BarMove.Value <= 100 ? (int)(pos.X - posSelect.X) - (int)(BarMove.Value / 100f * (pos.X - posSelect.X)) : 0;
                        BarMoveY = BarMove.Value <= 100 ? (int)(pos.Y - posSelect.Y) - (int)(BarMove.Value / 100f * (pos.Y - posSelect.Y)) : 0;


                        if (_bar != null)
                        {
                            _bar.Scale = 1.0f;
                            _bar.Draw(pos.X + BarAnimeX - BarMoveX, pos.Y + BarAnimeY - BarMoveY);
                        }

                        if (ModeSelect_Bar[Menu.MenuCount] != null)
                        {
                            Texture _overlap = ModeSelect_Bar[Menu.MenuCount];

                            _overlap.Scale = 1.0f;
                            _overlap.Draw(pos.X + BarAnimeX - BarMoveX, pos.Y + BarAnimeY - BarMoveY);
                        }


                        Drawing.Text(pos.X + BarAnimeX - BarMoveX + Skin.ModeSelect_Title_Offset[0], pos.Y + BarAnimeY - BarMoveY + Skin.ModeSelect_Title_Offset[1],
                            _menu.Name, 0xffffff, _menu.ttkTitle.Handle, 0, false, ReferencePoint.Center);

                        //stageSongSelect.actSongList.ResolveTitleTexture(_menu.ttkTitle, Skin.VerticalText)?.t2D中心基準描画(pos.X + BarAnimeX - BarMoveX + Skin.ModeSelect_Offset[0], pos.Y + BarAnimeY - BarMoveY + Skin.ModeSelect_Offset[1]);
                    }
                }
            }

            for (int player = 0; player < 1; player++)//ConfigIni.nPlayerCount
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
                    Change.Play();
                    BarMove.Reset();
                    BarMove.Start();
                    Cursor++;
                    Downed = 1;

                    for (int i = 0; i < MenuCount; i++)
                    {
                        MenuPos[i] = i + 1 - Cursor;
                    }
                }
            }
            if (Key.IsPushed(EKey.D))
            {
                if (Cursor > 0)
                {
                    Change.Play();
                    BarMove.Reset();
                    BarMove.Start();
                    Cursor--;
                    Downed = -1;

                    for (int i = 0; i < MenuCount; i++)
                    {
                        MenuPos[i] = i + 1 - Cursor;
                    }
                }
            }

            if (Key.IsPushed(EKey.F) || Key.IsPushed(EKey.J))
            {
                /*bool operationSucceded = false;

                if (Menu.Menus[usedMenus[this.n現在の選択行モード選択]].rp == E戻り値.DANGAMESTART || Menu.Menus[usedMenus[this.n現在の選択行モード選択]].rp == E戻り値.TAIKOTOWERSSTART)
                {
                    if (Songs管理.list曲ルート_Dan.Count > 0 && ConfigIni.nPlayerCount == 1)
                        operationSucceded = true;
                }
                else if (Menu.Menus[usedMenus[this.n現在の選択行モード選択]].implemented == true
                    && (Menu.Menus[usedMenus[this.n現在の選択行モード選択]]._1pRestricted == false
                    || ConfigIni.nPlayerCount == 1))
                    operationSucceded = true;

                if (operationSucceded == true)
                {
                    Skin.soundDecideSFX.tPlay();
                    this.actFO.tフェードアウト開始(0, 500);
                    base.ePhaseID = CStage.EPhase.Common_FADEOUT;
                }
                else
                    Skin.soundError.tPlay();*/
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
