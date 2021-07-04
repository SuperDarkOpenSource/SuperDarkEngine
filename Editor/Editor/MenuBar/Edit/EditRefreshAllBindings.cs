using System.Threading.Tasks;
using Backend.Common.MessagePropagator;
using Backend.Messages.Menu;
using Editor.MenuBar.Reflection;

namespace Editor.MenuBar.Edit
{
    [MenuBarItem("Refresh All Bindings", "Edit", 5)]
    public class EditRefreshAllBindings : MessageMenuBarItem<RefreshAllBindingsMessage>
    {
        public EditRefreshAllBindings(IMessagePropagator messagePropagator) :
            base(messagePropagator)
        {
        }
    }
}