using System;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public interface ITaskDispatcher
    {
        public Task Dispatch(Func<Task> task);

        public Task Dispatch<T>(Func<T, Task> task, T parameter);
    }
}