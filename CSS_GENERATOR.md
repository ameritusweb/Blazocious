# Blazocious CSS Generator

The Blazocious CSS Generator is a build-time tool that automatically generates CSS from your YAML style definitions and actual element builder usage in your code. It analyzes your components through reflection and creates an optimized CSS file containing only the styles that are actually used in your application.

## Setup

### 1. Project Structure
```
MyApp.sln
├── MyApp.Builders/        # Console app containing your element builders
│   ├── Components/       
│   └── Program.cs        # Entry point for CSS generation
├── MyApp.Web/            # Your Blazor application
│   └── wwwroot/css/      # Where generated CSS will go
└── styles.yaml           # YAML style definitions
```

### 2. Install Required Packages
```xml
<!-- In MyApp.Builders.csproj -->
<PackageReference Include="Blazocious" Version="x.x.x" />
```

### 3. Configure Build Integration
```xml
<!-- In MyApp.Builders.csproj -->
<Import Project="$(MSBuildThisFileDirectory)../packages/Blazocious/build/Blazocious.targets" />

<PropertyGroup>
  <BlazociousYamlPath>../styles.yaml</BlazociousYamlPath>
  <BlazociousCssOutput>../MyApp.Web/wwwroot/css/generated.css</BlazociousCssOutput>
</PropertyGroup>
```

## YAML Style Definition

Your YAML file defines the styles for your components. The generator uses this as a source of truth for CSS generation:

```yaml
components:
  button:
    base:
      class: "btn"        # Class name to use in HTML
      styles:             # Styles to generate in CSS
        - padding: "1rem 2rem"
        - background: "#fff"
    variants:
      primary:
        class: "btn-primary"
        styles:
          - background: "#007bff"
          - color: "#fff"
    states:
      disabled:
        class: "btn--disabled"
        styles:
          - opacity: "0.5"
```

## Element Builder Usage

Create your element builders in the MyApp.Builders project:

```csharp
public class ButtonBuilders
{
    public RenderFragment CreatePrimaryButton()
    {
        return builder => Element.Button()
            .YApply("components.button.base")
            .YApply("components.button.variants.primary")
            .When(isDisabled, b => b
                .YApply("components.button.states.disabled"))
            .Build()(builder);
    }
}
```

## Generated CSS

The generator will create CSS based on actual usage:

```css
.btn {
    padding: 1rem 2rem;
    background: #fff;
}

.btn-primary {
    background: #007bff;
    color: #fff;
}

.btn--disabled {
    opacity: 0.5;
}
```

## Responsive Design

The generator handles responsive designs through the Responsive builder:

```csharp
Element.Div()
    .YApply("components.card.base")
    .Responsive(r => r
        .At(Breakpoint.MD, b => b
            .Class("card-md")))
```

Generates:
```css
.card {
    /* base styles */
}

@media (min-width: 768px) {
    .card-md {
        /* md styles */
    }
}
```

## Advanced Features

### Nested Components
The generator correctly handles nested element structures:

```csharp
Element.Div()
    .YApply("components.card.base")
    .Child(
        Element.Div()
            .YApply("components.card.header")
    )
```

### Media Query Ordering
Media queries are automatically ordered by breakpoint size:
- sm (640px)
- md (768px)
- lg (1024px)
- xl (1280px)
- 2xl (1536px)

### Style Deduplication
The generator automatically deduplicates styles and only includes classes that are actually used in your components.

## Best Practices

1. **Component Organization**
   - Keep related element builders in focused classes
   - Use semantic naming for your YAML components and classes
   - Structure YAML to match your component hierarchy

2. **Style Definition**
   ```yaml
   components:
     button:
       base:
         class: "btn"
         styles:
           # Core styles that define the component
           - display: "inline-flex"
           - padding: "0.5rem 1rem"
   ```

3. **Responsive Design**
   - Use the `Responsive` builder for media queries
   - Follow mobile-first approach
   - Use standard breakpoints when possible

4. **Performance**
   - The generator only includes used styles
   - Group related styles in YAML
   - Use variants and states for variations

## Troubleshooting

### Common Issues

1. **Styles Not Generating**
   - Ensure element builders are in the correct project
   - Verify YAML syntax
   - Check build configuration

2. **Missing Styles**
   - Verify YApply paths match YAML structure
   - Check if styles are defined in YAML
   - Ensure element builders are being discovered

3. **Build Errors**
   - Verify project references
   - Check YAML file path
   - Ensure output directory exists

## API Reference

### BlazociousCssGenerator
```csharp
public class BlazociousCssGenerator
{
    public BlazociousCssGenerator(string yamlContent);
    public Task GenerateCssAsync(string assemblyPath, string outputPath);
}
```

### Build Properties
```xml
<BlazociousGenerateCss>true</BlazociousGenerateCss>
<BlazociousYamlPath>path/to/styles.yaml</BlazociousYamlPath>
<BlazociousCssOutput>path/to/output.css</BlazociousCssOutput>
```
