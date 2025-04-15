using Blazocious.Core.Theme;
using Microsoft.AspNetCore.Components;

namespace Blazocious.Core.Builder;

public static class ElementBuilderExtensions
{
    public static ElementBuilder AddParsedAttributes(this ElementBuilder builder, string attributes)
    {
        foreach (var (name, value) in AttributeParser.Parse(attributes))
        {
            builder.Attr(name, value);
        }
        return builder;
    }

    public static ElementBuilder MaybeAttr(this ElementBuilder builder, string name, string? value)
        => string.IsNullOrWhiteSpace(value) ? builder : builder.Attr(name, value);

    public static ElementBuilder MaybeText(this ElementBuilder builder, string? text)
        => string.IsNullOrWhiteSpace(text) ? builder : builder.Text(text);

    public static ElementBuilder Class(this ElementBuilder builder, string @class)
    {
        var classes = @class.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
        foreach (var classValue in classes)
        {
            if (builder.MediaQuery != null)
            {
                builder.Classes.Add(classValue);
            }
            else
            {
                builder.Attr("class", classValue);
            }
        }

        return builder;
    }

    public static ElementBuilder Style(this ElementBuilder builder, string property, string value)
        => builder.AddStyle(property, value);

    public static ElementBuilder OnClick(this ElementBuilder builder, object? value)
        => builder.Attr("onclick", value);

    public static ElementBuilder Hide(this ElementBuilder builder, bool condition = true)
        => condition ? builder.Style("display", "none") : builder;

    public static ElementBuilder Show(this ElementBuilder builder, bool condition = true)
        => builder.Hide(!condition);

    public static ElementBuilder When(this ElementBuilder builder, bool condition, Func<ElementBuilder, ElementBuilder> configure)
        => condition ? configure(builder) : builder;

    public static ElementBuilder ToggleClass(this ElementBuilder builder, string @class, bool? condition = null)
        => builder.ClassIf(@class, condition ?? true);

    public static ElementBuilder Classes(this ElementBuilder builder, params (string Class, bool Condition)[] classes)
    {
        foreach (var (className, condition) in classes)
        {
            builder.ClassIf(className, condition);
        }
        return builder;
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
            .CaptureRef(default)
            .OnMounted(callback);
    }

    public static ElementBuilder ResponsiveIf(this ElementBuilder builder, bool condition, Action<ResponsiveBuilder> configure)
        => condition ? builder.Responsive(configure) : builder;

    public static ElementBuilder ApplyThemeVariant(this ElementBuilder builder, string variant)
        => builder.YApply($"theme.{variant}");

    public static ElementBuilder ResponsiveTheme(this ElementBuilder builder, Action<ResponsiveThemeBuilder> configure)
    {
        var themeBuilder = new ResponsiveThemeBuilder(builder);
        configure(themeBuilder);
        return builder;
    }
}