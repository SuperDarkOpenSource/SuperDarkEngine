using Backend.Common.MessagePropagator;
using Backend.Common.Tools;
using ReactiveUI;
using System.Threading.Tasks;
using System.Windows.Input;
using Tool.Example.Messages;

namespace Tool.Example.Document
{
    [ToolWindow("Example Document", ToolWindowType.Document, typeof(ExampleDocumentView))]
    [DefaultToolWindow()]
    public class ExampleDocumentViewModel : BaseToolWindow
    {
        public ExampleDocumentViewModel(IMessagePropagator messagePropagator) : 
            base(messagePropagator)
        {
            CreateCommands();
        }

        public ExampleDocumentViewModel(BaseToolWindow copy) : 
            base(copy)
        {
            CreateCommands();
        }

        public string TestMessage
        {
            get => _testMessage;
            set => this.RaiseAndSetIfChanged(ref _testMessage, value);
        }

        public ICommand FireTestMessageCommand
        {
            get => _fireTestMessageCommand;
        }


        private async Task FireTestMessage()
        {
            await _MessagePropagator.GetMessage<ExampleTestMessage>().Publish(_testMessage);
        }

        private void CreateCommands()
        {
            _fireTestMessageCommand = ReactiveCommand.CreateFromTask(FireTestMessage);
        }

        private string _testMessage = "This is a test message!";
        private ICommand _fireTestMessageCommand = null;
    }
}