using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Backend.Common.MessagePropagator;

namespace Backend.UI.MessagePropagator
{
    public class AvaloniaUITaskDispatcher : ITaskDispatcher
    {
        public async Task Dispatch(Func<Task> task)
        {
            await _dispatcher.InvokeAsync(task);
        }

        public async Task Dispatch<T>(Func<T, Task> task, T parameter)
        {
            await _dispatcher.InvokeAsync(async () => await task(parameter));
        }

        private readonly Dispatcher _dispatcher = Dispatcher.UIThread;
    }
}