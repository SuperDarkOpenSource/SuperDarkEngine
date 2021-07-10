using System;
using System.Collections;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Backend.Common.MessagePropagator;
using Backend.UI.MessagePropagator;
using Dock.Model;
using Dock.Model.Core;
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
                    mainWindowViewModel.Layout.Close.Execute(null);
                };

                desktop.MainWindow = mainWindow;

                desktop.Exit += (sender, args) =>
                {
                    mainWindowViewModel.Layout.Close.Execute(null);
                };
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