using System.Drawing;
using static DxLibDLL.DX;

namespace SeaDrop
{
    public class Drawing
    {
        public static void Line(double x, double y, double x1, double y1, int color = 0xffffff, int thick = 1, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            DrawLineAA((float)x, (float)y, (float)(x + x1), (float)(y + y1), (uint)color, thick);
            SetDrawBlendMode((int)BlendMode.None, 255);
        }
        public static void LineZ(double x, double y, double x1, double y1, int color = 0xffffff, int thick = 1, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            DrawLineAA((float)x, (float)y, (float)x1, (float)y1, (uint)color, thick);
            SetDrawBlendMode((int)BlendMode.None, 255);
        }
        public static void Box(double x, double y, double x1, double y1, int color = 0xffffff, bool fill = true, int thick = 1, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            DrawBoxAA((float)x, (float)y, (float)(x + x1), (float)(y + y1), (uint)color, fill ? 1 : 0, thick);
            SetDrawBlendMode((int)BlendMode.None, 255);
        }
        public static void BoxZ(double x, double y, double x1, double y1, int color = 0xffffff, bool fill = true, int thick = 1, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            DrawBoxAA((float)x, (float)y, (float)x1, (float)y1, (uint)color, fill ? 1 : 0, thick);
            SetDrawBlendMode((int)BlendMode.None, 255);
        }
        public static void Circle(double x, double y, double r, int color = 0xffffff, bool fill = true, int thick = 1, double opacity = 1.0, BlendMode blend = BlendMode.None, int pos = 255)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            DrawCircleAA((float)x, (float)y, (float)r, pos, (uint)color, fill ? 1 : 0, thick);
            SetDrawBlendMode((int)BlendMode.None, 255);
        }

        public static void Text(double x, double y, object? str, int color = 0xffffff, Handle? handle = null, int edgecolor = 0, bool vertical = false, ReferencePoint point = ReferencePoint.TopLeft, double opacity = 1.0, BlendMode blend = BlendMode.None)
        {
            SetDrawBlendMode((int)blend, (int)(255.0 * opacity));
            if (str == null) return;
            if (str.ToString() == null) return;
            var po = TextPoint(point, str, -1, handle);
            float x1 = (float)x - po.X, y1 = (float)y - po.Y;
            if (handle != null && handle.Enable) DrawStringFToHandle(x1, y1, str.ToString(), (uint)color, handle.ID, (uint)edgecolor, vertical ? 1 : 0);
            else DrawStringF(x1, y1, str.ToString(), (uint)color);
            SetDrawBlendMode((int)BlendMode.None, 255);
        }

        public static (int Width, int Height, int Count) TextSize(object? str, int length = -1, Handle? handle = null)
        {
            if (str == null) return (0, 0, 0);
            var s = str.ToString();
            if (s == null) return (0, 0, 0);
            if (length < 0) length = s.Length;
            int s1, s2, c;
            if (handle != null && handle.Enable) GetDrawStringSizeToHandle(out s1, out s2, out c, s, length, handle.ID);
            else GetDrawStringSize(out s1, out s2, out c, s, length);
            return (s1, s2, c);
        }
        private static Point TextPoint(ReferencePoint refpoint, object? str, int length = -1, Handle? handle = null)
        {
            var size = TextSize(str, length, handle);
            Point point = new Point();
            switch (refpoint)
            {
                case ReferencePoint.TopLeft:
                    point.X = 0;
                    point.Y = 0;
                    break;

                case ReferencePoint.TopCenter:
                    point.X = size.Width / 2;
                    point.Y = 0;
                    break;

                case ReferencePoint.TopRight:
                    point.X = size.Width;
                    point.Y = 0;
                    break;

                case ReferencePoint.CenterLeft:
                    point.X = 0;
                    point.Y = size.Height / 2;
                    break;

                case ReferencePoint.Center:
                    point.X = size.Width / 2;
                    point.Y = size.Height / 2;
                    break;

                case ReferencePoint.CenterRight:
                    point.X = size.Width;
                    point.Y = size.Height / 2;
                    break;

                case ReferencePoint.BottomLeft:
                    point.X = 0;
                    point.Y = size.Height;
                    break;

                case ReferencePoint.BottomCenter:
                    point.X = size.Width / 2;
                    point.Y = size.Height;
                    break;

                case ReferencePoint.BottomRight:
                    point.X = size.Width;
                    point.Y = size.Height;
                    break;

                default:
                    point.X = 0;
                    point.Y = 0;
                    break;
            }
            return point;
        }
    }

    public class Handle
    {
        public int ID;
        public bool Enable;
        public string? Font;
        public int Size, Thick, Edge;
        public bool Italic;
        public EFontType Type;

        public Handle(string fontpath, string font, int size = 16, int thick = 1, int edge = 1, bool italic = false, EFontType type = EFontType.Normal)
        {
            AddFont(fontpath);
            Font = font;
            Set(size, thick, edge, italic, type);
        }
        public Handle(string? font = null, int size = 16, int thick = 1, int edge = 1, bool italic = false, EFontType type = EFontType.Normal)
        {
            Font = font;
            Set(size, thick, edge, italic, type);
        }
        public Handle(int size, int thick = 1, int edge = 1, bool italic = false, EFontType type = EFontType.Normal)
        {
            Set(size, thick, edge, italic, type);
        }
        public void Set(int size, int thick, int edge, bool italic, EFontType type)
        {
            Size = size;
            Thick = thick;
            Edge = edge;
            Italic = italic;
            Type = type;
            Set();
        }

        public void Set()
        {
            DeleteFontToHandle(ID);
            ID = CreateFontToHandle(Font, Size, Thick, (int)Type, -1, Edge, Italic ? 1 : 0);
            Enable = ID >= 0;
        }

        public static void AddFont(string path)
        {
            if (File.Exists(path)) AddFontFile(path);
        }
    }

    public enum EFontType
    {
        Normal = DX_FONTTYPE_NORMAL,
        Edge = DX_FONTTYPE_EDGE,
        Antialiasing = DX_FONTTYPE_ANTIALIASING,
        Antialiasing4 = DX_FONTTYPE_ANTIALIASING_4X4,
        Antialiasing8 = DX_FONTTYPE_ANTIALIASING_8X8,
        Antialiasing16 = DX_FONTTYPE_ANTIALIASING_16X16,
        AntialiasingEdge = DX_FONTTYPE_ANTIALIASING_EDGE,
        AntialiasingEdge4 = DX_FONTTYPE_ANTIALIASING_EDGE_4X4,
        AntialiasingEdge8 = DX_FONTTYPE_ANTIALIASING_EDGE_8X8,
        AntialiasingEdge16 = DX_FONTTYPE_ANTIALIASING_EDGE_16X16,
    }
}