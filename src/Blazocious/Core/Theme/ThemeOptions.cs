using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public class ThemeOptions
    {
        public string DefaultTheme { get; set; } = string.Empty;
        public string? LightOverride { get; set; }
        public string? DarkOverride { get; set; }
        public string? InitialVariant { get; set; }
    }
}
