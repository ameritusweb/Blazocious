using Microsoft.AspNetCore.Components;

namespace Blazocious.Core.Builder;

public partial class ElementBuilder
{
    private readonly Dictionary<string, List<(string Property, string Value)>> _mediaQueries = new();

    public ElementBuilder AddMediaQuery(string mediaQuery, Action<ElementBuilder> configure)
    {
        // Create a temporary builder to capture styles
        var tempBuilder = new ElementBuilder("temp");
        configure(tempBuilder);

        // Extract styles from temporary builder
        var styles = tempBuilder._styles
            .Select(s => (s.Key, s.Value.ToString() ?? ""))
            .ToList();

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

            // Convert classes to a style
            if (classes.Any())
            {
                _mediaQueries[mediaQuery].Add(("class", string.Join(" ", classes)));
            }
        }

        return this;
    }
}