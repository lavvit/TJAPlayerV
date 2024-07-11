using Loader;
using SeaDrop;
using System.Collections.Concurrent;

namespace TJAPlayerV
{
    public class Songs
    {
        public static List<(string Name, bool Enable)> FolderList = [];
        public static List<string> SongPaths = [];
        public static List<Song> SongList = [];
        public static List<Song> NowSongList = [];
        public static string Loading = "";

        public static void Load()
        {
            DateTime time = DateTime.Now;
            FolderList = [];
            SongList = [];
            foreach (string p in Text.Read($@"{DXLib.AppPath}\Data\Path.ini"))
            {
                Loading = "";
                if (p.StartsWith("1)"))
                {
                    string path = p.Substring(2).Replace("/", "\\");
                    path = new FileInfo(path).FullName;
                    if (path.StartsWith("C:\\Users\\"))
                    {
                        string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        string pa = path.Substring(path.IndexOf('\\', 9));
                        path = new FileInfo($@"{user}{pa}").FullName;
                    }
                    if (Directory.Exists(path))
                    {
                        Loading = $"Loading Folders : {path}";
                        FolderList.Add((path, true));
                        var maindir = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
                        var inidir = maindir.Where
                            (s => s.EndsWith("genre.ini", StringComparison.CurrentCultureIgnoreCase) || s.EndsWith("box.def", StringComparison.CurrentCultureIgnoreCase));
                        var dandir = maindir.Where
                            (s => s.EndsWith(".tbd", StringComparison.CurrentCultureIgnoreCase));
                        var tjadir = maindir.Where
                            (s => s.EndsWith(".tja", StringComparison.CurrentCultureIgnoreCase));// || s.EndsWith(".tmg", StringComparison.CurrentCultureIgnoreCase)

                        SongPaths = tjadir.ToList();
                    }
                }
                else if (p.StartsWith("0)"))
                {
                    string path = p.Substring(2).Replace("/", "\\");
                    Loading = $"Loading Folders : {path}";
                    FolderList.Add((path, false));
                }
            }

            ConcurrentBag<Song> songs = new();
            //foreach (var path in SongPaths)
            Parallel.ForEach(SongPaths, path =>
            {
                Loading = $"Loading Songs : {path}";
                Song song = new()
                {
                    Path = path,
                    Name = Path.GetFileNameWithoutExtension(path),
                    TJA = new(path)
                };
                songs.Add(song);
                SongList = songs.ToList();
            });//
            SongList = songs.ToList();

            //foreach (var song in SongList)
            Parallel.ForEach(SongList, song =>
            {
                Loading = $"Loading Song Length : {song.Name}";
                song.TJA.SetLen();
            });//


            Loading = "Sorting Songs...";
            SongList.Sort((a, b) => { return new NaturalComparer().Compare(a.Path, b.Path); });
            var ltime = DateTime.Now - time;
            Loading = $"LoadTime:{ltime.TotalSeconds:0.0}s";
        }
    }

    public class Song
    {
        public string Path = "";
        public string Name = "";
        public TJA TJA;

        public override string ToString() { return $"{Name} {TJA.Length}"; }
    }
}
