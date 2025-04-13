using Blazocious.Core.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public static class ElementBuilderThemeExtensions
    {
        public static ElementBuilder ApplyTheme(
            this ElementBuilder builder,
            string component,
            string? theme = null,
            string? variant = null)
        {
            return builder.UseService<ThemeMerger>((merger, b) =>
            {
                var styles = merger.GetStyles(component, theme);

                if (!string.IsNullOrEmpty(styles.Class))
                {
                    b.Class(styles.Class);
                }

                if (styles.Styles?.Any() == true)
                {
                    foreach (var (prop, value) in styles.Styles)
                    {
                        b.Style(prop, value);
                    }
                }

                if (variant != null &&
                    styles.Variants?.TryGetValue(variant, out var variantStyles) == true)
                {
                    if (!string.IsNullOrEmpty(variantStyles.Class))
                    {
                        b.Class(variantStyles.Class);
                    }

                    if (variantStyles.Styles?.Any() == true)
                    {
                        foreach (var (prop, value) in variantStyles.Styles)
                        {
                            b.Style(prop, value);
                        }
                    }
                }
            });
        }
    }
}
