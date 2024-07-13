using SeaDrop;

namespace TJAPlayerV
{
    /// <summary>
    /// 各地域言語ファイルをまとめる
    /// </summary>
    public class Language
    {
        public static Dictionary<string, string> Texts = [];

        public static void Load(string lang = "jp")
        {
            Texts = Lang.AllTexts();
            string path = $@"{DXLib.AppPath}\Lang\{lang}\lang.json";
            if (File.Exists(path))
            {
                var file = Json.Get<Dictionary<string, string>>(path);
                Texts = Texts.Concat(file).GroupBy(pair => pair.Key, (_, pairs) => pairs.Last()).ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);
            }
            else Lang.Save(lang);

        }

        public static string Get(string key)
        {
            try
            {
                return Texts[key];
            }
            catch { return ""; }
        }
    }

    /// <summary>
    /// デフォルトの表示テキスト
    /// </summary>
    public class Lang
    {
        public static string
            SetUp =
            "SeaDrop (using DXLib Engine) product by LABY(@LAVVIT_SUSHI)\n" +
            $"Release:{Program.Version} {File.GetLastWriteTime($"{DXLib.AppPath}\\TJAPlayerV.exe"):g}\n\n" +
            "TJAPlayerV edited by LABY(@LAVVIT_SUSHI)\n" +
            "This software is Taiko no Tatsujin and more simulator.\n" +
            "More help is in https://discord.gg/467xQnteQx\n\n";


        public static Dictionary<string, string> AllTexts()
        {
            Dictionary<string, string> keys = new(StringComparer.OrdinalIgnoreCase)
            {
                { "setup", SetUp },
            };

            return keys;
        }

        public static void Save(string lang = "jp")
        {
            string path = $@"{DXLib.AppPath}\Lang\{lang}\lang.json";
            if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
            Json.Save(AllTexts(), path);
        }
    }
}