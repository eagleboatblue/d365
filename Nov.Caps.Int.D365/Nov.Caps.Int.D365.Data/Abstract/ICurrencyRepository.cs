using Nov.Caps.Int.D365.Models.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Data.Abstract
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<Currency>> FindByCodes(string[] codes, DateTime dateTime);

        Task<IEnumerable<Currency>> FindByCodes(string[] codes);
    }
}
