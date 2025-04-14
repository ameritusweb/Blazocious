using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public class ThemeContext
    {
        private ParsedTheme? _currentTheme;
        private string _currentVariant = "default";
        private event Action? ThemeChanged;

        public ParsedTheme CurrentTheme => _currentTheme ?? throw new InvalidOperationException("Theme not set");
        public string CurrentVariant => _currentVariant;

        public async Task SetVariantAsync(string variant, IThemeRegistry registry)
        {
            var theme = registry.Get(variant);
            if (theme != null)
            {
                _currentTheme = theme;
                _currentVariant = variant;
                ThemeChanged?.Invoke();
            }
        }

        public void OnThemeChanged(Action callback)
        {
            ThemeChanged += callback;
        }

        public void RemoveChangeListener(Action callback)
        {
            ThemeChanged -= callback;
        }

    }
}
