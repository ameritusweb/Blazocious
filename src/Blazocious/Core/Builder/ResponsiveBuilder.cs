using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public class ResponsiveBuilder
    {
        private readonly ElementBuilder _builder;

        public ResponsiveBuilder(ElementBuilder builder)
        {
            _builder = builder;
        }

        public ResponsiveBuilder At(Breakpoint breakpoint, Action<ElementBuilder> configure)
        {
            var mediaQuery = $"@media (min-width: {breakpoint.Width})";
            _builder.AddMediaQuery(mediaQuery, configure);
            return this;
        }

        public ResponsiveBuilder Below(Breakpoint breakpoint, Action<ElementBuilder> configure)
        {
            var mediaQuery = $"@media (max-width: {breakpoint.Width})";
            _builder.AddMediaQuery(mediaQuery, configure);
            return this;
        }

        public ResponsiveBuilder Between(Breakpoint min, Breakpoint max, Action<ElementBuilder> configure)
        {
            var mediaQuery = $"@media (min-width: {min.Width}) and (max-width: {max.Width})";
            _builder.AddMediaQuery(mediaQuery, configure);
            return this;
        }

        public ResponsiveBuilder Mobile(Action<ElementBuilder> configure) =>
            Below(Breakpoint.MD, configure);

        public ResponsiveBuilder Tablet(Action<ElementBuilder> configure) =>
            Between(Breakpoint.MD, Breakpoint.LG, configure);

        public ResponsiveBuilder Desktop(Action<ElementBuilder> configure) =>
            At(Breakpoint.LG, configure);

        public ResponsiveBuilder Dark(Action<ElementBuilder> configure)
        {
            _builder.AddMediaQuery("@media (prefers-color-scheme: dark)", configure);
            return this;
        }

        public ResponsiveBuilder ReducedMotion(Action<ElementBuilder> configure)
        {
            _builder.AddMediaQuery("@media (prefers-reduced-motion: reduce)", configure);
            return this;
        }
    }
}
