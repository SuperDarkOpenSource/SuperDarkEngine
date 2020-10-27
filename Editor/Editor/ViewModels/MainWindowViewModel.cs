using Backend.EngineInterop;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand InteropTest => ReactiveCommand.Create(DoInteropTest);

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

        private ulong _LowerValue = 0;
        private ulong _HigherValue = 10;
    }
}
