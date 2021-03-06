using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public abstract class MessageBase
    {
        public Dictionary<ThreadHandler, ITaskDispatcher> TaskDispatchers { get; set; }
        
        public IMessageExceptionHandler MessageExceptionHandler { get; set; }
        
        protected ISubscriptionToken AddSubscriber(ISubscriptionToken token)
        {
            lock (_subscriptionTokens)
            {
                _subscriptionTokens.Add(token);

                return token;
            }
        }

        public void Unsubscribe(ISubscriptionToken token)
        {
            lock (_subscriptionTokens)
            {
                _subscriptionTokens.Remove(token);
            }
        }

        protected async Task InvokeAll(object[] parameters)
        {
            IReadOnlyList<ISubscriptionToken> subscribers = null;

            lock (_subscriptionTokens)
            {
                subscribers = new List<ISubscriptionToken>(_subscriptionTokens);
            }
            
            foreach(ISubscriptionToken token in subscribers)
            {
                try
                {
                    await token.Invoke(parameters);
                }
                catch (Exception e)
                {
                    await MessageExceptionHandler.OnExceptionRaised(e);
                }
            }
        }

        private readonly List<ISubscriptionToken> _subscriptionTokens = new List<ISubscriptionToken>();
    }

    public class Message : MessageBase 
    {
        public async Task Publish()
        {
            await InvokeAll(null);
        }

        public ISubscriptionToken Subscribe(Func<Task> func, ThreadHandler runOnThread, Func<bool> shouldRunPredicate = null)
        {
            ITaskDispatcher taskDispatcher = TaskDispatchers[runOnThread];
            
            return AddSubscriber(new SubscriptionToken(this, taskDispatcher, func, shouldRunPredicate));
        }
    }

    public class Message<T> : MessageBase
    {

        public async Task Publish(T arg)
        {
            await InvokeAll(new object[] { arg });
        }

        public ISubscriptionToken Subscribe(Func<T, Task> func, ThreadHandler runOnThread, Func<T, bool> shouldRunPredicate = null)
        {
            ITaskDispatcher taskDispatcher = TaskDispatchers[runOnThread];

            return AddSubscriber(new SubscriptionToken<T>(this, taskDispatcher, func, shouldRunPredicate));
        }
    }

    
}
