namespace Backend.Common.MessagePropagator
{
    public interface IMessagePropagator
    {

        T GetMessage<T>() where T : MessageBase, new();

    }
}
