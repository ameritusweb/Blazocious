using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public class ResponsiveThemeBuilder
    {
        private readonly ElementBuilder _builder;
        private readonly ResponsiveBuilder _responsive;

        public ResponsiveThemeBuilder(ElementBuilder builder)
        {
            _builder = builder;
            _responsive = new ResponsiveBuilder(builder);
        }

        public ResponsiveThemeBuilder ForTheme(string theme, Action<ElementBuilder> configure)
        {
            _builder.ApplyThemeVariant(theme);
            configure(_builder);
            return this;
        }

        public ResponsiveThemeBuilder ForBreakpoint(Breakpoint breakpoint, Action<ElementBuilder> configure)
        {
            _responsive.At(breakpoint, configure);
            return this;
        }

        public ResponsiveThemeBuilder WhenDark(Action<ElementBuilder> configure)
        {
            _responsive.Dark(configure);
            return this;
        }
    }
}
