using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Threading;
using ReactiveUI;

namespace Backend.Common.MessagePropagator
{
    public abstract class MessageBase
    {
        protected ISubscriptionToken AddSubscriber(ISubscriptionToken token)
        {
            _subscriptionTokens.Add(token);

            return token;
        }

        public void Unsubscribe(ISubscriptionToken token)
            => _subscriptionTokens.Remove(token);

        protected async Task InvokeAll(object[] paramz)
        {
            foreach(ISubscriptionToken token in _subscriptionTokens)
            {
                await token.Invoke(paramz);
            }
        }

        protected IDispatcher GetDispatcher(ThreadHandler handler)
        {
            switch(handler)
            {
                case ThreadHandler.UIThread:
                    return Dispatcher.UIThread;

                case ThreadHandler.Background:
                    return null;

                default:
                    return Dispatcher.UIThread;
                
            }
        }

        List<ISubscriptionToken> _subscriptionTokens = new List<ISubscriptionToken>();
        
    }

    public class Message : MessageBase 
    {
        public async Task Publish()
        {
            await InvokeAll(new object[] { });
        }

        public ISubscriptionToken Subscribe(Func<Task> func, ThreadHandler runOnThread = ThreadHandler.Default, Func<bool> shouldRunPredicate = null)
            => AddSubscriber(new SubscriptionToken(GetDispatcher(runOnThread), func, shouldRunPredicate));
    }

    public class Message<T> : MessageBase
    {

        public async Task Publish(T arg)
        {
            await InvokeAll(new object[] { arg });
        }

        public ISubscriptionToken Subscribe(Func<T, Task> func, ThreadHandler runOnThread = ThreadHandler.Default, Func<bool> shouldRunPredicate = null)
            => AddSubscriber(new SubscriptionToken<T>(GetDispatcher(runOnThread), func, shouldRunPredicate));

    }

    
}
