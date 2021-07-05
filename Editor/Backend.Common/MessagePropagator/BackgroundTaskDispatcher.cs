using System;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public class BackgroundTaskDispatcher : ITaskDispatcher
    {
        public Task Dispatch(Func<Task> task)
        {
            return Task.Run(task);
        }

        public Task Dispatch<T>(Func<T, Task> task, T parameter)
        {
            return Task.Run(async () => await task.Invoke(parameter));
        }
    }
}