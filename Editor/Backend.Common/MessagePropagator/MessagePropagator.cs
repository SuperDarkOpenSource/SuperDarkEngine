using System.Collections;
using System.Collections.Generic;

namespace Backend.Common.MessagePropagator
{
    public class MessagePropagator : IMessagePropagator
    {
        public MessagePropagator(IMessageExceptionHandler messageExceptionHandler)
        {
            _messageExceptionHandler = messageExceptionHandler;
            
            RegisterDefaultTaskDispatchers();
        }

        public void RegisterTaskDispatcher(ThreadHandler threadHandler, ITaskDispatcher taskDispatcher)
        {
            _taskDispatchers.Add(threadHandler, taskDispatcher);
        }

        public T GetMessage<T>() where T : MessageBase, new()
        {
            T message = null;

            lock(_messages)
            {
                if(!_messages.ContainsKey(typeof(T)))
                {
                    // Create the new Message
                    message = new T();

                    // Register the stuff it needs
                    message.TaskDispatchers = new Dictionary<ThreadHandler, ITaskDispatcher>(_taskDispatchers);
                    message.MessageExceptionHandler = _messageExceptionHandler;

                    // Keep it around for next time.
                    _messages.Add(typeof(T), message);
                }
                else
                {
                    // Previously created
                    message = _messages[typeof(T)] as T;
                }
            }

            return message;
        }
        
        private void RegisterDefaultTaskDispatchers()
        {
            RegisterTaskDispatcher(ThreadHandler.Default, new CurrentThreadTaskDispatcher());
            RegisterTaskDispatcher(ThreadHandler.Background, new BackgroundTaskDispatcher());
        }

        private readonly Hashtable _messages = new Hashtable();
        private IMessageExceptionHandler _messageExceptionHandler;

        private Dictionary<ThreadHandler, ITaskDispatcher> _taskDispatchers =
            new Dictionary<ThreadHandler, ITaskDispatcher>();
    }
}
