using SeaDrop;
using static Loader.Skin;

namespace Loader
{
    public class Program : Scene
    {

        public static string Song = "Song\\バナナのナナチ";
        public static Lane Lane = new();

        public static int Size = 512;

        public static void Main(string[] args)
        {
            var size = Texture.GetSize(@$"{DXLib.AppPath}\System\img\lane.png");
            LoadSkin();
            DXLib.SetDrop(true);
            DXLib.Init(new Program());//, LaneW, LaneH, (double)Size / LaneW
        }

        public override void Enable()
        {
            Lane = new(Song);
            base.Enable();
        }

        public override void Draw()
        {
            Drawing.Text(20, 20, Lane.TJA);
            base.Draw();
        }

        public override void Drag(string str)
        {
            Song = str;
            Lane = new(Song);
            base.Drag(str);
        }
    }
}