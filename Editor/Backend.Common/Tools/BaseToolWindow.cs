using Backend.Common.MessagePropagator;
using Dock.Model.Controls;
using System.Reflection;

namespace Backend.Common.Tools
{
    public class BaseToolWindow : Tool
    {
        protected BaseToolWindow(IMessagePropagator messagePropagator)
        {
            _MessagePropagator = messagePropagator;

            ToolWindowAttribute toolWindowAttribute =
                this.GetType().GetCustomAttribute<ToolWindowAttribute>();

            Id = toolWindowAttribute.DisplayName;
            Title = toolWindowAttribute.DisplayName;
        }

        public BaseToolWindow(BaseToolWindow copy)
        {
            _MessagePropagator = copy._MessagePropagator;
        }

        protected IMessagePropagator _MessagePropagator;
    }
}
