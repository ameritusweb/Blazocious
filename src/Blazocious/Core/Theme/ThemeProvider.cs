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
        [Inject] private IThemeRegistry ThemeRegistry { get; set; } = default!;

        [Parameter] public string Variant { get; set; } = "default";
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private bool _initialized;
        private Action? _onChangeHandler;

        protected override async Task OnInitializedAsync()
        {
            _onChangeHandler = () => InvokeAsync(StateHasChanged);
            ThemeContext.OnThemeChanged(_onChangeHandler);

            if (!_initialized)
            {
                await ThemeContext.SetVariantAsync(Variant, ThemeRegistry);
                _initialized = true;
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (_initialized)
            {
                await ThemeContext.SetVariantAsync(Variant, ThemeRegistry);
            }
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
            if (_onChangeHandler is not null)
            {
                ThemeContext.RemoveChangeListener(_onChangeHandler);
            }
        }
    }
}
