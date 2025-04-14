using Blazocious.Core.Builder.Models;
using Bunit;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Blazocious.Core.Builder;

namespace Blazocious.Test
{
    public class ElementStateTests : TestContext
    {
        [Fact]
        public void ElementState_Constructor_ShouldInitializeWithValues()
        {
            // Arrange
            var initialValue = "test-value";
            bool notificationCalled = false;
            Action notifyAction = () => notificationCalled = true;

            // Act
            var state = new ElementState<string>(initialValue, notifyAction);

            // Assert
            Assert.Equal(initialValue, state.Value);
            Assert.False(notificationCalled); // Constructor shouldn't trigger notification
        }

        [Fact]
        public void ElementState_ValueChange_ShouldTriggerNotification()
        {
            // Arrange
            var initialValue = "initial";
            var newValue = "updated";
            bool notificationCalled = false;
            var state = new ElementState<string>(initialValue, () => notificationCalled = true);

            // Act
            state.Value = newValue;

            // Assert
            Assert.Equal(newValue, state.Value);
            Assert.True(notificationCalled);
        }

        [Fact]
        public void ElementStateWithComponent_ShouldUpdateUI()
        {
            // Arrange & Act
            var cut = RenderComponent<TestStateComponent>();

            // Get initial state
            var initialCounter = cut.Find("#counter").TextContent;

            // Click the increment button
            cut.Find("button").Click();

            // Get updated state
            var updatedCounter = cut.Find("#counter").TextContent;

            // Assert
            Assert.Equal("Counter: 0", initialCounter);
            Assert.Equal("Counter: 1", updatedCounter);
        }

        [Fact]
        public void ElementStateWithBuilder_ShouldUpdateUI()
        {
            // Arrange & Act
            var cut = RenderComponent<TestStateBuilderComponent>();

            // Get initial state
            var initialText = cut.Find("div").TextContent;

            // Click the button to toggle visibility
            cut.Find("button").Click();

            // Check updated UI
            var div = cut.Find("div");

            // Assert
            Assert.Equal("Content is visible", initialText);
            Assert.Contains("display: none", div.GetAttribute("style"));
        }

        [Fact]
        public void MultipleElementStates_ShouldUpdateIndependently()
        {
            // Arrange & Act
            var cut = RenderComponent<TestMultiStateComponent>();

            // Get initial states
            var counterElement = cut.Find("#counter");
            var nameElement = cut.Find("#name");

            // Initial values
            var initialCounter = counterElement.TextContent;
            var initialName = nameElement.TextContent;

            // Update counter
            cut.Find("#increment-button").Click();

            // Update name
            cut.Find("#name-input").Change("Updated Name");

            // Get updated values
            var updatedCounter = counterElement.TextContent;
            var updatedName = nameElement.TextContent;

            // Assert
            Assert.Equal("Counter: 0", initialCounter);
            Assert.Equal("Name: Initial Name", initialName);

            Assert.Equal("Counter: 1", updatedCounter);
            Assert.Equal("Name: Updated Name", updatedName);
        }

        [Fact]
        public void ElementState_WithOnUpdateCallback_ShouldTriggerCallback()
        {
            // Arrange & Act
            var cut = RenderComponent<TestUpdateCallbackComponent>();

            // Get initial state
            var updateCountElement = cut.Find("#update-count");
            var initialUpdateCount = updateCountElement.TextContent;

            // Update the state multiple times
            cut.Find("#update-button").Click();
            cut.Find("#update-button").Click();
            cut.Find("#update-button").Click();

            // Get final update count
            var finalUpdateCount = updateCountElement.TextContent;

            // Assert
            Assert.Equal("Updates: 0", initialUpdateCount);
            Assert.Equal("Updates: 3", finalUpdateCount);
        }

        // Test components for Element State testing

        private class TestStateComponent : ComponentBase
        {
            private ElementState<int> _counter;

            protected override void OnInitialized()
            {
                _counter = new ElementState<int>(0, StateHasChanged);
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, "div");

                builder.OpenElement(1, "span");
                builder.AddAttribute(2, "id", "counter");
                builder.AddContent(3, $"Counter: {_counter.Value}");
                builder.CloseElement();

                builder.OpenElement(4, "button");
                builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => _counter.Value++));
                builder.AddContent(6, "Increment");
                builder.CloseElement();

                builder.CloseElement();
            }
        }

        private class TestStateBuilderComponent : ComponentBase
        {
            private ElementState<bool> _isVisible;

            protected override void OnInitialized()
            {
                _isVisible = new ElementState<bool>(true, StateHasChanged);
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                Element.Div()
                    .WithState(_isVisible)
                    .StyleObject(new { Display = _isVisible.Value ? "block" : "none" })
                    .Text("Content is visible")
                    .Build()(builder);

                Element.Button()
                    .Text("Toggle Visibility")
                    .OnClick(EventCallback.Factory.Create<MouseEventArgs>(this, () => _isVisible.Value = !_isVisible.Value))
                    .Build()(builder);
            }
        }

        private class TestMultiStateComponent : ComponentBase
        {
            private ElementState<int> _counter;
            private ElementState<string> _name;

            protected override void OnInitialized()
            {
                _counter = new ElementState<int>(0, StateHasChanged);
                _name = new ElementState<string>("Initial Name", StateHasChanged);
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, "div");

                // Counter display
                builder.OpenElement(1, "div");
                builder.AddAttribute(2, "id", "counter");
                builder.AddContent(3, $"Counter: {_counter.Value}");
                builder.CloseElement();

                // Name display
                builder.OpenElement(4, "div");
                builder.AddAttribute(5, "id", "name");
                builder.AddContent(6, $"Name: {_name.Value}");
                builder.CloseElement();

                // Counter update button
                builder.OpenElement(7, "button");
                builder.AddAttribute(8, "id", "increment-button");
                builder.AddAttribute(9, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => _counter.Value++));
                builder.AddContent(10, "Increment");
                builder.CloseElement();

                // Name input
                builder.OpenElement(11, "input");
                builder.AddAttribute(12, "id", "name-input");
                builder.AddAttribute(13, "value", _name.Value);
                builder.AddAttribute(14, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this,
                    e => _name.Value = e.Value?.ToString() ?? ""));
                builder.CloseElement();

                builder.CloseElement();
            }
        }

        private class TestUpdateCallbackComponent : ComponentBase
        {
            private ElementState<int> _value;
            private int _updateCount;

            protected override void OnInitialized()
            {
                _value = new ElementState<int>(0, StateHasChanged);
                _updateCount = 0;
            }

            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                Element.Div()
                    .Child(
                        Element.Span()
                            .Attr("id", "update-count")
                            .Text($"Updates: {_updateCount}")
                    )
                    .Child(
                        Element.Button()
                            .Attr("id", "update-button")
                            .Text("Update Value")
                            .OnClick(EventCallback.Factory.Create<MouseEventArgs>(this, () => {
                                _value.Value++;
                                _updateCount++;
                            }))
                    )
                    .WithState(_value)
                    .OnUpdate(() => StateHasChanged())
                    .Build()(builder);
            }
        }
    }
}
