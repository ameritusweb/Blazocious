using Blazocious.Core.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Theme
{
    public static class ThemeContextExtensions
    {
        public static ElementBuilder ApplyThemeVariant(this ElementBuilder builder)
        {
            return builder.WithThemeContext((context, b) =>
                b.YApply($"theme.{context.CurrentVariant}"));
        }

        public static ElementBuilder ApplyThemeVariant(this ElementBuilder builder, string? variant)
        {
            return variant != null
                ? builder.YApply($"theme.{variant}")
                : builder.ApplyThemeVariant();
        }

        public static ElementBuilder WithThemeContext(
            this ElementBuilder builder,
            Action<ThemeContext, ElementBuilder> configure)
        {
            return builder.UseService<ThemeContext>((context, b) => configure(context, b));
        }

        // Helper for service injection in builders
        internal static ElementBuilder UseService<T>(
            this ElementBuilder builder,
            Action<T, ElementBuilder> configure) where T : class
        {
            var serviceProvider = builder.GetServiceProvider();
            if (serviceProvider == null) return builder;

            var service = serviceProvider.GetService<T>();
            if (service != null)
            {
                configure(service, builder);
            }

            return builder;
        }
    }
}
