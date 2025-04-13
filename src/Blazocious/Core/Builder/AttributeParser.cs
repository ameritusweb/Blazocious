using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public static class AttributeParser
    {
        private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        // Matches attribute name=value pairs, handling spaces around equals signs
        // and values that might contain interpolated expressions with semicolons inside them
        private static readonly Regex AttributePattern = new(
            @"([^=\s]+)\s*=\s*([^;]*(?:{[^}]*}[^;]*)*);?\s*",
            RegexOptions.Compiled
        );

        public static IEnumerable<(string Name, object Value)> Parse(string attributeString)
        {
            // Normalize whitespace but preserve spaces within interpolated expressions
            attributeString = NormalizeWhitespace(attributeString.Trim());

            var cacheKey = $"attrs_{attributeString.GetHashCode()}";

            // Try to get parsed pattern from cache
            if (_cache.TryGetValue(cacheKey, out List<(string Name, object Value)>? cached))
            {
                return cached;
            }

            var attributes = new List<(string Name, object Value)>();

            foreach (Match match in AttributePattern.Matches(attributeString))
            {
                var name = match.Groups[1].Value;
                var value = match.Groups[2].Value.Trim();

                // Handle boolean attributes
                if (string.IsNullOrEmpty(value))
                {
                    attributes.Add((name, true));
                    continue;
                }

                // Handle literal values vs interpolated expressions
                attributes.Add((name, value));
            }

            // Cache the parsed pattern
            _cache.Set(cacheKey, attributes, TimeSpan.FromHours(1));

            return attributes;
        }

        private static string NormalizeWhitespace(string input)
        {
            var inExpression = false;
            var result = new System.Text.StringBuilder();
            var lastChar = '\0';

            foreach (var c in input)
            {
                if (c == '{') inExpression = true;
                if (c == '}') inExpression = false;

                // Collapse multiple spaces outside expressions
                if (char.IsWhiteSpace(c) && !inExpression)
                {
                    if (!char.IsWhiteSpace(lastChar))
                        result.Append(' ');
                }
                else
                {
                    result.Append(c);
                }

                lastChar = c;
            }

            return result.ToString();
        }
    }
}
