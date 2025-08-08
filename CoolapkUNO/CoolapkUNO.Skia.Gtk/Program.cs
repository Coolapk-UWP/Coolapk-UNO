using GLib;
using System;
using Uno.UI.Runtime.Skia.Gtk;

namespace CoolapkUNO.Skia.Gtk
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            ExceptionManager.UnhandledException += expArgs =>
            {
                Console.WriteLine($"GLIB UNHANDLED EXCEPTION {expArgs.ExceptionObject}");
                expArgs.ExitApplication = true;
            };

            GtkHost host = new(() => _ = new App());

            host.Run();
        }
    }
}
