using SeaDrop;
using static SeaDrop.Drawing;
using static TJAPlayerV.taiko.Sfx;
using static TJAPlayerV.taiko.Skin;
using static TJAPlayerV.taiko.Tx;

namespace TJAPlayerV.taiko
{
    public class Entry : Scene
    {
        public static Counter CoinIn = new(0, 2000, 1000, true),
            LoadedTimer = new(0, 3655),
            FailedTimer = new(0, 4000),

            BarSelectTimer = new(0, 1055),
            BarAnimation = new (0, 1055);

        public static bool Loading, SaveLoaded, PlayerEntry, PlayerDecide, ModeSelect;
        public static int PlayerCursor = 1;

        public override void Enable()
        {
            Skin.Load();
            Entry_BGM_In.Play();
            CoinIn.Start();
            base.Enable();
        }
        public override void Disable()
        {
            base.Disable();
        }

        public override void Draw()
        {
            Entry_BG.Draw(0, 0);
            Entry_Card[1]?.Draw(0, 0);
            Entry_Header[2]?.Draw(0, 0);
            Network[0]?.Draw(0, 0);
            if (!SaveLoaded)
            {
                #region Wait
                if (!Loading)
                {
                    Entry_Bar.Draw(0, 0);

                    CoinIn.Tick();
                    double bartextopa;
                    if (CoinIn.Value <= 255)
                        bartextopa = CoinIn.Value;
                    else if (CoinIn.Value <= 2000 - 355)
                        bartextopa = 255;
                    else
                        bartextopa = 255 - (CoinIn.Value - (2000 - 355));
                    Entry_Bar_Text.BlendDepth = (int)bartextopa;

                    Entry_Bar_Text.SetRectangle(0, 0, Entry_Bar_Text.Width, Entry_Bar_Text.Height / 2);
                    Entry_Bar_Text.Draw(Entry_BarTextX[0], Entry_BarTextY[0]);
                    Entry_Bar_Text.SetRectangle(0, Entry_Bar_Text.Height / 2, Entry_Bar_Text.Width, Entry_Bar_Text.Height / 2);
                    Entry_Bar_Text.Draw(Entry_BarTextX[1], Entry_BarTextY[1]);
                }
                #endregion
                #region Loading
                else
                {
                    LoadedTimer.Tick();
                    FailedTimer.Tick();
                    if (LoadedTimer.Value <= 1000 && FailedTimer.Value <= 1128)
                    {
                        if (Loading)
                        {
                            Blackout(LoadedTimer.Value <= 2972 ? 0.5 : 0.5 - (LoadedTimer.Value - 2972) / 255.0);

                            Entry_Card_Load[0].BlendDepth = (int)(LoadedTimer.Value >= 872 ? 255 - (LoadedTimer.Value - 872) * 2 : LoadedTimer.Value * 2);
                            Entry_Card_Load[0].XYScale = (1.0, LoadedTimer.Value <= 100 ? LoadedTimer.Value * 0.01f : 1.0f);
                            Entry_Card_Load[0].Draw(0, 0);

                            Entry_Card_Load[1].BlendDepth = (int)(LoadedTimer.Value >= 872 ? 255 - (LoadedTimer.Value - 872) * 2 : LoadedTimer.Value <= 96 ? (int)((LoadedTimer.Value - 96) * 7.96875f) : 255);
                            Entry_Card_Load[1].Draw(0, 0);

                            if (Entry_Card_Load[2] != null)
                            {
                                int step = Entry_Card_Load[2].Width / Entry_LoadingPinFrameCount;
                                int cycle = Entry_LoadingPinCycle;
                                int _stamp = (int)((LoadedTimer.Value - 200) % (Entry_LoadingPinInstances * cycle));

                                for (int i = 0; i < Entry_LoadingPinInstances; i++)
                                {
                                    Entry_Card_Load[2].BlendDepth = (int)(LoadedTimer.Value >= 872 ? 255 - (LoadedTimer.Value - 872) * 2 : LoadedTimer.Value <= 96 ? (int)((LoadedTimer.Value - 96) * 7.96875f) : 255);
                                    Entry_Card_Load[2].ReferencePoint = ReferencePoint.Center;
                                    int re = (_stamp + -i * cycle) / (cycle / Entry_LoadingPinFrameCount);
                                    int rec = _stamp >= i * cycle ? _stamp <= (i + 1) * cycle ? re : 0 : 0;
                                    Entry_Card_Load[2].SetRectangle(step * rec,
                                            0,
                                            step,
                                            Entry_Card_Load[2].Height);
                                    Entry_Card_Load[2].Draw(
                                        Entry_LoadingPinBase[0] + Entry_LoadingPinDiff[0] * i,
                                        Entry_LoadingPinBase[1] + Entry_LoadingPinDiff[1] * i);
                                    //Text(Entry_LoadingPinBase[0] + Entry_LoadingPinDiff[0] * i, Entry_LoadingPinBase[1] + Entry_LoadingPinDiff[1] * i + 40, re, 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Loading)
                        {
                            Blackout(LoadedTimer.Value <= 2972 ? 0.6 : 0.6 - (LoadedTimer.Value - 2972) / 255.0);

                            if (Card.NonPlayed) Card.Play();

                            int count = (int)LoadedTimer.Value - 1000;
                            Entry_Card_Clear[0].BlendDepth = count >= 1872 ? 255 - (count - 1872) * 2 : count * 2;
                            Entry_Card_Clear[0].XYScale = (1.0, count <= 100 ? count * 0.01f : 1.0f);
                            Entry_Card_Clear[0].Draw(0, 0);

                            float anime = 0f;
                            float scalex = 0f;
                            float scaley = 0f;

                            if (count >= 300)
                            {
                                if (count <= 300 + 270)
                                {
                                    anime = (float)Math.Sin((float)(count - 300) / 1.5f * (Math.PI / 180)) * 95f;
                                    scalex = -(float)Math.Sin((float)(count - 300) / 1.5f * (Math.PI / 180)) * 0.15f;
                                    scaley = (float)Math.Sin((float)(count - 300) / 1.5f * (Math.PI / 180)) * 0.2f;
                                }
                                else if (count <= 300 + 270 + 100)
                                {
                                    scalex = (float)Math.Sin((float)(count - (300 + 270)) * 1.8f * (Math.PI / 180)) * 0.13f;
                                    scaley = -(float)Math.Sin((float)(count - (300 + 270)) * 1.8f * (Math.PI / 180)) * 0.1f;
                                    anime = 0;
                                }
                                else if (count <= 300 + 540 + 100)
                                {
                                    anime = (float)Math.Sin((float)(count - (300 + 270 + 100)) / 1.5f * (Math.PI / 180)) * 95f;
                                    scalex = -(float)Math.Sin((float)(count - (300 + 270 + 100)) / 1.5f * (Math.PI / 180)) * 0.15f;
                                    scaley = (float)Math.Sin((float)(count - (300 + 270 + 100)) / 1.5f * (Math.PI / 180)) * 0.2f;
                                }
                                else if (count <= 300 + 540 + 100 + 100)
                                {
                                    scalex = (float)Math.Sin((float)(count - (300 + 540 + 100)) * 1.8f * (Math.PI / 180)) * 0.13f;
                                    scaley = -(float)Math.Sin((float)(count - (300 + 540 + 100)) * 1.8f * (Math.PI / 180)) * 0.1f;
                                }
                            }

                            Entry_Card_Clear[1].XYScale = (1.0f + scalex, 1.0f + scaley);
                            Entry_Card_Clear[1].ReferencePoint = ReferencePoint.BottomCenter;
                            Entry_Card_Clear[1].BlendDepth = count >= 1872 ? 255 - (count - 1872) * 2 : count * 2;
                            Entry_Card_Clear[1].Draw(Entry_Card_Clear_Anime[0], Entry_Card_Clear_Anime[1] - anime);

                            if (LoadedTimer.Value >= 2000)
                            {
                                PlayerEntry = true;
                            }
                        }
                    }
                }
                #endregion

            }
            #region Player
            if (PlayerEntry)
            {
                /*if (!this.bキャラカウンター初期化)
                {
                    //this.ctキャラエントリーループ = new CCounter(0, Chara_Entry.Length - 1, 1000 / 60, TJAPlayer3.Timer);
                    CMenuCharacter.tMenuResetTimer(CMenuCharacter.ECharacterAnimation.ENTRY);

                    this.bキャラカウンター初期化 = true;
                }*/

                BarSelectTimer.Tick();
                int alpha = (int)(BarSelectTimer.Value >= 800 ? 255 - (BarSelectTimer.Value - 800) : (LoadedTimer.Value - 3400));

                Entry_Player[0].BlendDepth = alpha;
                Entry_Player[1].BlendDepth = alpha;

                Entry_Player[0].Draw(0, 0);

                /*int _actual = TJAPlayer3.GetActualPlayer(0);

                int _charaId = TJAPlayer3.SaveFileInstances[_actual].data.Character;

                int chara_x = Entry_NamePlate[0] + NamePlateBase.szTextureSize.Width / 2;
                int chara_y = Entry_NamePlate[1];

                int puchi_x = chara_x + TJAPlayer3.Skin.Adjustments_MenuPuchichara_X[0];
                int puchi_y = chara_y + TJAPlayer3.Skin.Adjustments_MenuPuchichara_Y[0];

                CMenuCharacter.tMenuDisplayCharacter(
                    0,
                    chara_x,
                    chara_y,
                    CMenuCharacter.ECharacterAnimation.ENTRY, alpha
                    );

                this.PuchiChara.On進行描画(puchi_x, puchi_y, false, alpha);*/

                Entry_Player[2].BlendDepth = (int)(BarSelectTimer.Value >= 800 ? 255 - (BarSelectTimer.Value - 800) :
                            (LoadedTimer.Value - 3400) - (BarSelectTimer.Value <= 255 ? BarSelectTimer.Value : 255 - (BarSelectTimer.Value - 255)));
                Entry_Player[2].SetRectangle(Entry_Player_Select_Rect[0][PlayerCursor == 1 ? 1 : 0][0],
                    Entry_Player_Select_Rect[0][PlayerCursor == 1 ? 1 : 0][1],
                    Entry_Player_Select_Rect[0][PlayerCursor == 1 ? 1 : 0][2],
                    Entry_Player_Select_Rect[0][PlayerCursor == 1 ? 1 : 0][3]);
                Entry_Player[2].Draw(Entry_Player_Select_X[PlayerCursor], Entry_Player_Select_Y[PlayerCursor]);

                Entry_Player[2].BlendDepth = alpha;
                Entry_Player[2].SetRectangle(Entry_Player_Select_Rect[1][PlayerCursor == 1 ? 1 : 0][0],
                    Entry_Player_Select_Rect[1][PlayerCursor == 1 ? 1 : 0][1],
                    Entry_Player_Select_Rect[1][PlayerCursor == 1 ? 1 : 0][2],
                    Entry_Player_Select_Rect[1][PlayerCursor == 1 ? 1 : 0][3]);
                Entry_Player[2].Draw(Entry_Player_Select_X[PlayerCursor], Entry_Player_Select_Y[PlayerCursor]);

                Entry_Player[1].Draw(0, 0);

                #region [ 透明度 ]

                int BlendDepth = 0;

                if (BarSelectTimer.Value <= 100)
                    BlendDepth = (int)(BarSelectTimer.Value * 2.55f);
                else if (BarSelectTimer.Value <= 200)
                    BlendDepth = 255 - (int)((BarSelectTimer.Value - 100) * 2.55f);
                else if (BarSelectTimer.Value <= 300)
                    BlendDepth = (int)((BarSelectTimer.Value - 200) * 2.55f);
                else if (BarSelectTimer.Value <= 400)
                    BlendDepth = 255 - (int)((BarSelectTimer.Value - 300) * 2.55f);
                else if (BarSelectTimer.Value <= 500)
                    BlendDepth = (int)((BarSelectTimer.Value - 400) * 2.55f);
                else if (BarSelectTimer.Value <= 600)
                    BlendDepth = 255 - (int)((BarSelectTimer.Value - 500) * 2.55f);

                #endregion

                Entry_Player[2].BlendDepth = BlendDepth;
                Entry_Player[2].SetRectangle(Entry_Player_Select_Rect[2][PlayerCursor == 1 ? 1 : 0][0],
                    Entry_Player_Select_Rect[2][PlayerCursor == 1 ? 1 : 0][1],
                    Entry_Player_Select_Rect[2][PlayerCursor == 1 ? 1 : 0][2],
                    Entry_Player_Select_Rect[2][PlayerCursor == 1 ? 1 : 0][3]);
                Entry_Player[2].Draw(Entry_Player_Select_X[PlayerCursor], Entry_Player_Select_Y[PlayerCursor]);

                BlendDepth = (int)(BarSelectTimer.Value >= 800 ? 255 - (BarSelectTimer.Value - 800) : (LoadedTimer.Value - 3400));
                if (BlendDepth > 0)
                {
                    if (Entry_Side.NonPlayed) Entry_Side.Play();
                    NamePlate.Draw(Entry_NamePlate[0], Entry_NamePlate[1]);//, 0, true, BlendDepth);
                }
            }
            #endregion

            #region Mode
            if (ModeSelect)
            {
                BarAnimation.Tick();
            }
            #endregion

            base.Draw();
        }

        public override void Update()
        {
            if (Entry_BGM_In.Played)
            {
                Entry_BGM.PlayLoopUp();
                if (!Loading) Entry_Wait.PlayLoopUp();
            }

            if (LoadedTimer.State == 0)
            {
                if (Key.IsPushed(EKey.Esc))
                {
                    if (Loading)
                    {
                        LoadedTimer.Reset();
                        Loading = false;
                        SaveLoaded = false;
                        PlayerEntry = false;
                        PlayerDecide = false;
                        ModeSelect = false;
                        Card.Stop();
                        Card.Time = 0;
                        Entry_Side.Stop();
                        Entry_Side.Time = 0;
                        CoinIn.Start();
                        LoadedTimer.Reset();
                        BarSelectTimer.Reset();
                    }
                    else DXLib.End();
                }
                if (Key.IsPushed(EKey.F) || Key.IsPushed(EKey.J))
                {
                    if (!Loading)
                    {
                        Entry_Wait.Stop();
                        LoadedTimer.Start();
                        Loading = true;
                        //bキャラカウンター初期化 = false;
                    }
                    else if (PlayerCursor == 0 || PlayerCursor == 2)
                    {
                        if (!PlayerDecide)
                        {
                            Decide.Play();
                            BarSelectTimer.Start();
                            PlayerDecide = true;
                            /*TJAPlayer3.PlayerSide = (PlayerCursor == 2) ? 1 : 0;
                            if (TJAPlayer3.PlayerSide == 1)
                                TJAPlayer3.ConfigIni.nPlayerCount = 1;*/
                            SaveLoaded = true;
                        }
                    }
                    else
                    {
                        Decide.Play();
                        Loading = false;
                        PlayerEntry = false;
                        Card.Stop();
                        Card.Time = 0;
                        Entry_Side.Stop();
                        Entry_Side.Time = 0;
                        CoinIn.Start();
                        LoadedTimer.Reset();
                    }
                }
                if (Key.IsPushed(EKey.K))
                {
                    if (PlayerEntry && !PlayerDecide && LoadedTimer.Value == LoadedTimer.End)
                    {
                        if (PlayerCursor + 1 <= 2)
                        {
                            Change.Play();
                            PlayerCursor++;
                        }
                    }
                }
                if (Key.IsPushed(EKey.D))
                {
                    if (PlayerEntry && !PlayerDecide && LoadedTimer.Value == LoadedTimer.End)
                    {
                        if (PlayerCursor - 1 >= 0)
                        {
                            Change.Play();
                            PlayerCursor--;
                        }
                    }
                }
                if (BarSelectTimer.Value >= 1055)
                {
                    if (!ModeSelect)
                    {
                        Join[PlayerCursor == 2 ? 1 : 0].Play();

                        /*if (TJAPlayer3.Skin.voiceTitleSanka[TJAPlayer3.SaveFile] != null && !TJAPlayer3.Skin.voiceTitleSanka[TJAPlayer3.SaveFile].bPlayed)
                            TJAPlayer3.Skin.voiceTitleSanka[TJAPlayer3.SaveFile]?.tPlay();*/

                        //ctキャライン.Start(0, 180, 2, TJAPlayer3.Timer);
                        //ctBarAnimeIn.Start(0, 1295, 1, TJAPlayer3.Timer);
                        ModeSelect = true;
                    }
                }
            }

            base.Update();
        }
    }
}