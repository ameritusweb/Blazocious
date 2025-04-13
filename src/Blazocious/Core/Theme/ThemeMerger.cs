using Blazocious.Core.YAML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public static class ThemeMerger
    {
        public static ParsedTheme Merge(ParsedTheme defaultTheme, ParsedTheme overrideTheme)
        {
            return new ParsedTheme
            {
                Tokens = MergeTokens(defaultTheme.Tokens, overrideTheme.Tokens),
                Components = MergeComponents(defaultTheme.Components, overrideTheme.Components),
                Styles = MergeStyles(defaultTheme.Styles, overrideTheme.Styles)
            };
        }

        private static Dictionary<string, TokenDefinition> MergeTokens(
            Dictionary<string, TokenDefinition> defaultTokens,
            Dictionary<string, TokenDefinition> overrideTokens)
        {
            var merged = new Dictionary<string, TokenDefinition>(defaultTokens);

            foreach (var (key, token) in overrideTokens)
            {
                // Override or add new tokens
                merged[key] = token;
            }

            return merged;
        }

        private static Dictionary<string, ComponentDefinition> MergeComponents(
            Dictionary<string, ComponentDefinition> defaultComponents,
            Dictionary<string, ComponentDefinition> overrideComponents)
        {
            var merged = new Dictionary<string, ComponentDefinition>();

            // Process all default components first
            foreach (var (name, defaultComponent) in defaultComponents)
            {
                if (overrideComponents.TryGetValue(name, out var overrideComponent))
                {
                    // Merge the component with its override
                    merged[name] = MergeComponent(defaultComponent, overrideComponent);
                }
                else
                {
                    // No override, use default as is
                    merged[name] = defaultComponent;
                }
            }

            // Add any new components from override that weren't in default
            foreach (var (name, overrideComponent) in overrideComponents)
            {
                if (!merged.ContainsKey(name))
                {
                    merged[name] = overrideComponent;
                }
            }

            return merged;
        }

        private static ComponentDefinition MergeComponent(
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

        private static ComponentBaseDefinition? MergeComponentBase(
            ComponentBaseDefinition? defaultBase,
            ComponentBaseDefinition? overrideBase)
        {
            if (defaultBase == null) return overrideBase;
            if (overrideBase == null) return defaultBase;

            return new ComponentBaseDefinition
            {
                // If override specifies a class, use it completely (don't merge classes)
                Class = overrideBase.Class ?? defaultBase.Class,

                // Merge styles with override taking precedence
                Styles = MergeStyles(defaultBase.Styles, overrideBase.Styles),

                // Merge media queries and states
                MediaQueries = MergeDictionaries(defaultBase.MediaQueries, overrideBase.MediaQueries),
                States = MergeDictionaries(defaultBase.States, overrideBase.States)
            };
        }

        private static Dictionary<string, ComponentBaseDefinition>? MergeComponentParts(
            Dictionary<string, ComponentBaseDefinition>? defaultParts,
            Dictionary<string, ComponentBaseDefinition>? overrideParts)
        {
            if (defaultParts == null) return overrideParts;
            if (overrideParts == null) return defaultParts;

            var merged = new Dictionary<string, ComponentBaseDefinition>();

            // Process all default parts
            foreach (var (name, defaultPart) in defaultParts)
            {
                if (overrideParts.TryGetValue(name, out var overridePart))
                {
                    // Merge the part with its override
                    merged[name] = MergeComponentBase(defaultPart, overridePart)
                        ?? new ComponentBaseDefinition();
                }
                else
                {
                    // No override, use default
                    merged[name] = defaultPart;
                }
            }

            // Add any new parts from override
            foreach (var (name, overridePart) in overrideParts)
            {
                if (!merged.ContainsKey(name))
                {
                    merged[name] = overridePart;
                }
            }

            return merged;
        }

        private static List<StylePropertyDefinition>? MergeStyles(
            List<StylePropertyDefinition>? defaultStyles,
            List<StylePropertyDefinition>? overrideStyles)
        {
            if (defaultStyles == null) return overrideStyles;
            if (overrideStyles == null) return defaultStyles;

            var styleDict = defaultStyles.ToDictionary(
                s => s.Property,
                s => s.Value
            );

            // Override or add new styles
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

        private static Dictionary<string, Dictionary<string, string>>? MergeDictionaries(
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
                    // Merge the nested dictionaries
                    var mergedValues = new Dictionary<string, string>(defaultValues);
                    foreach (var (prop, value) in overrideValues)
                    {
                        mergedValues[prop] = value;
                    }
                    merged[key] = mergedValues;
                }
                else
                {
                    // Add new entry
                    merged[key] = overrideValues;
                }
            }

            return merged;
        }
    }
}
