using Blazocious.Core.Builder;
using Blazocious.Core.Extensions;
using Blazocious.Core.Styling;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blazocious.Core.Trackers;

namespace Blazocious
{
    public static class CssGenerator
    {
        private static BlazociousStyles blazociousStyles;

        public static async Task GenerateFromAssembly()
        {
            // Get configuration from DI
            var services = new ServiceCollection();
            services.AddBlazocious(); // This configures BlazociousOptions from appsettings.json

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<BlazociousOptions>>().Value;

            blazociousStyles = serviceProvider.GetRequiredService<BlazociousStyles>();

            var classUsageTracker = serviceProvider.GetRequiredService<IClassUsageTracker>();

            if (string.IsNullOrEmpty(options.DefaultStylesPath))
            {
                throw new InvalidOperationException("DefaultStylesPath must be configured in BlazociousOptions");
            }

            // Initialize styles from YAML
            var yamlContent = await File.ReadAllTextAsync(options.DefaultStylesPath);
            ElementBuilderStylingExtensions.InitializeStyles(yamlContent);

            // Start collecting
            classUsageTracker.StartCollecting();

            // Use reflection to find and invoke all RenderFragment-returning methods
            var assembly = Assembly.GetEntryAssembly();
            var builderTypes = assembly.GetTypes()
                .Where(t => t.GetMethods()
                    .Any(m => m.ReturnType == typeof(RenderFragment)));

            var renderTreeBuilder = new RenderTreeBuilder();

            foreach (var type in builderTypes)
            {
                var instance = Activator.CreateInstance(type);
                var methods = type.GetMethods()
                    .Where(m => m.ReturnType == typeof(RenderFragment));

                foreach (var method in methods)
                {
                    var fragment = (RenderFragment)method.Invoke(instance, Array.Empty<object>());
                    fragment?.Invoke(renderTreeBuilder);
                }
            }

            // Stop collecting
            classUsageTracker.StopCollecting();

            // Generate CSS
            var css = GenerateCss(
                classUsageTracker.GetUsedClasses(),
                classUsageTracker.GetMediaQueries()
            );

            // Save to file - output path would be configured in appsettings.json
            var outputPath = Path.Combine(
                Path.GetDirectoryName(options.DefaultStylesPath),
                options.CssOutputPath
            );

            await File.WriteAllTextAsync(outputPath, css);

            // Clear the tracker for next run
            classUsageTracker.Clear();
        }

        private static string GenerateCss(
            IReadOnlySet<string> usedClasses,
            IReadOnlyDictionary<string, HashSet<string>> mediaQueries)
        {
            var css = new StringBuilder();

            // Base styles
            foreach (var className in usedClasses)
            {
                var styles = blazociousStyles.GetStyles(className);
                if (!string.IsNullOrEmpty(styles.Style))
                {
                    css.AppendLine($".{className} {{");
                    css.AppendLine($"    {styles.Style}");
                    css.AppendLine("}");
                    css.AppendLine();
                }
            }

            // Media queries
            foreach (var (mediaQuery, classes) in mediaQueries.OrderBy(kv => GetBreakpointOrder(kv.Key)))
            {
                css.AppendLine($"{mediaQuery} {{");
                foreach (var className in classes)
                {
                    var styles = blazociousStyles.GetStyles(className);
                    if (!string.IsNullOrEmpty(styles.Style))
                    {
                        css.AppendLine($"    .{className} {{");
                        css.AppendLine($"        {styles.Style}");
                        css.AppendLine($"    }}");
                    }
                }
                css.AppendLine("}");
                css.AppendLine();
            }

            return css.ToString();
        }

        private static int GetBreakpointOrder(string mediaQuery)
        {
            if (mediaQuery.Contains("640px")) return 1;  // sm
            if (mediaQuery.Contains("768px")) return 2;  // md
            if (mediaQuery.Contains("1024px")) return 3; // lg
            if (mediaQuery.Contains("1280px")) return 4; // xl
            if (mediaQuery.Contains("1536px")) return 5; // 2xl
            return 99;
        }
    }
}
