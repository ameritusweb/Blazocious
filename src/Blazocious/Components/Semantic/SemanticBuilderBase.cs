using Blazocious.Core.Theme;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private string? _cacheKey;
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

            var key = GetOrCreateCacheKey();

            if (Options.Cache?.PreferStale == true && _cache.TryGetValue(key, out RenderFragment? cached))
            {
                _ = Task.Run(() => RefreshCache(key));
                return cached;
            }

            return _cache.GetOrCreate(key, entry =>
            {
                entry.SetAbsoluteExpiration(Options.Cache?.Duration ?? TimeSpan.FromMinutes(5));
                return CreateFragmentWithCustomization();
            });
        }

        protected virtual bool ShouldCache() => Options.Cache?.Enabled == true;

        private RenderFragment CreateFragmentWithCustomization() => builder =>
        {
            CreateFragment()(builder);
            _customizer?.Invoke(builder);
        };

        private string GetOrCreateCacheKey()
        {
            if (_cacheKey != null) return _cacheKey;

            _cacheKey = ComputeCacheKey();
            return _cacheKey;
        }

        /// <summary>
        /// Override to control cache key generation specific to your semantic component
        /// </summary>
        protected abstract string ComputeCacheKey();

        /// <summary>
        /// Override to define the rendering logic for your semantic component
        /// </summary>
        protected abstract RenderFragment CreateFragment();

        private void RefreshCache(string key)
        {
            var fragment = CreateFragmentWithCustomization();
            _cache.Set(key, fragment, Options.Cache?.Duration ?? TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Helper to build class strings considering theme context
        /// </summary>
        protected string BuildClassString(params string?[] classes)
        {
            var list = classes.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

            if (!string.IsNullOrWhiteSpace(Options.CustomClass))
                list.Add(Options.CustomClass);

            if (Theme?.CustomClasses != null)
                list.AddRange(Theme.CustomClasses);

            return string.Join(" ", list);
        }

        /// <summary>
        /// Helper to build style strings considering theme context
        /// </summary>
        protected string BuildStyleString(params string?[] styles)
        {
            var list = styles.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            if (Theme?.Styles != null)
                list.AddRange(Theme.Styles);

            return string.Join("; ", list);
        }
    }
}
