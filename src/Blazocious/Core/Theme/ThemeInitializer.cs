using Blazocious.Core.YAML;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public class ThemeInitializer : IThemeInitializer
    {
        private readonly IOptions<BlazociousThemeOptions> _options;
        private readonly ThemeContext _themeContext;
        private readonly ThemeMerger _themeMerger;
        private readonly ILogger<ThemeInitializer> _logger;

        public ThemeInitializer(
            IOptions<BlazociousThemeOptions> options,
            ThemeContext themeContext,
            ThemeMerger themeMerger,
            ILogger<ThemeInitializer> logger)
        {
            _options = options;
            _themeContext = themeContext;
            _themeMerger = themeMerger;
            _logger = logger;
        }

        public async Task InitializeThemes()
        {
            try
            {
                var options = _options.Value;

                // Load default theme
                if (options.Debug)
                {
                    _logger.LogInformation("Loading default theme from {Path}", options.DefaultPath);
                }

                var defaultTheme = await LoadTheme(options.DefaultPath);
                await _themeContext.RegisterTheme("dark", defaultTheme);

                // Load and merge overrides
                foreach (var (name, path) in options.Overrides)
                {
                    if (options.Debug)
                    {
                        _logger.LogInformation("Loading {Name} theme from {Path}", name, path);
                    }

                    var overrideTheme = await LoadTheme(path);
                    var mergedTheme = _themeMerger.Merge(defaultTheme, overrideTheme);
                    await _themeContext.RegisterTheme(name, mergedTheme);
                }

                // Set default variant
                await _themeContext.SetVariant(options.DefaultVariant);

                if (options.Debug)
                {
                    _logger.LogInformation("Theme initialization complete");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize themes");
                throw;
            }
        }

        private async Task<ParsedTheme> LoadTheme(string path)
        {
            if (Directory.Exists(path))
            {
                // Load from directory
                return await LoadThemeFromDirectory(path);
            }
            else if (File.Exists(path))
            {
                // Load single file
                var yaml = await File.ReadAllTextAsync(path);
                return ParseYaml(yaml);
            }

            throw new FileNotFoundException($"Theme path not found: {path}");
        }

        private async Task<ParsedTheme> LoadThemeFromDirectory(string directory)
        {
            var mergedTheme = new ParsedTheme();
            var parser = new YamlParser();

            // Load tokens first
            var tokensPath = Path.Combine(directory, "tokens.yaml");
            if (File.Exists(tokensPath))
            {
                var tokensYaml = await File.ReadAllTextAsync(tokensPath);
                var tokens = parser.Parse(tokensYaml);
                mergedTheme.Tokens = tokens.Tokens;
            }

            // Load all component files
            foreach (var file in Directory.GetFiles(directory, "*.yaml"))
            {
                if (Path.GetFileName(file) == "tokens.yaml") continue;

                var yaml = await File.ReadAllTextAsync(file);
                var parsed = parser.Parse(yaml);

                // Merge components and styles
                foreach (var (key, value) in parsed.Components)
                {
                    mergedTheme.Components[key] = value;
                }
                foreach (var (key, value) in parsed.Styles)
                {
                    mergedTheme.Styles[key] = value;
                }
            }

            return mergedTheme;
        }

        private ParsedTheme ParseYaml(string yaml)
        {
            var parser = new YamlParser();
            var (tokens, components, styles) = parser.Parse(yaml);
            return new ParsedTheme
            {
                Tokens = tokens,
                Components = components,
                Styles = styles
            };
        }
    }
}
