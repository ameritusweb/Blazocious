using Blazocious.Core.Builder;
using Blazocious.Core.Styling;
using Blazocious.Core.Theme;
using Blazocious.Core.Trackers;
using Microsoft.Extensions.DependencyInjection;

namespace Blazocious.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazocious(
            this IServiceCollection services,
            Action<BlazociousOptions>? configure = null)
        {
            // Create and configure options
            var options = new BlazociousOptions();
            configure?.Invoke(options);

            // Add core services
            services.AddMemoryCache();

            // Add theme services with default configuration if not specified
            if (!services.Any(s => s.ServiceType == typeof(IThemeRegistry)))
            {
                services.AddBlazociousThemes(themeOptions =>
                {
                    themeOptions.DefaultPath = options.DefaultThemePath ?? "themes/default";
                    themeOptions.DefaultVariant = options.DefaultThemeVariant ?? "light";
                    themeOptions.Debug = options.Debug;
                    themeOptions.ValidateTokens = options.ValidateTokens;
                    themeOptions.CacheThemes = options.CacheThemes;
                    themeOptions.CacheDuration = options.CacheDuration ?? TimeSpan.FromHours(1);

                    if (options.ThemeOverrides != null)
                    {
                        themeOptions.Overrides.AddRange(options.ThemeOverrides);
                    }
                });
            }

            services.Configure<BlazociousStylesOptions>(stylesOptions =>
            {
                if (!string.IsNullOrEmpty(options.DefaultStylesPath))
                {
                    stylesOptions.YamlPath = options.DefaultStylesPath;
                }
            });

            // Add styling services
            services.AddScoped<BlazociousStyles>();

            services.AddSingleton<IClassUsageTracker>();

            // Initialize element builder extensions
            if (!string.IsNullOrEmpty(options.DefaultStylesPath))
            {
                var stylesContent = File.ReadAllText(options.DefaultStylesPath);
                ElementBuilderStylingExtensions.InitializeStyles(stylesContent);
            }

            return services;
        }
    }
}
