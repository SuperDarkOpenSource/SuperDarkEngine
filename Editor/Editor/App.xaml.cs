using System.Collections;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Backend.Common.MessagePropagator;
using Dock.Model;
using Editor.Dock;
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
            this.CreateDependencies();
            
            var factory = new ToolDockFactory(_messagePropagator, _dependencies);

            var mainWindowViewModel = new MainWindowViewModel(factory, CreateLayout(factory), _messagePropagator);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };

                mainWindow.Closing += (sender, args) =>
                {
                    if (mainWindowViewModel.Layout is IDock dock)
                    {
                        dock.Close();
                    }
                };

                desktop.MainWindow = mainWindow;

                desktop.Exit += (sender, args) =>
                {
                    if (mainWindowViewModel.Layout is IDock dock)
                    {
                        dock.Close();
                    }
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void CreateDependencies()
        {
            _messagePropagator = new MessagePropagator();

            _dependencies.Add(typeof(IMessagePropagator), _messagePropagator);
        }

        private IDock CreateLayout(IFactory factory)
        {
            var layout = factory.CreateLayout();
            factory.InitLayout(layout);

            return layout;
        }

        private readonly Hashtable _dependencies = new Hashtable();
        private IMessagePropagator _messagePropagator = null;
    }
}