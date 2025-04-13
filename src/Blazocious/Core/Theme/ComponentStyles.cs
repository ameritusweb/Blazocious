using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public class ComponentStyles
    {
        public string? Class { get; init; }
        public Dictionary<string, string>? Styles { get; init; }
        public Dictionary<string, ComponentStyles>? Variants { get; init; }
    }
}
