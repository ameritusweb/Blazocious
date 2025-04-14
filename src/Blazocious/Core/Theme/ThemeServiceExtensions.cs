using Blazocious.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public static class ThemeServiceExtensions
    {
        public static IServiceCollection AddBlazociousThemes(
            this IServiceCollection services,
            string defaultPath,
            IEnumerable<(string Name, string Path)> overrides)
        {
            return services.AddBlazociousThemes(options =>
            {
                options.DefaultPath = defaultPath;
                options.Overrides.AddRange(overrides);
            });
        }

        public static IServiceCollection AddBlazociousThemes(
            this IServiceCollection services,
            Action<BlazociousThemeOptions> configure)
        {
            services.Configure(configure);
            services.AddSingleton<IThemeInitializer, ThemeInitializer>();
            services.AddScoped<ThemeContext>();
            services.AddMemoryCache()
                .AddScoped<IThemeLoader, YamlThemeLoader>()
                .AddSingleton<IThemeRegistry, ThemeRegistry>()
                .AddScoped<ThemeContext>()
                .AddHostedService<BlazociousThemeInitializer>();
            return services;
        }
    }
}
