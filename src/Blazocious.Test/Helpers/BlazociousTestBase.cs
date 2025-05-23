﻿using Blazocious.Core.Builder;
using Blazocious.Core.Extensions;
using Blazocious.Core.Styling;
using Blazocious.Core.Theme;
using Bunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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

        public new static IServiceProvider Services => Element.ServiceProvider;

        protected BlazociousTestBase()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var blazociousOptions = configuration.GetSection("Blazocious").Get<BlazociousOptions>();

            base.Services.AddBlazocious(options =>
            {
                options.DefaultThemePath = blazociousOptions.DefaultThemePath;
                options.DefaultThemeVariant = blazociousOptions.DefaultThemeVariant;
                options.DefaultStylesPath = blazociousOptions.DefaultStylesPath;
                options.CssOutputPath = blazociousOptions.CssOutputPath;
                options.Debug = blazociousOptions.Debug;
                options.ValidateTokens = blazociousOptions.ValidateTokens;
                options.CacheThemes = blazociousOptions.CacheThemes;
                options.CacheDuration = blazociousOptions.CacheDuration;
            });

            // Add additional services that might be needed for testing
            base.Services.AddScoped<IThemeMerger, ThemeMerger>();
            base.Services.AddSingleton<IThemeInitializer, ThemeInitializer>();

            // Configure default test styles
            var method = GetType()
    .GetMethod("ConfigureTestStyles", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

            bool isOverridden = method?.DeclaringType != typeof(BlazociousTestBase);

            if (isOverridden)
            {
                ConfigureTestStyles(DefaultTestStyles);
            }

            var provider = base.Services.BuildServiceProvider();

            Element.ConfigureServices(provider);
        }

        // Helper method to override default styles for specific tests
        protected virtual void ConfigureTestStyles(string yamlContent)
        {
            base.Services.Configure<BlazociousStylesOptions>(options =>
            {
                options.YamlContent = yamlContent;
            });

            ElementBuilderStylingExtensions.InitializeStyles(yamlContent);
        }
    }
}