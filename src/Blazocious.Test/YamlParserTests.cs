using Blazocious.Core.YAML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Test
{
    public class YamlParserTests
    {
        [Fact]
        public void Parse_WithTokens_ShouldParseTokensCorrectly()
        {
            // Arrange
            var yamlParser = new YamlParser();
            var yamlContent = @"
tokens:
  color-primary: '#1a2b3c'
  color-secondary: '#3c4d5e'
  spacing-base: '1rem'
";

            // Act
            var (tokens, _, _) = yamlParser.Parse(yamlContent);

            // Assert
            Assert.Equal(3, tokens.Count);
            Assert.Contains("color-primary", tokens.Keys);
            Assert.Contains("color-secondary", tokens.Keys);
            Assert.Contains("spacing-base", tokens.Keys);

            Assert.Equal("#1a2b3c", tokens["color-primary"].Value);
            Assert.Equal("#3c4d5e", tokens["color-secondary"].Value);
            Assert.Equal("1rem", tokens["spacing-base"].Value);
        }

        [Fact]
        public void Parse_WithComponents_ShouldParseComponentsCorrectly()
        {
            // Arrange
            var yamlParser = new YamlParser();
            var yamlContent = @"
components:
  button:
    base:
      class: 'btn'
      styles:
        - border-radius: '0.25rem'
    variants:
      primary:
        class: 'btn-primary'
  card:
    base:
      class: 'card'
    parts:
      header:
        class: 'card-header'
      body:
        class: 'card-body'
";

            // Act
            var (_, components, _) = yamlParser.Parse(yamlContent);

            // Assert
            Assert.Equal(2, components.Count);
            Assert.Contains("button", components.Keys);
            Assert.Contains("card", components.Keys);

            // Check button component
            var button = components["button"];
            Assert.Equal("btn", button.Base.Class);
            Assert.Single(button.Base.Styles);
            Assert.Equal("border-radius", button.Base.Styles[0].Property);
            Assert.Equal("0.25rem", button.Base.Styles[0].Value);
            Assert.Single(button.Variants);
            Assert.Contains("primary", button.Variants.Keys);
            Assert.Equal("btn-primary", button.Variants["primary"].Class);

            // Check card component
            var card = components["card"];
            Assert.Equal("card", card.Base.Class);
            Assert.Equal(2, card.Parts.Count);
            Assert.Contains("header", card.Parts.Keys);
            Assert.Contains("body", card.Parts.Keys);
            Assert.Equal("card-header", card.Parts["header"].Class);
            Assert.Equal("card-body", card.Parts["body"].Class);
        }

        [Fact]
        public void Parse_WithStreetStyles_ShouldParseStylesCorrectly()
        {
            // Arrange
            var yamlParser = new YamlParser();
            var yamlContent = @"
flex-container:
  class: 'flex-container'
  styles:
    - display: 'flex'
    - flex-direction: 'column'
button-group:
  class: 'button-group'
  styles:
    - display: 'flex'
    - gap: '0.5rem'
";

            // Act
            var (_, _, streetStyles) = yamlParser.Parse(yamlContent);

            // Assert
            Assert.Equal(2, streetStyles.Count);
            Assert.Contains("flex-container", streetStyles.Keys);
            Assert.Contains("button-group", streetStyles.Keys);

            // Check flex-container style
            var flexContainer = streetStyles["flex-container"];
            Assert.Equal("flex-container", flexContainer.Class);
            Assert.Equal(2, flexContainer.Styles.Count);
            Assert.Equal("display", flexContainer.Styles[0].Property);
            Assert.Equal("flex", flexContainer.Styles[0].Value);
            Assert.Equal("flex-direction", flexContainer.Styles[1].Property);
            Assert.Equal("column", flexContainer.Styles[1].Value);

            // Check button-group style
            var buttonGroup = streetStyles["button-group"];
            Assert.Equal("button-group", buttonGroup.Class);
            Assert.Equal(2, buttonGroup.Styles.Count);
            Assert.Equal("display", buttonGroup.Styles[0].Property);
            Assert.Equal("flex", buttonGroup.Styles[0].Value);
            Assert.Equal("gap", buttonGroup.Styles[1].Property);
            Assert.Equal("0.5rem", buttonGroup.Styles[1].Value);
        }

        [Fact]
        public void Parse_WithMixedContent_ShouldParseAllSections()
        {
            // Arrange
            var yamlParser = new YamlParser();
            var yamlContent = @"
tokens:
  color-primary: '#1a2b3c'
components:
  button:
    base:
      class: 'btn'
flex-container:
  class: 'flex-container'
  styles:
    - display: 'flex'
";

            // Act
            var (tokens, components, streetStyles) = yamlParser.Parse(yamlContent);

            // Assert
            Assert.Single(tokens);
            Assert.Single(components);
            Assert.Single(streetStyles);

            Assert.Contains("color-primary", tokens.Keys);
            Assert.Contains("button", components.Keys);
            Assert.Contains("flex-container", streetStyles.Keys);
        }

        [Fact]
        public void Parse_WithNestedObjects_ShouldParseCorrectly()
        {
            // Arrange
            var yamlParser = new YamlParser();
            var yamlContent = @"
components:
  button:
    base:
      class: 'btn'
      states:
        hover:
          background-color: 'lightblue'
          transform: 'scale(1.05)'
        active:
          transform: 'scale(0.98)'
      media:
        '(min-width: 768px)':
          font-size: '1rem'
          padding: '0.75rem 1.5rem'
";

            // Act
            var (_, components, _) = yamlParser.Parse(yamlContent);

            // Assert
            Assert.Single(components);
            Assert.Contains("button", components.Keys);

            var button = components["button"];

            // Check states
            Assert.NotNull(button.Base.States);
            Assert.Equal(2, button.Base.States.Count);
            Assert.Contains("hover", button.Base.States.Keys);
            Assert.Contains("active", button.Base.States.Keys);

            var hoverState = button.Base.States["hover"];
            Assert.Equal(2, hoverState.Count);
            Assert.Equal("lightblue", hoverState["background-color"]);
            Assert.Equal("scale(1.05)", hoverState["transform"]);

            var activeState = button.Base.States["active"];
            Assert.Single(activeState);
            Assert.Equal("scale(0.98)", activeState["transform"]);

            // Check media queries
            Assert.NotNull(button.Base.MediaQueries);
            Assert.Single(button.Base.MediaQueries);
            Assert.Contains("(min-width: 768px)", button.Base.MediaQueries.Keys);

            var mediaQuery = button.Base.MediaQueries["(min-width: 768px)"];
            Assert.Equal(2, mediaQuery.Count);
            Assert.Equal("1rem", mediaQuery["font-size"]);
            Assert.Equal("0.75rem 1.5rem", mediaQuery["padding"]);
        }

        [Fact]
        public void Parse_WithEmptyYaml_ShouldReturnEmptyCollections()
        {
            // Arrange
            var yamlParser = new YamlParser();
            var yamlContent = @"
# This is an empty YAML file
";

            // Act
            var (tokens, components, streetStyles) = yamlParser.Parse(yamlContent);

            // Assert
            Assert.Empty(tokens);
            Assert.Empty(components);
            Assert.Empty(streetStyles);
        }

        [Fact]
        public void Parse_WithInvalidYaml_ShouldThrowException()
        {
            // Arrange
            var yamlParser = new YamlParser();
            var yamlContent = @"
tokens:
  color-primary: '#1a2b3c'
  indentation:is:incorrect
";

            // Act & Assert
            var exception = Assert.Throws<YamlDotNet.Core.YamlException>(() => yamlParser.Parse(yamlContent));
            Assert.Contains("incorrect", exception.Message);
        }
    }
}
