using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public interface ISubscriptionToken
    {

        Task Invoke(object[] paramz);

    }
}
