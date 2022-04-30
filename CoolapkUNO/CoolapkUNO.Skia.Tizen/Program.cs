using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace CoolapkUNO.Skia.Tizen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new CoolapkUNO.App(), args);
            host.Run();
        }
    }
}
