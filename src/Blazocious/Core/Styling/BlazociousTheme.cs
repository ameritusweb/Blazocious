using Blazocious.Core.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Blazocious.Core.Styling
{
    public class BlazociousTheme
    {
        private static Theme _currentTheme = new();
        private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        // Simple theme switching
        public static void SwitchTheme(Theme theme)
        {
            _currentTheme = theme;
            _cache.Clear(); // Clear cached styles since tokens changed
        }

        // Get current token value
        public static string Token(string name) =>
            _currentTheme.Tokens.TryGetValue(name, out var value) ? value : "";
    }
}
