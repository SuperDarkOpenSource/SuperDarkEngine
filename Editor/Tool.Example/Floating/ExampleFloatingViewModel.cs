using Backend.Common.MessagePropagator;
using Backend.UI.Tools;
using ReactiveUI;
using System.Threading.Tasks;
using Tool.Example.Floating;
using Tool.Example.Messages;

namespace Tool.Example.Floating
{
    [ToolWindow("Example Floating", ToolWindowType.Floating, typeof(ExampleFloatingView))]
    [DefaultToolWindow(DefaultFloatingPostion = DefaultFloatingPostion.Right)]
    public class ExampleFloatingViewModel : BaseToolWindow
    {
        
        public ExampleFloatingViewModel(IMessagePropagator messagePropagator) :
            base(messagePropagator)
        {
            RegisterMessages();
        }

        public ExampleFloatingViewModel(ExampleFloatingViewModel copy) :
            base(copy)
        {
            RegisterMessages();
        }

        public string TestMessage
        {
            get => _testMessage;
            set => this.RaiseAndSetIfChanged(ref _testMessage, value);
        }


        private void RegisterMessages()
        {
            _messagePropagator.GetMessage<ExampleTestMessage>()
                .Subscribe(OnExampleTestMessage, ThreadHandler.UIThread);

        }

        private Task OnExampleTestMessage(string msg)
        {
            TestMessage = msg;

            return Task.CompletedTask;
        }


        private string _testMessage = "";
    }
}
