using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Components.Semantic
{
    /// <summary>
    /// Base interface for all semantic builder options
    /// </summary>
    public interface ISemanticOptions
    {
        CacheOptions? Cache { get; }
        string? CustomClass { get; }
    }
}
