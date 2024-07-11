using System.Drawing;
using static DxLibDLL.DX;

namespace SeaDrop
{
    /// <summary>
    /// 画像クラス。
    /// </summary>
    public class Texture : IDisposable
    {
        public bool Enable;
        public string? Path;
        public int ID;

        public int Width, Height;
        public double Scale = 1.0, Angle;
        public (double X, double Y)? XYScale = null;
        public ReferencePoint ReferencePoint;
        public Rectangle? Rectangle;
        public BlendMode Blend = BlendMode.None;
        public int BlendDepth = 255;
        public Color Color = Color.White;

        public bool TurnX, TurnY;

        /// <summary>
        /// 画像データを生成します。
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public Texture(string path)
        {
            Path = path;
            ID = LoadGraph(path);
            Enable = ID > 0;

            if (Width == 0) GetGraphSize(ID, out Width, out Height);
        }

        public void Set(string subpath)
        {
            if (!Enable)
            {
                Path = subpath;
                ID = LoadGraph(subpath);
                Enable = ID > 0;

                if (Width == 0) GetGraphSize(ID, out Width, out Height);
            }
        }

        ~Texture()
        {
            Dispose();
        }
        public void Dispose()
        {
            DeleteGraph(ID);
            Enable = false;
            ID = -1;
        }

        public static void Init()
        {
            InitGraph();
        }

        public void Draw(double x, double y)
        {
            if (!Enable) return;

            SetDrawBlendMode((int)Blend, BlendDepth);
            SetDrawBright(Color.R, Color.G, Color.B);

            Point point = Point(Rectangle);
            if (Rectangle.HasValue) DrawRect(x, y);
            else if (XYScale.HasValue)
            {
                DrawRotaGraph3F((float)x, (float)y, point.X, point.Y, XYScale.Value.X, XYScale.Value.Y, Angle * Math.PI, ID, TRUE, TurnX ? 1 : 0, TurnY ? 1 : 0);
            }
            else
            {
                DrawRotaGraph2F((float)x, (float)y, point.X, point.Y, Scale, Angle * Math.PI, ID, TRUE, TurnX ? 1 : 0, TurnY ? 1 : 0);
            }

            SetDrawBlendMode(0, 0);
            SetDrawBright(255, 255, 255);
        }
        public void DrawExtend(double x, double y, double width, double height)
        {
            if (!Enable) return;

            Point point = Point(Rectangle);
            float x1 = (float)x - point.X;
            float y1 = (float)y - point.Y;
            float x2 = x1 + (float)width;
            float y2 = y1 + (float)height;
            DrawExtendGraphF(x1, y1, x2, y2, ID, TRUE);
        }
        public void DrawRect(double x, double y)
        {
            if (!Enable || !Rectangle.HasValue) return;
            Point point = Point(Rectangle);
            if (XYScale.HasValue) DrawRectRotaGraph3F((float)x, (float)y,
                Rectangle.Value.X, Rectangle.Value.Y, Rectangle.Value.Width, Rectangle.Value.Height,
                point.X, point.Y, XYScale.Value.X, XYScale.Value.Y, Angle * Math.PI, ID, TRUE, TurnX ? 1 : 0, TurnY ? 1 : 0);
            else DrawRectRotaGraph2F((float)x, (float)y,
                Rectangle.Value.X, Rectangle.Value.Y, Rectangle.Value.Width, Rectangle.Value.Height,
                point.X, point.Y, Scale, Angle * Math.PI, ID, TRUE, TurnX ? 1 : 0, TurnY ? 1 : 0);
        }

        public void SetRectangle(int x, int y, int width, int height)
        {
            if (width < 0) width = Width;
            if (height < 0) height = Height;
            Rectangle = new Rectangle(x, y, width, height);
        }

        public void SetOpacity(double opacity)
        {
            BlendDepth = (int)(255.0 * opacity);
        }

        private Point Point(Rectangle? rectangle = null)
        {
            if (!rectangle.HasValue) rectangle = new Rectangle(0, 0, Width, Height);
            Point point = new Point();
            switch (ReferencePoint)
            {
                case ReferencePoint.TopLeft:
                    point.X = 0;
                    point.Y = 0;
                    break;

                case ReferencePoint.TopCenter:
                    point.X = rectangle.Value.Width / 2;
                    point.Y = 0;
                    break;

                case ReferencePoint.TopRight:
                    point.X = rectangle.Value.Width;
                    point.Y = 0;
                    break;

                case ReferencePoint.CenterLeft:
                    point.X = 0;
                    point.Y = rectangle.Value.Height / 2;
                    break;

                case ReferencePoint.Center:
                    point.X = rectangle.Value.Width / 2;
                    point.Y = rectangle.Value.Height / 2;
                    break;

                case ReferencePoint.CenterRight:
                    point.X = rectangle.Value.Width;
                    point.Y = rectangle.Value.Height / 2;
                    break;

                case ReferencePoint.BottomLeft:
                    point.X = 0;
                    point.Y = rectangle.Value.Height;
                    break;

                case ReferencePoint.BottomCenter:
                    point.X = rectangle.Value.Width / 2;
                    point.Y = rectangle.Value.Height;
                    break;

                case ReferencePoint.BottomRight:
                    point.X = rectangle.Value.Width;
                    point.Y = rectangle.Value.Height;
                    break;

                default:
                    point.X = 0;
                    point.Y = 0;
                    break;
            }
            return point;
        }

        public override string ToString()
        {
            if (!Enable) return $"{Path}";
            string size = $"{Width}*{Height}";
            if (Rectangle.HasValue) size = $"Rec:{Rectangle.Value.X},{Rectangle.Value.Y} {Rectangle.Value.Width}*{Rectangle.Value.Height}";
            string turn = "";
            if (TurnX) { turn += "RevX"; if (TurnY) turn += " "; }
            if (TurnY) turn += "RevY";
            return $"{Path} {size} {Scale:0.0}x {Angle:0.0}rot {ReferencePoint} {turn} {Blend}:{BlendDepth}";
        }

        public static bool IsEnable(Texture? texture)
        {
            return texture != null && texture.Enable;
        }

        public static (int Width, int Height) GetSize(string path)
        {
            if (!File.Exists(path)) return (0, 0);
            FileStream fs = new FileStream("", FileMode.Open, FileAccess.Read);
            fs.Seek(16, SeekOrigin.Begin);
            byte[] buf = new byte[8];
            fs.Read(buf, 0, 8);
            fs.Dispose();
            uint width = ((uint)buf[0] << 24) | ((uint)buf[1] << 16) | ((uint)buf[2] << 8) | (uint)buf[3];
            uint height = ((uint)buf[4] << 24) | ((uint)buf[5] << 16) | ((uint)buf[6] << 8) | (uint)buf[7];
            return ((int)width, (int)height);
        }
    }

    public enum BlendMode
    {
        /// <summary>
        /// なし
        /// </summary>
        None = DX_BLENDMODE_ALPHA,

        /// <summary>
        /// 加算合成
        /// </summary>
        Add = DX_BLENDMODE_ADD,

        /// <summary>
        /// 減算合成
        /// </summary>
        Subtract = DX_BLENDMODE_SUB,

        /// <summary>
        /// 乗算合成
        /// </summary>
        Multiply = DX_BLENDMODE_MULA,

        /// <summary>
        /// 反転合成
        /// </summary>
        Reverse = DX_BLENDMODE_INVSRC,

        PMAAlpha = DX_BLENDMODE_PMA_ALPHA,
        PMAAdd = DX_BLENDMODE_PMA_ADD,
        PMASubtract = DX_BLENDMODE_PMA_SUB,
        PMAReverse = DX_BLENDMODE_PMA_INVSRC,

    }
    public enum ReferencePoint
    {
        /// <summary>
        /// 左上
        /// </summary>
        TopLeft,

        /// <summary>
        /// 中央上
        /// </summary>
        TopCenter,

        /// <summary>
        /// 右上
        /// </summary>
        TopRight,

        /// <summary>
        /// 左中央
        /// </summary>
        CenterLeft,

        /// <summary>
        /// 中央
        /// </summary>
        Center,

        /// <summary>
        /// 右中央
        /// </summary>
        CenterRight,

        /// <summary>
        /// 左下
        /// </summary>
        BottomLeft,

        /// <summary>
        /// 中央下
        /// </summary>
        BottomCenter,

        /// <summary>
        /// 右下
        /// </summary>
        BottomRight
    }
}
