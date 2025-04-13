using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public class ThemeContext
    {
        private string _currentVariant = "light";
        private readonly List<Action> _listeners = new();

        public string CurrentVariant => _currentVariant;

        public void SetVariant(string variant)
        {
            if (_currentVariant == variant) return;
            _currentVariant = variant;
            NotifyListeners();
        }

        public IDisposable OnChange(Action listener)
        {
            _listeners.Add(listener);
            return new ThemeSubscription(() => _listeners.Remove(listener));
        }

        private void NotifyListeners() => _listeners.ForEach(l => l());

        private record ThemeSubscription(Action Cleanup) : IDisposable
        {
            public void Dispose() => Cleanup();
        }
    }
}
