using SeaDrop;

namespace TJAPlayerV.taiko
{
    /// <summary>
    /// スキン全般を扱うクラス
    /// </summary>
    public class Skin
    {
        public static string Root = Loader.Skin.Root("taiko");
        public static string GraphRoot = $"{Root}\\Graphics\\";
        public static string SoundRoot = $"{Root}\\Sounds\\";

        public static void Load()
        {
            Loader.taiko.Skin.Load();

            string root = GraphRoot;

            string title = $@"{root}1_Title\";
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

            for (int i = 0; i < Menu.MenuCount; i++)
            {
                Tx.ModeSelect_Bar[i] = new(@$"{mode}ModeSelect_Bar_{i.ToString()}.png");
            }

            for (int i = 0; i < Menu.MenuCount; i++)
            {
                Tx.ModeSelect_Bar_Chara[i] = new(@$"{mode}ModeSelect_Bar_Chara_{i.ToString()}.png");
            }

            Tx.ModeSelect_Bar[Menu.MenuCount] = new(@$"{mode}ModeSelect_Bar_Overlay.png");

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

        public static int NamePlate_Ptn_Title;
        public static int[] NamePlate_Ptn_Title_Boxes = [];
        public static int[] SongSelect_NamePlate_X = { 36, 1020, 216, 840, 396 };
        public static int[] SongSelect_NamePlate_Y = { 615, 615, 561, 561, 615 };
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
            Entry_Bar_Text = new()
            ;
        public static Texture[]
            Entry_Card = new Texture[2],
            Entry_Card_Load = new Texture[3],
            Entry_Card_Clear = new Texture[2],
            Entry_Card_Failed = new Texture[2],
            Entry_Header = new Texture[4],
            Entry_Player = new Texture[3],
            ModeSelect_Bar = [],
            ModeSelect_Bar_Chara = []
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
            Join = new Sound[2];
        #endregion
    }
}
