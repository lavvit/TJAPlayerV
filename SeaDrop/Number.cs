namespace SeaDrop
{
    public class Number
    {
        public NumPart[] Nums;
        public Texture Texture;
        public int Width, Height, Space;

        public Number(string path, char[]? parts = null)
        {
            Texture = new Texture(path);
            if (parts == null) parts = new char[]
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            Width = Texture.Width / parts.Length;
            Height = Texture.Height;
            Nums = new NumPart[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                Nums[i] = new NumPart(parts[i], i, 0, Width, Height);
            };
        }
        public Number(string path, int xamount, int yamount, char[]? parts = null, int startx = 0, int starty = 0)
        {
            Texture = new Texture(path);
            if (parts == null) parts = new char[]
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            Width = Texture.Width / xamount;
            Height = Texture.Height / yamount;
            Nums = new NumPart[parts.Length];
            int x = 0;
            int y = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                Nums[i] = new NumPart(parts[i], x, y, Width, Height);
                Nums[i].x += Width * startx;
                Nums[i].y += Height * starty;

                if (Nums[i].x + Width >= Width * (xamount + startx))
                {
                    x = 0;
                    y++;
                }
                else x++;
            };
        }

        public void Draw(double x, double y, object num, int type = 0, double opacity = 1, ReferencePoint point = ReferencePoint.TopLeft, int left = 0)
        {
            Draw(x, y, num, 1, type, opacity, 1, point, left);
        }
        public void Draw(double x, double y, object num, double size, int type = 0, double opacity = 1, double comprate = 1, ReferencePoint point = ReferencePoint.TopLeft, int left = 0)
        {
            Draw(x, y, num, size * comprate, size, type, opacity, point, left);
        }
        public void Draw(double x, double y, object num, double scaleX, double scaleY, int type = 0, double opacity = 1, ReferencePoint point = ReferencePoint.TopLeft, int left = 0)
        {
            if (left > 0) x -= Size(num) / (left > 1 ? 1 : 2);
            foreach (char ch in $"{num}")
            {
                for (int i = 0; i < Nums.Length; i++)
                {
                    if (ch == ' ')
                    {
                        break;
                    }
                    if (Nums[i].ch == ch)
                    {
                        Texture.SetRectangle(Nums[i].x, Nums[i].y + Height * type, Width, Height);
                        Texture.XYScale = (scaleX, scaleY);
                        Texture.SetOpacity(opacity);
                        Texture.ReferencePoint = point;
                        Texture.Draw(x, y);
                        break;
                    }
                }
                x += (Width + Space) * scaleX;
            }
        }
        public double Size(object num, double size = 1, double comprate = 1)
        {
            double x = 0;
            foreach (char ch in $"{num}")
            {
                x += (Width + Space) * size * comprate;
            }
            return x;
        }

        public override string ToString()
        {
            return $"{Texture.Path},{Width}*{Height},{Nums}";
        }
    }

    public struct NumPart
    {
        public char ch;
        public int x;
        public int y;

        public NumPart(char ch, int x, int y, int width, int height)
        {
            this.ch = ch;
            this.x = x * width;
            this.y = y * height;
        }
        public NumPart(string str)
        {
            ch = str[0];
            string[] split = str.Substring(2).Split(',');
            int.TryParse(split[0], out x);
            int.TryParse(split[1], out y);
        }
        public override string ToString()
        {
            return $"{ch},{x},{y}";
        }
    }
}