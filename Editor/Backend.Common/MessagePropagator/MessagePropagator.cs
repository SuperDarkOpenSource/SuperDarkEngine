using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public class MessagePropagator : IMessagePropagator
    {
        public T GetMessage<T>() where T : MessageBase, new()
        {
            T message = null;

            lock(_Messages)
            {
                if(!_Messages.ContainsKey(typeof(T)))
                {
                    message = new T();

                    _Messages.Add(typeof(T), message);
                }
                else
                {
                    message = _Messages[typeof(T)] as T;
                }
            }

            return message;
        }


        private Hashtable _Messages = new Hashtable();
    }
}
