using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Trackers
{
    internal class MediaQueryComparer : IComparer<string>
    {
        private readonly Dictionary<string, int> _breakpointOrder = new()
        {
            { "640px", 1 },  // sm
            { "768px", 2 },  // md
            { "1024px", 3 }, // lg
            { "1280px", 4 }, // xl
            { "1536px", 5 }  // 2xl
        };

        public int Compare(string? x, string? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            int xOrder = GetBreakpointOrder(x);
            int yOrder = GetBreakpointOrder(y);

            return xOrder.CompareTo(yOrder);
        }

        private int GetBreakpointOrder(string mediaQuery)
        {
            foreach (var (breakpoint, order) in _breakpointOrder)
            {
                if (mediaQuery.Contains(breakpoint))
                {
                    return order;
                }
            }
            return 99; // Unknown breakpoint goes last
        }
    }
}
