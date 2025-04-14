using Blazocious.Core.Theme;
using Blazocious.Core.YAML.Models;
using Bunit;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Blazocious.Test
{
    public class ThemeRegistryTests : TestContext
    {
        [Fact]
        public void Register_ShouldAddThemeToRegistry()
        {
            // Arrange
            var registry = new ThemeRegistry();
            var theme = CreateTestTheme("test");

            // Act
            registry.Register("test-theme", theme);

            // Assert
            var registeredTheme = registry.Get("test-theme");
            Assert.Same(theme, registeredTheme);
        }

        [Fact]
        public void Register_WithSameNameTwice_ShouldOverwriteFirstTheme()
        {
            // Arrange
            var registry = new ThemeRegistry();
            var theme1 = CreateTestTheme("theme1");
            var theme2 = CreateTestTheme("theme2");

            // Act
            registry.Register("test-theme", theme1);
            registry.Register("test-theme", theme2);

            // Assert
            var registeredTheme = registry.Get("test-theme");
            Assert.Same(theme2, registeredTheme);
            Assert.NotSame(theme1, registeredTheme);
        }

        [Fact]
        public void Get_WithNonExistentTheme_ShouldReturnNull()
        {
            // Arrange
            var registry = new ThemeRegistry();

            // Act
            var result = registry.Get("non-existent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetRegisteredThemes_ShouldReturnAllRegisteredThemeNames()
        {
            // Arrange
            var registry = new ThemeRegistry();
            registry.Register("theme1", CreateTestTheme("theme1"));
            registry.Register("theme2", CreateTestTheme("theme2"));
            registry.Register("theme3", CreateTestTheme("theme3"));

            // Act
            var themeNames = registry.GetRegisteredThemes().ToList();

            // Assert
            Assert.Equal(3, themeNames.Count);
            Assert.Contains("theme1", themeNames);
            Assert.Contains("theme2", themeNames);
            Assert.Contains("theme3", themeNames);
        }

        [Fact]
        public void ThemeRegistryInServiceProvider_ShouldBeSingleton()
        {
            // Arrange
            var theme1 = CreateTestTheme("theme1");
            var theme2 = CreateTestTheme("theme2");

            // Register the theme registry as a singleton
            Services.AddSingleton<IThemeRegistry, ThemeRegistry>();

            // Act - Get the registry from the service provider twice
            var registry1 = Services.GetRequiredService<IThemeRegistry>();
            registry1.Register("theme1", theme1);

            var registry2 = Services.GetRequiredService<IThemeRegistry>();
            registry2.Register("theme2", theme2);

            // Assert - Both should be the same instance, so theme1 should be accessible from registry2
            Assert.Same(registry1, registry2);
            Assert.Same(theme1, registry2.Get("theme1"));
            Assert.Same(theme2, registry1.Get("theme2"));
        }

        [Fact]
        public void ThemeRegistry_WithComponentRegistration_ShouldProvideThemesToComponents()
        {
            // Arrange
            var registry = new ThemeRegistry();
            var theme = CreateTestTheme("test-theme");
            registry.Register("default", theme);

            // Register services
            Services.AddSingleton<IThemeRegistry>(registry);
            Services.AddScoped<ThemeContext>();

            // Act - Render a component that uses the theme context
            var cut = RenderComponent<TestThemedComponent>();

            // Assert
            var renderedMarkup = cut.Markup;
            Assert.Contains("default", renderedMarkup);

            // The component should have the theme context injected
            Assert.NotNull(cut.Instance.ThemeContext);
            Assert.Equal("default", cut.Instance.ThemeContext.CurrentVariant);
        }

        private ParsedTheme CreateTestTheme(string identifierPrefix)
        {
            return new ParsedTheme
            {
                Tokens = new Dictionary<string, TokenDefinition>
                {
                    [$"{identifierPrefix}-token"] = new TokenDefinition
                    {
                        Name = $"{identifierPrefix}-token",
                        Value = $"{identifierPrefix}-value"
                    }
                },
                Components = new Dictionary<string, ComponentDefinition>
                {
                    [$"{identifierPrefix}-component"] = new ComponentDefinition
                    {
                        Name = $"{identifierPrefix}-component",
                        Base = new ComponentBaseDefinition
                        {
                            Class = $"{identifierPrefix}-class"
                        }
                    }
                }
            };
        }

        // Helper component for testing
        private class TestThemedComponent : ComponentBase, IDisposable
        {
            [Inject] public ThemeContext ThemeContext { get; set; }
            [Inject] public IThemeRegistry ThemeRegistry { get; set; }

            private bool _initialized;
            private Action _themeChangedHandler;

            protected override async Task OnInitializedAsync()
            {
                _themeChangedHandler = () => InvokeAsync(StateHasChanged);
                ThemeContext.OnThemeChanged(_themeChangedHandler);

                if (!_initialized)
                {
                    await ThemeContext.SetVariantAsync("default", ThemeRegistry);
                    _initialized = true;
                }
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", $"themed-component theme-{ThemeContext.CurrentVariant}");
                builder.AddContent(2, $"Current theme: {ThemeContext.CurrentVariant}");
                builder.CloseElement();
            }

            public void Dispose()
            {
                if (_themeChangedHandler != null)
                {
                    ThemeContext.RemoveChangeListener(_themeChangedHandler);
                }
            }
        }
    }
}
