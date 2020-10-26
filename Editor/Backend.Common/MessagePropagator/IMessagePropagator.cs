﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public interface IMessagePropagator
    {

        T GetMessage<T>() where T : MessageBase, new();

    }
}
