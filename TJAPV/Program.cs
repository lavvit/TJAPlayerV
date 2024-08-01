using SeaDrop;
using System.Reflection;
using static SeaDrop.DXLib;

namespace TJAPlayerV
{
    public class Program : Scene
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            SetDrop(true);
            Init(new taiko.Startup(), 1280, 720);
            //Init(new taiko.Entry(), 1280, 720);
            //Init(new Program(), 3840, 2160, 0.5);
        }

        public static string Version = "0.1.0";

        private static Assembly? Resolver(object? sender, ResolveEventArgs args)
        {
            string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
            string? appbase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string assemblyPath = Path.Combine(appbase != null ? appbase : "",
                                                       "dll",
                                                       assemblyName);

            return File.Exists(assemblyPath) ? Assembly.LoadFile(assemblyPath) : null;
        }
    }
}