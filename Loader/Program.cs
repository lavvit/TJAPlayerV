using SeaDrop;

namespace Loader
{
    public class Program : Scene
    {

        public static string Path = "Song\\バナナのナナチ";
        public static TJA TJA = new(Path);
        public static void Main(string[] args)
        {
            DXLib.SetDrop(true);
            DXLib.Init(new Program());
        }

        public override void Draw()
        {
            Drawing.Text(20, 20, TJA);
            base.Draw();
        }

        public override void Drag(string str)
        {
            Path = str;
            TJA = new(Path);
            base.Drag(str);
        }
    }
}