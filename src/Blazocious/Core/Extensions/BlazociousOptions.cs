using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Extensions
{
    public class BlazociousOptions
    {
        public string? DefaultThemePath { get; set; }
        public string? DefaultThemeVariant { get; set; }
        public string? DefaultStylesPath { get; set; }
        public List<(string Name, string Path)>? ThemeOverrides { get; set; }
        public bool Debug { get; set; }
        public bool ValidateTokens { get; set; } = true;
        public bool CacheThemes { get; set; } = true;
        public TimeSpan? CacheDuration { get; set; }
    }
}
