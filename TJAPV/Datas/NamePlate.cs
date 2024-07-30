using SeaDrop;

namespace TJAPlayerV.taiko
{
    public class NamePlate
    {
        public static Handle Name = new(), Dan = new();

        public static void Draw(double x, double y, int player, bool bTitle = false, int Opacity = 255)
        {
            int w = Tx.NamePlate.Width, h = Tx.NamePlate.Height / 22;
            Tx.NamePlate.BlendDepth = Opacity;

            Tx.NamePlate.SetRectangle(0, h * 3, w, h);
            Tx.NamePlate.Draw(x, y);
            Tx.NamePlate.SetRectangle(0, h * 7, w, h);
            Tx.NamePlate.Draw(x, y);

            if (!bTitle)
            {
                Tx.NamePlate.SetRectangle(0, h * (player == 0 ? 0 : 2), w, h);
                Tx.NamePlate.Draw(x, y);
            }

            //DrawStar(player, 1.0f, (float)x, (float)y);
        }

        /*public static void DrawStar(int player, float Scale, float x, float y)
        {
            int tt = 1;
            if (tt >= 0 && tt < Skin.NamePlate_Ptn_Title && Tx.NamePlate_Title_Small[tt] != null)
            {
                Tx.NamePlate_Title_Small[tt].Scale = Scale;
                Tx.NamePlate_Title_Small[tt].ReferencePoint = ReferencePoint.Center;
                Tx.NamePlate_Title_Small[tt].Draw(x, y);
            }
        }*/
    }
}
