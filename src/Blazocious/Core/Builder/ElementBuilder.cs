using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text;

namespace Blazocious.Core.Builder;

public partial class ElementBuilder
{
    private readonly string _tag;
    private readonly List<(string Name, object? Value)> _attributes = new();
    private readonly List<RenderFragment> _children = new();
    private readonly List<string> _classes = new();
    private RenderFragment? _textContent;
    private ElementReference? _elementRef;
    private Action<ElementReference>? _onMounted;
    private Action<ElementBuilder>? _customizer;
    private IServiceProvider? _serviceProvider;

    internal Action<RenderTreeBuilder>? BuildOverride { get; init; }

    public ElementBuilder(string tag)
    {
        _tag = tag;
    }

    public ElementBuilder Attr(string name, object? value)
    {
        _attributes.Add((name, value));
        return this;
    }

    public ElementBuilder Attrs(string attributeString)
    {
        foreach (var (name, value) in AttributeParser.Parse(attributeString))
        {
            _attributes.Add((name, value));
        }
        return this;
    }

    public ElementBuilder Text(string? content)
    {
        if (content != null)
        {
            _textContent = builder => builder.AddContent(0, content);
        }
        return this;
    }

    public ElementBuilder If(bool condition, Action<ElementBuilder> configure)
    {
        if (condition)
        {
            configure(this);
        }
        return this;
    }

    public ElementBuilder Unless(bool condition, Action<ElementBuilder> configure)
        => If(!condition, configure);

    public ElementBuilder ForEach<T>(IEnumerable<T> items, Func<T, ElementBuilder> createElement)
    {
        foreach (var item in items)
        {
            Child(createElement(item));
        }
        return this;
    }

    public ElementBuilder Customize(Action<ElementBuilder> customizer)
    {
        _customizer = customizer;
        return this;
    }

    public ElementBuilder Class(string @class)
    {
        _classes.Add(@class);
        return this;
    }

    public ElementBuilder ClassIf(string @class, bool condition)
    {
        if (condition)
        {
            _classes.Add(@class);
        }
        return this;
    }

    public ElementBuilder ClassWhen(string @class, Func<bool> predicate)
    {
        if (predicate())
        {
            _classes.Add(@class);
        }
        return this;
    }

    public ElementBuilder CaptureRef(ElementReference elementRef)
    {
        _elementRef = elementRef;
        return this;
    }

    public ElementBuilder OnMounted(Action<ElementReference> callback)
    {
        _onMounted = callback;
        return this;
    }

    public ElementBuilder Child(ElementBuilder? child)
    {
        if (child != null)
        {
            _children.Add(child.Build());
        }
        return this;
    }

    public ElementBuilder Children(params ElementBuilder[] children)
    {
        foreach (var child in children.Where(c => c != null))
        {
            _children.Add(child.Build());
        }
        return this;
    }

    public RenderFragment Build() => CreateFragment();

    protected virtual RenderFragment CreateFragment() => builder =>
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

        // Apply any customizations
        _customizer?.Invoke(this);
    };

    public ElementBuilder WithServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        return this;
    }

    public IServiceProvider? GetServiceProvider() => _serviceProvider;

    internal T? UseService<T>() where T : class
    {
        return _serviceProvider?.GetService(typeof(T)) as T;
    }
}