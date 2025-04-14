using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public interface IThemeMerger
    {
        ParsedTheme Merge(ParsedTheme defaultTheme, ParsedTheme overrideTheme);
        ComponentStyles GetStyles(string component, string? theme = null);
    }
}
