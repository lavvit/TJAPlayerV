using SeaDrop;
namespace Loader
{
    public class Skin
    {
        public static string SkinPath = "jiro";
        public static int Width, Height, LaneW, LaneH;

        public static void LoadSkin()
        {
            string root = Root;

            Tx.Lane = new($"{root}sfieldbg.png");

            LaneW = 512; LaneH = 56;
            if (File.Exists($"{root}dispconf.ini"))
            {

                foreach (string line in Text.Read($"{root}dispconf.ini"))
                {
                    var split = line.Split('=');
                    if (split.Length < 2) continue;
                    int.TryParse(split[1], out int value);
                    switch (split[0])
                    {
                        case "ScrollFieldHeight":
                            LaneH = value;
                            break;
                    }
                }
            }
        }
        public static string Root
        {
            get
            {
                string root = @$"{Path.GetFullPath($@"{DXLib.AppPath}\System")}\";
                if (!string.IsNullOrEmpty(SkinPath)) root += @$"{SkinPath}\";
                return root;
            }
        }
    }

    public class Tx
    {
        public static Texture Lane = new("");
    }
}
