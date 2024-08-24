namespace Loader
{
    public struct Header
    {
        public string Title = "";
        public string SubTitle = "";
        public string Wave = "";
        public string Genre = "";
        public double Offset = 0;
        public double BPM = 120;
        public double Demo = 0;

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
                        case "offset":
                            if (float.TryParse(value, out float fval)) Offset = fval;
                            else double.TryParse(value, out Offset);
                            break;
                        case "bpm":
                            if (float.TryParse(value, out fval)) BPM = fval;
                            else double.TryParse(value, out BPM);
                            break;
                        case "genre":
                            Genre = value;
                            break;
                        case "demostart":
                            if (float.TryParse(value, out fval)) Demo = fval;
                            else double.TryParse(value, out Demo);
                            break;
                    }
                }
                if (line.StartsWith("#START")) break;
            }
        }

        public override string ToString()
        {
            return $"{Title}\n{SubTitle}\n{Wave}\n{Genre}";
        }
    }
}
