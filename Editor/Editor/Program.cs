using System;
using System.IO;
using System.Reflection;
using Avalonia;
using Avalonia.ReactiveUI;

namespace Editor
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            LoadPluginAssemblies();
            
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI();

        private static void LoadPluginAssemblies()
        {
            LoadAssemblies(Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Tool.*.dll"));
            
            LoadAssemblies(Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Plugins.*.dll"));
            
        }

        private static void LoadAssemblies(string[] assemblies)
        {
            foreach (string asm in assemblies)
            {
                try
                {
                    Assembly.Load(AssemblyName.GetAssemblyName(asm));
                }
                catch (BadImageFormatException e)
                {
                    Console.Error.WriteLine("Could not load: [" + e.FileName + "] as a plugin dll! BadImageFormatException.");
                }
            }
        }
    }
}
