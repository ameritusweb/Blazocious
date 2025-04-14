using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
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
    }
}
