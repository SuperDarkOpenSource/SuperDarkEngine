using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public class MessagePropagator : IMessagePropagator
    {
        public Task Publish<T>(params object[] par) where T : MessageBase
        {
            throw new NotImplementedException();
        }

        public SubscriptionToken<T> Subscribe<T>() where T : MessageBase
        {
            throw new NotImplementedException();
        }
    }
}
