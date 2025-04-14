using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public interface IThemeLoader
    {
        Task<ParsedTheme> LoadThemeAsync(string defaultYamlPath, string? overrideYamlPath = null);
    }
}
