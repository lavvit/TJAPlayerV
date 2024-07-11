namespace Loader
{
    public class Lane
    {
        public static string Path = "";
        public static TJA TJA = new(Path);

        public Lane() { }
        public Lane(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            Path = path;
            TJA = new(Path);
        }
    }
}
