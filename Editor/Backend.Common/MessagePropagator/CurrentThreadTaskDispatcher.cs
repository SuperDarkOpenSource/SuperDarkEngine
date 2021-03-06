using System;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public class CurrentThreadTaskDispatcher : ITaskDispatcher
    {
        public async Task Dispatch(Func<Task> task)
        {
            await task();
        }

        public async Task Dispatch<T>(Func<T, Task> task, T parameter)
        {
            await task(parameter);
        }
    }
}