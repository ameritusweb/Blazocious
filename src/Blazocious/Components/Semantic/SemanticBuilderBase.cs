using Blazocious.Components.Semantic;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;

namespace Blazocious.Components.Semantic
{
    public abstract class SemanticBuilderBase<TData, TOptions>
    where TData : class, ISemanticData
    where TOptions : class, ISemanticOptions, new()
    {
        private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        protected TData Data { get; }
        protected TOptions Options { get; private set; } = new();
        protected SemanticThemeContext? Theme { get; private set; }

        private Action<RenderTreeBuilder>? _customizer;

        protected SemanticBuilderBase(TData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public SemanticBuilderBase<TData, TOptions> WithOptions(TOptions options)
        {
            Options = options ?? new TOptions();
            return this;
        }

        public SemanticBuilderBase<TData, TOptions> WithTheme(SemanticThemeContext theme)
        {
            Theme = theme;
            return this;
        }

        public SemanticBuilderBase<TData, TOptions> WithCustomizer(Action<RenderTreeBuilder> customizer)
        {
            _customizer = customizer;
            return this;
        }

        public RenderFragment Build()
        {
            if (!ShouldCache())
                return CreateFragmentWithCustomization();

            var key = ComputeCacheKey();

            // If PreferStale is true and we have a cached version, use it and refresh in background
            if (Options.Cache?.PreferStale == true && _cache.TryGetValue(key, out RenderFragment? cached))
            {
                _ = Task.Run(() => RefreshCache(key));
                return cached;
            }

            // Get or create the cached fragment
            return _cache.GetOrCreate(key, entry =>
            {
                entry.SetAbsoluteExpiration(Options.Cache?.Duration ?? TimeSpan.FromMinutes(5));
                return CreateFragmentWithCustomization();
            });
        }

        protected virtual bool ShouldCache() => Options.Cache?.Enabled == true;

        private RenderFragment CreateFragmentWithCustomization()
        {
            // Create a single instance of the fragment builder
            var fragment = CreateFragment();

            return builder =>
            {
                fragment(builder);
                _customizer?.Invoke(builder);
            };
        }

        private void RefreshCache(string key)
        {
            var fragment = CreateFragmentWithCustomization();
            _cache.Set(key, fragment, Options.Cache?.Duration ?? TimeSpan.FromMinutes(5));
        }

        protected abstract string ComputeCacheKey();
        protected abstract RenderFragment CreateFragment();

        protected string BuildClassString(params string?[] classes)
        {
            var list = classes.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

            if (!string.IsNullOrWhiteSpace(Options.CustomClass))
                list.Add(Options.CustomClass);

            if (Theme?.CustomClasses != null)
                list.AddRange(Theme.CustomClasses);

            return string.Join(" ", list);
        }

        protected string BuildStyleString(params string?[] styles)
        {
            var list = styles.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            if (Theme?.Styles != null)
                list.AddRange(Theme.Styles);

            return string.Join("; ", list);
        }
    }
}