using Nov.Caps.Int.D365.Messaging.Models;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Messaging
{
    public interface IHandler<TIn, TOut>
    {
        Task<TOut> Handle(Payload<TIn> input);
    }
}
