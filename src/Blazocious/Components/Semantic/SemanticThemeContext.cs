using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Components.Semantic
{
    public record SemanticThemeContext
    {
        public string? BackgroundColor { get; init; }
        public string? TextColor { get; init; }
        public string? FontFamily { get; init; }
        public bool UseGlass { get; init; }
        public IReadOnlyList<string>? CustomClasses { get; init; }
        public IReadOnlyList<string>? Styles { get; init; }
    }
}
