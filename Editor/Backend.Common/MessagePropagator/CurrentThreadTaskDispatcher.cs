using System;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public class CurrentThreadTaskDispatcher : ITaskDispatcher
    {
        public Task Dispatch(Func<Task> task)
        {
            return task();
        }

        public Task Dispatch<T>(Func<T, Task> task, T parameter)
        {
            return task(parameter);
        }
    }
}