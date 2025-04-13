using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public record Breakpoint(string Name, string Width)
    {
        public static Breakpoint SM = new("sm", "640px");
        public static Breakpoint MD = new("md", "768px");
        public static Breakpoint LG = new("lg", "1024px");
        public static Breakpoint XL = new("xl", "1280px");
        public static Breakpoint XXL = new("2xl", "1536px");

        public static implicit operator Breakpoint(string width) => new(width, width);
    }
}
