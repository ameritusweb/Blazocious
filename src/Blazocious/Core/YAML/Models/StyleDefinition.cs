using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.YAML.Models
{
    public class StyleDefinition
    {
        public Dictionary<string, TokenDefinition> Tokens { get; set; } = new();
        public Dictionary<string, ComponentDefinition> Components { get; set; } = new();
        public Dictionary<string, ComponentBaseDefinition> StreetStyles { get; set; } = new();
    }

    public class StylePropertyDefinition
    {
        public string Property { get; set; }
        public string Value { get; set; }
        public bool Important { get; set; }
        public string Comment { get; set; }
    }
}
