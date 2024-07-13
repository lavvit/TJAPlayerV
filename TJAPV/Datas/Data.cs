using SeaDrop;

namespace TJAPlayerV
{
    public class Data
    {
        private static readonly string rootdir = $@"{DXLib.AppPath}\Data\";
        public static string DataDir = rootdir;
        public static void Init()
        {
            bool add = false;

            foreach (var line in Text.Read($@"{DXLib.AppPath}\share.ini"))
            {
                var spl = line.ToLower().Split('=');
                if (spl.Length > 1)
                {
                    if (spl[0] == "desktopname")
                    {
                        add = spl[1] == "1";
                    }
                }
            }

            if (add)
            {
                DataDir += Environment.MachineName + "\\";
                if (!Directory.Exists(DataDir))
                {
                    Directory.CreateDirectory(DataDir);
                }
            }

            Check("Config.ini");
            Check("Path.ini");
        }
        public static void Check(string path)
        {
            if (!File.Exists($"{DataDir}{path}")) Copy(path);
        }
        public static void Copy(string path)
        {
            if (!File.Exists($"{rootdir}{path}")) return;
            File.Copy($"{rootdir}{path}", $"{DataDir}{path}");
        }
    }
}