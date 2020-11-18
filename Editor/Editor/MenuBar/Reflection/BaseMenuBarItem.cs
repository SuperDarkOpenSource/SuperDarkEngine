using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace Editor.MenuBar.Reflection
{
    public abstract class BaseMenuBarItem
    {
        protected BaseMenuBarItem()
        {
            _command = ReactiveCommand.CreateFromTask<object>(OnClick);
        }
        
        public ICommand Command => _command;

        protected abstract Task OnClick(object parameter);

        private readonly ICommand _command;
    }
}