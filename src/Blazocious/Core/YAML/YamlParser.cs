using Blazocious.Core.YAML.Models;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Blazocious.Core.YAML
{
    public class YamlParser
    {
        private readonly IDeserializer _deserializer;
        private readonly ISerializer _serializer;

        public YamlParser()
        {
            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();

            _serializer = new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();
        }

        public (Dictionary<string, TokenDefinition> Tokens, Dictionary<string, ComponentDefinition> Components, Dictionary<string, ComponentBaseDefinition> Styles) Parse(string yamlContent)
        {
            var tokens = new Dictionary<string, TokenDefinition>();
            var components = new Dictionary<string, ComponentDefinition>();
            var streetStyles = new Dictionary<string, ComponentBaseDefinition>();

            using var reader = new StringReader(yamlContent);
            var yaml = new YamlStream();
            yaml.Load(reader);

            var root = (YamlMappingNode)yaml.Documents[0].RootNode;

            foreach (var entry in root.Children)
            {
                var key = ((YamlScalarNode)entry.Key).Value;

                switch (key.ToLowerInvariant())
                {
                    case "tokens":
                        tokens = ParseTokens((YamlMappingNode)entry.Value);
                        break;
                    case "components":
                        components = ParseComponents((YamlMappingNode)entry.Value);
                        break;
                    default:
                        // Treat as "street YAML" style
                        if (entry.Value is YamlMappingNode styleNode)
                        {
                            streetStyles[key] = ParseStreetStyle(key, styleNode);
                        }
                        break;
                }
            }

            return (tokens, components, streetStyles);
        }

        private Dictionary<string, TokenDefinition> ParseTokens(YamlMappingNode tokensNode)
        {
            var tokens = new Dictionary<string, TokenDefinition>();

            foreach (var token in tokensNode.Children)
            {
                var name = ((YamlScalarNode)token.Key).Value;
                var value = token.Value is YamlScalarNode valueNode
                    ? valueNode.Value
                    : _serializer.Serialize(token.Value);

                tokens[name] = new TokenDefinition
                {
                    Name = name,
                    Value = value
                };
            }

            return tokens;
        }

        private Dictionary<string, ComponentDefinition> ParseComponents(YamlMappingNode componentsNode)
        {
            var components = new Dictionary<string, ComponentDefinition>();

            foreach (var component in componentsNode.Children)
            {
                var name = ((YamlScalarNode)component.Key).Value;
                var compNode = (YamlMappingNode)component.Value;
                var definition = new ComponentDefinition { Name = name };

                foreach (var part in compNode.Children)
                {
                    var partKey = ((YamlScalarNode)part.Key).Value;
                    var partNode = (YamlMappingNode)part.Value;

                    switch (partKey.ToLowerInvariant())
                    {
                        case "base":
                            definition.Base = ParseComponentBase(partNode);
                            break;
                        case "parts":
                            definition.Parts = ParseComponentParts((YamlMappingNode)part.Value);
                            break;
                        case "variants":
                            definition.Variants = ParseComponentParts((YamlMappingNode)part.Value);
                            break;
                        case "description":
                            definition.Description = ((YamlScalarNode)part.Value).Value;
                            break;
                    }
                }

                components[name] = definition;
            }

            return components;
        }

        private ComponentBaseDefinition ParseComponentBase(YamlMappingNode node)
        {
            var baseDefinition = new ComponentBaseDefinition();

            foreach (var entry in node.Children)
            {
                var key = ((YamlScalarNode)entry.Key).Value;

                switch (key.ToLowerInvariant())
                {
                    case "class":
                        baseDefinition.Class = ((YamlScalarNode)entry.Value).Value;
                        break;
                    case "styles":
                        baseDefinition.Styles = ParseStyles((YamlSequenceNode)entry.Value);
                        break;
                    case "media":
                        baseDefinition.MediaQueries = ParseDictionary((YamlMappingNode)entry.Value);
                        break;
                    case "states":
                        baseDefinition.States = ParseDictionary((YamlMappingNode)entry.Value);
                        break;
                }
            }

            return baseDefinition;
        }

        private Dictionary<string, ComponentBaseDefinition> ParseComponentParts(YamlMappingNode node)
        {
            var parts = new Dictionary<string, ComponentBaseDefinition>();

            foreach (var part in node.Children)
            {
                var name = ((YamlScalarNode)part.Key).Value;
                parts[name] = ParseComponentBase((YamlMappingNode)part.Value);
            }

            return parts;
        }

        private ComponentBaseDefinition ParseStreetStyle(string className, YamlMappingNode node)
        {
            var style = new ComponentBaseDefinition { Class = className };

            foreach (var entry in node.Children)
            {
                var key = ((YamlScalarNode)entry.Key).Value;

                if (key.ToLowerInvariant() == "styles")
                {
                    style.Styles = ParseStyles((YamlSequenceNode)entry.Value);
                }
            }

            return style;
        }

        private List<StylePropertyDefinition> ParseStyles(YamlSequenceNode stylesNode)
        {
            var styles = new List<StylePropertyDefinition>();

            foreach (var styleNode in stylesNode)
            {
                if (styleNode is YamlMappingNode styleDef)
                {
                    foreach (var prop in styleDef.Children)
                    {
                        styles.Add(new StylePropertyDefinition
                        {
                            Property = ((YamlScalarNode)prop.Key).Value,
                            Value = ((YamlScalarNode)prop.Value).Value
                        });
                    }
                }
            }

            return styles;
        }

        private Dictionary<string, Dictionary<string, string>> ParseDictionary(YamlMappingNode node)
        {
            var dict = new Dictionary<string, Dictionary<string, string>>();

            foreach (var entry in node.Children)
            {
                var key = ((YamlScalarNode)entry.Key).Value;
                var subDict = new Dictionary<string, string>();

                if (entry.Value is YamlMappingNode subNode)
                {
                    foreach (var subEntry in subNode.Children)
                    {
                        var subKey = ((YamlScalarNode)subEntry.Key).Value;
                        var subValue = ((YamlScalarNode)subEntry.Value).Value;
                        subDict[subKey] = subValue;
                    }
                }

                dict[key] = subDict;
            }

            return dict;
        }
    }
}
