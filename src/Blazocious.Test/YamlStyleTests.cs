using Blazocious.Core.Builder;
using Blazocious.Test.Extensions;
using Bunit;

namespace Blazocious.Test
{
    public class YamlStyleTests : TestContext
    {
        [Fact]
        public void YApply_ShouldApplyStylesFromTheme()
        {
            // Arrange
            const string testYaml = @"
components:
  button:
    base:
      class: 'btn'
      styles:
        - border-radius: '0.25rem'
        - padding: '0.5rem 1rem'
    variants:
      primary:
        class: 'btn-primary'
        styles:
          - background-color: '#007bff'
          - color: 'white'
";
            ElementBuilderStylingExtensions.InitializeStyles(testYaml);

            // Act
            var cut = Render(builder =>
            {
                Element.Button().WithServiceProvider(Services)
                    .YApply("button")
                    .YApply("button", "primary")
                    .Text("Click me")
                    .Build()(builder);
            });

            // Assert
            var button = cut.Find("button");
            Assert.Contains("btn", button.ClassList);
            Assert.Contains("btn-primary", button.ClassList);
            Assert.Equal("Click me", button.TextContent);
            Assert.Contains("background-color: #007bff", button.GetAttribute("style"));
            Assert.Contains("color: white", button.GetAttribute("style"));
            Assert.Contains("border-radius: 0.25rem", button.GetAttribute("style"));
            Assert.Contains("padding: 0.5rem 1rem", button.GetAttribute("style"));
        }

        [Fact]
        public void YApply_WithBemComponentAndPart_ShouldApplyCorrectStyles()
        {
            // Arrange
            const string testYaml = @"
components:
  card:
    base:
      class: 'card'
      styles:
        - border-radius: '0.25rem'
    parts:
      header:
        class: 'card-header'
        styles:
          - font-weight: 'bold'
      body:
        class: 'card-body'
        styles:
          - padding: '1rem'
";
            ElementBuilderStylingExtensions.InitializeStyles(testYaml);

            // Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .YApply("card")
                    .Child(Element.Div()
                        .YApply("card.header")
                        .Text("Card Header"))
                    .Child(Element.Div()
                        .YApply("card.body")
                        .Text("Card Content"))
                    .Build()(builder);
            });

            // Assert
            var card = cut.Find("div");
            var header = card.FindFirst("div");
            var body = card.Children[1];

            Assert.Contains("card", card.ClassList);
            Assert.Contains("border-radius: 0.25rem", card.GetAttribute("style"));

            Assert.Contains("card-header", header.ClassList);
            Assert.Contains("font-weight: bold", header.GetAttribute("style"));
            Assert.Equal("Card Header", header.TextContent);

            Assert.Contains("card-body", body.ClassList);
            Assert.Contains("padding: 1rem", body.GetAttribute("style"));
            Assert.Equal("Card Content", body.TextContent);
        }

        [Fact]
        public void YApply_WithStreetStyle_ShouldApplyDirectStyles()
        {
            // Arrange
            const string testYaml = @"
flex-container:
  class: 'flex-container'
  styles:
    - display: 'flex'
    - flex-direction: 'column'
    - gap: '1rem'
";
            ElementBuilderStylingExtensions.InitializeStyles(testYaml);

            // Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .YApply("flex-container")
                    .Child(Element.Div().Text("Item 1"))
                    .Child(Element.Div().Text("Item 2"))
                    .Child(Element.Div().Text("Item 3"))
                    .Build()(builder);
            });

            // Assert
            var container = cut.Find("div");
            Assert.Contains("flex-container", container.ClassList);
            var style = container.GetAttribute("style");
            Assert.Contains("display: flex", style);
            Assert.Contains("flex-direction: column", style);
            Assert.Contains("gap: 1rem", style);
        }

        [Fact]
        public void YApply_WithTokens_ShouldResolveTokenReferences()
        {
            // Arrange
            const string testYaml = @"
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
            ElementBuilderStylingExtensions.InitializeStyles(testYaml);

            // Act
            var cut = Render(builder =>
            {
                Element.Button().WithServiceProvider(Services)
                    .YApply("button")
                    .Text("Token Button")
                    .Build()(builder);
            });

            // Assert
            var button = cut.Find("button");
            Assert.Contains("btn", button.ClassList);
            Assert.Contains("color: #007bff", button.GetAttribute("style"));
            Assert.Contains("padding: 1rem", button.GetAttribute("style"));
        }

        [Fact]
        public void YApply_WithUnknownComponent_ShouldNotAddClasses()
        {
            // Arrange
            const string testYaml = @"
components:
  button:
    base:
      class: 'btn'
";
            ElementBuilderStylingExtensions.InitializeStyles(testYaml);

            // Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .YApply("non-existent-component")
                    .Text("No styles")
                    .Build()(builder);
            });

            // Assert
            var div = cut.Find("div");
            Assert.Empty(div.ClassList);
            Assert.Null(div.GetAttribute("style"));
            Assert.Equal("No styles", div.TextContent);
        }

        [Fact]
        public void BlazociousStyles_WithComplexTheme_ShouldRenderCorrectly()
        {
            // Arrange
            const string testYaml = @"
tokens:
  color-primary: '#007bff'
  color-secondary: '#6c757d'
  border-radius: '0.25rem'
  spacing-sm: '0.5rem'
  spacing-md: '1rem'
  spacing-lg: '1.5rem'

components:
  card:
    base:
      class: 'card'
      styles:
        - border-radius: 'var(--border-radius)'
        - box-shadow: '0 2px 4px rgba(0,0,0,0.1)'
      media:
        '(max-width: 768px)':
          margin: '0.5rem'
    parts:
      header:
        class: 'card-header'
        styles:
          - padding: 'var(--spacing-md)'
          - font-weight: 'bold'
          - border-bottom: '1px solid #eee'
      body:
        class: 'card-body'
        styles:
          - padding: 'var(--spacing-md)'
      footer:
        class: 'card-footer'
        styles:
          - padding: 'var(--spacing-sm)'
          - border-top: '1px solid #eee'
    variants:
      primary:
        class: 'card-primary'
        styles:
          - border-color: 'var(--color-primary)'
      secondary:
        class: 'card-secondary'
        styles:
          - border-color: 'var(--color-secondary)'
";
            ElementBuilderStylingExtensions.InitializeStyles(testYaml);

            // Act
            var cut = Render(builder =>
            {
                Element.Div().WithServiceProvider(Services)
                    .YApply("card")
                    .YApply("card", "primary")
                    .Child(Element.Div()
                        .YApply("card.header")
                        .Text("Card Title"))
                    .Child(Element.Div()
                        .YApply("card.body")
                        .Text("Card Content"))
                    .Child(Element.Div()
                        .YApply("card.footer")
                        .Text("Card Footer"))
                    .Build()(builder);
            });

            // Assert
            var card = cut.Find("div");
            Assert.Contains("card", card.ClassList);
            Assert.Contains("card-primary", card.ClassList);

            var header = card.FindAll("div")[0];
            var body = card.FindAll("div")[1];
            var footer = card.FindAll("div")[2];

            Assert.Contains("card-header", header.ClassList);
            Assert.Contains("card-body", body.ClassList);
            Assert.Contains("card-footer", footer.ClassList);

            // Check that token values were substituted
            Assert.Contains("border-radius: 0.25rem", card.GetAttribute("style"));
            Assert.Contains("border-color: #007bff", card.GetAttribute("style"));
            Assert.Contains("padding: 1rem", header.GetAttribute("style"));
            Assert.Contains("padding: 0.5rem", footer.GetAttribute("style"));
        }
    }
}
