using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Common.MessagePropagator
{
    public enum ThreadHandler
    {
        /// <summary>
        /// The Publisher's internal system can decide. Will usually be the thread that 
        /// publishes the message
        /// </summary>
        Default,

        /// <summary>
        /// Explicit Avalonia UI Thread.
        /// </summary>
        UIThread,

        /// <summary>
        /// Generic background thread in the threadpool
        /// </summary>
        Background
    }
}
