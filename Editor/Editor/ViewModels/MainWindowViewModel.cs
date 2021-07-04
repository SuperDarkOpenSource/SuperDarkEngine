using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using Backend.Common.MessagePropagator;
using Backend.EngineInterop;
using Backend.Messages.Menu;
using Backend.UI.Tools;
using Editor.Factory;
using Editor.MenuBar.Reflection;
using ReactiveUI;

namespace Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IMessagePropagator messagePropagator)
        {
            _messagePropagator = messagePropagator;
            _factory = new ToolWindowFactory(_messagePropagator);

            _menuItems = MenuBarFactory.GetAllMenuBarItems(_messagePropagator);
            //_menuItems.Add(factory.GetMenuBarItems());

            Task.Run(() =>
            {
                GameEngine engine = new GameEngine(_messagePropagator);
                engine.Run();
            });

            foreach (var tool in _factory.GetDefaultDocumentTools())
            {
                DocumentToolWindows.Add(tool);
            }

            foreach (var tool in _factory.GetDefaultFloatingTools())
            {
                FloatingToolWindows.Add(tool);
            }
            
            RegisterMessages();
        }

        public IList<MenuItemViewModel> MenuBarItems
        {
            get => _menuItems;
        }

        public ObservableCollection<BaseToolWindow> DocumentToolWindows
        {
            get => _documentToolWindows;
            set => this.RaiseAndSetIfChanged(ref _documentToolWindows, value);
        }

        public BaseToolWindow SelectedDocumentToolWindow
        {
            get => _selectedDocumentToolWindow;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedDocumentToolWindow, value);

                var toolWindowAttribute =
                    _selectedDocumentToolWindow.GetType().GetCustomAttribute<ToolWindowAttribute>();

                if (toolWindowAttribute != null)
                {
                    var control = (Control) Activator.CreateInstance(toolWindowAttribute.ViewType);
                    control.DataContext = _selectedDocumentToolWindow;
                    DocumentControl = control;
                }
            }
        }

        public ObservableCollection<BaseToolWindow> FloatingToolWindows
        {
            get => _floatingToolWindows;
            set => this.RaiseAndSetIfChanged(ref _floatingToolWindows, value);
        }
        
        public BaseToolWindow SelectedFloatingToolWindow
        {
            get => _selectedFloatingToolWindow;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFloatingToolWindow, value);
                
                var toolWindowAttribute =
                    _selectedFloatingToolWindow.GetType().GetCustomAttribute<ToolWindowAttribute>();

                if (toolWindowAttribute != null)
                {
                    var control = (Control) Activator.CreateInstance(toolWindowAttribute.ViewType);
                    control.DataContext = _selectedFloatingToolWindow;
                    FloatingControl = control;
                }
            }
        }

        public Control DocumentControl
        {
            get => _documentControl;
            set => this.RaiseAndSetIfChanged(ref _documentControl, value);
        }

        public Control FloatingControl
        {
            get => _floatingControl;
            set => this.RaiseAndSetIfChanged(ref _floatingControl, value);
        }

        private void RegisterMessages()
        {
            _messagePropagator.GetMessage<RefreshAllBindingsMessage>()
                .Subscribe(OnRefreshAllBindingsMessage, ThreadHandler.UIThread);
        }

        private Task OnRefreshAllBindingsMessage()
        {
            var temp = DocumentControl;
            DocumentControl = null;
            DocumentControl = temp;

            temp = FloatingControl;
            FloatingControl = null;
            FloatingControl = temp;
            
            return Task.CompletedTask;
        }

        private IMessagePropagator _messagePropagator;
        
        private List<MenuItemViewModel> _menuItems;
        private ObservableCollection<BaseToolWindow> _documentToolWindows = new();
        private BaseToolWindow _selectedDocumentToolWindow = null;
        private ObservableCollection<BaseToolWindow> _floatingToolWindows = new();
        private BaseToolWindow _selectedFloatingToolWindow = null;

        private Control _documentControl = null;
        private Control _floatingControl = null;
        
        private ToolWindowFactory _factory;
    }
}
