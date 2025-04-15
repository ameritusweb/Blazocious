using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Trackers
{
    public interface IClassUsageTracker
    {
        void StartCollecting();
        void StopCollecting();
        void TrackClass(string className);
        void TrackMediaQuery(string mediaQuery, string className);
        IReadOnlySet<string> GetUsedClasses();
        IReadOnlyDictionary<string, HashSet<string>> GetMediaQueries();
        void Clear();
    }
}
