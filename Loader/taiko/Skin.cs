using SeaDrop;

namespace Loader.taiko
{
    public class Skin
    {
        public static string Root = Loader.Skin.Root("taiko");
        public static string GraphRoot = $"{Root}\\Graphic\\";

        public static void Load()
        {
            string root = GraphRoot;

            string game = $@"{root}5_Game\";

            Tx.Lane = new($@"{game}12_Lane\Base_Normal.png");
        }
    }

    public class Tx
    {
        public static Texture Lane = new("");
    }
}
