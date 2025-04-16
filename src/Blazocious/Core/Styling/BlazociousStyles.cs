using Blazocious.Core.YAML.Models;
using Blazocious.Core.YAML;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Blazocious.Core.Styling
{
    public class BlazociousStyles
    {
        private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private readonly Dictionary<string, TokenDefinition> _tokens;
        private readonly Dictionary<string, ComponentDefinition> _components;
        private readonly Dictionary<string, ComponentBaseDefinition> _streetStyles;

        public BlazociousStyles(IOptions<BlazociousStylesOptions> options)
        {
            var parser = new YamlParser();
            string yamlContent;

            if (!string.IsNullOrEmpty(options.Value.YamlContent))
            {
                yamlContent = options.Value.YamlContent;
            }
            else if (!string.IsNullOrEmpty(options.Value.YamlPath))
            {
                yamlContent = File.ReadAllText(options.Value.YamlPath);
            }
            else
            {
                throw new InvalidOperationException("Either YamlContent or YamlPath must be provided in BlazociousStylesOptions");
            }

            (_tokens, _components, _streetStyles) = parser.Parse(yamlContent);
        }

        internal BlazociousStyles(string yamlContent)
        {
            var parser = new YamlParser();
            (_tokens, _components, _streetStyles) = parser.Parse(yamlContent);
        }

        public record StyleResult
        {
            public string? Class { get; init; }
            public string? Style { get; init; }
            public Dictionary<string, Dictionary<string, string>>? States { get; init; }
            public Dictionary<string, Dictionary<string, string>>? MediaQueries { get; init; }
        }

        public StyleResult GetStyles(string path, string? variant = null)
        {
            var cacheKey = $"styles_{path}_{variant}";

            return _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                // First check if it's a direct "street style"
                if (_streetStyles.TryGetValue(path, out var streetStyle))
                {
                    return new StyleResult
                    {
                        Class = streetStyle.Class,
                        Style = BuildStyleString(streetStyle.Styles),
                        States = streetStyle.States,
                        MediaQueries = streetStyle.MediaQueries
                    };
                }

                // Then check if it's a BEM component
                var parts = path.Split('.');

                if (parts[0] == "components")
                {
                    parts = parts.Skip(1).ToArray();
                }

                var componentName = parts[0];
                var elementName = parts.Length > 1 ? parts[1] : null;

                if (!_components.TryGetValue(componentName, out var component))
                {
                    return new StyleResult();
                }

                var classes = new List<string>();
                var styles = new List<StylePropertyDefinition>();
                var states = new Dictionary<string, Dictionary<string, string>>();
                var mediaQueries = new Dictionary<string, Dictionary<string, string>>();

                // Add base component styles
                if (component.Base != null)
                {
                    AddStyleDefinition(component.Base, classes, styles, states, mediaQueries);
                }

                // Add element styles for BEM
                if (elementName != null && component.Parts?.TryGetValue(elementName, out var elementDef) == true)
                {
                    AddStyleDefinition(elementDef, classes, styles, states, mediaQueries);
                }

                // Add variant styles
                if (variant != null && component.Variants?.TryGetValue(variant, out var variantDef) == true)
                {
                    AddStyleDefinition(variantDef, classes, styles, states, mediaQueries);
                }

                return new StyleResult
                {
                    Class = classes.Any() ? string.Join(" ", classes) : null,
                    Style = BuildStyleString(styles),
                    States = states.Any() ? states : null,
                    MediaQueries = mediaQueries.Any() ? mediaQueries : null
                };
            });
        }

        public List<(string Property, string Value)> GetStylesFormatted(string className)
        {
            var result = GetStyles(className);
            if (result?.Style == null) return new List<(string Property, string Value)>();

            // Parse the style string into property-value pairs
            return result.Style
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s =>
                {
                    var parts = s.Split(':', 2);
                    return (parts[0].Trim(), parts[1].Trim());
                })
                .ToList();
        }

        private void AddStyleDefinition(
            ComponentBaseDefinition def,
            List<string> classes,
            List<StylePropertyDefinition> styles,
            Dictionary<string, Dictionary<string, string>> states,
            Dictionary<string, Dictionary<string, string>> mediaQueries)
        {
            if (!string.IsNullOrEmpty(def.Class))
            {
                classes.Add(def.Class);
            }

            if (def.Styles?.Any() == true)
            {
                styles.AddRange(def.Styles);
            }

            if (def.States?.Any() == true)
            {
                foreach (var (state, props) in def.States)
                {
                    // Merge state properties if the state already exists
                    if (states.TryGetValue(state, out var existingProps))
                    {
                        foreach (var prop in props)
                        {
                            existingProps[prop.Key] = prop.Value;
                        }
                    }
                    else
                    {
                        states[state] = new Dictionary<string, string>(props);
                    }
                }
            }

            if (def.MediaQueries?.Any() == true)
            {
                foreach (var (query, props) in def.MediaQueries)
                {
                    // Merge media query properties if the query already exists
                    if (mediaQueries.TryGetValue(query, out var existingProps))
                    {
                        foreach (var prop in props)
                        {
                            existingProps[prop.Key] = prop.Value;
                        }
                    }
                    else
                    {
                        mediaQueries[query] = new Dictionary<string, string>(props);
                    }
                }
            }
        }

        private string? BuildStyleString(List<StylePropertyDefinition>? styles)
        {
            if (styles?.Any() != true) return null;

            return string.Join("; ", styles.Select(s =>
                $"{s.Property}: {ResolveTokens(s.Value)}"));
        }

        private string ResolveTokens(string value)
        {
            if (!value.Contains("var(--")) return value;

            var tokenName = value.Replace("var(--", "").Replace(")", "");
            return _tokens.TryGetValue(tokenName, out var token) ? token.Value : value;
        }
    }
}
