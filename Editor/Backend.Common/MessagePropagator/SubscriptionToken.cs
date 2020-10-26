using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public class SubscriptionToken : ISubscriptionToken
    {
        internal SubscriptionToken(IDispatcher dispatcher, Func<Task> func, Func<bool> pred)
        {
            _dispacher = dispatcher;

            _func = func;

            _pred = pred;
        }

        

        async Task ISubscriptionToken.Invoke(object[] paramz)
        {
            if (!(_pred?.Invoke() ?? true))
            {
                return;
            }


            if(_dispacher == null)
            {
                await Task.Run(_func);
            }
            else
            {
                await _dispacher.InvokeAsync(_func);
            }
            
        }

        private Func<Task> _func;
        private Func<bool> _pred;
        private IDispatcher _dispacher;
    }

    public class SubscriptionToken<T>: ISubscriptionToken
    {
        internal SubscriptionToken(IDispatcher dispatcher, Func<T, Task> func, Func<bool> pred)
        {
            _dispacher = dispatcher;

            _func = func;

            _pred = pred;
        }

        public async Task Invoke(object[] paramz)
        {
            if(!(_pred?.Invoke() ?? true))
            {
                return;
            }

            var func = _func.Invoke((T)paramz[0]);

            if(_dispacher == null)
            {
                await Task.Run(() => _func((T)paramz[0]));
            }
            else
            {
                await _dispacher.InvokeAsync(() => _func((T)paramz[0]));
            }
        }

        private IDispatcher _dispacher;
        private Func<T, Task> _func;
        private Func<bool> _pred;
    }
}
