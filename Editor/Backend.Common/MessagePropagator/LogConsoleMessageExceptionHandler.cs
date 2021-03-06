using System;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public class LogConsoleMessageExceptionHandler : IMessageExceptionHandler
    {
        public Task OnExceptionRaised(Exception e)
        {
            Console.WriteLine(e.Message);

            return Task.CompletedTask;
        }
    }
}