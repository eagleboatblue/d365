using Nov.Caps.Int.D365.Common;
using Nov.Caps.Int.D365.Models.Products;
using System.Threading;
using System.Threading.Channels;

namespace Nov.Caps.Int.D365.Data.Abstract
{
    public interface IProductRepository
    {
        ChannelReader<Result<Product>> FindAsync(CancellationToken cancellation);
    }
}