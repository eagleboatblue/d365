using Nov.Caps.Int.D365.Common;
using Nov.Caps.Int.D365.Data.Queries;
using Nov.Caps.Int.D365.Models.Customers;
using System.Threading;
using System.Threading.Channels;

namespace Nov.Caps.Int.D365.Data.Abstract
{
    public interface ICustomerRepository
    {
        ChannelReader<Result<Customer>> FindAsync(CustomerFindFilter filter, CancellationToken cancellation);
    }
}
