using Blazocious.Core.Theme;
using Blazocious.Core.YAML.Models;
using Blazocious.Test.Helpers;
using Bunit;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Blazocious.Test
{
    public class ThemeContextTests : TestContext
    {
        [Fact]
        public async Task SetVariantAsync_ShouldChangeCurrentTheme()
        {
            // Arrange
            var themeRegistry = new TestThemeRegistry();
            var defaultTheme = CreateTestTheme("default");
            var darkTheme = CreateTestTheme("dark");

            themeRegistry.Register("default", defaultTheme);
            themeRegistry.Register("dark", darkTheme);

            var themeContext = new ThemeContext();

            // Act
            await themeContext.SetVariantAsync("default", themeRegistry);
            var initialVariant = themeContext.CurrentVariant;
            var initialTheme = themeContext.CurrentTheme;

            await themeContext.SetVariantAsync("dark", themeRegistry);
            var newVariant = themeContext.CurrentVariant;
            var newTheme = themeContext.CurrentTheme;

            // Assert
            Assert.Equal("default", initialVariant);
            Assert.Same(defaultTheme, initialTheme);
            Assert.Equal("dark", newVariant);
            Assert.Same(darkTheme, newTheme);
        }

        [Fact]
        public async Task ThemeChanged_ShouldTriggerCallback()
        {
            // Arrange
            var themeRegistry = new TestThemeRegistry();
            var defaultTheme = CreateTestTheme("default");
            themeRegistry.Register("default", defaultTheme);

            var themeContext = new ThemeContext();
            bool callbackInvoked = false;
            themeContext.OnThemeChanged(() => callbackInvoked = true);

            // Act
            await themeContext.SetVariantAsync("default", themeRegistry);

            // Assert
            Assert.True(callbackInvoked);
        }

        [Fact]
        public async Task RemoveChangeListener_ShouldStopCallbacks()
        {
            // Arrange
            var themeRegistry = new TestThemeRegistry();
            var defaultTheme = CreateTestTheme("default");
            var darkTheme = CreateTestTheme("dark");

            themeRegistry.Register("default", defaultTheme);
            themeRegistry.Register("dark", darkTheme);

            var themeContext = new ThemeContext();
            int callCount = 0;
            Action callback = () => callCount++;

            themeContext.OnThemeChanged(callback);

            // Act
            await themeContext.SetVariantAsync("default", themeRegistry);
            themeContext.RemoveChangeListener(callback);
            await themeContext.SetVariantAsync("dark", themeRegistry);

            // Assert
            Assert.Equal(1, callCount);
        }

        [Fact]
        public async Task WithThemeProvider_ShouldPropagateContextToChildren()
        {
            // Arrange
            var themeRegistry = new TestThemeRegistry();
            var darkTheme = CreateTestTheme("dark");
            themeRegistry.Register("dark", darkTheme);

            // Register the theme services
            Services.AddSingleton<IThemeRegistry>(themeRegistry);
            Services.AddScoped<ThemeContext>();

            // Act - Render a ThemeProvider with a test component
            var cut = RenderComponent<TestThemeProviderComponent>(parameters => parameters
                .Add(p => p.Variant, "dark"));

            // Assert
            var themeContext = Services.GetRequiredService<ThemeContext>();
            Assert.Equal("dark", themeContext.CurrentVariant);

            // The test component should have received the theme context through DI
            var testComponent = cut.FindComponent<TestThemedComponent>();
            Assert.Equal("dark", testComponent.Instance.CurrentThemeVariant);
        }

        [Fact]
        public async Task ThemeProvider_ChangingVariant_ShouldUpdateTheme()
        {
            // Arrange
            var themeRegistry = new TestThemeRegistry();
            var lightTheme = CreateTestTheme("light");
            var darkTheme = CreateTestTheme("dark");

            themeRegistry.Register("light", lightTheme);
            themeRegistry.Register("dark", darkTheme);

            // Register the theme services
            Services.AddSingleton<IThemeRegistry>(themeRegistry);
            Services.AddScoped<ThemeContext>();

            // Act - Render with initial "light" theme
            var cut = RenderComponent<TestThemeProviderComponent>(parameters => parameters
                .Add(p => p.Variant, "light"));

            // Get the initial theme
            var initialThemeVariant = cut.FindComponent<TestThemedComponent>().Instance.CurrentThemeVariant;

            // Now re-render with "dark" theme
            cut.SetParametersAndRender(parameters => parameters
                .Add(p => p.Variant, "dark"));

            // Get the updated theme
            var updatedThemeVariant = cut.FindComponent<TestThemedComponent>().Instance.CurrentThemeVariant;

            // Assert
            Assert.Equal("light", initialThemeVariant);
            Assert.Equal("dark", updatedThemeVariant);
        }

        private ParsedTheme CreateTestTheme(string name)
        {
            return new ParsedTheme
            {
                Tokens = new Dictionary<string, TokenDefinition>
                {
                    [$"{name}-token"] = new TokenDefinition { Name = $"{name}-token", Value = $"{name}-value" }
                },
                Components = new Dictionary<string, ComponentDefinition>
                {
                    [$"{name}-component"] = new ComponentDefinition
                    {
                        Name = $"{name}-component",
                        Base = new ComponentBaseDefinition { Class = $"{name}-class" }
                    }
                }
            };
        }

        // Helper components for testing
        private class TestThemeProviderComponent : ComponentBase
        {
            [Inject] public IThemeRegistry ThemeRegistry { get; set; }
            [Parameter] public string Variant { get; set; } = "default";

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenComponent<ThemeProvider>(0);
                builder.AddAttribute(1, "Variant", Variant);
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder => {
                    childBuilder.OpenComponent<TestThemedComponent>(0);
                    childBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }
        }

        private class TestThemedComponent : ComponentBase
        {
            [Inject] public ThemeContext ThemeContext { get; set; }

            public string CurrentThemeVariant => ThemeContext.CurrentVariant;

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", $"themed-component theme-{ThemeContext.CurrentVariant}");
                builder.AddContent(2, $"Current theme: {ThemeContext.CurrentVariant}");
                builder.CloseElement();
            }
        }
    }
}
