# 🚀 Blazocious Best Practices Guide

## Table of Contents
- [Theme Organization](#theme-organization)
- [Component Structure](#component-structure)
- [Style Management](#style-management)
- [Performance Optimization](#performance-optimization)
- [Common Patterns](#common-patterns)
- [Anti-patterns to Avoid](#anti-patterns-to-avoid)

## Theme Organization

### 📁 File Structure
```
/themes
    /default         # Your base theme (usually dark)
        tokens.yaml  # Design tokens
        card.yaml    # Component definitions
        button.yaml
    /overrides      # Theme overrides
        light.yaml  # Light mode overrides
```

### 🎨 Design Tokens
```yaml
# tokens.yaml
tokens:
  # Use semantic names
  surface-primary: "#1f2937"
  surface-secondary: "#374151"
  
  # Define spacing scales
  spacing-sm: "0.5rem"
  spacing-md: "1rem"
  
  # Define consistent radii
  radius-sm: "0.25rem"
  radius-lg: "0.5rem"
```

### ✨ Best Practices
1. **Default Dark Theme**
   - Use dark theme as default
   - Override only what changes in light theme
   - Keep overrides minimal and focused

2. **Token Organization**
   - Use semantic token names
   - Group related tokens together
   - Document token usage

3. **Component Structure**
   - One component per file
   - Include documentation
   - List dependencies clearly

## Component Structure

### 🧱 Basic Component
```yaml
# card.yaml
card:
  base:
    class: "bg-gray-900 rounded-lg"
    styles:
      - border-radius: var(--radius-lg)
  
  header:
    class: "p-4 font-bold"
  
  variants:
    interactive:
      class: "hover:scale-105"
```

### 🔄 Override Component
```yaml
# light.yaml
card:
  base:
    class: "bg-white"  # Only override what changes
```

### ✨ Best Practices
1. **Component Organization**
   - Keep base styles minimal
   - Use semantic part names
   - Document variants clearly

2. **Class Management**
   - Use Tailwind utilities when possible
   - Group related classes together
   - Avoid mixing concerns in one class string

3. **Style Handling**
   - Use CSS variables for dynamic values
   - Keep inline styles minimal
   - Use classes for repetitive styles

## Style Management

### 🎯 Using YApply
```csharp
Element.Div()
    .YApply("card")  // Apply base styles
    .YApply("card.header")  // Apply part styles
    .Child(...)
```

### 🌗 Theme Variants
```csharp
Element.Div()
    .YApply("card")
    .ApplyThemeVariant()  // Use current theme
    .ApplyThemeVariant("dark")  // Force specific theme
```

### ✨ Best Practices
1. **Style Application**
   - Use `.YApply()` for component styles
   - Use `.ApplyThemeVariant()` for theme switches
   - Keep style logic centralized

2. **Class Resolution**
   - Let Blazocious handle class conflicts
   - Trust the theme merger
   - Avoid manual class string manipulation

3. **Style Overrides**
   - Use the theme system for overrides
   - Avoid inline style overrides
   - Keep modifications semantic

## Performance Optimization

### 🚀 Caching Strategies
```csharp
// Cache parsed themes
await ThemeContext.LoadPresetFromFile("dark", "themes/dark.yaml", cache: true);

// Cache component styles
Element.Div()
    .YApply("card", cacheKey: "main-card")
```

### ✨ Best Practices
1. **Theme Loading**
   - Load themes at startup
   - Cache parsed results
   - Use async loading for large themes

2. **Style Caching**
   - Cache frequently used components
   - Clear cache on theme changes
   - Monitor cache size

3. **Component Optimization**
   - Use semantic builders for complex components
   - Leverage built-in caching
   - Profile render performance

## Common Patterns

### 🏗️ Semantic Builders
```csharp
public class CardBuilder : SemanticBuilder<CardData, CardOptions>
{
    protected override string ComputeCacheKey() =>
        $"card|{Data.Id}|{Options.Interactive}";
}
```

### 🎨 Theme Switching
```csharp
await ThemeContext.SetVariant("light");  // Switch theme
Element.Div().ApplyThemeVariant();  // Automatically uses current theme
```

### ✨ Best Practices
1. **Builder Usage**
   - Create semantic builders for complex components
   - Use proper cache keys
   - Handle state changes efficiently

2. **Theme Handling**
   - Use theme context for global changes
   - Handle theme changes gracefully
   - Provide fallbacks for missing styles

## Anti-patterns to Avoid

### ❌ Don't Do This
```csharp
// Don't mix dark/light classes manually
Element.Div()
    .Class("bg-gray-900 dark:bg-white")  // ❌

// Don't override theme styles inline
Element.Div()
    .YApply("card")
    .Style("background", "#000")  // ❌

// Don't duplicate theme logic
Element.Div()
    .YApply("card")
    .ApplyThemeVariant("dark")
    .Class("dark:bg-gray-900")  // ❌
```

### ✅ Do This Instead
```csharp
// Use the theme system
Element.Div()
    .YApply("card")
    .ApplyThemeVariant()  // ✅

// Override in theme files
// light.yaml
card:
  base:
    class: "bg-white"  // ✅

// Use semantic builders for complex components
new CardBuilder(data)
    .WithOptions(options)
    .Build()  // ✅
```

### Common Mistakes to Avoid
1. **Manual Theme Switching**
   - Don't manually toggle theme classes
   - Don't mix dark/light prefixes
   - Don't duplicate theme logic

2. **Style Management**
   - Don't bypass the theme system
   - Don't mix styling approaches
   - Don't hardcode theme values

3. **Component Structure**
   - Don't mix semantic concerns
   - Don't bypass builders
   - Don't reinvent theme logic

## Further Reading
- [Detailed Theme Documentation](theme-docs.md)
- [Component API Reference](api-reference.md)
- [Performance Guide](performance.md)
- [Migration Guide](migration.md)
