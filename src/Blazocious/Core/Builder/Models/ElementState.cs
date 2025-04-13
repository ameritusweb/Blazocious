using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder.Models
{
    public class ElementState<T>
    {
        private T _value;
        private readonly Action _notifyStateChanged;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _notifyStateChanged();
            }
        }

        public ElementState(T initialValue, Action notifyStateChanged)
        {
            _value = initialValue;
            _notifyStateChanged = notifyStateChanged;
        }
    }
}
