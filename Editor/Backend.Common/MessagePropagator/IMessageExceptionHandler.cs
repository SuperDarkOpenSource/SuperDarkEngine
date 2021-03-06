using System;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public interface IMessageExceptionHandler
    {
        public Task OnExceptionRaised(Exception e);
    }
}