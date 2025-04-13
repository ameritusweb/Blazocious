using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public class ThemeProvider : ComponentBase, IDisposable
    {
        [Inject] private ThemeContext ThemeContext { get; set; } = default!;
        [Parameter] public string Variant { get; set; } = "light";
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private IDisposable? _subscription;

        protected override void OnInitialized()
        {
            ThemeContext.SetVariant(Variant);
            _subscription = ThemeContext.OnChange(() => StateHasChanged());
        }

        protected override void OnParametersSet()
        {
            ThemeContext.SetVariant(Variant);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (ChildContent != null)
            {
                builder.AddContent(0, ChildContent);
            }
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
