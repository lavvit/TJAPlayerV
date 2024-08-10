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
            FolderPaths = [];
            FolderList = [];
            SongPaths = [];
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
                            var inilist = inidir.ToList();
                            inilist.RemoveAll(folder => Path.GetDirectoryName(folder) == path);
                            FolderPaths.AddRange(inilist);
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
                    var tja = new TJA(path);
                    Song song = new()
                    {
                        Path = path,
                        Name = Path.GetFileNameWithoutExtension(path),
                        Directory = DirName(path),
                        Type = ESongType.Song,
                        TJA = tja,
                        Genre = tja.Header.Genre,
                    };
                    songs.Add(song);
                    SongList = songs.ToList();
                });//


                foreach (var path in FolderPaths)
                {
                    Loading = $"Adding to Folders : {path}";
                    string dir = Path.GetDirectoryName(path) ?? "";
                    var dsong = songs.ToList().Find(s => dir == s.Path);
                    var song = dsong ?? new()
                    {
                        Path = dir,
                        Name = Path.GetFileNameWithoutExtension(path),
                        Directory = DirName(Path.GetDirectoryName(dir) ?? ""),
                        Type = ESongType.Folder,
                        Folder = []
                    };
                    if (path.ToLower().EndsWith("box.def"))
                    {
                        foreach (var line in Text.Read(path))
                        {
                            string[] split = line.Split(':');
                            if (split.Length < 2 || !split[0].StartsWith("#")) continue;
                            string value = split[1];
                            switch (split[0].Substring(1).ToLower())
                            {
                                case "title":
                                    song.Name = value;
                                    break;
                                case "genre":
                                    song.Genre = value;
                                    break;
                            }
                        }
                    }
                    if (path.ToLower().EndsWith("genre.ini"))
                    {
                        foreach (var line in Text.Read(path))
                        {
                            string[] split = line.Split('=');
                            if (split.Length < 2) continue;
                            string value = split[1];
                            switch (split[0].ToLower())
                            {
                                case "genrename":
                                    song.Name = value;
                                    break;
                            }
                        }
                    }
                    if (dsong == null)
                    {
                        foreach (var s in SongList)
                        {
                            if (s.Type == ESongType.Song && s.Directory == dir)
                            {
                                song.Folder.Add(s);
                            }
                        }
                        songs.Add(song);
                    }
                    SongList = [.. songs];
                };//

                SongList = songs.ToList();
                SongList.Sort((a, b) => { return new NaturalComparer().Compare(a.Path, b.Path); });

                //foreach (var song in SongList)
                Parallel.ForEach(SongList, song =>
                {
                    Loading = $"Loading Song Length : {song.Name}";
                    //song.TJA.SetLen();
                });//


                Loading = "Sorting Songs...";
                SongList.Sort((a, b) =>
                {
                    Loading = $"Sorting Songs... : {a.Path} - {b.Path}";
                    int r = RootDirNum(a.Path) - RootDirNum(b.Path);
                    int d = r != 0 ? r : a.Directory.CompareTo(b.Directory);
                    int t = d != 0 ? d : a.Type - b.Type;
                    return t != 0 ? t : new NaturalComparer().Compare(a.Path, b.Path);
                });
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

            return Path.GetDirectoryName(FolderPaths[n]) ?? "";
        }

        private static int DirNum(string path)
        {
            int n = -1;
            for (int i = 0; i < FolderPaths.Count; i++)
            {
                if (path.Contains(Path.GetDirectoryName(FolderPaths[i]) ?? ""))
                    n = i;
            }
            return n;
        }
        public static int RootDirNum(string path)
        {
            int n = -1;
            for (int i = 0; i < FolderList.Count; i++)
            {
                if (FolderList[i].Enable && path.Contains(Path.GetDirectoryName(FolderList[i].Name) ?? ""))
                    n = i;
            }
            return n;
        }

        public static ESongGenre GenreName(string genre)
        {
            switch (genre.ToUpper())
            {
                case "J-POP":
                case "ポップス":
                    return ESongGenre.JPOP;
                case "アニメ":
                    return ESongGenre.Anime;
                case "ゲームミュージック":
                    return ESongGenre.GameMusic;
                case "ナムコオリジナル":
                    return ESongGenre.NamcoOriginal;
                case "クラシック":
                    return ESongGenre.Classic;
                case "どうよう":
                case "キッズ":
                    return ESongGenre.Kids;
                case "バラエティ":
                    return ESongGenre.Variety;
                case "ボーカロイド":
                case "ボーカロイド曲":
                case "VOCALOID":
                    return ESongGenre.Vocaloid;
                /*case "段位道場":
                    nGenre = 0;
                    break;
                case "段位-薄木":
                case "段位-濃木":
                case "段位-黒":
                case "段位-赤":
                case "段位-銀":
                case "段位-金":
                case "段位-外伝":
                    nGenre = 9;
                    break;
                case "ExCats":
                    nGenre = 10;
                    break;
                case "BEMANI":
                    nGenre = 11;
                    break;
                case "BMS":
                    nGenre = 12;
                    break;
                case "TaikoCatsSoundTeam":
                    nGenre = 13;
                    break;
                case "アーティストオリジナル":
                    nGenre = 14;
                    break;
                case "スマホ音ゲー":
                    nGenre = 15;
                    break;
                case "ゲキチュウマイ":
                    nGenre = 16;
                    break;
                case "WACCA":
                    nGenre = 17;
                    break;
                case "東方":
                    nGenre = 18;
                    break;
                case "スピカオリジナル":
                    nGenre = 19;
                    break;
                case "Another":
                    nGenre = 20;
                    break;
                case "niconico":
                    nGenre = 21;
                    break;
                case "海外音ゲー":
                    nGenre = 22;
                    break;
                case "その他音ゲー":
                    nGenre = 23;
                    break;*/
                default:
                    return ESongGenre.None;

            }
        }

        public static int GenreNum(ESongGenre genre)
        {
            switch (genre)
            {
                case ESongGenre.None:
                default:
                    return 0;
                case ESongGenre.JPOP:
                    return 1;
                case ESongGenre.Anime:
                    return 2;
                case ESongGenre.Vocaloid:
                    return 8;
                case ESongGenre.Kids:
                    return 7;
                case ESongGenre.Variety:
                    return 4;
                case ESongGenre.Classic:
                    return 6;
                case ESongGenre.GameMusic:
                    return 3;
                case ESongGenre.NamcoOriginal:
                    return 5;
            }
        }
    }

    public class Song
    {
        public string Path = "";
        public string Name = "";
        public string Directory = "";
        public string Genre = "";
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
        Back,
        Folder,
        Random,

        Song,
        Tower,
        Dan,
    }

    public enum ESongGenre
    {
        None = 0,
        JPOP,
        Anime,
        Vocaloid,
        Kids,

        Variety,
        //Touhou,
        //niconico,

        Classic,

        GameMusic,

        NamcoOriginal,
    }
}
