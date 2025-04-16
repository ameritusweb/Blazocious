using Blazocious.Core.Builder;
using Blazocious.Test.Extensions;
using Blazocious.Test.Helpers;
using Bunit;

namespace Blazocious.Test
{
    public class ElementBuilderTests : BlazociousTestBase
    {
        [Fact]
        public void ElementBuilder_CreateDiv_ShouldRenderCorrectly()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div().Build()(builder);
            });

            // Assert
            var div = cut.Find("div");
            Assert.NotNull(div);
        }

        [Fact]
        public void ElementBuilder_WithAttributes_ShouldRenderCorrectly()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .Class("test-class")
                    .Attr("id", "test-id")
                    .Style("color", "red")
                    .Build()(builder);
            });

            // Assert
            var div = cut.Find("div");
            Assert.Contains("test-class", div.ClassList);
            Assert.Equal("test-id", div.Id);
            Assert.Contains("color: red", div.GetAttribute("style"));
        }

        [Fact]
        public void ElementBuilder_WithChildren_ShouldRenderCorrectly()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .Child(
                        Element.H1().Text("Title")
                    )
                    .Child(
                        Element.Paragraph().Text("Content")
                    )
                    .Build()(builder);
            });

            // Assert
            var div = cut.Find("div");
            var h1 = div.Children[0];
            var p = div.Children[1];

            Assert.Equal("H1", h1.TagName);
            Assert.Equal("Title", h1.TextContent);
            Assert.Equal("P", p.TagName);
            Assert.Equal("Content", p.TextContent);
        }

        [Fact]
        public void ElementBuilder_WithConditionals_ShouldRenderCorrectly()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .If(true, b => b.Class("active"))
                    .If(false, b => b.Class("inactive"))
                    .When(true, b => b.Text("Visible Content"))
                    .When(false, b => b.Text("Invisible Content"))
                    .Build()(builder);
            });

            // Assert
            var div = cut.Find("div");
            Assert.Contains("active", div.ClassList);
            Assert.DoesNotContain("inactive", div.ClassList);
            Assert.Equal("Visible Content", div.TextContent);
        }

        [Fact]
        public void ElementBuilder_ClassIf_ShouldApplyClassConditionally()
        {
            // Arrange & Act
            var cutTrue = Render(builder =>
            {
                Element.Div().ClassIf("active", true).Build()(builder);
            });

            var cutFalse = Render(builder =>
            {
                Element.Div().ClassIf("active", false).Build()(builder);
            });

            // Assert
            var divTrue = cutTrue.Find("div");
            var divFalse = cutFalse.Find("div");

            Assert.Contains("active", divTrue.ClassList);
            Assert.DoesNotContain("active", divFalse.ClassList);
        }

        [Fact]
        public void ElementBuilder_ForEach_ShouldRenderIteratedContent()
        {
            // Arrange
            var items = new[] { "Item 1", "Item 2", "Item 3" };

            // Act
            var cut = Render(builder =>
            {
                Element.Ul()
                    .ForEach(items, item =>
                        Element.Li().Text(item)
                    )
                    .Build()(builder);
            });

            // Assert
            var ul = cut.Find("ul");
            var listItems = ul.FindAll("li");

            Assert.Equal(3, listItems.Count);
            Assert.Equal("Item 1", listItems[0].TextContent);
            Assert.Equal("Item 2", listItems[1].TextContent);
            Assert.Equal("Item 3", listItems[2].TextContent);
        }

        [Fact]
        public void Element_StaticMethods_ShouldCreateCorrectBuilders()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .Child(Element.H1("Heading"))
                    .Child(Element.Paragraph("Paragraph"))
                    .Child(Element.Button("Click"))
                    .Build()(builder);
            });

            // Assert
            var div = cut.Find("div");
            var h1 = div.FindFirst("h1");
            var p = div.FindFirst("p");
            var button = div.FindFirst("button");

            Assert.Equal("Heading", h1.TextContent);
            Assert.Equal("Paragraph", p.TextContent);
            Assert.Equal("Click", button.TextContent);
        }

        [Fact]
        public void ElementBuilder_MaybeAttr_ShouldOnlyAddAttributeIfValueExists()
        {
            // Arrange & Act
            var cutWithAttr = Render(builder =>
            {
                Element.Div().MaybeAttr("title", "exists").Build()(builder);
            });

            var cutWithoutAttr = Render(builder =>
            {
                Element.Div().MaybeAttr("title", null).Build()(builder);
            });

            // Assert
            var divWithAttr = cutWithAttr.Find("div");
            var divWithoutAttr = cutWithoutAttr.Find("div");

            Assert.Equal("exists", divWithAttr.GetAttribute("title"));
            Assert.Null(divWithoutAttr.GetAttribute("title"));
        }

        [Fact]
        public void ElementBuilder_MaybeText_ShouldOnlyAddTextIfValueExists()
        {
            // Arrange & Act
            var cutWithText = Render(builder =>
            {
                Element.Div().MaybeText("exists").Build()(builder);
            });

            var cutWithoutText = Render(builder =>
            {
                Element.Div().MaybeText(null).Build()(builder);
            });

            // Assert
            var divWithText = cutWithText.Find("div");
            var divWithoutText = cutWithoutText.Find("div");

            Assert.Equal("exists", divWithText.TextContent);
            Assert.Empty(divWithoutText.TextContent);
        }

        [Fact]
        public void ElementBuilder_Disabled_ShouldAddDisabledAttributeAndClass()
        {
            // Arrange & Act
            var cutDisabled = Render(builder =>
            {
                Element.Button().Disabled(true).Build()(builder);
            });

            var cutEnabled = Render(builder =>
            {
                Element.Button().Disabled(false).Build()(builder);
            });

            // Assert
            var buttonDisabled = cutDisabled.Find("button");
            var buttonEnabled = cutEnabled.Find("button");

            Assert.True(buttonDisabled.HasAttribute("disabled"));
            Assert.Contains("disabled", buttonDisabled.ClassList);
            Assert.False(buttonEnabled.HasAttribute("disabled"));
            Assert.DoesNotContain("disabled", buttonEnabled.ClassList);
        }

        [Fact]
        public void ElementBuilder_Hide_ShouldSetDisplayNone()
        {
            // Arrange & Act
            var cutHidden = Render(builder =>
            {
                Element.Div().Hide().Build()(builder);
            });

            var cutVisible = Render(builder =>
            {
                Element.Div().Hide(false).Build()(builder);
            });

            // Assert
            var divHidden = cutHidden.Find("div");
            var divVisible = cutVisible.Find("div");

            Assert.Contains("display: none", divHidden.GetAttribute("style"));
            Assert.Null(divVisible.GetAttribute("style"));
        }

        [Fact]
        public void ElementBuilder_StyleObject_ShouldApplyStyles()
        {
            // Arrange & Act
            var cut = Render(builder =>
            {
                Element.Div()
                    .StyleObject(new
                    {
                        BackgroundColor = "#f5f5f5",
                        FontSize = "16px",
                        MarginTop = "10px"
                    })
                    .Build()(builder);
            });

            // Assert
            var div = cut.Find("div");
            var style = div.GetAttribute("style");

            Assert.Contains("background-color: #f5f5f5", style);
            Assert.Contains("font-size: 16px", style);
            Assert.Contains("margin-top: 10px", style);
        }

        [Fact]
        public void ElementBuilder_ToggleClass_ShouldAddClassConditionally()
        {
            // Arrange & Act
            var cutToggleTrue = Render(builder =>
            {
                Element.Div().ToggleClass("toggled", true).Build()(builder);
            });

            var cutToggleFalse = Render(builder =>
            {
                Element.Div().ToggleClass("toggled", false).Build()(builder);
            });

            var cutToggleDefault = Render(builder =>
            {
                Element.Div().ToggleClass("toggled").Build()(builder);
            });

            // Assert
            var divToggleTrue = cutToggleTrue.Find("div");
            var divToggleFalse = cutToggleFalse.Find("div");
            var divToggleDefault = cutToggleDefault.Find("div");

            Assert.Contains("toggled", divToggleTrue.ClassList);
            Assert.DoesNotContain("toggled", divToggleFalse.ClassList);
            Assert.Contains("toggled", divToggleDefault.ClassList);
        }
    }
}
