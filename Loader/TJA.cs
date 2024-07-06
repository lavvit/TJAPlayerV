using SeaDrop;
using System.Text;

namespace Loader
{
    public struct TJA
    {
        public string FilePath = "";
        public string SoundPath = "";
        public string Hash = "";
        public bool Enable;
        public Header Header;
        public Course[] Courses = new Course[5];

        public int Length;

        public TJA(string path)
        {
            Init(path);
            if (!Enable) return;

            var list = Text.Read(FilePath);
            Header = new Header(list);

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
            Length = Sound.GetLength(SoundPath);
        }

        public override readonly string ToString()
        {
            if (!Enable) return FilePath;
            var time = new DateTime().AddMilliseconds(Length > -1 ? Length : 0);
            return $"{FilePath}\n{Hash}\n\n{Header}\n\n{SoundPath}\n{time:mm:ss.fff}";
        }

        public static string GetTJAHash(string filename)
        {
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
