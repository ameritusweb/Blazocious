using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Test.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue? GetValueByPartialKey<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary,
            string partialKey,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
            where TKey : notnull
        {
            foreach (var kvp in dictionary)
            {
                if (kvp.Key.ToString()?.Contains(partialKey, comparison) == true)
                {
                    return kvp.Value;
                }
            }

            return default;
        }
    }

}
