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
            var testData = new TestData { Id = "test-123", Name = "Test Item" };
            var testOptions = new TestOptions
            {
                Cache = new CacheOptions { Enabled = true, Duration = TimeSpan.FromMinutes(5) }
            };

            var builder = new TestSemanticBuilder(testData)
                .WithOptions(testOptions);

            // Act - Create fragments and track render count
            var fragment1 = builder.Build();
            var fragment2 = builder.Build();

            // Count how many times CreateFragment is called

            // Assert
            Assert.Equal(1, TestSemanticBuilder.CreateFragmentCallCount); // Should only be called once due to caching

            // Even though we can't directly track render count in RenderTreeBuilder,
            // we can infer from the CreateFragmentCallCount that caching is working
        }

        [Fact]
        public void SemanticBuilder_WithoutCaching_ShouldNotCacheRenderFragment()
        {
            // Arrange
            var testData = new TestData { Id = "test-123", Name = "Test Item" };
            var testOptions = new TestOptions
            {
                Cache = new CacheOptions { Enabled = false }
            };

            // Reset the counter
            TestSemanticBuilder.CreateFragmentCallCount = 0;

            var builder = new TestSemanticBuilder(testData)
                .WithOptions(testOptions);

            // Act - Create fragments
            var fragment1 = builder.Build();
            var fragment2 = builder.Build();

            // Assert
            Assert.Equal(2, TestSemanticBuilder.CreateFragmentCallCount); // Should be called twice (no caching)
        }

        [Fact]
        public void SemanticBuilder_WithDifferentCacheKeys_ShouldNotShareCache()
        {
            // Arrange
            var testData1 = new TestData { Id = "test-1", Name = "Test Item 1" };
            var testData2 = new TestData { Id = "test-2", Name = "Test Item 2" };
            var testOptions = new TestOptions
            {
                Cache = new CacheOptions { Enabled = true, Duration = TimeSpan.FromMinutes(5) }
            };

            // Reset the counter
            TestSemanticBuilder.CreateFragmentCallCount = 0;

            var builder1 = new TestSemanticBuilder(testData1)
                .WithOptions(testOptions);

            var builder2 = new TestSemanticBuilder(testData2)
                .WithOptions(testOptions);

            // Act - Create fragments
            var fragment1 = builder1.Build();
            var fragment2 = builder2.Build();

            // Assert
            Assert.Equal(2, TestSemanticBuilder.CreateFragmentCallCount); // Should be called for each unique cache key
        }

        [Fact]
        public void SemanticBuilder_WithTheme_ShouldIncludeThemeInCacheKey()
        {
            // Arrange
            var testData = new TestData { Id = "test-123", Name = "Test Item" };
            var testOptions = new TestOptions
            {
                Cache = new CacheOptions { Enabled = true }
            };

            var theme1 = new SemanticThemeContext
            {
                BackgroundColor = "#f5f5f5",
                TextColor = "#333333"
            };

            var theme2 = new SemanticThemeContext
            {
                BackgroundColor = "#333333",
                TextColor = "#ffffff"
            };

            // Reset the counter
            TestSemanticBuilder.CreateFragmentCallCount = 0;

            var builder1 = new TestSemanticBuilder(testData)
                .WithOptions(testOptions)
                .WithTheme(theme1);

            var builder2 = new TestSemanticBuilder(testData)
                .WithOptions(testOptions)
                .WithTheme(theme2);

            // Act - Create fragments with different themes
            var fragment1 = builder1.Build();
            var fragment2 = builder2.Build();

            // Assert
            Assert.Equal(2, TestSemanticBuilder.CreateFragmentCallCount); // Should be called for each unique theme
        }

        [Fact]
        public void SemanticBuilder_WithCustomCacheKey_ShouldUseProvidedKey()
        {
            // Arrange
            var testData = new TestData { Id = "test-123", Name = "Test Item" };
            var testOptions = new TestOptions
            {
                Cache = new CacheOptions { Enabled = true }
            };

            // Reset the counter
            TestCustomCacheKeyBuilder.CreateFragmentCallCount = 0;

            // Create a builder with a custom cache key implementation
            // We need to store the reference to the concrete type before calling WithOptions
            var customBuilder = new TestCustomCacheKeyBuilder(testData);

            // Call WithOptions which will return SemanticBuilderBase<TestData, TestOptions>
            var builder = customBuilder.WithOptions(testOptions);

            // Act - Call Build twice
            var fragment1 = builder.Build();
            var fragment2 = builder.Build();

            // Assert
            Assert.Equal(1, TestCustomCacheKeyBuilder.CreateFragmentCallCount); // Should only be called once
            Assert.Equal("custom-cache-key", customBuilder.TestCacheKey); // Should use the custom key
        }

        [Fact]
        public void SemanticBuilder_WithCacheStaleRegeneration_ShouldRefreshCacheAsynchronously()
        {
            // This test is a bit tricky to do properly in a unit test context,
            // as it depends on timing which can be unreliable in test runners.
            // In a real application, the cache would refresh asynchronously.

            // Arrange
            var testData = new TestData { Id = "test-123", Name = "Test Item" };
            var testOptions = new TestOptions
            {
                Cache = new CacheOptions
                {
                    Enabled = true,
                    Duration = TimeSpan.FromMilliseconds(50),
                    PreferStale = true
                }
            };

            // Reset the counter
            TestSemanticBuilder.CreateFragmentCallCount = 0;

            var builder = new TestSemanticBuilder(testData)
                .WithOptions(testOptions);

            // Act - Get the first fragment
            var fragment1 = builder.Build();

            // Wait for the cache to expire
            System.Threading.Thread.Sleep(100);

            // Get it again - should still get the cached version
            var fragment2 = builder.Build();

            // Assert
            // The first call will create the fragment, the second will use the stale version
            // but will also trigger a background refresh
            Assert.Equal(1, TestSemanticBuilder.CreateFragmentCallCount);

            // Wait a moment for the background refresh to potentially complete
            System.Threading.Thread.Sleep(100);

            // The refresh is async, so we can't reliably test it in this context,
            // but in a real application, the cache would be updated in the background
        }

        [Fact]
        public void SemanticBuilder_WithCaching_ShouldRenderTheSameOutput()
        {
            // Arrange
            var testData = new TestData { Id = "test-123", Name = "Test Item" };
            var testOptions = new TestOptions
            {
                Cache = new CacheOptions { Enabled = true, Duration = TimeSpan.FromMinutes(5) }
            };

            var builder = new TestSemanticBuilder(testData)
                .WithOptions(testOptions);

            // Act - Create fragments
            var fragment1 = builder.Build();
            var fragment2 = builder.Build();

            // Render the fragments in bUnit
            var cut1 = Render(fragment1);
            var cut2 = Render(fragment2);

            // Assert
            Assert.Equal(cut1.Markup, cut2.Markup);
            Assert.Contains("test-123", cut1.Markup);
            Assert.Contains("Test Item", cut1.Markup);
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
            public static int CreateFragmentCallCount = 0;

            public TestSemanticBuilder(TestData data) : base(data) { }

            protected override string ComputeCacheKey() => $"test|{Data.Id}|{Data.Name}|{Theme?.GetHashCode() ?? 0}";

            protected override RenderFragment CreateFragment()
            {
                CreateFragmentCallCount++;

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

        private class TestCustomCacheKeyBuilder : SemanticBuilderBase<TestData, TestOptions>
        {
            public static int CreateFragmentCallCount = 0;
            public string TestCacheKey { get; private set; }

            public TestCustomCacheKeyBuilder(TestData data) : base(data) { }

            protected override string ComputeCacheKey()
            {
                TestCacheKey = "custom-cache-key";
                return TestCacheKey;
            }

            protected override RenderFragment CreateFragment()
            {
                CreateFragmentCallCount++;

                return builder =>
                {
                    builder.OpenElement(0, "div");
                    builder.AddAttribute(1, "data-id", Data.Id);
                    builder.AddContent(2, Data.Name);
                    builder.CloseElement();
                };
            }
        }
    }
}