using Blazocious.Core.Builder;
using Blazocious.Core.Extensions;
using Blazocious.Core.Styling;
using Blazocious.Core.Trackers;
using Blazocious.Test.Extensions;
using Blazocious.Test.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Blazocious.Test
{
    public class TestComponent
    {
        public RenderFragment RenderTest() => builder =>
        {
            Element.Div("test-component")
                .Child(Element.Button("Click me")
                    .Class("btn btn-primary"))
                .Child(Element.Div("card")
                    .Responsive(x =>
                        x.At(Breakpoint.MD, e => e.Class("card-md"))))
                .Build()(builder);
        };
    }

    public class BlazociousCssGeneratorTests : BlazociousTestBase
    {
        [Fact]
        public async Task GenerateFromAssembly_ShouldGenerateExpectedCss()
        {
            // Arrange
            var testComponent = new TestComponent();
            var renderFragment = testComponent.RenderTest();
            var classUsageTracker = Element.GetRequiredService<IClassUsageTracker>();

            // Start collecting
            classUsageTracker.StartCollecting();

            // Act
            renderFragment(new RenderTreeBuilder());

            // Stop collecting
            classUsageTracker.StopCollecting();

            // Get tracked classes
            var usedClasses = classUsageTracker.GetUsedClasses();
            var mediaQueries = classUsageTracker.GetMediaQueries();

            // Assert
            Assert.Contains("test-component", usedClasses);
            Assert.Contains("btn", usedClasses);
            Assert.Contains("btn-primary", usedClasses);
            Assert.Contains("card", usedClasses);
            
            Assert.True(mediaQueries.Keys.PartialContainsKey("(min-width: 768px)"));
            Assert.Contains("card-md", mediaQueries.GetValueByPartialKey("(min-width: 768px)")!);
        }

        [Fact]
        public async Task GenerateFromAssembly_ShouldOrderMediaQueriesCorrectly()
        {
            // Arrange
            var testComponent = new TestComponent();
            var renderFragment = testComponent.RenderTest();
            var classUsageTracker = Element.GetRequiredService<IClassUsageTracker>();

            classUsageTracker.StartCollecting();

            // Add multiple media queries
            Element.Div("responsive")
                .Responsive(x => x.At(Breakpoint.SM, y => y.Class("sm")))
                .Responsive(x => x.At(Breakpoint.XL, y => y.Class("xl")))
                .Responsive(x => x.At(Breakpoint.MD, y => y.Class("md")))
                .Build()(new RenderTreeBuilder());

            classUsageTracker.StopCollecting();

            var mediaQueries = classUsageTracker.GetMediaQueries().Keys;

            // Assert - verify media queries are ordered correctly
            Assert.True(mediaQueries.PartialIndexOf("(min-width: 640px)") < mediaQueries.PartialIndexOf("(min-width: 768px)"));
            Assert.True(mediaQueries.PartialIndexOf("(min-width: 768px)") < mediaQueries.PartialIndexOf("(min-width: 1280px)"));
        }

        [Fact]
        public async Task GenerateFromAssembly_ShouldGenerateValidCssSyntax()
        {
            // Arrange
            var testComponent = new TestComponent();
            var renderFragment = testComponent.RenderTest();
            var classUsageTracker = Element.GetRequiredService<IClassUsageTracker>();

            // Act
            classUsageTracker.StartCollecting();
            renderFragment(new RenderTreeBuilder());
            classUsageTracker.StopCollecting();

            await CssGenerator.GenerateFromAssembly();

            // Get the generated CSS from the output file
            var options = Element.Options;
            var outputPath = options.CssOutputPath;

            FileInfo outputInfo = new FileInfo(outputPath);

            var css = await File.ReadAllTextAsync(outputInfo.FullName);

            // Assert
            Assert.Contains("{", css);
            Assert.Contains("}", css);
            Assert.DoesNotContain("{{", css);  // No double braces
            Assert.DoesNotContain("}}", css);  // No double braces
            Assert.True(AreBracesBalanced(css));
        }

        private string GenerateCssForTest(IClassUsageTracker tracker, BlazociousStyles styles)
        {
            var method = typeof(CssGenerator)
                .GetMethod("GenerateCss", BindingFlags.NonPublic | BindingFlags.Static);

            return (string)method.Invoke(null, new object[] 
            { 
                tracker.GetUsedClasses(),
                tracker.GetMediaQueries()
            });
        }

        private bool AreBracesBalanced(string css)
        {
            int count = 0;
            foreach (char c in css)
            {
                if (c == '{') count++;
                if (c == '}') count--;
                if (count < 0) return false;
            }
            return count == 0;
        }
    }
}