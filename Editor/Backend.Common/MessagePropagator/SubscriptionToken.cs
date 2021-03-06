using System;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public class SubscriptionToken : ISubscriptionToken
    {
        internal SubscriptionToken(MessageBase messageBase, ITaskDispatcher dispatcher, Func<Task> func, Func<bool> pred)
        {
            _dispatcher = dispatcher;

            _func = func;

            _pred = pred;

            _messageBaseBase = messageBase;
        }

        async Task ISubscriptionToken.Invoke(object[] _)
        {
            if (!(_pred?.Invoke() ?? true))
            {
                return;
            }
            
            await _dispatcher.Dispatch(_func);
        }

        public void Unsubscribe()
        {
            _messageBaseBase.Unsubscribe(this);
        }

        private readonly Func<Task> _func;
        private readonly Func<bool> _pred;
        private readonly ITaskDispatcher _dispatcher;
        private readonly MessageBase _messageBaseBase;
    }

    public class SubscriptionToken<T> : ISubscriptionToken
    {
        internal SubscriptionToken(MessageBase messageBase, ITaskDispatcher dispatcher, Func<T, Task> func, Func<T, bool> pred)
        {
            _dispatcher = dispatcher;

            _func = func;

            _pred = pred;

            _messageBase = messageBase;
        }

        public async Task Invoke(object[] p)
        {
            T parameter = (T)p[0];
            
            if(!(_pred?.Invoke(parameter) ?? true))
            {
                return;
            }

            await _dispatcher.Dispatch(_func, parameter);
        }

        public void Unsubscribe()
        {
            _messageBase.Unsubscribe(this);
        }

        private readonly ITaskDispatcher _dispatcher;
        private readonly Func<T, Task> _func;
        private readonly Func<T, bool> _pred;
        private readonly MessageBase _messageBase;
    }
}
