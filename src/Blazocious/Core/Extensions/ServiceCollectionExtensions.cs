using Blazocious.Core.Builder;
using Blazocious.Core.Styling;
using Blazocious.Core.Theme;
using Blazocious.Core.Trackers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

            if (configure == null)
            {
                var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

                var blazociousSection = configuration.GetSection("Blazocious");

                var blazociousOptions = blazociousSection
                    .Get<BlazociousOptions>();

                if (blazociousOptions != null)
                {
                    options.DefaultThemePath = blazociousOptions.DefaultThemePath;
                    options.DefaultThemeVariant = blazociousOptions.DefaultThemeVariant;
                    options.DefaultStylesPath = blazociousOptions.DefaultStylesPath;
                    options.CssOutputPath = blazociousOptions.CssOutputPath;
                    options.Debug = blazociousOptions.Debug;
                    options.ValidateTokens = blazociousOptions.ValidateTokens;
                    options.CacheThemes = blazociousOptions.CacheThemes;
                    options.CacheDuration = blazociousOptions.CacheDuration;

                    services.Configure<BlazociousOptions>(blazociousSection);
                }
            }

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

            services.AddSingleton<IClassUsageTracker, ClassUsageTracker>();

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
