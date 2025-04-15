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
        private readonly Dictionary<string, HashSet<string>> _mediaQueries = new();
        private bool _isCollecting;

        public void StartCollecting() => _isCollecting = true;

        public void StopCollecting() => _isCollecting = false;

        public void TrackClass(string className)
        {
            if (_isCollecting && !string.IsNullOrEmpty(className))
            {
                _usedClasses.Add(className);
            }
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

        public IReadOnlySet<string> GetUsedClasses() => _usedClasses;

        public IReadOnlyDictionary<string, HashSet<string>> GetMediaQueries() => _mediaQueries;

        public void Clear()
        {
            _usedClasses.Clear();
            _mediaQueries.Clear();
        }
    }

}
