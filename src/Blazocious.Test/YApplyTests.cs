using Blazocious.Core.Builder;
using Blazocious.Test.Helpers;
using Bunit;

namespace Blazocious.Test
{
    public class YApplyTests : BlazociousTestBase
    {
        protected override void ConfigureTestStyles(string yamlContent)
        {
            base.ConfigureTestStyles(@"
components:
  button:
    base:
      class: 'btn-base'
      styles:                      # These styles are for CSS generation
        - padding: '1rem 2rem'     # not for runtime use
    variants:
      primary:
        class: 'btn-primary'
        styles:
          - background-color: '#007bff'
          - color: '#ffffff'
      danger:
        class: 'btn-danger'
        styles:
          - background-color: '#dc3545'
    states:
      disabled:
        class: 'btn-disabled'
        styles:
          - opacity: '0.5'
          - cursor: 'not-allowed'
  
  card:
    base:
      class: 'card-base'
      styles:
        - background-color: '#ffffff'
        - border-radius: '0.5rem'
    header:
      class: 'card-header'
      styles:
        - padding: '1rem'
    body:
      class: 'card-body'
      styles:
        - padding: '1rem'
    variants:
      outlined:
        class: 'card-outlined'
        styles:
          - border: '1px solid #e5e7eb'
");
        }

        [Fact]
        public void YApply_WithBaseComponent_ShouldApplyOnlyClass()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Button()
                    .YApply("components.button.base")
                    .Build()(builder);
            });

            // Assert
            var button = cut.Find("button");
            Assert.Contains("btn-base", button.ClassList);
            Assert.Null(button.GetAttribute("style")); // No inline styles should be applied
        }

        [Fact]
        public void YApply_WithVariant_ShouldApplyBaseAndVariantClasses()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Button()
                    .YApply("components.button.base")
                    .YApply("components.button.variants.primary")
                    .Build()(builder);
            });

            // Assert
            var button = cut.Find("button");
            Assert.Contains("btn-base", button.ClassList);
            Assert.Contains("btn-primary", button.ClassList);
            Assert.Null(button.GetAttribute("style")); // No inline styles
        }

        [Fact]
        public void YApply_WithState_ShouldApplyBaseAndStateClasses()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Button()
                    .YApply("components.button.base")
                    .YApply("components.button.states.disabled")
                    .Build()(builder);
            });

            // Assert
            var button = cut.Find("button");
            Assert.Contains("btn-base", button.ClassList);
            Assert.Contains("btn-disabled", button.ClassList);
            Assert.Null(button.GetAttribute("style")); // No inline styles
        }

        [Fact]
        public void YApply_WithNestedComponent_ShouldApplyComponentPartClasses()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .YApply("components.card.base")
                    .Child(
                        Element.Div()
                            .YApply("components.card.header")
                    )
                    .Build()(builder);
            });

            // Assert
            var card = cut.Find("div");
            var header = card.Children[0];

            Assert.Contains("card-base", card.ClassList);
            Assert.Contains("card-header", header.ClassList);
            Assert.Null(card.GetAttribute("style")); // No inline styles
            Assert.Null(header.GetAttribute("style")); // No inline styles
        }

        [Fact]
        public void YApply_WithComplexNestedStructure_ShouldApplyAllClasses()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .YApply("components.card.base")
                    .YApply("components.card.variants.outlined")
                    .Child(
                        Element.Div()
                            .YApply("components.card.header")
                            .Text("Header")
                    )
                    .Child(
                        Element.Div()
                            .YApply("components.card.body")
                            .Text("Content")
                    )
                    .Build()(builder);
            });

            // Assert
            var card = cut.Find("div");
            var header = card.Children[0];
            var body = card.Children[1];

            Assert.Contains("card-base", card.ClassList);
            Assert.Contains("card-outlined", card.ClassList);
            Assert.Contains("card-header", header.ClassList);
            Assert.Contains("card-body", body.ClassList);

            // No inline styles anywhere
            Assert.Null(card.GetAttribute("style"));
            Assert.Null(header.GetAttribute("style"));
            Assert.Null(body.GetAttribute("style"));
        }

        [Fact]
        public void YApply_WithInvalidPath_ShouldNotThrowAndReturnOriginalBuilder()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Button()
                    .YApply("components.nonexistent.base")
                    .Class("should-remain")
                    .Build()(builder);
            });

            // Assert
            var button = cut.Find("button");
            Assert.Contains("should-remain", button.ClassList);
            Assert.Null(button.GetAttribute("style")); // No inline styles
        }
    }
}
