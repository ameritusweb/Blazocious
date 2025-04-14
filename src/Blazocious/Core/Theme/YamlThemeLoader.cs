using Blazocious.Core.YAML;
using Microsoft.Extensions.Caching.Memory;

namespace Blazocious.Core.Theme;

public class YamlThemeLoader : IThemeLoader
{
    private readonly IMemoryCache _cache;
    private readonly YamlParser _parser;
    private readonly IThemeMerger _themeMerger;

    public YamlThemeLoader(IMemoryCache cache, IThemeMerger themeMerger)
    {
        _cache = cache;
        _parser = new YamlParser();
        _themeMerger = themeMerger;
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
            defaultTheme = _themeMerger.Merge(defaultTheme, overrideTheme);
        }

        _cache.Set(cacheKey, defaultTheme, TimeSpan.FromMinutes(30));
        return defaultTheme;
    }

    private async Task<ParsedTheme> LoadAndParseYamlAsync(string path)
    {
        var yaml = await File.ReadAllTextAsync(path);
        var (tokens, components, styles) = _parser.Parse(yaml);

        return new ParsedTheme
        {
            Tokens = tokens,
            Components = components,
            Styles = styles?.SelectMany(s => s.Value.Styles).ToList()
        };
    }
}