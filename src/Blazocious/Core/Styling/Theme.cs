using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Styling
{
    public class Theme
    {
        public string Name { get; init; } = "default";
        public Dictionary<string, string> Tokens { get; init; } = new();
    }
}
