using SeaDrop;
using System.Text;

namespace Loader
{
    public class TJA : IDisposable
    {
        public string FilePath = "";
        public string SoundPath = "";
        public string Hash = "";
        public bool Enable;
        public Header Header;
        public Course[] Courses = new Course[5];

        public int Length;

        ~TJA() { Dispose(); }
        public void Dispose()
        {
            Enable = false;
            Header = new();
            for (int i = 0; i < Courses.Length; i++)
            {
                if (Courses[i] == null) continue;
                for (int j = 0; j < Courses[i].LongList.Length; j++)
                    Courses[i].LongList[j].Clear();
                Courses[i].Texts.Clear();
                for (int j = 0; j < Courses[i].Lanes.Length; j++)
                    for (int k = 0; k < Courses[i].Lanes[j].Length; k++)
                    {
                        Courses[i].Lanes[j][k].Chips.Clear();
                        Courses[i].Lanes[j][k].Commands.Clear();
                    }
            }
            Courses = new Course[5];
        }

        public TJA(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            Init(path);
            if (!Enable) return;

            var list = Text.Read(FilePath);
            bool endcomma = false;
            for (int i = 0; i < list.Count; i++)
            {
                //コメント削除
                if (list[i].Replace("/ /", "//").Contains("//"))
                {
                    string s = list[i].Replace("/ /", "//");
                    list[i] = s.Substring(0, s.IndexOf("//") == -1 ? 0 : s.IndexOf("//"));
                }
                //バグ対策
                if (list.Count > 0 && list[i] == "," && (i == 0 || endcomma))
                    list[i] = list[i].Replace(",", "0,");
                if (!string.IsNullOrEmpty(list[i].Trim()) && !list[i].StartsWith("#")) endcomma = list[i].EndsWith(",");
                if (list[i].StartsWith("#START")) endcomma = true;
            }
            Header = new Header(list);

            for (int i = 0; i < Courses.Length; i++)
            {
                bool read = false;
                int nowcourse = 3;
                List<string> strings = [];
                foreach (string str in list)
                {
                    string[] header = str.Split(':');
                    if (header.Length > 1)
                    {
                        switch (header[0])
                        {
                            case "COURSE":
                                nowcourse = Course.GetCourse(header[1]);
                                break;
                        }
                    }
                    else if (str.StartsWith("#START"))
                    {
                        if (nowcourse == i)
                        {
                            read = true;
                        }
                    }
                    else if (str.StartsWith("#END"))
                    {
                        read = false;
                    }
                    else if (read)
                    {
                        strings.Add(str);
                    }
                }
                Courses[i] = new Course(Header, strings);
            }

            SoundPath = $"{Path.GetDirectoryName(FilePath)}\\{Header.Wave}";
            //Length = (int)GetMoviePlaybackTime(SoundPath).TotalMilliseconds;
        }
        public void Init(string path)
        {
            if (Path.GetExtension(path) != ".tja" && Path.GetExtension(path) != ".tbc")
                path += File.Exists($"\\{Path.GetFileName(path)}.tbc") ? $"\\{Path.GetFileName(path)}.tbc" : $"\\{Path.GetFileName(path)}.tja";
            string ext = Path.GetExtension(path);
            FilePath = path;
            Hash = GetTJAHash(path);
            Enable = File.Exists(path) && (ext == ".tja" || ext == ".tbc");
        }

        public void SetLen()
        {
            using (Sound sound = new(SoundPath))
            {
                Length = sound.Length;
            }
        }

        public override string ToString()
        {
            if (!Enable) return FilePath;
            var time = new DateTime().AddMilliseconds(Length > -1 ? Length : 0);
            return $"{FilePath}\n{Hash}\n\n{Header}\n\n{SoundPath}\n{time:mm:ss.fff}";
        }

        public static string GetTJAHash(string filename)
        {
            if (!File.Exists(filename)) return "";
            //ファイルを開く
            FileStream fs = new(
                filename,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            //MD5CryptoServiceProviderオブジェクトを作成
            var md5 = System.Security.Cryptography.MD5.Create();

            //ハッシュ値を計算する
            byte[] bs = md5.ComputeHash(fs);

            //リソースを解放する
            md5.Clear();
            //ファイルを閉じる
            fs.Close();

            //byte型配列を16進数の文字列に変換
            StringBuilder result = new StringBuilder();
            foreach (byte b in bs)
            {
                result.Append(b.ToString("x2"));
            }

            return result.ToString();
        }

        [STAThread]
        public TimeSpan GetMoviePlaybackTime(string filename)
        {
            if (!File.Exists(filename)) return new TimeSpan();
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            dynamic shell = Activator.CreateInstance(shellAppType);
            Shell32.Folder objFolder = shell.NameSpace(Path.GetDirectoryName(filename));
            Shell32.FolderItem folderItem = objFolder.ParseName(Path.GetFileName(filename));
            string strDuration = objFolder.GetDetailsOf(folderItem, 27);

            // TimeSpanに変換
            TimeSpan ts = TimeSpan.Parse(strDuration);
            return ts;
        }
    }
}
