using Blazocious.Core.Builder;
using Bunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Test
{
    public class ResponsiveThemeBuilderTests : TestContext
    {
        [Fact]
        public void ForTheme_ShouldApplyThemeVariant()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .ResponsiveTheme(rt => rt
                        .ForTheme("dark", b => b.Class("dark-theme")))
                    .Build()(builder);
            });

            // Assert
            var div = cut.Find("div");

            // In a real app with ThemeContext, we'd see "theme.dark" applied.
            // However, in tests without ThemeContext, we can still verify that 
            // the class was applied
            Assert.Contains("dark-theme", div.ClassList);
        }

        [Fact]
        public void ForBreakpoint_ShouldApplyBreakpointStyles()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .ResponsiveTheme(rt => rt
                        .ForBreakpoint(Breakpoint.MD, b => b.Class("md-theme")))
                    .Build()(builder);
            });

            // Assert
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];

            // Check that the correct media query is generated
            Assert.Contains("@media (min-width: 768px)", styleElement.TextContent);
            Assert.Contains("md-theme", styleElement.TextContent);

            // Check that the div has a unique class to attach the styles to
            var div = cut.Find("div");
            var divClass = div.ClassList[0];
            Assert.StartsWith("blz-", divClass);
        }

        [Fact]
        public void WhenDark_ShouldApplyDarkModeStyles()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .ResponsiveTheme(rt => rt
                        .WhenDark(b => b.Class("dark-mode")))
                    .Build()(builder);
            });

            // Assert
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];

            // Check that the dark mode media query is applied
            Assert.Contains("@media (prefers-color-scheme: dark)", styleElement.TextContent);
            Assert.Contains("dark-mode", styleElement.TextContent);
        }

        [Fact]
        public void ResponsiveTheme_CombiningMultipleConfigurations_ShouldApplyAll()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .ResponsiveTheme(rt => rt
                        .ForTheme("dark", b => b.Class("dark-theme"))
                        .ForBreakpoint(Breakpoint.MD, b => b.Class("md-theme"))
                        .WhenDark(b => b.Class("dark-mode")))
                    .Build()(builder);
            });

            // Assert
            var div = cut.Find("div");
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];

            // Check that theme variant was applied
            Assert.Contains("dark-theme", div.ClassList);

            // Check that responsive styles were applied via media queries
            Assert.Contains("@media (min-width: 768px)", styleElement.TextContent);
            Assert.Contains("md-theme", styleElement.TextContent);

            // Check that dark mode media query was applied
            Assert.Contains("@media (prefers-color-scheme: dark)", styleElement.TextContent);
            Assert.Contains("dark-mode", styleElement.TextContent);
        }

        [Fact]
        public void ResponsiveTheme_WithComplexConfiguration_ShouldApplyCorrectly()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .ResponsiveTheme(rt => rt
                        .ForTheme("dark", b => b
                            .Class("dark-theme")
                            .Style("background-color", "#121212")
                            .Style("color", "#ffffff"))
                        .ForBreakpoint(Breakpoint.LG, b => b
                            .Class("large-screen")
                            .Style("max-width", "1200px"))
                        .WhenDark(b => b
                            .Class("dark-mode-preference")
                            .Style("color-scheme", "dark")))
                    .Build()(builder);
            });

            // Assert
            var div = cut.Find("div");
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];

            // Check that all classes and styles are applied correctly
            Assert.Contains("dark-theme", div.ClassList);
            var style = div.GetAttribute("style");
            Assert.Contains("background-color: #121212", style);
            Assert.Contains("color: #ffffff", style);

            // Check media queries
            var styleContent = styleElement.TextContent;
            Assert.Contains("@media (min-width: 1024px)", styleContent);
            Assert.Contains("large-screen", styleContent);
            Assert.Contains("max-width: 1200px", styleContent);

            Assert.Contains("@media (prefers-color-scheme: dark)", styleContent);
            Assert.Contains("dark-mode-preference", styleContent);
            Assert.Contains("color-scheme: dark", styleContent);
        }

        [Fact]
        public void ResponsiveTheme_ChainingSameMethod_ShouldOverrideConfigurations()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .ResponsiveTheme(rt => rt
                        .ForTheme("light", b => b.Class("light-theme"))
                        .ForTheme("dark", b => b.Class("dark-theme"))) // This should override the light theme
                    .Build()(builder);
            });

            // Assert
            var div = cut.Find("div");

            // The div should have the dark-theme class, not light-theme
            Assert.Contains("dark-theme", div.ClassList);
            Assert.DoesNotContain("light-theme", div.ClassList);
        }
    }
}
