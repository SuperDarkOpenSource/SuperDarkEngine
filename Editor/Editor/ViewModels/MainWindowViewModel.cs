using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Backend.Common.MessagePropagator;
using Backend.EngineInterop;
using Dock.Model;
using Dock.Model.Core;
using Editor.Dock;
using Editor.MenuBar.Reflection;

namespace Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(ToolDockFactory factory, IDock layout, IMessagePropagator messagePropagator)
        {
            _messagePropagator = messagePropagator;

            _layout = layout;
            
            _factory = factory;

            _menuItems = MenuBarFactory.GetAllMenuBarItems();
            _menuItems.Add(factory.GetMenuBarItems());

            Task.Run(() =>
            {
                GameEngine engine = new GameEngine(_messagePropagator);
                engine.Run();
            });
        }
        
        public IFactory Factory
        {
            get => _factory;
            set => this.RaiseAndSetIfChanged(ref _factory, value);
        }

        public IDock Layout
        {
            get => _layout;
            set => this.RaiseAndSetIfChanged(ref _layout, value);
        }

        public IList<MenuItemViewModel> MenuBarItems
        {
            get => _menuItems;
        }

        private IMessagePropagator _messagePropagator;
        
        private IFactory _factory;
        private IDock _layout;

        private List<MenuItemViewModel> _menuItems;
    }
}
