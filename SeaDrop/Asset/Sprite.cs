namespace SeaDrop
{
    public class Sprite
    {
        public Texture? texture;
        public double x;
        public double y;
        public int width;
        public int height;

        public double momentx;
        public double momenty;

        public void Draw()
        {
            if (texture != null) texture.Draw(x, y);
            else Drawing.Box(x, y, width, height);
        }
    }
}
