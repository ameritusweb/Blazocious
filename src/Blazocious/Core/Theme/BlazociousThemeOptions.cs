using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public class BlazociousThemeOptions
    {
        public string DefaultPath { get; set; } = "themes/default";
        public List<(string Name, string Path)> Overrides { get; set; } = new();
        public bool Debug { get; set; } = false;
        public bool ValidateTokens { get; set; } = true;
        public bool CacheThemes { get; set; } = true;
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(1);
        public string DefaultVariant { get; set; } = "dark";
    }
}
