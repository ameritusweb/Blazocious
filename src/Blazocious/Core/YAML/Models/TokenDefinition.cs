using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.YAML.Models
{
    public class TokenDefinition
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> ThemeOverrides { get; set; } = new();
    }
}
