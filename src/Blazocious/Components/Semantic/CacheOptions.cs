using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Components.Semantic
{
    public record CacheOptions
    {
        public bool Enabled { get; init; } = true;
        public TimeSpan? Duration { get; init; }
        public bool PreferStale { get; init; }
    }
}
