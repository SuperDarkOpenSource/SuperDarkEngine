using Backend.Common.MessagePropagator;
using System;
using System.Threading.Tasks;

namespace Backend.Messages.Application
{
    public class ApplicationOpened : Message { }

    public class ApplicationClosing : Message<ApplicationClosing.ApplicationClosingArguments>
    {
        public class ApplicationClosingArguments
        {
            public bool CancelClosing = false;
        }
    }

    public class ApplicationClosed : Message { }

    public class PushUndo : Message<PushUndo.PushUndoArguments>
    {
        public class PushUndoArguments
        {
            public Func<Task> RedoFunc_;
        }
    }
}
