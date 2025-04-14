using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public class ThemeRegistry : IThemeRegistry
    {
        private readonly Dictionary<string, ParsedTheme> _themes = new();
        private readonly object _lock = new();

        public void Register(string name, ParsedTheme theme)
        {
            lock (_lock)
            {
                _themes[name] = theme;
            }
        }

        public ParsedTheme? Get(string name)
        {
            lock (_lock)
            {
                return _themes.TryGetValue(name, out var theme) ? theme : null;
            }
        }

        public IEnumerable<string> GetRegisteredThemes()
        {
            lock (_lock)
            {
                return _themes.Keys.ToList();
            }
        }
    }
}
