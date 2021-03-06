using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Backend.Common.MessagePropagator;
using Backend.Messages.EngineInterop;

namespace Backend.EngineInterop
{
    public class GameEngine
    {
        public GameEngine(IMessagePropagator messagePropagator)
        {
            _messagePropagator = messagePropagator;
            
            IntPtr external_window = EngineInterop.externalwindow_create();

            EngineInterop.ExternalWindowDeliverFn deliverFn = new EngineInterop.ExternalWindowDeliverFn(recieve_msg);
            EngineInterop.externalwindow_set_deliver_fn(external_window, Marshal.GetFunctionPointerForDelegate(deliverFn));

            EngineInterop.ExternalWindowReceiveFn receiveFn = new EngineInterop.ExternalWindowReceiveFn(send_msg);
            EngineInterop.externalwindow_set_receive_fn(external_window, Marshal.GetFunctionPointerForDelegate(receiveFn));

            EngineInterop.ExternalWindowUpdateFn updateFn = new EngineInterop.ExternalWindowUpdateFn(update);
            EngineInterop.externalwindow_set_update_fn(external_window, Marshal.GetFunctionPointerForDelegate(updateFn));

            IntPtr creation_info = EngineInterop.gameenginecreationinfo_create();
            EngineInterop.gameenginecreationinfo_set_externalwindow(creation_info, external_window);

            _engineInstance = EngineInterop.gameengine_create(creation_info);

            _messagePropagator.GetMessage<SendRawMsg>().Subscribe(OnSendRawMsgMessage);
        }

        public void Run()
        {
            EngineInterop.gameengine_run(_engineInstance);
        }

        private Task OnSendRawMsgMessage(string msg)
        {
            lock (_sendQueue)
            {
                _sendQueue.Enqueue(msg);
            }
            
            return Task.CompletedTask;
        }

        private void recieve_msg(string msg)
        {
            Task.Run(async () =>
            {
                // TODO: Investigate possible string memory problems
                await _messagePropagator.GetMessage<RawMsgReceived>().Publish(msg);
            });
        }

        private string send_msg()
        {
            string msg = null;
            
            lock (_sendQueue)
            {
                if (_sendQueue.Count > 0)
                {
                    msg = _sendQueue.Dequeue();
                }
            }

            return msg;
        }

        private void update()
        {
        }
        
        private IntPtr _engineInstance;
        private Queue<string> _sendQueue = new Queue<string>();
        private IMessagePropagator _messagePropagator;
    }
}