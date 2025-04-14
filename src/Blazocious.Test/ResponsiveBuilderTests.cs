using Blazocious.Core.Builder;
using Blazocious.Test.Helpers;
using Bunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Test
{
    public class ResponsiveBuilderTests : BlazociousTestBase
    {
        [Fact]
        public void ResponsiveBuilder_At_ShouldApplyMinWidthMediaQuery()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .Responsive(r => r
                        .At(Breakpoint.MD, b => b
                            .Class("md-class")
                            .Style("color", "blue")))
                    .Build()(builder);
            });

            // Assert
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];
            Assert.Contains("@media (min-width: 768px)", styleElement.TextContent);
            Assert.Contains("color: blue", styleElement.TextContent);

            // Check that the div has a unique class for the media query to target
            var div = cut.Find("div");
            var divClasses = div.ClassList;
            Assert.Single(divClasses); // Should have exactly one class (the generated ID)
            Assert.StartsWith("blz-", divClasses[0]); // Should start with "blz-"
        }

        [Fact]
        public void ResponsiveBuilder_Below_ShouldApplyMaxWidthMediaQuery()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .Responsive(r => r
                        .Below(Breakpoint.SM, b => b
                            .Class("sm-class")
                            .Style("font-size", "14px")))
                    .Build()(builder);
            });

            // Assert
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];
            Assert.Contains("@media (max-width: 640px)", styleElement.TextContent);
            Assert.Contains("font-size: 14px", styleElement.TextContent);
        }

        [Fact]
        public void ResponsiveBuilder_Between_ShouldApplyRangeMediaQuery()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .Responsive(r => r
                        .Between(Breakpoint.MD, Breakpoint.LG, b => b
                            .Class("md-lg-class")
                            .Style("padding", "1rem")))
                    .Build()(builder);
            });

            // Assert
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];
            Assert.Contains("@media (min-width: 768px) and (max-width: 1024px)", styleElement.TextContent);
            Assert.Contains("padding: 1rem", styleElement.TextContent);
        }

        [Fact]
        public void ResponsiveBuilder_Mobile_ShouldApplyMobileStyles()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .Responsive(r => r
                        .Mobile(b => b
                            .Class("mobile-class")
                            .Style("font-size", "12px")))
                    .Build()(builder);
            });

            // Assert
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];
            Assert.Contains("@media (max-width: 768px)", styleElement.TextContent);
            Assert.Contains("font-size: 12px", styleElement.TextContent);
        }

        [Fact]
        public void ResponsiveBuilder_Tablet_ShouldApplyTabletStyles()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .Responsive(r => r
                        .Tablet(b => b
                            .Class("tablet-class")
                            .Style("padding", "0.75rem")))
                    .Build()(builder);
            });

            // Assert
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];
            Assert.Contains("@media (min-width: 768px) and (max-width: 1024px)", styleElement.TextContent);
            Assert.Contains("padding: 0.75rem", styleElement.TextContent);
        }

        [Fact]
        public void ResponsiveBuilder_Desktop_ShouldApplyDesktopStyles()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .Responsive(r => r
                        .Desktop(b => b
                            .Class("desktop-class")
                            .Style("margin", "2rem")))
                    .Build()(builder);
            });

            // Assert
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];
            Assert.Contains("@media (min-width: 1024px)", styleElement.TextContent);
            Assert.Contains("margin: 2rem", styleElement.TextContent);
        }

        [Fact]
        public void ResponsiveBuilder_Dark_ShouldApplyDarkModeStyles()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .Responsive(r => r
                        .Dark(b => b
                            .Class("dark-mode")
                            .Style("background-color", "#121212")
                            .Style("color", "#ffffff")))
                    .Build()(builder);
            });

            // Assert
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];
            Assert.Contains("@media (prefers-color-scheme: dark)", styleElement.TextContent);
            Assert.Contains("background-color: #121212", styleElement.TextContent);
            Assert.Contains("color: #ffffff", styleElement.TextContent);
        }

        [Fact]
        public void ResponsiveBuilder_ReducedMotion_ShouldApplyReducedMotionStyles()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .Responsive(r => r
                        .ReducedMotion(b => b
                            .Style("transition", "none")))
                    .Build()(builder);
            });

            // Assert
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];
            Assert.Contains("@media (prefers-reduced-motion: reduce)", styleElement.TextContent);
            Assert.Contains("transition: none", styleElement.TextContent);
        }

        [Fact]
        public void ResponsiveBuilder_MultipleMediaQueries_ShouldApplyAllCorrectly()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .Responsive(r => r
                        .Mobile(b => b.Style("font-size", "14px"))
                        .Tablet(b => b.Style("font-size", "16px"))
                        .Desktop(b => b.Style("font-size", "18px"))
                        .Dark(b => b.Style("color", "#ffffff")))
                    .Build()(builder);
            });

            // Assert
            var styleElements = cut.FindAll("style");
            Assert.NotEmpty(styleElements);
            var styleElement = styleElements[0];
            var styleContent = styleElement.TextContent;

            Assert.Contains("@media (max-width: 768px)", styleContent);
            Assert.Contains("font-size: 14px", styleContent);

            Assert.Contains("@media (min-width: 768px) and (max-width: 1024px)", styleContent);
            Assert.Contains("font-size: 16px", styleContent);

            Assert.Contains("@media (min-width: 1024px)", styleContent);
            Assert.Contains("font-size: 18px", styleContent);

            Assert.Contains("@media (prefers-color-scheme: dark)", styleContent);
            Assert.Contains("color: #ffffff", styleContent);
        }
    }
}
