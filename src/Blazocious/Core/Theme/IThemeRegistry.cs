using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public interface IThemeRegistry
    {
        void Register(string name, ParsedTheme theme);
        ParsedTheme? Get(string name);
        IEnumerable<string> GetRegisteredThemes();
    }
}
