using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Backend.Common.MessagePropagator;
using Editor.MenuBar.Reflection;

namespace Editor.MenuBar.File
{
    [MenuBarItem("Close", "File", 2048)]
    class FileMenuCloseApplication : BaseMenuBarItem
    {
        public FileMenuCloseApplication(IMessagePropagator messagePropagator) :
            base(messagePropagator)
        {
        }
        
        protected override Task OnClick(object parameter)
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime applicationLifetime)
            {
                applicationLifetime.MainWindow.Close();
            }
            
            return Task.CompletedTask;
        }
    }
}