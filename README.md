
# 🚀 **Blazocious** – A Semantic UI Framework for Blazor

## 🧩 Key Concept

**Blazocious** brings a new way of building UI in Blazor: starting from **semantic content** and transforming it into styled, structured UI components using a **declarative, composable AST**.

Instead of wiring up UI with repetitive markup or scattered logic, you define the **intent** of content — like a `Header`, `Body`, or `Footer` — and let Blazocious handle **decoration, styling, and rendering**.

This pattern separates **what** the UI *means* from **how** it *looks* or *behaves* — a clean break between **structure and meaning**, much like HTML5 did with semantic tags.

---

## 🧱 Declarative AST Representation

Blazocious models UI as a tree of **semantic nodes**:

```csharp
abstract record UiNode;

record ContentNode(string Role, UiNode Child) : UiNode; // Roles: "header", "body", "footer", "actions"
record TextNode(string Text) : UiNode;
record ElementNode(string Tag, Dictionary<string, object> Attributes, List<UiNode> Children) : UiNode;
```

This structure allows for **render-time transformation**, decoration, and optimization.

---

## ✨ Composing and Decorating Content

Start with raw, meaningful content blocks:

```csharp
var contentAst = new List<UiNode>
{
    new ContentNode("header", new TextNode("My Card Title")),
    new ContentNode("body", new TextNode("Main content goes here.")),
    new ContentNode("footer", new TextNode("Footer actions or links"))
};
```

Then apply a `DecorateCard` function to wrap each semantic block with layout and style:

```csharp
UiNode DecorateCard(List<UiNode> contentNodes)
{
    var children = contentNodes.Select(n =>
    {
        return n switch
        {
            ContentNode { Role: "header" } => new ElementNode("div",
                new() { ["class"] = "meritocious-card-header" },
                new() { n.Child }),

            ContentNode { Role: "body" } => new ElementNode("div",
                new() { ["class"] = "meritocious-card-body" },
                new() { n.Child }),

            ContentNode { Role: "footer" } => new ElementNode("div",
                new() { ["class"] = "meritocious-card-footer" },
                new() { n.Child }),

            _ => n
        };
    }).ToList();

    return new ElementNode("div",
        new() { ["class"] = "meritocious-card meritocious-card-interactive" },
        children
    );
}
```

---

## 🎨 Default Card Styles

```css
.meritocious-card {
    backdrop-filter: blur(8px);
    border-radius: var(--meritocious-radius-lg);
    overflow: hidden;
    transition: var(--meritocious-transition);
    max-width: 72rem;
    margin-left: auto;
    margin-right: auto;
}

.meritocious-card-body {
    background-color: rgba(31, 41, 55, 0.5);
    backdrop-filter: blur(4px);
    border-radius: 0.5rem;
    padding: 2rem;
    border: 1px solid rgba(55, 65, 81, 0.5);
    margin-bottom: 2rem;
}

.meritocious-card-content {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    margin-bottom: 1.5rem;
    padding: var(--meritocious-spacing-md);
}

.meritocious-card-interactive {
    cursor: pointer;
}

.meritocious-card-interactive:hover {
    border-color: rgba(45, 212, 191, 0.3);
}

.meritocious-card-header {
    border-bottom: 1px solid var(--meritocious-border);
}

.meritocious-card-header h3 {
    font-size: 1.875rem;
    line-height: 2.25rem;
    font-weight: 700;
}

.meritocious-card-footer {
    padding: var(--meritocious-spacing-md);
    border-top: 1px solid var(--meritocious-border);
}
```

---

## ⚡ Performance Features

Blazocious supports **caching**, **memoization**, and **background revalidation** with:

- `CardOptions.Cache` for per-instance control
- Content-aware `SHA256` cache keys
- `PreferStale` option for smooth UX during background refresh
- Pre-rendered templates (e.g. loading states, empty cards)

---

## 🧠 Benefits

- ✅ **Semantic-first UI** — write what you mean, not what it looks like
- ✅ **Encapsulated design** — centralized layout + style logic
- ✅ **Composable decorations** — consistent, layered transformations
- ✅ **Declarative + imperative flexibility**
- ✅ **Performant by default** — caching, precomputed fragments
- ✅ **Testable AST structure** — great for snapshot, structural, or integration testing

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
