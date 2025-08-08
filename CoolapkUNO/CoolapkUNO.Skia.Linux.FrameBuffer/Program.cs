using System;
using Uno.UI.Runtime.Skia.Linux.FrameBuffer;

namespace CoolapkUNO
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Console.CursorVisible = false;

                FrameBufferHost host = new(() => _ = new App());
                host.Run();
            }
            finally
            {
                Console.CursorVisible = true;
            }
        }
    }
}
