using Blazocious.Core.Styling;
using Microsoft.AspNetCore.Components;

namespace Blazocious.Core.Builder;

public partial class ElementBuilder
{
    private readonly Dictionary<string, List<(string Property, string Value)>> _mediaQueries = new();

    public ElementBuilder AddMediaQuery(string mediaQuery, Action<ElementBuilder> configure)
    {
        var tempBuilder = new ElementBuilder("temp");
        configure(tempBuilder);

        var styles = this.UseService<BlazociousStyles>();
        if (styles == null) return this;

        var mediaStyles = new List<(string Property, string Value)>();

        // Get styles for each class
        foreach (var className in tempBuilder._classes)
        {
            mediaStyles.AddRange(styles.GetStylesFormatted(className));
        }

        // Add any direct styles
        mediaStyles.AddRange(tempBuilder._styles
            .Select(s => (s.Key, s.Value.ToString() ?? "")));

        if (mediaStyles.Any())
        {
            _mediaQueries[mediaQuery] = mediaStyles;
        }

        return this;
    }
}