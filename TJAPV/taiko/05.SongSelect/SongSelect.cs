using SeaDrop;

namespace TJAPlayerV.taiko
{
    public class SongSelect : Scene
    {
        public static Sound BGMIn
        {
           get
           {
               return Sfx.SongSelect_BGM_In;
           }
        }
        public static Sound BGM
        {
           get
           {
               return Sfx.SongSelect_BGM;
           }
        }

        public override void Enable()
        {
            BGMIn.Play();
            base.Enable();
        }

        public override void Draw()
        {
            base.Draw();
        }
        public override void Debug()
        {
            base.Debug();
        }

        public override void Drag(string str)
        {
            base.Drag(str);
        }
        
        public override void Update()
        {
            if (BGMIn.Played)
            {
                BGM.PlayLoopUp();
            }
            base.Update();
        }
    }
}