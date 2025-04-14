# Examples

``` csharp
public class CardBuilder : SemanticBuilder<CardData, CardOptions>
{
    protected override RenderFragment CreateFragment() => builder =>
    {
        Element.Div("meritocious-card")
            .Attrs($"data-id={Data.Id}; role=region; tabindex=0")
            .When(Options.Interactive, e => e.Class("meritocious-card-interactive"))
            .Child(Element.H2()
                .Class("meritocious-card-title")
                .Text(Data.Title))
            .Child(Element.Div("meritocious-card-body")
                .Child(Element.Paragraph().Text(Data.Content)))
            .ForEach(Data.Actions, action =>
                Element.Button()
                    .Class("meritocious-card-action")
                    .Text(action.Text)
                    .OnClick(action.Handler))
            .WithTheme(Theme)
            .Build()(builder);
    };
}

public class NotificationBuilder : SemanticBuilder<NotificationData, NotificationOptions>
{
    protected override RenderFragment CreateFragment() => builder =>
    {
        Element.Div($"blz-notification blz-notification-{Options.Type}")
            .If(Options.Dismissible, e => e.Class("blz-notification-dismissible"))
            .Child(Element.Span("blz-notification-icon").Text(Data.Icon))
            .Child(Element.Paragraph().Text(Data.Message))
            .If(Options.Dismissible, e => e
                .Child(Element.Button("blz-notification-close")
                    .Text("×")
                    .OnClick(Data.OnDismiss)))
            .Build()(builder);
    };
}

// Usage in a page/component
protected override void BuildRenderTree(RenderTreeBuilder builder)
{
    Element.Main("dashboard")
        .Child(Element.H1("Dashboard Overview"))
        .Child(
            Element.Section("dashboard-cards")
                .ForEach(cards, card =>
                    Element.Div("dashboard-card")
                        .Child(Element.H3(card.Title))
                        .Child(Element.Paragraph(card.Description))
                        .If(card.IsActionable, e => e
                            .Child(Element.Button()
                                .Class("btn btn-primary")
                                .Text("View Details")
                                .OnClick(() => NavigateToCard(card.Id)))))
        )
        .Build()(builder);
}
```

---

``` csharp
public class AuthFormBuilder : SemanticBuilder<AuthFormData, AuthFormOptions>
{
    private readonly EventCallback<AuthFormData> _onSubmit;
    private readonly EventCallback _onBack;
    private readonly EventCallback _onSignUp;
    private readonly EventCallback _onForgotPassword;
    private readonly EditContext _editContext;

    public AuthFormBuilder(
        AuthFormData data,
        EventCallback<AuthFormData> onSubmit,
        EventCallback onBack,
        EventCallback onSignUp,
        EventCallback onForgotPassword) : base(data)
    {
        _onSubmit = onSubmit;
        _onBack = onBack;
        _onSignUp = onSignUp;
        _onForgotPassword = onForgotPassword;
        _editContext = new EditContext(data);
    }

    protected override string ComputeCacheKey() =>
        $"auth-form|{Options.ShowBackButton}|{Options.ShowRememberMe}|{Options.ShowForgotPassword}|{Options.ShowSignUp}";

    private ElementBuilder BuildHeader() =>
        Element.Div("meritocious-auth-header")
            .Child(Element.Div("meritocious-logo-icon")
                .Child(Element.Icon("fas fa-brain")))
            .Child(Element.H1().Text("Welcome Back"))
            .Child(Element.Paragraph().Text("Continue your journey of idea evolution"));

    private ElementBuilder BuildFormField<TValue>(
        Expression<Func<AuthFormData, TValue>> field,
        string type,
        string icon,
        string placeholder)
    {
        var fieldIdentifier = FieldIdentifier.Create(field);
        
        return Element.Component<MeritociousAuthInput>()
            .Attrs($@"
                Type={type};
                Icon={icon};
                Placeholder={placeholder};
                Value={Data.GetType().GetProperty(fieldIdentifier.FieldName)?.GetValue(Data)};
                ValueChanged={EventCallback.Factory.Create<string>(this, 
                    value => {
                        Data.GetType().GetProperty(fieldIdentifier.FieldName)?.SetValue(Data, value);
                        _editContext.NotifyFieldChanged(fieldIdentifier);
                    }
                )}
            ");
    }

    private ElementBuilder BuildFormOptions() =>
        Element.Div("meritocious-form-options")
            .If(Options.ShowRememberMe, e => e
                .Child(Element.Label("meritocious-checkbox")
                    .Child(Element.Input("checkbox")
                        .Attrs($"checked={Data.RememberMe}; onchange={EventCallback.Factory.Create<ChangeEventArgs>(this, 
                            e => Data.RememberMe = (bool)(e.Value ?? false))}"))
                    .Child(Element.Span().Text("Remember me"))))
            .If(Options.ShowForgotPassword, e => e
                .Child(Element.Button("meritocious-link")
                    .Attr("type", "button")
                    .Text("Forgot password?")
                    .OnClick(_onForgotPassword)));

    private ElementBuilder BuildForm() =>
        Element.Form("meritocious-auth-form")
            .OnSubmit(async _ => 
            {
                if (_editContext.Validate())
                    await _onSubmit.InvokeAsync(Data);
            })
            .Child(BuildFormField(x => x.Email, "email", "fas fa-envelope", "Email address"))
            .Child(BuildFormField(x => x.Password, "password", "fas fa-lock", "Password"))
            .Child(BuildFormOptions())
            .Child(Element.Component<MeritociousButton>()
                .Attrs($@"
                    Type=submit;
                    Disabled={!_editContext.Validate()};
                    FullWidth=true
                ")
                .Text("Log In"));

    private ElementBuilder BuildFooter() =>
        Element.Paragraph("meritocious-auth-footer")
            .Text("Don't have an account? ")
            .If(Options.ShowSignUp, e => e
                .Child(Element.Anchor("#")
                    .Class("meritocious-link")
                    .Text("Sign up")
                    .OnClick(_onSignUp)));

    protected override RenderFragment CreateFragment() => builder =>
    {
        Element.Div("meritocious-auth-container")
            .If(Options.ShowBackButton, e => e
                .Child(Element.Button("meritocious-back-button")
                    .Child(Element.Icon("fas fa-arrow-left"))
                    .Text("Back")
                    .OnClick(_onBack)))
            .Child(Element.Div("meritocious-auth-content")
                .Child(BuildHeader())
                .Child(BuildForm())
                .Child(BuildFooter()))
            .Build()(builder);
    };
}
```

---

``` csharp
public class EnhancedExample : ComponentBase
{
    private ElementReference formRef;
    private bool isLoading = false;
    private bool isValid = true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        Element.Form("login-form")
            .WithRef(this, InitializeForm)
            .Classes(
                ("loading", isLoading),
                ("invalid", !isValid),
                ("interactive", !isLoading)
            )
            .Child(
                Element.Button("submit-button")
                    .Disabled(isLoading || !isValid)
                    .Text(isLoading ? "Loading..." : "Submit")
            )
            .Build()(builder);
    }

    private async Task InitializeForm(ElementReference formRef)
    {
        // Initialize form validation, attach event handlers, etc.
    }
}
```

---

``` csharp
public class SearchComponent : ComponentBase
{
    private ElementState<string> _searchText;
    private ElementState<bool> _isLoading;

    protected override void OnInitialized()
    {
        _searchText = new ElementState<string>("", StateHasChanged);
        _isLoading = new ElementState<bool>(false, StateHasChanged);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        Element.Div("search-container")
            .StyleObject(new 
            { 
                Display = "flex",
                FlexDirection = "column",
                Gap = "1rem",
                Padding = "1rem",
                BackgroundColor = _isLoading.Value ? "#f5f5f5" : "#fff"
            })
            .Child(
                Element.Input("search-input")
                    .Attr("placeholder", "Search...")
                    .Attr("value", _searchText.Value)
                    .OnChangeDebounced(300, async e => 
                    {
                        _isLoading.Value = true;
                        _searchText.Value = e.Value?.ToString() ?? "";
                        await SearchItems();
                        _isLoading.Value = false;
                    })
            )
            .WithState(_searchText)
            .WithState(_isLoading)
            .OnUpdate(() => Console.WriteLine($"State updated: {_searchText.Value}"))
            .Build()(builder);
    }

    private async Task SearchItems()
    {
        await Task.Delay(1000); // Simulate API call
    }
}

public class FormComponent : ComponentBase
{
    private ElementState<string> _email = new("", () => {});
    private ElementState<bool> _isDirty = new(false, () => {});

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        Element.Form("contact-form")
            .StyleObject(new
            {
                MaxWidth = "400px",
                Margin = "0 auto",
                BorderRadius = "8px",
                BoxShadow = _isDirty.Value ? "0 0 0 2px #007bff" : "none"
            })
            .Child(
                Element.Input()
                    .Attr("type", "email")
                    .Attr("value", _email.Value)
                    .OnInputThrottled(100, e =>
                    {
                        _email.Value = e.Value?.ToString() ?? "";
                        _isDirty.Value = true;
                    })
            )
            .WithState(_email)
            .WithState(_isDirty)
            .Build()(builder);
    }
}
```

---

``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBlazociousTheming(options =>
{
    options.DefaultTheme = "themes/default/theme.yaml";
    options.LightOverride = "themes/light/override.yaml";
    options.DarkOverride = "themes/dark/override.yaml";
    options.InitialVariant = "light";
});
```
