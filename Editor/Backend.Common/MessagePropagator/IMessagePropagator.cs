using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public interface IMessagePropagator
    {
        Task Publish<T>(params object[] par) where T : MessageBase;

        SubscriptionToken<T> Subscribe<T>() where T : MessageBase;

    }
}
