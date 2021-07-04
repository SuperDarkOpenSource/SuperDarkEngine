using System.Threading.Tasks;
using System.Windows.Input;
using Backend.Common.MessagePropagator;
using ReactiveUI;

namespace Editor.MenuBar.Reflection
{
    public abstract class BaseMenuBarItem
    {
        protected BaseMenuBarItem(IMessagePropagator messagePropagator)
        {
            _messagePropagator = messagePropagator;
            _command = ReactiveCommand.CreateFromTask<object>(OnClick);
        }
        
        public ICommand Command => _command;

        protected abstract Task OnClick(object parameter);

        protected readonly IMessagePropagator _messagePropagator;
        
        private readonly ICommand _command;
    }
}