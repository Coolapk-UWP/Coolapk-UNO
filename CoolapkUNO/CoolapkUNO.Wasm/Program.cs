using Uno.Foundation;
using Windows.UI.Xaml;

namespace CoolapkUNO.Wasm
{
    public static class Program
    {
        private static int Main(string[] args)
        {
            WebAssemblyRuntime.InvokeJS(
                """
                const meta = document.createElement("meta");
                meta.name = "referrer";
                meta.content = "same-origin";
                document.head.appendChild(meta);
                """);
            Application.Start(x => _ = new App());
            return 0;
        }
    }
}
