using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Extensions
{
    public static class MemoryCacheExtensions
    {
        public static void Clear(this IMemoryCache cache)
        {
            if (cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
}
