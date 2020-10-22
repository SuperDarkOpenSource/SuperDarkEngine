using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Common.MessagePropagator
{
    public class MessageBase
    {
        internal MessageBase()
        {

        }
    }

    public class Message<T> : MessageBase {}

    public class Message : MessageBase {}
}
