using Backend.Common.MessagePropagator;
using Backend.EngineInterop;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            _MessagePropagator.GetMessage<TestMessage>().Subscribe(OnTestMessage);
            _MessagePropagator.GetMessage<TestMessage1>().Subscribe(OnTestMessage1);
        }


        public ICommand InteropTest => ReactiveCommand.Create(DoInteropTest);

        public ICommand MessagePropagatorTest => ReactiveCommand.Create(DoMessageTest);

        public ulong LowerValue 
        { 
            get => _LowerValue; 
            set => this.RaiseAndSetIfChanged(ref _LowerValue, value); 
        }

        public ulong HigherValue 
        { 
            get => _HigherValue; 
            set => this.RaiseAndSetIfChanged(ref _HigherValue, value); 
        }


        private void DoInteropTest()
        {
            ulong sum = EngineInterop.super_dark_sum(LowerValue, HigherValue);

            var msgbox = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Super Dark Engine Interop", 
                    $"Result from pub fn super_dark_sum() = {sum}");

            msgbox.Show();

        }

        private async Task DoMessageTest()
        {
            await _MessagePropagator.GetMessage<TestMessage>().Publish();
            await _MessagePropagator.GetMessage<TestMessage1>().Publish("Hello World Again!");
        }

        Task OnTestMessage()
        {

            var msgbox = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Super Dark MessageTest",
                    "Hello World");

            msgbox.Show();

            return Task.CompletedTask;
        }

        Task OnTestMessage1(string msg)
        {
            var msgbox = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Super Dark MessageTest",
                    msg);

            msgbox.Show();

            return Task.CompletedTask;
        }

        public class TestMessage : Message { }

        public class TestMessage1 : Message<string> { }

        private ulong _LowerValue = 0;
        private ulong _HigherValue = 10;

        private IMessagePropagator _MessagePropagator = new MessagePropagator();
    }
}
