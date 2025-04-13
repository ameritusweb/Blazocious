
# 🚀 **Blazocious** – A Semantic UI Framework for Blazor

## 🧩 Key Concept

**Blazocious** brings a new way of building UI in Blazor: starting from **semantic content** and transforming it into styled, structured UI components using a **declarative, composable AST**.

Instead of wiring up UI with repetitive markup or scattered logic, you define the **intent** of content — like a `Header`, `Body`, or `Footer` — and let Blazocious handle **decoration, styling, and rendering**.

This pattern separates **what** the UI *means* from **how** it *looks* or *behaves* — a clean break between **structure and meaning**, much like HTML5 did with semantic tags.

> "Compose. Cache. Control. Without compromise."

---

## 🚀 What is Blazocious?

It is a **semantic-first**, **builder-based**, and **performance-optimized** UI framework for Blazor. It helps you write UI components that are expressive, reusable, and cacheable — all without being locked into Razor syntax.

Blazocious is a new kind of Blazor UI framework that:

- Uses **semantic data models** instead of raw markup
- Offers a **builder-based API** for defining render fragments
- Applies the **decorator pattern** to wrap content in visual layers
- Handles **caching** and **re-render optimization** out of the box
- Makes **bindings and event callbacks intuitive** and type-safe
- Supports full layout composition via nested builders

---

## ✨ Features

- ✅ **Semantic builders**: Components like `Card`, `Select`, `Layout`, `Notification`, etc.
- ⚙️ **Builder pattern**: Fluent, expressive configuration of UI components
- 🎛️ **Theming support**: Apply reusable visual themes across your app
- 🔁 **Render caching**: Optional memory caching for expensive or frequently-used fragments
- 📦 **Composable layouts**: Nest and arrange semantic components with `LayoutBuilder`
- 🧠 **No boilerplate**: Eliminate repetitive `RenderTreeBuilder` code forever

---

## 🧱 Example: Building a Card

```csharp
new CardSemanticBuilder(new CardContent
{
    Title = builder => builder.AddContent(0, "Welcome!"),
    Content = builder => builder.AddContent(1, "This is a blazing fast card."),
    Footer = builder => builder.AddContent(2, "— Blazocious")
})
.WithOptions(new CardOptions
{
    Interactive = true,
    Shadowed = true,
    Cache = new CacheOptions { Duration = TimeSpan.FromMinutes(10) }
})
.Build()(builder);
```

---

## 🧩 Use in Razor

```razor
<MeritoCard>
    <Title>
        <h2>Hello from Blazocious</h2>
    </Title>
    <Content>
        <p>This card is declarative, reusable, and fast.</p>
    </Content>
    <Footer>
        <span>Built with ❤️</span>
    </Footer>
</MeritoCard>
```

---

## 🧬 Build Composable Layouts

```csharp
var layout = new LayoutBuilder()
    .WithOptions(new LayoutOptions
    {
        Type = LayoutType.Grid,
        Gap = "1rem"
    })
    .AddChild(new CardSemanticBuilder(...).Build())
    .AddChild(new NotificationSemanticBuilder(...).Build())
    .Build();
```

---

## 🎨 Theming & Style

```csharp
.WithTheme(new SemanticTheme
{
    BackgroundColor = "var(--bg-glass)",
    FontClass = "font-sans",
    UseGlass = true
})
```

---

## 🧠 Philosophy

Blazocious is built on 3 principles:

1. **Semantic** — Structure should reflect *meaning*, not markup.
2. **Composable** — UI should be *composed like code*, not duplicated.
3. **Performant** — Rendering should be *smart, not redundant*.

---

## 🧩 Extending Blazocious with Custom Semantic Builders

One of the most powerful aspects of **Blazocious** is that you can easily **create your own semantic components** by inheriting from the base builder:

```csharp
public abstract class SemanticBuilder<TOptions, TData>
{
    protected TData Data { get; }
    protected TOptions Options { get; private set; }

    public RenderFragment Build();
    protected abstract string GenerateCacheKeyString();
    protected abstract RenderFragment CreateFragment();
}
```

### Example: Custom `NotificationBuilder`

```csharp
public record NotificationOptions
{
    public string Type { get; init; } = "info";
    public bool Dismissible { get; init; } = true;
    public CacheOptions? Cache { get; init; }
}

public class NotificationBuilder : SemanticBuilder<NotificationOptions, string>
{
    public NotificationBuilder(string message) : base(message) { }

    protected override string GenerateCacheKeyString() =>
        $"{Options.Type}|{Options.Dismissible}|{Data}";

    protected override CacheOptions? GetCacheOptions() => Options.Cache;

    protected override RenderFragment CreateFragment() => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", $"blz-notification {Options.Type}");
        builder.AddContent(2, Data);

        if (Options.Dismissible)
        {
            builder.OpenElement(3, "button");
            builder.AddAttribute(4, "class", "dismiss");
            builder.AddContent(5, "×");
            builder.CloseElement();
        }

        builder.CloseElement();
    };
}
```

### Use it like:

```csharp
new NotificationBuilder("Something went wrong.")
    .WithOptions(new NotificationOptions
    {
        Type = "error",
        Dismissible = true,
        Cache = new CacheOptions { Duration = TimeSpan.FromMinutes(1) }
    })
    .Build()(builder);
```

---

## 📦 What's Coming

- `<SemanticSelect<T>>` with intuitive `@bind`
- `ExpandableContainerBuilder`
- `FormBuilder<T>` with validation and state models
- `ThemeProvider` and dynamic theming
- JSON-driven UIs

---

## 🧰 Getting Started

> Coming soon: NuGet package + documentation + quickstart

---

## 🔮 Future Ideas for Blazocious

- `SlotNode("actions")` for flexible insertion points
- Layout composition: rows, columns, grids via AST
- JSON / C# DSL-driven UI schema
- Theming + design tokens support
- Component registry: reusable templates (e.g. `<Blazocious.InfoCard>`)
- Designer tool integration / visual schema builder
- Server-side rendering + hydration-aware cache layer

---

## 💥 Summary

**Blazocious** gives you a smarter, faster, and more maintainable way to build UI in Blazor — where you focus on **intent**, and the framework handles the rest.

It’s not just a UI library — it’s a **UI engine**.  
Built on semantics. Tuned for performance. Ready for scale.

---

## 🧡 License

MIT — free to use and extend.

---

> Made with love, by developers who believe UI should be fun again.


