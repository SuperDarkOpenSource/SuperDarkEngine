using System.Threading.Tasks;
using Backend.Common.MessagePropagator;

namespace Editor.MenuBar.Reflection
{
    public abstract class MessageMenuBarItem<T> : BaseMenuBarItem where T : Message, new()
    {
        protected MessageMenuBarItem(IMessagePropagator messagePropagator) : 
            base(messagePropagator)
        {
        }

        protected override Task OnClick(object parameter)
        {
            return _messagePropagator.GetMessage<T>().Publish();
        }
    }
}