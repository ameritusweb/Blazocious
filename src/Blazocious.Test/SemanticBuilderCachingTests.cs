using Blazocious.Components.Semantic;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace Blazocious.Test
{
    public class SemanticBuilderCachingTests : TestContext
    {
        [Fact]
        public void SemanticBuilder_WithCaching_ShouldCacheRenderFragment()
        {
            // Arrange
            TestSemanticBuilder.ResetCounter();
            var testData = new TestData { Id = "test-123", Name = "Test Item" };
            var testOptions = new TestOptions
            {
                Cache = new CacheOptions
                {
                    Enabled = true,
                    Duration = TimeSpan.FromMinutes(5)
                }
            };

            var builder = new TestSemanticBuilder(testData)
                .WithOptions(testOptions);

            // Act - Call Build() multiple times
            var fragment1 = builder.Build();
            var fragment2 = builder.Build();
            var fragment3 = builder.Build();

            // Render each fragment
            var render1 = Render(fragment1);
            var render2 = Render(fragment2);
            var render3 = Render(fragment3);

            // Assert
            Assert.Equal(1, TestSemanticBuilder.CreateFragmentCallCount); // Should only create once

            // Verify all renders produced the same output
            var element1 = render1.Find("div");
            var element2 = render2.Find("div");
            var element3 = render3.Find("div");

            Assert.Equal("test-123", element1.GetAttribute("data-id"));
            Assert.Equal("test-123", element2.GetAttribute("data-id"));
            Assert.Equal("test-123", element3.GetAttribute("data-id"));
            Assert.Equal("Test Item", element1.TextContent);
            Assert.Equal("Test Item", element2.TextContent);
            Assert.Equal("Test Item", element3.TextContent);
        }

        [Fact]
        public void SemanticBuilder_WithoutCaching_ShouldNotCacheRenderFragment()
        {
            // Arrange
            TestSemanticBuilder.ResetCounter();
            var testData = new TestData { Id = "test-123", Name = "Test Item" };
            var testOptions = new TestOptions
            {
                Cache = new CacheOptions
                {
                    Enabled = false
                }
            };

            var builder = new TestSemanticBuilder(testData)
                .WithOptions(testOptions);

            // Act - Call Build() multiple times
            var fragment1 = builder.Build();
            var fragment2 = builder.Build();

            // Render the fragments
            Render(fragment1);
            Render(fragment2);

            // Assert
            Assert.Equal(2, TestSemanticBuilder.CreateFragmentCallCount); // Should create new fragments
        }

        [Fact]
        public void SemanticBuilder_WithDifferentCacheKeys_ShouldNotShareCache()
        {
            // Arrange
            TestSemanticBuilder.ResetCounter();
            var testData1 = new TestData { Id = "test-1", Name = "Test Item 1" };
            var testData2 = new TestData { Id = "test-2", Name = "Test Item 2" };
            var testOptions = new TestOptions
            {
                Cache = new CacheOptions
                {
                    Enabled = true,
                    Duration = TimeSpan.FromMinutes(5)
                }
            };

            var builder1 = new TestSemanticBuilder(testData1)
                .WithOptions(testOptions);
            var builder2 = new TestSemanticBuilder(testData2)
                .WithOptions(testOptions);

            // Act - Create and render fragments
            var fragment1 = builder1.Build();
            RenderFragment(fragment1);

            var fragment2 = builder2.Build();
            RenderFragment(fragment2);

            // Assert
            Assert.Equal(2, TestSemanticBuilder.CreateFragmentCallCount);
        }


        [Fact]
        public async Task SemanticBuilder_WithPreferStale_ShouldUseStaleCache()
        {
            // Arrange
            TestSemanticBuilder.ResetCounter();
            var testData = new TestData { Id = "test-123", Name = "Test Item" };
            var testOptions = new TestOptions
            {
                Cache = new CacheOptions
                {
                    Enabled = true,
                    Duration = TimeSpan.FromMilliseconds(50), // Short duration
                    PreferStale = true
                }
            };

            var builder = new TestSemanticBuilder(testData)
                .WithOptions(testOptions);

            // Act - Get initial fragment
            var fragment1 = builder.Build();
            var render1 = Render(fragment1);

            // Wait for cache to expire
            await Task.Delay(100);

            // Get another fragment - should use stale cache
            var fragment2 = builder.Build();
            var render2 = Render(fragment2);

            // Wait a bit for background refresh
            await Task.Delay(100);

            // Assert
            Assert.Equal(2, TestSemanticBuilder.CreateFragmentCallCount); // Initial + Background refresh

            // Verify both renders produced correct output
            var element1 = render1.Find("div");
            var element2 = render2.Find("div");
            Assert.Equal("test-123", element1.GetAttribute("data-id"));
            Assert.Equal("test-123", element2.GetAttribute("data-id"));
            Assert.Equal("Test Item", element1.TextContent);
            Assert.Equal("Test Item", element2.TextContent);
        }

        private void RenderFragment(RenderFragment fragment)
        {
            var component = Render(fragment);
            Assert.NotNull(component);  // Ensure render succeeded
        }

        // Test implementations
        private class TestData : ISemanticData
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        private class TestOptions : ISemanticOptions
        {
            public CacheOptions Cache { get; set; }
            public string CustomClass { get; set; }
        }

        private class TestSemanticBuilder : SemanticBuilderBase<TestData, TestOptions>
        {
            private static int _createFragmentCallCount;
            public static int CreateFragmentCallCount => _createFragmentCallCount;

            public static void ResetCounter()
            {
                _createFragmentCallCount = 0;
            }

            public TestSemanticBuilder(TestData data) : base(data) { }

            protected override string ComputeCacheKey() => $"test|{Data.Id}|{Data.Name}|{Theme?.GetHashCode() ?? 0}";

            protected override RenderFragment CreateFragment()
            {
                System.Threading.Interlocked.Increment(ref _createFragmentCallCount);

                return builder =>
                {
                    builder.OpenElement(0, "div");
                    builder.AddAttribute(1, "data-id", Data.Id);
                    builder.AddAttribute(2, "class", "test-component");

                    if (Theme != null)
                    {
                        builder.AddAttribute(3, "style", $"background-color: {Theme.BackgroundColor}; color: {Theme.TextColor}");
                    }

                    builder.AddContent(4, Data.Name);
                    builder.CloseElement();
                };
            }
        }
    }
}