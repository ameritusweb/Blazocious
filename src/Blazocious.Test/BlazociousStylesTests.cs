using Blazocious.Core.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Test
{
    public class BlazociousStylesTests
    {
        [Fact]
        public void Constructor_WithValidYaml_ShouldParseStyles()
        {
            // Arrange
            var yamlContent = @"
tokens:
  color-primary: '#1a2b3c'
  spacing-base: '1rem'
components:
  button:
    base:
      class: 'btn'
      styles:
        - border-radius: '0.25rem'
    variants:
      primary:
        class: 'btn-primary'
";

            // Act
            var styles = new BlazociousStyles(yamlContent);

            // Assert - No exception means constructor worked
            Assert.NotNull(styles);
        }

        [Fact]
        public void GetStyles_WithValidComponent_ShouldReturnCorrectStyles()
        {
            // Arrange
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
        styles:
          - background-color: '#007bff'
";
            var styles = new BlazociousStyles(yamlContent);

            // Act
            var baseResult = styles.GetStyles("button");
            var variantResult = styles.GetStyles("button", "primary");

            // Assert
            Assert.NotNull(baseResult);
            Assert.Equal("btn", baseResult.Class);
            Assert.Contains("border-radius: 0.25rem", baseResult.Style);

            Assert.NotNull(variantResult);
            Assert.Equal("btn btn-primary", variantResult.Class);
            Assert.Contains("background-color: #007bff", variantResult.Style);
        }

        [Fact]
        public void GetStyles_WithBEMComponent_ShouldReturnComponentPartStyles()
        {
            // Arrange
            var yamlContent = @"
components:
  card:
    base:
      class: card
      styles:
        - background-color: var(--color-primary)
        - border-radius: var(--radius-md)
        - padding: 2rem

    header:
      class: card__header
      styles:
        - font-weight: bold
        - padding: 1rem

    body:
      class: card__body
      styles:
        - font-weight: bold
        - padding: 1rem

    variants:
      compact:
        class: card--compact
        styles:
          - padding: 1rem
";
            var styles = new BlazociousStyles(yamlContent);

            // Act
            var headerResult = styles.GetStyles("components.card.header");
            var bodyResult = styles.GetStyles("components.card.body");

            // Assert
            Assert.NotNull(headerResult);
            Assert.Equal("card__header", headerResult.Class);

            Assert.NotNull(bodyResult);
            Assert.Equal("card__body", bodyResult.Class);
        }

        [Fact]
        public void GetStyles_WithStreetStyle_ShouldReturnDirectStyles()
        {
            // Arrange
            var yamlContent = @"
flex-container:
  class: 'flex-container'
  styles:
    - display: 'flex'
    - flex-direction: 'column'
";
            var styles = new BlazociousStyles(yamlContent);

            // Act
            var result = styles.GetStyles("flex-container");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("flex-container", result.Class);
            Assert.Contains("display: flex", result.Style);
            Assert.Contains("flex-direction: column", result.Style);
        }

        [Fact]
        public void GetStyles_WithUnknownComponent_ShouldReturnEmptyResult()
        {
            // Arrange
            var yamlContent = @"
components:
  button:
    base:
      class: 'btn'
";
            var styles = new BlazociousStyles(yamlContent);

            // Act
            var result = styles.GetStyles("unknown-component");

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Class);
            Assert.Null(result.Style);
        }

        [Fact]
        public void GetStyles_WithTokenReferences_ShouldResolveTokens()
        {
            // Arrange
            var yamlContent = @"
tokens:
  color-primary: '#007bff'
  spacing-md: '1rem'
components:
  button:
    base:
      class: 'btn'
      styles:
        - color: 'var(--color-primary)'
        - padding: 'var(--spacing-md)'
";
            var styles = new BlazociousStyles(yamlContent);

            // Act
            var result = styles.GetStyles("button");

            // Assert
            Assert.NotNull(result);
            Assert.Contains("color: #007bff", result.Style);
            Assert.Contains("padding: 1rem", result.Style);
        }

        [Fact]
        public void GetStyles_WithUnknownTokenReferences_ShouldPreserveOriginalValue()
        {
            // Arrange
            var yamlContent = @"
components:
  button:
    base:
      class: 'btn'
      styles:
        - color: 'var(--unknown-token)'
";
            var styles = new BlazociousStyles(yamlContent);

            // Act
            var result = styles.GetStyles("button");

            // Assert
            Assert.NotNull(result);
            Assert.Contains("color: var(--unknown-token)", result.Style);
        }

        [Fact]
        public void GetStyles_WithComponentStates_ShouldIncludeStates()
        {
            // Arrange
            var yamlContent = @"
components:
  button:
    base:
      class: 'btn'
      states:
        hover:
          background-color: 'lightblue'
        active:
          transform: 'scale(0.98)'
";
            var styles = new BlazociousStyles(yamlContent);

            // Act
            var result = styles.GetStyles("button");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.States);
            Assert.Equal(2, result.States.Count);
            Assert.Contains("hover", result.States.Keys);
            Assert.Contains("active", result.States.Keys);
            Assert.Equal("lightblue", result.States["hover"]["background-color"]);
            Assert.Equal("scale(0.98)", result.States["active"]["transform"]);
        }

        [Fact]
        public void GetStyles_WithMediaQueries_ShouldIncludeMediaQueries()
        {
            // Arrange
            var yamlContent = @"
components:
  responsive:
    base:
      class: 'responsive'
      media:
        '(min-width: 768px)':
          font-size: '1rem'
        '(max-width: 480px)':
          font-size: '0.875rem'
";
            var styles = new BlazociousStyles(yamlContent);

            // Act
            var result = styles.GetStyles("responsive");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.MediaQueries);
            Assert.Equal(2, result.MediaQueries.Count);
            Assert.Contains("(min-width: 768px)", result.MediaQueries.Keys);
            Assert.Contains("(max-width: 480px)", result.MediaQueries.Keys);
            Assert.Equal("1rem", result.MediaQueries["(min-width: 768px)"]["font-size"]);
            Assert.Equal("0.875rem", result.MediaQueries["(max-width: 480px)"]["font-size"]);
        }

        [Fact]
        public void GetStyles_WithCaching_ShouldReturnSameInstanceForSamePath()
        {
            // Arrange
            var yamlContent = @"
components:
  button:
    base:
      class: 'btn'
";
            var styles = new BlazociousStyles(yamlContent);

            // Act
            var result1 = styles.GetStyles("button");
            var result2 = styles.GetStyles("button");

            // Assert
            Assert.Same(result1, result2);
        }
    }
}
