using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public static class ElementBuilderResponsiveExtensions
    {
        public static ElementBuilder Responsive(this ElementBuilder builder, Action<ResponsiveBuilder> configure)
        {
            var responsive = new ResponsiveBuilder(builder);
            configure(responsive);
            return builder;
        }
    }
}
