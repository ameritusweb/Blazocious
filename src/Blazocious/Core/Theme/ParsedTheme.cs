using Blazocious.Core.YAML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public class ParsedTheme
    {
        public Dictionary<string, TokenDefinition> Tokens { get; set; } = new();
        public Dictionary<string, ComponentDefinition> Components { get; set; } = new();
        public List<StylePropertyDefinition>? Styles { get; set; }
    }
}
