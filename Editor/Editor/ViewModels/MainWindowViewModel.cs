using System.Windows.Input;
using ReactiveUI;
using Backend.Common.MessagePropagator;
using Dock.Model;

namespace Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IFactory factory, IDock layout, IMessagePropagator messagePropagator)
        {
            _messagePropagator = messagePropagator;

            _layout = layout;
            
            _factory = factory;
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

        private IMessagePropagator _messagePropagator;
        
        private IFactory _factory;
        private IDock _layout;
    }
}
