using Blazocious.Core.YAML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public record ThemeLayer(
    Dictionary<string, TokenDefinition> Tokens,
    Dictionary<string, ComponentDefinition> Components,
    Dictionary<string, ComponentBaseDefinition> Styles
);
}
