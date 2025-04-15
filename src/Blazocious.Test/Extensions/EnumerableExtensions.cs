using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Test.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool PartialContainsKey(
            this IEnumerable<string> source,
            string partialKey,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (source == null || partialKey == null)
                return false;

            return source.Any(s => s != null && s.Contains(partialKey, comparison));
        }
    }
}
