namespace Loader
{
    public struct Header
    {
        public string Title = "";
        public string SubTitle = "";
        public string Wave = "";

        public Header() { }

        public Header(List<string> list)
        {
            foreach (string line in list)
            {
                if (string.IsNullOrEmpty(line)) continue;
                string[] split = line.Split(':');
                if (split.Length > 1)
                {
                    var spls = split.ToList();
                    spls.RemoveAt(0);
                    string value = string.Join(":", spls);
                    switch (split[0].ToLower())
                    {
                        case "title":
                            Title = value;
                            break;
                        case "subtitle":
                            {
                                if (value.StartsWith("++") || value.StartsWith("--")) SubTitle = value.Substring(2);
                                else SubTitle = value;
                            }
                            break;
                        case "wave":
                            Wave = value;
                            break;
                    }
                }
                if (line.StartsWith("#START")) break;
            }
        }

        public override string ToString()
        {
            return $"{Title}\n{SubTitle}\n{Wave}";
        }
    }
}
