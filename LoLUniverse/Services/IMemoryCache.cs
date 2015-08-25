using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLUniverse.Services
{
    public interface IMemoryCache
    {
        T Get<T>(string key, DateTime expiry, Func<T> getFromRiotFunc);
    }
}
