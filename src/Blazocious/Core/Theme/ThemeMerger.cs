using Blazocious.Core.YAML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public class ThemeMerger : IThemeMerger
    {
        private readonly IThemeRegistry _themeRegistry;

        public ThemeMerger(IThemeRegistry themeRegistry)
        {
            _themeRegistry = themeRegistry;
        }

        public ComponentStyles GetStyles(string component, string? theme = null)
        {
            var currentTheme = theme != null
                ? _themeRegistry.Get(theme)
                : _themeRegistry.Get("default");

            if (currentTheme?.Components?.TryGetValue(component, out var componentDef) != true)
            {
                return new ComponentStyles();
            }

            return new ComponentStyles
            {
                Class = componentDef.Base?.Class,
                Styles = componentDef.Base?.Styles?
                    .ToDictionary(s => s.Property, s => s.Value),
                Variants = componentDef.Variants?
                    .ToDictionary(
                        kv => kv.Key,
                        kv => new ComponentStyles
                        {
                            Class = kv.Value.Class,
                            Styles = kv.Value.Styles?
                                .ToDictionary(s => s.Property, s => s.Value)
                        }
                    )
            };
        }

        public ParsedTheme Merge(ParsedTheme defaultTheme, ParsedTheme overrideTheme)
        {
            return new ParsedTheme
            {
                Tokens = MergeTokens(defaultTheme.Tokens, overrideTheme.Tokens),
                Components = MergeComponents(defaultTheme.Components, overrideTheme.Components),
                Styles = MergeStyles(defaultTheme.Styles, overrideTheme.Styles)
            };
        }

        private Dictionary<string, TokenDefinition> MergeTokens(
            Dictionary<string, TokenDefinition> defaultTokens,
            Dictionary<string, TokenDefinition> overrideTokens)
        {
            var merged = new Dictionary<string, TokenDefinition>(defaultTokens);

            foreach (var (key, token) in overrideTokens)
            {
                merged[key] = token;
            }

            return merged;
        }

        private Dictionary<string, ComponentDefinition> MergeComponents(
            Dictionary<string, ComponentDefinition> defaultComponents,
            Dictionary<string, ComponentDefinition> overrideComponents)
        {
            var merged = new Dictionary<string, ComponentDefinition>();

            foreach (var (name, defaultComponent) in defaultComponents)
            {
                if (overrideComponents.TryGetValue(name, out var overrideComponent))
                {
                    merged[name] = MergeComponent(defaultComponent, overrideComponent);
                }
                else
                {
                    merged[name] = defaultComponent;
                }
            }

            foreach (var (name, overrideComponent) in overrideComponents)
            {
                if (!merged.ContainsKey(name))
                {
                    merged[name] = overrideComponent;
                }
            }

            return merged;
        }

        private ComponentDefinition MergeComponent(
            ComponentDefinition defaultComponent,
            ComponentDefinition overrideComponent)
        {
            return new ComponentDefinition
            {
                Name = overrideComponent.Name ?? defaultComponent.Name,
                Description = overrideComponent.Description ?? defaultComponent.Description,
                Base = MergeComponentBase(defaultComponent.Base, overrideComponent.Base),
                Parts = MergeComponentParts(defaultComponent.Parts, overrideComponent.Parts),
                Variants = MergeComponentParts(defaultComponent.Variants, overrideComponent.Variants)
            };
        }

        private ComponentBaseDefinition? MergeComponentBase(
            ComponentBaseDefinition? defaultBase,
            ComponentBaseDefinition? overrideBase)
        {
            if (defaultBase == null) return overrideBase;
            if (overrideBase == null) return defaultBase;

            return new ComponentBaseDefinition
            {
                Class = overrideBase.Class ?? defaultBase.Class,
                Styles = MergeStyles(defaultBase.Styles, overrideBase.Styles),
                MediaQueries = MergeDictionaries(defaultBase.MediaQueries, overrideBase.MediaQueries),
                States = MergeDictionaries(defaultBase.States, overrideBase.States)
            };
        }

        private Dictionary<string, ComponentBaseDefinition>? MergeComponentParts(
            Dictionary<string, ComponentBaseDefinition>? defaultParts,
            Dictionary<string, ComponentBaseDefinition>? overrideParts)
        {
            if (defaultParts == null) return overrideParts;
            if (overrideParts == null) return defaultParts;

            var merged = new Dictionary<string, ComponentBaseDefinition>();

            foreach (var (name, defaultPart) in defaultParts)
            {
                if (overrideParts.TryGetValue(name, out var overridePart))
                {
                    merged[name] = MergeComponentBase(defaultPart, overridePart)
                        ?? new ComponentBaseDefinition();
                }
                else
                {
                    merged[name] = defaultPart;
                }
            }

            foreach (var (name, overridePart) in overrideParts)
            {
                if (!merged.ContainsKey(name))
                {
                    merged[name] = overridePart;
                }
            }

            return merged;
        }

        private List<StylePropertyDefinition>? MergeStyles(
            List<StylePropertyDefinition>? defaultStyles,
            List<StylePropertyDefinition>? overrideStyles)
        {
            if (defaultStyles == null) return overrideStyles;
            if (overrideStyles == null) return defaultStyles;

            var styleDict = defaultStyles.ToDictionary(
                s => s.Property,
                s => s.Value
            );

            foreach (var style in overrideStyles)
            {
                styleDict[style.Property] = style.Value;
            }

            return styleDict.Select(kvp => new StylePropertyDefinition
            {
                Property = kvp.Key,
                Value = kvp.Value
            }).ToList();
        }

        private Dictionary<string, Dictionary<string, string>>? MergeDictionaries(
            Dictionary<string, Dictionary<string, string>>? defaultDict,
            Dictionary<string, Dictionary<string, string>>? overrideDict)
        {
            if (defaultDict == null) return overrideDict;
            if (overrideDict == null) return defaultDict;

            var merged = new Dictionary<string, Dictionary<string, string>>(defaultDict);

            foreach (var (key, overrideValues) in overrideDict)
            {
                if (merged.TryGetValue(key, out var defaultValues))
                {
                    var mergedValues = new Dictionary<string, string>(defaultValues);
                    foreach (var (prop, value) in overrideValues)
                    {
                        mergedValues[prop] = value;
                    }
                    merged[key] = mergedValues;
                }
                else
                {
                    merged[key] = overrideValues;
                }
            }

            return merged;
        }
    }
}
