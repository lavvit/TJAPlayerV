using Loader;
using SeaDrop;
using System.Collections.Concurrent;

namespace TJAPlayerV
{
    public class Songs
    {
        public static List<(string Name, bool Enable)> FolderList = [];
        public static List<string> SongPaths = [];
        public static List<string> FolderPaths = [];
        public static List<Song> SongList = [];
        public static ELoadState LoadStatus = ELoadState.None;
        public static string Loading = "";

        public static void Load()
        {
            LoadStatus = ELoadState.Loading;
            DateTime time = DateTime.Now;
            FolderList = [];
            SongList = [];
            try
            {
                foreach (string p in Text.Read($@"{Data.DataDir}Path.ini"))
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
                                (s => s.ToLower().EndsWith("genre.ini", StringComparison.CurrentCultureIgnoreCase) || s.ToLower().EndsWith("box.def", StringComparison.CurrentCultureIgnoreCase));
                            var dandir = maindir.Where
                                (s => s.ToLower().EndsWith(".tbd", StringComparison.CurrentCultureIgnoreCase));
                            var tjadir = maindir.Where
                                (s => s.ToLower().EndsWith(".tja", StringComparison.CurrentCultureIgnoreCase));// || s.EndsWith(".tmg", StringComparison.CurrentCultureIgnoreCase)

                            SongPaths.AddRange(tjadir.ToList());
                            FolderPaths.AddRange(inidir.ToList());
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
                        Directory = DirName(path),
                        Type = ESongType.Song,
                        TJA = new(path)
                    };
                    songs.Add(song);
                    SongList = songs.ToList();
                });//


                foreach (var path in FolderPaths)
                {
                    Loading = $"Adding to Folders : {path}";
                    Song song = new()
                    {
                        Path = path,
                        Name = Path.GetFileNameWithoutExtension(path),
                        Directory = DirName(Path.GetDirectoryName(path)),
                        Type = ESongType.Folder,
                        Folder = []
                    };
                    foreach (var s in SongList)
                    {
                        if (s.Type == ESongType.Song && s.Directory == path)
                        {
                            song.Folder.Add(s);
                        }
                    }
                    songs.Add(song);
                    SongList = songs.ToList();
                };//

                SongList = songs.ToList();
                SongList.Sort((a, b) => { return new NaturalComparer().Compare(a.Path, b.Path); });

                //foreach (var song in SongList)
                Parallel.ForEach(SongList, song =>
                {
                    Loading = $"Loading Song Length : {song.Name}";
                    song.TJA.SetLen();
                });//


                Loading = "Sorting Songs...";
                SongList.Sort((a, b) => { int r = DirNum(a.Path) - DirNum(b.Path); return r != 0 ? r : new NaturalComparer().Compare(a.Path, b.Path); });
                var ltime = DateTime.Now - time;
                Loading = $"LoadTime:{ltime.TotalSeconds:0.0}s";
                LoadStatus = ELoadState.Success;
            }
            catch (Exception)
            {
                LoadStatus = ELoadState.Error;
            }
        }

        private static string DirName(string path)
        {
            int n = DirNum(path);
            if (n < 0) return "";
            return Path.GetDirectoryName(FolderPaths[n]);
        }

        private static int DirNum(string path)
        {
            int n = -1;
            for (int i = 0; i < FolderPaths.Count; i++)
            {
                if (path.Contains(Path.GetDirectoryName(FolderPaths[i])))
                    n = i;
            }
            return n;
        }
    }

    public class Song
    {
        public string Path = "";
        public string Name = "";
        public string Directory = "";
        public ESongType Type;

        public TJA TJA = new("");

        public List<Song> Folder = [];

        public override string ToString() { return $"{Directory} {Name} {TJA.Length} {Folder.Count}"; }
    }

    public enum ELoadState
    {
        None,
        Loading,
        Success,
        Error = -1,
    }

    public enum ESongType
    {
        Song,
        Tower,
        Dan,
        Folder,
        Back,
        Random,
    }
}
