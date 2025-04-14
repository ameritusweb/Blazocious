using Blazocious.Core.YAML;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blazocious.Core.Theme
{
    public class ThemeInitializer : IThemeInitializer
    {
        private readonly IOptions<BlazociousThemeOptions> _options;
        private readonly IThemeLoader _themeLoader;
        private readonly IThemeRegistry _registry;
        private readonly ThemeContext _themeContext;
        private readonly ILogger<ThemeInitializer> _logger;

        public ThemeInitializer(
            IOptions<BlazociousThemeOptions> options,
            IThemeLoader themeLoader,
            IThemeRegistry registry,
            ThemeContext themeContext,
            ILogger<ThemeInitializer> logger)
        {
            _options = options;
            _themeLoader = themeLoader;
            _registry = registry;
            _themeContext = themeContext;
            _logger = logger;
        }

        public async Task InitializeThemes()
        {
            try
            {
                var options = _options.Value;

                // Load and register base theme
                if (options.Debug)
                    _logger.LogInformation("Loading base theme from {Path}", options.DefaultPath);

                var defaultTheme = await _themeLoader.LoadThemeAsync(options.DefaultPath);
                _registry.Register("default", defaultTheme);

                // Load and merge overrides
                foreach (var (name, path) in options.Overrides)
                {
                    if (options.Debug)
                        _logger.LogInformation("Loading override theme '{Name}' from {Path}", name, path);

                    var merged = await _themeLoader.LoadThemeAsync(options.DefaultPath, path);
                    _registry.Register(name, merged);
                }

                // Set initial variant
                var variant = options.DefaultVariant ?? "default";
                await _themeContext.SetVariantAsync(variant, _registry);

                if (options.Debug)
                    _logger.LogInformation("Theme initialization complete. Current: {Variant}", variant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize themes");
                throw;
            }
        }
    }
}