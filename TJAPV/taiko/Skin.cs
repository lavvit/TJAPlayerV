using SeaDrop;
using System.Drawing;

namespace TJAPlayerV.taiko
{
    /// <summary>
    /// スキン全般を扱うクラス
    /// </summary>
    public class Skin
    {
        public static ELoadState LoadStatus = ELoadState.None;
        public static string Root = Loader.Skin.Root("taiko");
        public static string GraphRoot = $"{Root}\\Graphics\\";
        public static string SoundRoot = $"{Root}\\Sounds\\";

        public static void Load()
        {
            LoadStatus = ELoadState.Loading;
            try
            {
                Loader.taiko.Skin.Load();

                string root = GraphRoot;

                string title = $@"{root}1_Title\";
                string song = $@"{root}3_SongSelect\";
                string game = $@"{root}5_Game\";

                for (int i = 0; i < 3; i++)
                {
                    Tx.Network[i] = new($@"{title}NetworkStatus\{i}.png");
                }

                #region Entry
                Tx.Entry_BG = new($@"{title}Background.png");
                Tx.Entry_Bar = new($@"{title}Entry_Bar.png");
                Tx.Entry_Bar_Text = new($@"{title}Entry_Bar_Text.png");
                Tx.Entry_Card[0] = new($@"{title}Bana_NG.png");
                Tx.Entry_Card[1] = new($@"{title}Bana_OK.png");
                Tx.Entry_Card_Load[0] = new($@"{title}Banapas_Load.png");
                Tx.Entry_Card_Load[1] = new($@"{title}Banapas_Load_Text.png");
                Tx.Entry_Card_Load[2] = new($@"{title}Banapas_Load_Anime.png");
                Tx.Entry_Card_Clear[0] = new($@"{title}Banapas_Load_Clear.png");
                Tx.Entry_Card_Clear[1] = new($@"{title}Banapas_Load_Clear_Anime.png");
                Tx.Entry_Card_Failed[0] = new($@"{title}Banapas_Load_Failure.png");
                Tx.Entry_Card_Failed[1] = new($@"{title}Banapas_Load_Clear_Anime.png");
                for (int i = 0; i < 4; i++)
                {
                    Tx.Entry_Header[i] = new($@"{title}Header\{i}.png");
                }
                Tx.Entry_Player[0] = new($@"{title}Entry_Player.png");
                Tx.Entry_Player[1] = new($@"{title}Entry_Player_Select_Bar.png");
                Tx.Entry_Player[2] = new($@"{title}Entry_Player_Select.png");
                #endregion

                #region ModeSelect

                string mode = $@"{title}Mode\";
                Tx.ModeSelect_Bar = new Texture[Menu.MenuCount + 1];
                Tx.ModeSelect_Bar_Chara = new Texture[Menu.MenuCount];
                Tx.ModeSelect_Bar_Text = new Texture[Menu.MenuCount];
                Tx.ModeSelect_Bar_EpText = new Texture[Menu.MenuCount];

                for (int i = 0; i < Menu.MenuCount; i++)
                {
                    if (Directory.Exists(mode))
                    {
                        string box = $@"{mode}{Menu.GetBoxName(i)}\";
                        Tx.ModeSelect_Bar[i] = new(@$"{box}Bar.png");
                        Tx.ModeSelect_Bar_Chara[i] = new(@$"{box}Chara.png");
                        Tx.ModeSelect_Bar_Text[i] = new(@$"{box}Text.png");
                        Tx.ModeSelect_Bar_EpText[i] = new(@$"{box}EpText.png");

                        Tx.ModeSelect_Bar[Menu.MenuCount] = new(@$"{mode}Bar_White.png");
                        Tx.ModeSelect_Bar_Flash = new(@$"{mode}Bar_Flash.png");
                        Tx.ModeSelect_Bar_Frame[0] = new(@$"{mode}WhiteFrame.png");
                        Tx.ModeSelect_Bar_Frame[1] = new(@$"{mode}YellowFrame.png");
                        Tx.ModeSelect_Bar_Frame[2] = new(@$"{mode}YellowFrameBack.png");
                    }
                    else
                    {
                        Tx.ModeSelect_Bar[i] = new(@$"{title}ModeSelect_Bar_{i.ToString()}.png");
                        Tx.ModeSelect_Bar_Chara[i] = new(@$"{title}ModeSelect_Bar_Chara_{i.ToString()}.png");

                        Tx.ModeSelect_Bar[Menu.MenuCount] = new(@$"{title}ModeSelect_Bar_Overlay.png");
                    }
                }

                #endregion

                #region SongSelect
                Tx.SongSelect_BG = new($@"{song}Background.png");
                Tx.SongSelect_GenreBG = new Texture[Count($@"{song}Genre_Background\GenreBackground_")];
                for (int i = 0; i < Tx.SongSelect_GenreBG.Length; i++)
                {
                    Tx.SongSelect_GenreBG[i] = new($@"{song}Genre_Background\GenreBackground_{i.ToString()}.png");
                }
                Tx.SongSelect_GenreBar = new Texture[Count($@"{song}Bar_Genre\Bar_Genre_")];
                for (int i = 0; i < Tx.SongSelect_GenreBar.Length; i++)
                {
                    Tx.SongSelect_GenreBar[i] = new($@"{song}Bar_Genre\Bar_Genre_{i.ToString()}.png");
                }
                Tx.SongSelect_Bar_Back = new($@"{song}Bar_Genre_Back.png");
                Tx.SongSelect_Bar_Overlay = new($@"{song}Bar_Genre_Overlay.png");
                #endregion

                #region NamePlate
                Tx.NamePlate = new($@"{root}NamePlate.png");

                #endregion


                root = SoundRoot;
                string bgm = $@"{root}BGM\";

                Sfx.Don = new($@"{root}Taiko\0\dong.ogg");
                Sfx.Ka = new($@"{root}Taiko\0\ka.ogg");
                Sfx.Decide = new($@"{root}Decide.ogg");
                if (!Sfx.Decide.Enable) Sfx.Decide = Sfx.Don;
                Sfx.Change = new($@"{root}Change.ogg");
                if (!Sfx.Change.Enable) Sfx.Change = Sfx.Ka;
                Sfx.Error = new($@"{root}Error.ogg");

                Sfx.Entry_BGM = new($@"{bgm}Title.ogg");
                Sfx.Entry_BGM_In = new($@"{bgm}Title_Start.ogg");
                Sfx.Entry_Wait = new($@"{root}Entry.ogg");
                Sfx.Entry_Side = new($@"{root}SelectSide.ogg");
                Sfx.Card = new($@"{root}Banapas.ogg");
                if (!Sfx.Card.Enable) Sfx.Card = new($@"{root}CardSuccess.ogg");//
                Sfx.Join[0] = new($@"{root}Join.ogg");
                Sfx.Join[1] = new($@"{root}Join_2P.ogg");

                Sfx.SongSelect_BGM = new($@"{bgm}SongSelect.ogg");
                Sfx.SongSelect_BGM_In = new($@"{bgm}SongSelect_Start.ogg");

                var conf = new Settings($"{Root}SkinConfig.ini");


                Sfx.Mode = new Sound[Menu.MenuCount];
                for (int i = 0; i < Menu.MenuCount; i++)
                {
                    Sfx.Mode[i] = new($@"{root}Entry\{Menu.GetBoxName(i)}.ogg");
                }
                LoadStatus = ELoadState.Success;
            }
            catch (Exception)
            {
                LoadStatus = ELoadState.Error;
            }
        }

        public static int Count(string dir, string prefix = "", string ext = ".png")
        {
            int num = 0;
            while (File.Exists(dir + prefix + num + ext))
            {
                num++;
            }
            return num;
        }
        public static bool Enable(Texture[] texs, int num)
        {
            return texs.Length > num && texs[num].Enable;
        }

        #region Entry
        public static int
            Entry_LoadingPinInstances = 5,
            Entry_LoadingPinFrameCount = 8,
            Entry_LoadingPinCycle = 320;
        public static int[]
            Entry_LoadingPinBase = { 480, 410 },
            Entry_LoadingPinDiff = { 90, 0 },
            Entry_Card_Clear_Anime = { 198, 514 };
        public static int[]
            Entry_BarTextX = { 563, 563 },
            Entry_BarTextY = { 312, 430 };

        public static int[]
            Entry_Player_Select_X = { 337, 529, 743 },
            Entry_Player_Select_Y = { 488, 487, 486 },
            Entry_NamePlate = { 530, 385 };

        public static int[][][] Entry_Player_Select_Rect = [
            [ [ 0, 0, 199, 92 ] ,[ 199, 0, 224, 92 ] ],
            [ [ 0, 92, 199, 92 ] ,[ 199, 92, 224, 92 ] ],
            [ [ 0, 184, 199, 92 ] ,[ 199, 184, 224, 92 ] ]
        ];

        public static int[] ModeSelect_Bar_X = { 290, 319, 356 };
        public static int[] ModeSelect_Bar_Y = { 107, 306, 513 };

        public static int[] ModeSelect_Bar_Offset = { 20, 112 };

        public static int[] ModeSelect_Title_Offset = { 311, 72 };
        public static int[] ModeSelect_Title_Scale = { 36, 15 };

        public static int[] ModeSelect_Bar_Center_X = { 320, 320, 640 };
        public static int[] ModeSelect_Bar_Center_Y = { 338, 360, 360 };
        public static int[][] ModeSelect_Bar_Center_Rect = {
            [ 0, 0, 641, 27 ],
            [ 0, 76, 641, 30 ],
            [ 0, 27, 641, 45 ],
        };

        public static int[] ModeSelect_Bar_Overlay_X = { 320, 320, 640 };
        public static int[] ModeSelect_Bar_Overlay_Y = { 306, 333, 333 };
        public static int[][] ModeSelect_Bar_Overlay_Rect = {
            [ 0, 0, 641, 27 ],
            [ 0, 71, 641, 35 ],
            [ 0, 27, 641, 1 ],
        };

        public static int[] ModeSelect_Bar_Move = { 40, 100 };
        public static int[] ModeSelect_Bar_Move_X = { 0, 0 };
        public static int[] ModeSelect_Overlay_Move = { 40, 120 };
        public static int[] ModeSelect_Overlay_Move_X = { 0, 0 };

        public static int[] ModeSelect_Bar_Chara_X = { 446, 835 };
        public static int[] ModeSelect_Bar_Chara_Y = { 360, 360 };

        public static int ModeSelect_Bar_Chara_Move = 45;

        public static int[] ModeSelect_Bar_Center_Title = { 631, 379 };
        public static int ModeSelect_Bar_Center_Move = 60;
        public static int ModeSelect_Bar_Center_Move_X = 0;

        public static int[] ModeSelect_Bar_Center_BoxText = { 640, 397 };

        public static bool VerticalText = false;
        public static bool VerticalBar = false;
        #endregion

        #region SongSelect
        //public int SongSelect_Overall_Y = 123;
        public static string[] SongSelect_GenreName = { "ポップス", "アニメ", "ゲームバラエティ", "ナムコオリジナル", "クラシック", "バラエティ", "キッズ", "ボーカロイド", "最近遊んだ曲" };

        public static int SongSelect_Bar_Count = 9;

        public static int[] SongSelect_Bar_X = new int[] { 214, 239, 263, 291, 324, 358, 386, 411, 436 };
        public static int[] SongSelect_Bar_Y = new int[] { -127, -36, 55, 145, 314, 485, 574, 665, 756 };
        public static int[] SongSelect_Bar_Anim_X = new int[] { 0, 600, 500, 400, 0, -400, -500, -600, 0 };
        public static int[] SongSelect_Bar_Anim_Y = new int[] { 0, 1800, 1500, 1200, 0, -1200, -1500, -1800, 0 };

        public static float SongSelect_Scroll_Interval = 0.12f;

        public static int[] SongSelect_Bar_Title_Offset = new int[] { 316, 62 };
        public static int[] SongSelect_Bar_Box_Offset = new int[] { 316, 62 };
        public static int[] SongSelect_Bar_BackBox_Offset = new int[] { 316, 62 };
        public static int[] SongSelect_Bar_Random_Offset = new int[] { 316, 62 };
        public static int[] SongSelect_Bar_SubTitle_Offset = new int[] { 316, 90 };

        public static int[] SongSelect_DanStatus_Offset_X = new int[] { 30, 602 };
        public static int[] SongSelect_DanStatus_Offset_Y = new int[] { 30, 30 };

        public static int[] SongSelect_TowerStatus_Offset_X = new int[] { 30, 602 };
        public static int[] SongSelect_TowerStatus_Offset_Y = new int[] { 30, 30 };

        public static int[] SongSelect_RegularCrowns_Offset_X = new int[] { 30, 602 };
        public static int[] SongSelect_RegularCrowns_Offset_Y = new int[] { 30, 30 };

        public static int[] SongSelect_RegularCrowns_ScoreRank_Offset_X = new int[] { 0, 0 };
        public static int[] SongSelect_RegularCrowns_ScoreRank_Offset_Y = new int[] { 0, 30 };

        public static int[] SongSelect_RegularCrowns_Difficulty_Cymbol_Offset_X = new int[] { 22, 22 };
        public static int[] SongSelect_RegularCrowns_Difficulty_Cymbol_Offset_Y = new int[] { 22, 52 };

        public static int[] SongSelect_FavoriteStatus_Offset = new int[] { 90, 30 };

        public static int SongSelect_BoxName_Scale = 28;
        public static int SongSelect_MusicName_Scale = 22;
        public static int SongSelect_Subtitle_Scale = 13;
        public static int SongSelect_BoxText_Scale = 14;
        public static bool SongSelect_VerticalText = false;

        public static int SongSelect_Title_MaxSize = 550;
        public static int SongSelect_SubTitle_MaxSize = 510;

        public static bool SongSelect_Maker_Show = false;
        public static int[] SongSelect_Maker = new int[] { 1285, 190 };
        public static int SongSelect_Maker_Size = 23;
        public static int SongSelect_Maker_MaxSize = 180;

        public static bool SongSelect_BPM_Text_Show = false;
        public static int[] SongSelect_BPM_Text = new int[] { 1240, 20 };
        public static int SongSelect_BPM_Text_MaxSize = 180;
        public static int SongSelect_BPM_Text_Size = 23;

        public static bool SongSelect_Shorten_Frame_Fade = false;
        public static bool SongSelect_Bar_Select_Skip_Fade = false;

        public static int[] SongSelect_Explicit = new int[] { 1240, 60 };
        public static int[] SongSelect_Movie = new int[] { 0, 0 };

        public static int SongSelect_Bar_Center_Move = 62;
        public static int SongSelect_Bar_Center_Move_X = 0;

        public static int[] SongSelect_Bar_Select = new int[] { 309, 235 };

        public static int[] SongSelect_Frame_Score_X = new int[] { 400, 522, 644, 766 };
        public static int[] SongSelect_Frame_Score_Y = new int[] { 228, 228, 228, 228 };

        public static int[] SongSelect_Level_Number_X = new int[] { 485, 607, 729, 851 };
        public static int[] SongSelect_Level_Number_Y = new int[] { 400, 400, 400, 400 };
        public static int[] SongSelect_Level_Number_Tower = new int[] { 485, 400 };
        public static int[] SongSelect_Tower_Side = new int[] { 485, 400 };

        public static int[] SongSelect_Level_X = new int[] { 485, 607, 729, 851 };
        public static int[] SongSelect_Level_Y = new int[] { 400, 400, 400, 400 };
        public static int[] SongSelect_Level_Move = new int[] { 0, -17 };

        public static int[] SongSelect_Unlock_Conditions_Text = new int[] { 72, 128 };

        public static int[] SongSelect_Level_Number_Interval = new int[] { 11, 0 };

        public static float SongSelect_Box_Opening_Interval = 1f;

        public static int[] SongSelect_Difficulty_Select_Title = new int[] { 640, 140 };
        public static int[] SongSelect_Difficulty_Select_SubTitle = new int[] { 640, 180 };

        public static int SongSelect_Box_Chara_Move = 114;

        public static int[] SongSelect_Box_Chara_X = new int[] { 434, 846 };
        public static int[] SongSelect_Box_Chara_Y = new int[] { 360, 360 };

        public static int SongSelect_BoxExplanation_X = 640;
        public static int SongSelect_BoxExplanation_Y = 360;

        public static int SongSelect_BoxExplanation_Interval = 30;

        public static int[] SongSelect_NamePlate_X = new int[] { 36, 1020, 216, 840, 396 };
        public static int[] SongSelect_NamePlate_Y = new int[] { 615, 615, 561, 561, 615 };
        public static int[] SongSelect_Auto_X = new int[] { 60, 950 };
        public static int[] SongSelect_Auto_Y = new int[] { 650, 650 };
        public static int[] SongSelect_ModIcons_X = new int[] { 40, 1020, 220, 840, 400 };
        public static int[] SongSelect_ModIcons_Y = new int[] { 672, 672, 618, 618, 672 };

        public static int[] SongSelect_Timer = new int[] { 1148, 57 };
        public static int[] SongSelect_Timer_Interval = new int[] { 46, 0 };

        public static bool SongSelect_Bpm_Show = false;
        public static int[] SongSelect_Bpm_X = new int[] { 1240, 1240, 1240 };
        public static int[] SongSelect_Bpm_Y = new int[] { 20, 66, 112 };
        public static int[] SongSelect_Bpm_Interval = new int[] { 22, 0 };

        public static bool SongSelect_FloorNum_Show = false;
        public static int SongSelect_FloorNum_X = 1200;
        public static int SongSelect_FloorNum_Y = 205;
        public static int[] SongSelect_FloorNum_Interval = new int[] { 30, 0 };

        public static bool SongSelect_DanInfo_Show = false;
        public static int[] SongSelect_DanInfo_Icon_X = new int[] { 1001, 1001, 1001 };
        public static int[] SongSelect_DanInfo_Icon_Y = new int[] { 269, 309, 349 };
        public static float SongSelect_DanInfo_Icon_Scale = 0.5f;
        public static int[] SongSelect_DanInfo_Difficulty_Cymbol_X = new int[] { 1028, 1028, 1028 };
        public static int[] SongSelect_DanInfo_Difficulty_Cymbol_Y = new int[] { 263, 303, 343 };
        public static float SongSelect_DanInfo_Difficulty_Cymbol_Scale = 0.5f;
        public static int[] SongSelect_DanInfo_Level_Number_X = new int[] { 1040, 1040, 1040 };
        public static int[] SongSelect_DanInfo_Level_Number_Y = new int[] { 267, 307, 347 };
        public static float SongSelect_DanInfo_Level_Number_Scale = 0.5f;
        public static int[] SongSelect_DanInfo_Title_X = new int[] { 1032, 1032, 1032 };
        public static int[] SongSelect_DanInfo_Title_Y = new int[] { 258, 298, 338 };
        public static int SongSelect_DanInfo_Title_Size = 12;
        public static int[] SongSelect_DanInfo_Exam_X = new int[] { 1030, 1030, 1030, 1030, 1030, 1030 };
        public static int[] SongSelect_DanInfo_Exam_Y = new int[] { 398, 426, 454, 482, 510, 538 };
        public static int SongSelect_DanInfo_Exam_Size = 10;
        public static int[] SongSelect_DanInfo_Exam_Value_X = new int[] { 1097, 1162, 1227 };
        public static int[] SongSelect_DanInfo_Exam_Value_Y = new int[] { 388, 416, 444, 472, 500, 528 };
        public static float SongSelect_DanInfo_Exam_Value_Scale = 0.5f;

        public static int[] SongSelect_Table_X = new int[] { 0, 1034, 180, 854, 360 };
        public static int[] SongSelect_Table_Y = new int[] { 0, 0, -204, -204, 0 };

        public static int[] SongSelect_High_Score_X = new int[] { 124, 1158, 304, 978, 484 };
        public static int[] SongSelect_High_Score_Y = new int[] { 416, 416, 212, 212, 416 };

        public static int[] SongSelect_High_Score_Difficulty_Cymbol_X = new int[] { 46, 1080, 226, 900, 406 };
        public static int[] SongSelect_High_Score_Difficulty_Cymbol_Y = new int[] { 418, 418, 214, 214, 418 };

        public static int[][] SongSelect_BoardNumber_X = new int[][] {
            new int[] { 62, 125, 190, 62, 125, 190, 190, 62, 125, -100, 190, 74, 114 },
            new int[] { 1096, 1159, 1224, 1096, 1159, 1224, 1224, 1096, 1159, -100, 1224, 1214, 1148 },

            new int[] { 242, 305, 370, 242, 305, 370, 370, 242, 305, -100, 370, 254, 294 },
            new int[] { 916, 979, 1044, 916, 979, 1044, 1044, 916, 979, -100, 1044, 1034, 968 },
            new int[] { 422, 485, 550, 422, 485, 550, 550, 422, 485, 550, -100, 434, 474 }
        };
        public static int[][] SongSelect_BoardNumber_Y = new int[][] {
            new int[] { 276, 276, 276, 251, 251, 251, 226, 304, 304, -100, 304, 353, 415 },
            new int[] { 276, 276, 276, 251, 251, 251, 226, 304, 304, -100, 304, 353, 415 },
            new int[] { 72,72,72,47,47,47,22,100,100, -100, 100, 149,211 },
            new int[] { 72,72,72,47,47,47,22,100,100, -100, 100, 149,211 },
            new int[] { 276, 276, 276, 251, 251, 251, 226, 304, 304, -100, 304, 353, 415 }
        };
        public static int[] SongSelect_BoardNumber_Interval = new int[] { 9, 0 };

        public static int[] SongSelect_SongNumber_X = new int[] { 1090, 1183 };
        public static int[] SongSelect_SongNumber_Y = new int[] { 167, 167 };
        public static int[] SongSelect_SongNumber_Interval = new int[] { 16, 0 };

        public static int[] SongSelect_Search_Bar_X = new int[] { 640, 640, 640, 640, 640 };
        public static int[] SongSelect_Search_Bar_Y = new int[] { 320, 420, 520, 620, 720 };

        public static int[] SongSelect_Difficulty_Back = new int[] { 640, 290 };
        public static int[] SongSelect_Level_Offset = new int[] { 610, 40 };
        public Color[] SongSelect_Difficulty_Colors = new Color[] {
            ColorTranslator.FromHtml("#88d2fd"),
            ColorTranslator.FromHtml("#58de85"),
            ColorTranslator.FromHtml("#ffc224"),
            ColorTranslator.FromHtml("#d80b2c"),
            ColorTranslator.FromHtml("#9065e2"),
            ColorTranslator.FromHtml("#e9943b"),
            ColorTranslator.FromHtml("#3b55a5")
        };

        public int[] SongSelect_Difficulty_Bar_X = new int[] { 255, 341, 426, 569, 712, 855, 855 };
        public int[] SongSelect_Difficulty_Bar_Y = new int[] { 270, 270, 270, 270, 270, 270, 270 };
        public int[] SongSelect_Branch_Text_Offset = new int[] { 276, 6 };
        public int[] SongSelect_Branch_Offset = new int[] { 6, 6 };

        public int[][] SongSelect_Difficulty_Bar_Rect = new int[][] {
            new int[] { 0, 0, 86, 236 },
            new int[] { 86, 0, 86, 236 },
            new int[] { 171, 0, 138, 236 },
            new int[] { 314, 0, 138, 236 },
            new int[] { 457, 0, 138, 236 },
            new int[] { 600, 0, 138, 236 },
            new int[] { 743, 0, 138, 236 },
        };

        public int[] SongSelect_Difficulty_Star_X = new int[] { 444, 587, 730, 873, 873 };
        public int[] SongSelect_Difficulty_Star_Y = new int[] { 459, 459, 459, 459, 459 };
        public int[] SongSelect_Difficulty_Star_Interval = new int[] { 10, 0 };

        public int[] SongSelect_Difficulty_Number_X = new int[] { 498, 641, 784, 927, 927 };
        public int[] SongSelect_Difficulty_Number_Y = new int[] { 435, 435, 435, 435, 435 };
        public int[] SongSelect_Difficulty_Number_Interval = new int[] { 11, 0 };

        public int[][] SongSelect_Difficulty_Crown_X = new int[][] {
            new int[] { 445, 589, 733, 877, 877 },
            new int[] { 519, 663, 807, 951, 951 },
        };
        public int[][] SongSelect_Difficulty_Crown_Y = new int[][] {
            new int[] { 284, 284, 284, 284, 284 },
            new int[] { 284, 284, 284, 284, 284 },
        };

        public int[][] SongSelect_Difficulty_ScoreRank_X = new int[][] {
            new int[] { 467, 611, 755, 899, 899 },
            new int[] { 491, 635, 779, 923, 923 },
        };
        public int[][] SongSelect_Difficulty_ScoreRank_Y = new int[][] {
            new int[] { 281, 281, 281, 281, 281 },
            new int[] { 281, 281, 281, 281, 281 },
        };

        public int[] SongSelect_Difficulty_Select_Bar_X = new int[] { 163, 252, 367, 510, 653, 796, 796 };
        public int[] SongSelect_Difficulty_Select_Bar_Y = new int[] { 176, 176, 176, 176, 176, 176, 176 };

        public int[] SongSelect_Difficulty_Select_Bar_Back_X = new int[] { 163, 252, 367, 510, 653, 796, 796 };
        public int[] SongSelect_Difficulty_Select_Bar_Back_Y = new int[] { 242, 242, 242, 242, 242, 242, 242 };

        public int[][] SongSelect_Difficulty_Select_Bar_Rect = new int[][] {
            new int[] { 0, 0, 259, 114 },
            new int[] { 0, 114, 259, 275 },
            new int[] { 0, 387, 259, 111 },
        };

        public int[] SongSelect_Difficulty_Select_Bar_Anime = new int[] { 0, 10 };
        public int[] SongSelect_Difficulty_Select_Bar_AnimeIn = new int[] { 0, 50 };
        public int[] SongSelect_Difficulty_Select_Bar_Move = new int[] { 25, 0 };

        public int[] SongSelect_Difficulty_Bar_ExExtra_AnimeDuration = new int[] { -1, -1 };

        public int[] SongSelect_Preimage = new int[] { 120, 110 };
        public int[] SongSelect_Preimage_Size = new int[] { 200, 200 };

        public int[] SongSelect_Option_Select_Offset = new int[] { 0, -286 };

        public int SongSelect_Option_Font_Scale = 13;
        public int[] SongSelect_Option_OptionType_X = new int[] { 16, 1004 };
        public int[] SongSelect_Option_OptionType_Y = new int[] { 93, 93 };
        public int[] SongSelect_Option_Value_X = new int[] { 200, 1188 };
        public int[] SongSelect_Option_Value_Y = new int[] { 93, 93 };
        public int[] SongSelect_Option_Interval = new int[] { 0, 41 };

        public int[] SongSelect_Option_ModMults1_X = new int[] { 108, 1096 };
        public int[] SongSelect_Option_ModMults1_Y = new int[] { 11, 11 };

        public int[] SongSelect_Option_ModMults2_X = new int[] { 108, 1096 };
        public int[] SongSelect_Option_ModMults2_Y = new int[] { 52, 52 };


        public int[] SongSelect_NewHeya_Close_Select = new int[] { 0, 0 };

        public int[] SongSelect_NewHeya_PlayerPlate_X = new int[] { 0, 256, 513, 770, 1026 };
        public int[] SongSelect_NewHeya_PlayerPlate_Y = new int[] { 66, 66, 66, 66, 66 };

        public int[] SongSelect_NewHeya_ModeBar_X = new int[] { 0, 256, 513, 770, 1026 };
        public int[] SongSelect_NewHeya_ModeBar_Y = new int[] { 200, 200, 200, 200, 200 };
        public int[] SongSelect_NewHeya_ModeBar_Font_Offset = new int[] { 128, 33 };


        public int SongSelect_NewHeya_Box_Count = 7;
        public int[] SongSelect_NewHeya_Box_X = new int[] { -424, -120, 184, 488, 792, 1096, 1400 };
        public int[] SongSelect_NewHeya_Box_Y = new int[] { 273, 273, 273, 273, 273, 273, 273 };
        public int[] SongSelect_NewHeya_Box_Chara_Offset = new int[] { 152, 200 };
        public int[] SongSelect_NewHeya_Box_Name_Offset = new int[] { 152, 386 };
        public int[] SongSelect_NewHeya_Box_Author_Offset = new int[] { 152, 413 };
        public int[] SongSelect_NewHeya_Lock_Offset = new int[] { 0, 73 };
        public int[] SongSelect_NewHeya_InfoSection_Offset = new int[] { 152, 206 };

        public Color SongSelect_ForeColor_JPOP = ColorTranslator.FromHtml("#FFFFFF");
        public Color SongSelect_ForeColor_Anime = ColorTranslator.FromHtml("#FFFFFF");
        public Color SongSelect_ForeColor_VOCALOID = ColorTranslator.FromHtml("#FFFFFF");
        public Color SongSelect_ForeColor_Children = ColorTranslator.FromHtml("#FFFFFF");
        public Color SongSelect_ForeColor_Variety = ColorTranslator.FromHtml("#FFFFFF");
        public Color SongSelect_ForeColor_Classic = ColorTranslator.FromHtml("#FFFFFF");
        public Color SongSelect_ForeColor_GameMusic = ColorTranslator.FromHtml("#FFFFFF");
        public Color SongSelect_ForeColor_Namco = ColorTranslator.FromHtml("#FFFFFF");
        public Color SongSelect_BackColor_JPOP = ColorTranslator.FromHtml("#01455B");
        public Color SongSelect_BackColor_Anime = ColorTranslator.FromHtml("#99001F");
        public Color SongSelect_BackColor_VOCALOID = ColorTranslator.FromHtml("#5B6278");
        public Color SongSelect_BackColor_Children = ColorTranslator.FromHtml("#9D3800");
        public Color SongSelect_BackColor_Variety = ColorTranslator.FromHtml("#366600");
        public Color SongSelect_BackColor_Classic = ColorTranslator.FromHtml("#875600");
        public Color SongSelect_BackColor_GameMusic = ColorTranslator.FromHtml("#412080");
        public Color SongSelect_BackColor_Namco = ColorTranslator.FromHtml("#980E00");

        public string[] SongSelect_CorrectionX_Chara = { "ここにX座標を補正したい文字をカンマで区切って記入" };
        public string[] SongSelect_CorrectionY_Chara = { "ここにY座標を補正したい文字をカンマで区切って記入" };
        public int SongSelect_CorrectionX_Chara_Value = 0;
        public int SongSelect_CorrectionY_Chara_Value = 0;
        public string[] SongSelect_Rotate_Chara = { "ここに90℃回転させたい文字をカンマで区切って記入" };

        #endregion

        public static int NamePlate_Ptn_Title;
        public static int[] NamePlate_Ptn_Title_Boxes = [];
    }

    /// <summary>
    /// 画像を扱うクラス
    /// </summary>
    public class Tx
    {
        public static Texture[]
            Network = new Texture[3];
        #region Entry
        public static Texture
            Entry_BG = new(),
            Entry_Bar = new(),
            Entry_Bar_Text = new(),
            ModeSelect_Bar_Flash = new()
            ;
        public static Texture[]
            Entry_Card = new Texture[2],
            Entry_Card_Load = new Texture[3],
            Entry_Card_Clear = new Texture[2],
            Entry_Card_Failed = new Texture[2],
            Entry_Header = new Texture[4],
            Entry_Player = new Texture[3],
            ModeSelect_Bar = [],
            ModeSelect_Bar_Chara = [],
            ModeSelect_Bar_Text = [],
            ModeSelect_Bar_EpText = [],
            ModeSelect_Bar_Frame = new Texture[3]
            ;
        #endregion

        #region SongSelect
        public static Texture
            SongSelect_BG = new(),
            SongSelect_Bar_Back = new(),
            SongSelect_Bar_Overlay = new()

            ;
        public static Texture[]
            SongSelect_GenreBG = [],
            SongSelect_GenreBar = []
            ;
        #endregion

        #region NamePlate
        public static Texture NamePlate = new();
        #endregion
    }

    /// <summary>
    /// 音を扱うクラス
    /// </summary>
    public class Sfx
    {
        public static Sound
            Don = new(),
            Ka = new(),
            Decide = new(),
            Change = new(),
            Error = new()
            ;

        #region Entry
        public static Sound
            Entry_BGM = new(),
            Entry_BGM_In = new(),
            Entry_Wait = new(),
            Entry_Side = new(),
            Card = new(),

            SongSelect_BGM = new(),
            SongSelect_BGM_In = new()
            ;
        public static Sound[]
            Join = new Sound[2],
            Mode = [];
        #endregion
    }
}
