using SeaDrop;
namespace TJAPlayerV.taiko
{
    public class Startup : Scene
    {
        public override void Enable()
        {
            Language.Load();
            base.Enable();
        }
        public override void Disable()
        {
            base.Disable();
        }

        public override void Draw()
        {
            Drawing.Text(20, 80, Language.Get("setup"));

            base.Draw();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}