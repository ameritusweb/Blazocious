using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Test.Extensions
{
    public static class IElementExtensions
    {
        public static IElement? FindFirst(this IElement element, string tag)
        {
            return element.QuerySelector(tag);
        }

        public static List<IElement> FindAll(this IElement element, string tag)
        {
            return element.QuerySelectorAll(tag).ToList();
        }
    }
}
