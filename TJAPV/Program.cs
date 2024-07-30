using SeaDrop;
using static SeaDrop.DXLib;

namespace TJAPlayerV
{
    public class Program : Scene
    {
        public static void Main(string[] args)
        {
            SetDrop(true);
            Init(new taiko.Startup(), 1280, 720);
            //Init(new taiko.Entry(), 1280, 720);
            //Init(new Program(), 3840, 2160, 0.5);
        }

        public static string Version = "0.1.0";
    }
}