using Blazocious.Core.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public static class ElementBuilderStylingExtensions
    {
        private static BlazociousStyles? _styles;

        public static void InitializeStyles(string yamlContent)
        {
            _styles = new BlazociousStyles(yamlContent);
        }

        public static ElementBuilder YApply(this ElementBuilder builder, string path, string? variant = null)
        {
            if (_styles == null)
                return builder;

            var result = _styles.GetStyles(path, variant);

            if (!string.IsNullOrEmpty(result.Class))
            {
                builder.Tracker.TrackClass(result.Class);
                builder.Class(result.Class);
            }

            //if (!string.IsNullOrEmpty(result.Style))
            //{
            //    builder.Attr("style", result.Style);
            //}

            // Add state styles if available
            if (result.States?.Any() == true)
            {
                foreach (var (state, styles) in result.States)
                {
                    var stateClass = $"{result.Class}--{state}";
                    builder.Tracker.TrackClass(stateClass);
                    builder.Class(stateClass);
                }
            }

            return builder;
        }
    }
}
