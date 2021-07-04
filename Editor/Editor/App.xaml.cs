using System.Collections;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Backend.Common.MessagePropagator;
using Backend.UI.MessagePropagator;
using Editor.ViewModels;
using Editor.Views;

namespace Editor
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            CreateDependencies();

            var mainWindowViewModel = new MainWindowViewModel(_messagePropagator);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };

                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void CreateDependencies()
        {
            MessagePropagator messagePropagator = new MessagePropagator(new LogConsoleMessageExceptionHandler());
            messagePropagator.RegisterTaskDispatcher(ThreadHandler.UIThread, new AvaloniaUITaskDispatcher());

            _messagePropagator = messagePropagator;

            _dependencies.Add(typeof(IMessagePropagator), _messagePropagator);
        }

        private readonly Hashtable _dependencies = new Hashtable();
        private IMessagePropagator _messagePropagator = null;
    }
}