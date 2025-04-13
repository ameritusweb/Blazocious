# 🚀 **Blazocious**

A semantic-first, builder-based UI framework for Blazor that lets you compose UIs with pure C# logic.

## 🧩 Key Concept

**Blazocious** brings a new way of building UI in Blazor: starting from **semantic content** and transforming it into styled, structured UI components using a **declarative, composable AST**.

Instead of wiring up UI with repetitive markup or scattered logic, you define the **intent** of content — like a `Header`, `Body`, or `Footer` — and let Blazocious handle **decoration, styling, and rendering**.

This pattern separates **what** the UI *means* from **how** it *looks* or *behaves* — a clean break between **structure and meaning**.

> "Compose. Cache. Control. Without compromise."

## ✨ Features

- ✅ **ElementBuilder**: Fluent, type-safe DOM building API
- 🔥 **Semantic Builders**: Components like `Card`, `Select`, `Layout`, `Notification`
- 🎯 **Smart Caching**: Memory caching for expensive or frequently-used fragments
- 🧩 **Composition**: Nest and arrange semantic components with ease
- 🎨 **Theming**: Apply reusable visual themes across your app
- ⚡ **Performance**: Built-in debouncing, throttling, and state management

## 🌟 Quick Examples

### Element Building
```csharp
Element.Div("card")
    .ClassIf("active", isActive)
    .StyleObject(new { 
        BackgroundColor = isDark ? "#000" : "#fff",
        Padding = "1rem"
    })
    .Child(Element.H2().Text("Hello from Blazocious"))
    .Child(Element.Paragraph().Text("This is a card"))
    .Build()(builder);
```

### Semantic Components
```csharp
new CardBuilder(new CardData 
{
    Title = "Welcome",
    Content = "This is a semantic card"
})
.WithOptions(new CardOptions 
{
    Interactive = true,
    Cache = new CacheOptions { Duration = TimeSpan.FromMinutes(5) }
})
.Build()(builder);
```

### Form Building
```csharp
Element.Form("login-form")
    .OnChangeDebounced(300, HandleChange)
    .WithState(_formState)
    .Child(
        Element.Input()
            .Attr("type", "email")
            .OnInputThrottled(100, HandleInput)
    )
    .Build()(builder);
```

## 🔥 Core Features

### ElementBuilder
- Conditional classes with `.ClassIf()` and `.ClassWhen()`
- Element references with `.CaptureRef()` and `.OnMounted()`
- Debounced events with `.OnChangeDebounced()` and `.OnInputThrottled()`
- State management with `.WithState()` and `.OnUpdate()`
- CSS-in-JS with `.StyleObject()`

### Semantic Builders
- Type-safe data and options
- Built-in caching strategies
- Theme support
- Event handling
- Composition patterns

### Smart Caching
```csharp
protected override string ComputeCacheKey() =>
    $"card|{Data.Id}|{Options.Interactive}|{Theme?.Name}";
```

## 🧱 Architecture

Blazocious is built on three key patterns:

1. **Builder Pattern**: Fluent APIs for building UI
2. **Semantic Model**: Content-first approach to UI
3. **Component AST**: Tree-based view composition

## 📦 Getting Started

1. Install the NuGet package:
```bash
dotnet add package Blazocious
```

2. Add services in `Program.cs`:
```csharp
builder.Services.AddBlazocious();
```

3. Start building semantic UI:
```csharp
public class WelcomePage : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        Element.Div("welcome-container")
            .Child(Element.H1("Welcome to Blazocious"))
            .Child(
                new CardBuilder(new CardData { ... })
                    .WithOptions(new CardOptions { ... })
                    .Build()
            )
            .Build()(builder);
    }
}
```

## 🧪 Best Practices

1. **Use Semantic Builders** for high-level components
2. **Use ElementBuilder** for custom layouts and simple components
3. **Leverage Caching** for expensive renders
4. **Apply Themes** consistently
5. **Compose** instead of duplicating

## 🔮 Coming Soon

- Form validation integration
- Animation support
- Portal system
- More semantic components
- Designer tools

## 💥 Summary

**Blazocious** gives you a smarter, faster, and more maintainable way to build UI in Blazor — where you focus on **intent**, and the framework handles the rest.

It’s not just a UI library — it’s a **UI engine**.  
Built on semantics. Tuned for performance. Ready for scale.

---

## 🧡 License

MIT — free to use and extend.

---

> Made with love, by developers who believe UI should be fun again.


