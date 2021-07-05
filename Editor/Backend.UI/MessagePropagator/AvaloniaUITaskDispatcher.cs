using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Backend.Common.MessagePropagator;

namespace Backend.UI.MessagePropagator
{
    public class AvaloniaUITaskDispatcher : ITaskDispatcher
    {
        public Task Dispatch(Func<Task> task)
        {
            return _dispatcher.InvokeAsync(task);
        }

        public Task Dispatch<T>(Func<T, Task> task, T parameter)
        {
            return _dispatcher.InvokeAsync(async () => await task(parameter));
        }

        private readonly Dispatcher _dispatcher = Dispatcher.UIThread;
    }
}