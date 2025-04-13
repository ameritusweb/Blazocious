using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public partial class ElementBuilder
    {
        private readonly Dictionary<string, List<(string Property, string Value)>> _mediaQueries = new();

        public ElementBuilder AddMediaQuery(string mediaQuery, Action<ElementBuilder> configure)
        {
            // Create a temporary builder to capture styles
            var tempBuilder = new ElementBuilder("temp");
            configure(tempBuilder);

            // Extract styles from temporary builder
            var styles = tempBuilder._styles.ToList();
            var classes = tempBuilder._classes.ToList();

            // If we have styles or classes for this media query, store them
            if (styles.Any() || classes.Any())
            {
                if (!_mediaQueries.ContainsKey(mediaQuery))
                {
                    _mediaQueries[mediaQuery] = new List<(string, string)>();
                }

                // Add styles
                _mediaQueries[mediaQuery].AddRange(styles);

                // Convert classes to a style that applies them
                if (classes.Any())
                {
                    _mediaQueries[mediaQuery].Add(("class", string.Join(" ", classes)));
                }
            }

            return this;
        }

        protected override RenderFragment CreateFragment() => builder =>
        {
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

            // Add base classes
            if (_classes.Any())
            {
                builder.AddAttribute(seq++, "class", string.Join(" ", _classes));
            }

            // Add base styles
            if (_styles.Any())
            {
                builder.AddAttribute(seq++, "style", string.Join("; ", _styles.Select(s => $"{s.Property}: {s.Value}")));
            }

            // Add media queries if we have any
            if (_mediaQueries.Any())
            {
                builder.OpenElement(seq++, "style");
                var styleBuilder = new StringBuilder();

                foreach (var (mediaQuery, styles) in _mediaQueries)
                {
                    styleBuilder.AppendLine(mediaQuery + " {");

                    // Generate a unique selector for this element
                    var uniqueId = $"blz-{Guid.NewGuid():N}";
                    _classes.Add(uniqueId);

                    styleBuilder.AppendLine($"  .{uniqueId} {{");

                    foreach (var (property, value) in styles)
                    {
                        if (property == "class")
                        {
                            // Handle classes differently - they need to be applied as is
                            _classes.Add(value);
                        }
                        else
                        {
                            styleBuilder.AppendLine($"    {property}: {value};");
                        }
                    }

                    styleBuilder.AppendLine("  }");
                    styleBuilder.AppendLine("}");
                }

                builder.AddContent(seq++, styleBuilder.ToString());
                builder.CloseElement();
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
