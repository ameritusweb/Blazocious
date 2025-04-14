using Blazocious.Core.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Test.Helpers
{
    /// <summary>
    /// Test implementation of IThemeRegistry for unit testing
    /// </summary>
    public class TestThemeRegistry : IThemeRegistry
    {
        private readonly Dictionary<string, ParsedTheme> _themes = new();

        public void Register(string name, ParsedTheme theme)
        {
            _themes[name] = theme;
        }

        public ParsedTheme Get(string name)
        {
            return _themes.TryGetValue(name, out var theme) ? theme : null;
        }

        public IEnumerable<string> GetRegisteredThemes()
        {
            return _themes.Keys;
        }
    }
}
