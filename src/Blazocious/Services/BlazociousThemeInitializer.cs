using Blazocious.Core.Theme;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Services
{
    public class BlazociousThemeInitializer : IHostedService
    {
        private readonly IThemeLoader _loader;
        private readonly IThemeRegistry _registry;
        private readonly ThemeContext _context;
        private readonly ThemeOptions _options;

        public BlazociousThemeInitializer(
            IThemeLoader loader,
            IThemeRegistry registry,
            ThemeContext context,
            IOptions<ThemeOptions> options)
        {
            _loader = loader;
            _registry = registry;
            _context = context;
            _options = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Load default theme
            var defaultTheme = await _loader.LoadThemeAsync(_options.DefaultTheme);
            _registry.Register("default", defaultTheme);

            // Load light theme if configured
            if (_options.LightOverride != null)
            {
                var light = await _loader.LoadThemeAsync(
                    _options.DefaultTheme,
                    _options.LightOverride
                );
                _registry.Register("light", light);
            }

            // Load dark theme if configured
            if (_options.DarkOverride != null)
            {
                var dark = await _loader.LoadThemeAsync(
                    _options.DefaultTheme,
                    _options.DarkOverride
                );
                _registry.Register("dark", dark);
            }

            // Set initial theme
            await _context.SetVariantAsync(
                _options.InitialVariant ?? "default",
                _registry
            );
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
