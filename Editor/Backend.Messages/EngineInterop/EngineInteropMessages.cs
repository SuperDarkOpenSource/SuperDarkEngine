using Backend.Common.MessagePropagator;

namespace Backend.Messages.EngineInterop
{
    public class SendRawMsg : Message<string> {}
    
    public class RawMsgReceived : Message<string> {}
}