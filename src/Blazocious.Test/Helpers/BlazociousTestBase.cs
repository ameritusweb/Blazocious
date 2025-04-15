using Blazocious.Core.Builder;
using Blazocious.Core.Styling;
using Blazocious.Core.Theme;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace Blazocious.Test.Helpers
{
    public abstract class BlazociousTestBase : TestContext
    {
        protected readonly string DefaultTestStyles = @"
button-primary:
  styles:
    - background-color: '#1f2937'
    - color: '#ffffff'
    - padding: '1rem 2rem'

md-class:
  styles:
    - background-color: '#1f2937'
    - color: '#ffffff'
    - padding: '1rem 2rem'

card:
  styles:
    - background-color: '#ffffff'
    - border-radius: '0.5rem'
    - padding: '1rem'";

        protected BlazociousTestBase()
        {
            // Add your core Blazocious services
            Services.AddMemoryCache();

            // Configure styles
            ConfigureTestStyles(DefaultTestStyles);

            // Add services
            Services.AddScoped<ThemeContext>();
            Services.AddSingleton<IThemeRegistry, ThemeRegistry>();
            Services.AddScoped<IThemeLoader, YamlThemeLoader>();
            Services.AddScoped<BlazociousStyles>();

            // Optionally preload or mock a theme
            var themeContext = Services.GetRequiredService<ThemeContext>();
            var themeRegistry = Services.GetRequiredService<IThemeRegistry>();
            themeRegistry.Register("default", new ParsedTheme());
            themeContext.SetVariantAsync("default", themeRegistry).GetAwaiter().GetResult();
        }

        // Helper method to override default styles for specific tests
        protected virtual void ConfigureTestStyles(string yamlContent)
        {
            Services.Configure<BlazociousStylesOptions>(options =>
            {
                options.YamlContent = yamlContent;
            });

            ElementBuilderStylingExtensions.InitializeStyles(yamlContent);
        }
    }
}