using Blazocious.Core.YAML;
using Microsoft.Extensions.Caching.Memory;

namespace Blazocious.Core.Theme
{
    public class YamlThemeLoader : IThemeLoader
    {
        private readonly IMemoryCache _cache;

        public YamlThemeLoader(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<ParsedTheme> LoadThemeAsync(string defaultYamlPath, string? overrideYamlPath = null)
        {
            var cacheKey = $"theme_{defaultYamlPath}_{overrideYamlPath}";

            if (_cache.TryGetValue(cacheKey, out ParsedTheme? cachedTheme))
            {
                return cachedTheme!;
            }

            var defaultTheme = await LoadAndParseYamlAsync(defaultYamlPath);

            if (overrideYamlPath != null)
            {
                var overrideTheme = await LoadAndParseYamlAsync(overrideYamlPath);
                defaultTheme = ThemeMerger.Merge(defaultTheme, overrideTheme);
            }

            _cache.Set(cacheKey, defaultTheme, TimeSpan.FromMinutes(30));
            return defaultTheme;
        }

        private async Task<ParsedTheme> LoadAndParseYamlAsync(string path)
        {
            var yaml = await File.ReadAllTextAsync(path);
            return YamlParser.Parse<ParsedTheme>(yaml);
        }
    }
}
