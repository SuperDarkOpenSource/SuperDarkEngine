using System;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public class BackgroundTaskDispatcher : ITaskDispatcher
    {
        public async Task Dispatch(Func<Task> task)
        {
            await Task.Run(task);
        }

        public async Task Dispatch<T>(Func<T, Task> task, T parameter)
        {
            await Task.Run(async () => await task.Invoke(parameter));
        }
    }
}