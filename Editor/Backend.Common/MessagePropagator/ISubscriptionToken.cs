using System.Threading.Tasks;

namespace Backend.Common.MessagePropagator
{
    public interface ISubscriptionToken
    {

        Task Invoke(object[] p);

        void Unsubscribe();
    }
}
