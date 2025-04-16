using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Trackers
{
    public class ClassUsageTracker : IClassUsageTracker
    {
        private readonly HashSet<string> _usedClasses = new();
        private readonly SortedDictionary<string, HashSet<string>> _mediaQueries;
        private bool _isCollecting;

        public ClassUsageTracker()
        {
            // Use custom comparer to order media queries by breakpoint size
            _mediaQueries = new SortedDictionary<string, HashSet<string>>(new MediaQueryComparer());
        }

        public void TrackMediaQuery(string mediaQuery, string className)
        {
            if (_isCollecting && !string.IsNullOrEmpty(className))
            {
                if (!_mediaQueries.ContainsKey(mediaQuery))
                {
                    _mediaQueries[mediaQuery] = new HashSet<string>();
                }
                _mediaQueries[mediaQuery].Add(className);
            }
        }

        public void StartCollecting() => _isCollecting = true;

        public void StopCollecting() => _isCollecting = false;

        public void TrackClass(string className)
        {
            if (_isCollecting && !string.IsNullOrEmpty(className))
            {
                _usedClasses.Add(className);
            }
        }

        public IReadOnlySet<string> GetUsedClasses() => _usedClasses;

        public IReadOnlyDictionary<string, HashSet<string>> GetMediaQueries() => _mediaQueries;

        public void Clear()
        {
            _usedClasses.Clear();
            _mediaQueries.Clear();
        }
    }

}
