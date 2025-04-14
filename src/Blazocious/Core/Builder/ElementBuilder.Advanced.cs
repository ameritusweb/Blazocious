using Blazocious.Core.Builder.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public partial class ElementBuilder
    {
        private static readonly ConcurrentDictionary<string, Timer> _debounceTimers = new();
        private readonly Dictionary<string, object> _styles = new();
        private Action? _onUpdate;
        private object? _previousState;

        public ElementBuilder OnChangeDebounced(int milliseconds, EventCallback<ChangeEventArgs> callback)
        {
            return OnChangeDebounced(milliseconds, async (args) => await callback.InvokeAsync(args));
        }

        public ElementBuilder OnChangeDebounced(int milliseconds, Action<ChangeEventArgs> callback)
        {
            var timerId = Guid.NewGuid().ToString();

            void Handler(object? sender, ChangeEventArgs args)
            {
                if (_debounceTimers.TryGetValue(timerId, out var existingTimer))
                {
                    existingTimer.Stop();
                    existingTimer.Dispose();
                }

                var timer = new Timer(milliseconds);
                timer.Elapsed += (s, e) =>
                {
                    callback(args);
                    timer.Dispose();
                    _debounceTimers.TryRemove(timerId, out _);
                };

                _debounceTimers.TryAdd(timerId, timer);
                timer.Start();
            }

            return Attr("onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, Handler));
        }

        public ElementBuilder OnInputThrottled(int milliseconds, EventCallback<ChangeEventArgs> callback)
        {
            DateTime lastRun = DateTime.MinValue;

            void Handler(object? sender, ChangeEventArgs args)
            {
                var now = DateTime.UtcNow;
                if ((now - lastRun).TotalMilliseconds >= milliseconds)
                {
                    lastRun = now;
                    callback.InvokeAsync(args);
                }
            }

            return Attr("oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, Handler));
        }

        public ElementBuilder WithState<T>(ElementState<T> state)
        {
            _previousState = state.Value;
            state = new ElementState<T>(
                state.Value,
                () =>
                {
                    _onUpdate?.Invoke();
                    _previousState = state.Value;
                }
            );
            return this;
        }

        public ElementBuilder OnUpdate(Action callback)
        {
            _onUpdate = callback;
            return this;
        }

        public ElementBuilder StyleObject(object styles)
        {
            var type = styles.GetType();
            var properties = type.GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(styles)?.ToString();
                if (value != null)
                {
                    var name = ToCssName(prop.Name);
                    _styles[name] = value;
                }
            }

            return this;
        }

        private static string ToCssName(string pascalCase)
        {
            return string.Concat(
                pascalCase.Select((c, i) => i > 0 && char.IsUpper(c) ? $"-{char.ToLower(c)}" : char.ToLower(c).ToString())
            );
        }

        public RenderFragment Build() => builder =>
        {
            if (BuildOverride != null)
            {
                BuildOverride(builder);
                return;
            }

            builder.OpenElement(0, _tag);

            int seq = 1;

            // Handle element reference and mounting
            if (_elementRef.HasValue)
            {
                builder.AddElementReferenceCapture(seq++, async capturedRef =>
                {
                    _elementRef = capturedRef;
                    if (_onMounted != null)
                    {
                        await Task.Yield();
                        _onMounted(capturedRef);
                    }
                });
            }

            // Add classes
            if (_classes.Any())
            {
                builder.AddAttribute(seq++, "class", string.Join(" ", _classes));
            }

            // Add styles
            if (_styles.Any())
            {
                var styleString = string.Join("; ", _styles.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
                builder.AddAttribute(seq++, "style", styleString);
            }

            // Add other attributes
            foreach (var (name, value) in _attributes)
            {
                builder.AddAttribute(seq++, name, value);
            }

            // Add content and children
            if (_textContent != null)
            {
                builder.AddContent(seq++, _textContent);
            }

            foreach (var child in _children)
            {
                builder.AddContent(seq++, child);
            }

            builder.CloseElement();
        };
    }
}
