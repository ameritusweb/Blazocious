using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public static class Element
    {
        private static IServiceProvider? _serviceProvider;

        public static void ConfigureServices(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        internal static T GetRequiredService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        private static ElementBuilder Create(string tag)
        {
            return new ElementBuilder(tag)
                .WithServiceProviderInternal(_serviceProvider!);
        }

        public static ElementBuilder RawHtml(string html)
        {
            return Create("span")
                .Child(new ElementBuilder("_raw")
                {
                    BuildOverride = builder => builder.AddMarkupContent(0, html)
                });
        }

        public static ElementBuilder Icon(string className) =>
            RawHtml($"<i class='{className}'></i>");

        public static ElementBuilder Div(string? @class = null) =>
            Create("div").MaybeAttr("class", @class);

        public static ElementBuilder H1(string? text = null) =>
            Create("h1").MaybeText(text);

        public static ElementBuilder H2(string? text = null) =>
            Create("h2").MaybeText(text);

        public static ElementBuilder H3(string? text = null) =>
            Create("h3").MaybeText(text);

        public static ElementBuilder Paragraph(string? text = null) =>
            Create("p").MaybeText(text);

        public static ElementBuilder Span(string? text = null) =>
            Create("span").MaybeText(text);

        public static ElementBuilder Button(string? text = null) =>
            Create("button").MaybeText(text);

        public static ElementBuilder Input(string? type = "text") =>
            Create("input").MaybeAttr("type", type);

        public static ElementBuilder Ul(string? @class = null) =>
            Create("ul").MaybeAttr("class", @class);

        public static ElementBuilder Li(string? @class = null) =>
            Create("li").MaybeAttr("class", @class);

        public static ElementBuilder Section(string? @class = null) =>
            Create("section").MaybeAttr("class", @class);

        public static ElementBuilder Article(string? @class = null) =>
            Create("article").MaybeAttr("class", @class);

        // Semantic elements
        public static ElementBuilder Header(string? @class = null) =>
            Create("header").MaybeAttr("class", @class);

        public static ElementBuilder Footer(string? @class = null) =>
            Create("footer").MaybeAttr("class", @class);

        public static ElementBuilder Nav(string? @class = null) =>
            Create("nav").MaybeAttr("class", @class);

        public static ElementBuilder Main(string? @class = null) =>
            Create("main").MaybeAttr("class", @class);

        public static ElementBuilder Aside(string? @class = null) =>
            Create("aside").MaybeAttr("class", @class);
    }
}
