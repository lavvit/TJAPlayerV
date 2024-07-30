using SeaDrop;

namespace Loader.taiko
{
    public class Skin
    {
        public static string Root = Loader.Skin.Root("taiko");
        public static string GraphRoot = $"{Root}Graphics\\";
        public static string SoundRoot = $"{Root}Sounds\\";

        public static int TaikoNum = 0;

        public static void Load()
        {
            string root = GraphRoot;

            string game = $@"{root}5_Game\";
            bool ena = File.Exists($@"{game}12_Lane\Background_Main.png");

            Tx.Lane = new($@"{game}12_Lane\Background_Main.png");
            Tx.Notes = new($@"{game}Notes.png");
            Tx.Bar = new($@"{game}Bar.png");

            root = SoundRoot;
            SFx.Don = new($@"{root}Taiko\{TaikoNum}\dong.ogg");
            SFx.Ka = new($@"{root}Taiko\{TaikoNum}\ka.ogg");
        }
    }

    public class Tx
    {
        public static Texture Lane = new(""),
            Notes = new(""),
            Bar = new("");
    }

    public class SFx
    {
        public static Sound Don = new(""),
            Ka = new("");
    }
}
