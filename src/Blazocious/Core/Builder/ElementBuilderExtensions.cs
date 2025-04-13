using Blazocious.Core.Theme;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public static class ElementBuilderExtensions
    {
        public static void AddParsedAttributes(this ElementBuilder builder, string attributes)
        {
            foreach (var (name, value) in AttributeParser.Parse(attributes))
            {
                builder.Attr(name, value);
            }
        }

        public static ElementBuilder MaybeAttr(this ElementBuilder builder, string name, string? value)
        => string.IsNullOrWhiteSpace(value) ? builder : builder.Attr(name, value);

        public static ElementBuilder MaybeText(this ElementBuilder builder, string? text)
            => string.IsNullOrWhiteSpace(text) ? builder : builder.Text(text);

        public static ElementBuilder Class(this ElementBuilder builder, string @class)
            => builder.Attr("class", @class);

        public static ElementBuilder Style(this ElementBuilder builder, string style)
            => builder.Attr("style", style);

        public static ElementBuilder OnClick(this ElementBuilder builder, object? value)
            => builder.Attr("onclick", value);

        public static ElementBuilder Hide(this ElementBuilder builder, bool condition = true)
            => condition ? builder.Style("display: none") : builder;

        public static ElementBuilder Show(this ElementBuilder builder, bool condition = true)
            => builder.Hide(!condition);

        public static ElementBuilder When(this ElementBuilder builder, bool condition, Action<ElementBuilder> configure)
            => condition ? configure(builder) : builder;

        public static ElementBuilder WithTheme(this ElementBuilder builder, ThemeContext theme)
            => builder
                .MaybeAttr("class", theme.CustomClass)
                .MaybeAttr("style", theme.CustomStyle);

        public static ElementBuilder ToggleClass(this ElementBuilder builder, string @class, bool? condition = null)
        {
            return builder.ClassIf(@class, condition ?? true);
        }

        public static ElementBuilder Classes(this ElementBuilder builder, params (string Class, bool Condition)[] classes)
        {
            foreach (var (className, condition) in classes)
            {
                builder.ClassIf(className, condition);
            }
            return builder;
        }

        public static ElementBuilder Show(this ElementBuilder builder, bool condition = true)
        {
            return builder.Hide(!condition);
        }

        public static ElementBuilder Disabled(this ElementBuilder builder, bool condition = true)
        {
            if (condition)
            {
                builder.Attr("disabled", true);
                builder.Class("disabled");
            }
            return builder;
        }

        public static ElementBuilder WithRef<T>(this ElementBuilder builder, T component, Action<ElementReference> callback) where T : ComponentBase
        {
            return builder
                .CaptureRef(default) // Will be set by capture
                .OnMounted(callback);
        }

        // Conditional responsive application
        public static ElementBuilder ResponsiveIf(
            this ElementBuilder builder,
            bool condition,
            Action<ResponsiveBuilder> configure)
        {
            return condition ? builder.Responsive(configure) : builder;
        }

        // Theme variant support
        public static ElementBuilder ApplyThemeVariant(
            this ElementBuilder builder,
            string variant)
        {
            return builder.YApply($"theme.{variant}");
        }

        // Combine both for theme-aware responsive design
        public static ElementBuilder ResponsiveTheme(
            this ElementBuilder builder,
            Action<ResponsiveThemeBuilder> configure)
        {
            var themeBuilder = new ResponsiveThemeBuilder(builder);
            configure(themeBuilder);
            return builder;
        }
    }
}
