using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.YAML.Models
{
    public class ComponentDefinition
    {
        public string Name { get; set; }
        public ComponentBaseDefinition Base { get; set; }
        public Dictionary<string, ComponentBaseDefinition> Parts { get; set; } = new();
        public Dictionary<string, ComponentBaseDefinition> Variants { get; set; } = new();
        public string Description { get; set; }
    }

    public class ComponentBaseDefinition
    {
        public string Class { get; set; }
        public List<StylePropertyDefinition> Styles { get; set; } = new();
        public Dictionary<string, Dictionary<string, string>> MediaQueries { get; set; } = new();
        public Dictionary<string, Dictionary<string, string>> States { get; set; } = new();
    }
}
