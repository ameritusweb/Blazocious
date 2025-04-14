using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazocious.Core.Builder
{
    public static class Element
    {

        public static ElementBuilder Icon(string className) =>
        Element.RawHtml($"<i class='{className}'></i>");

        public static ElementBuilder RawHtml(string html)
        {
            return new ElementBuilder("span")
                .Child(new ElementBuilder("_raw") // Using a dummy tag that won't actually render
                {
                    BuildOverride = builder => builder.AddMarkupContent(0, html)
                });
        }

        public static ElementBuilder Div(string? @class = null) =>
            new ElementBuilder("div").MaybeAttr("class", @class);

        public static ElementBuilder H1(string? text = null) =>
            new ElementBuilder("h1").MaybeText(text);

        public static ElementBuilder H2(string? text = null) =>
            new ElementBuilder("h2").MaybeText(text);

        public static ElementBuilder H3(string? text = null) =>
            new ElementBuilder("h3").MaybeText(text);

        public static ElementBuilder Paragraph(string? text = null) =>
            new ElementBuilder("p").MaybeText(text);

        public static ElementBuilder Span(string? text = null) =>
            new ElementBuilder("span").MaybeText(text);

        public static ElementBuilder Button(string? text = null) =>
            new ElementBuilder("button").MaybeText(text);

        public static ElementBuilder Input(string? type = "text") =>
            new ElementBuilder("input").MaybeAttr("type", type);

        public static ElementBuilder Section(string? @class = null) =>
            new ElementBuilder("section").MaybeAttr("class", @class);

        public static ElementBuilder Article(string? @class = null) =>
            new ElementBuilder("article").MaybeAttr("class", @class);

        // Semantic elements
        public static ElementBuilder Header(string? @class = null) =>
            new ElementBuilder("header").MaybeAttr("class", @class);

        public static ElementBuilder Footer(string? @class = null) =>
            new ElementBuilder("footer").MaybeAttr("class", @class);

        public static ElementBuilder Nav(string? @class = null) =>
            new ElementBuilder("nav").MaybeAttr("class", @class);

        public static ElementBuilder Main(string? @class = null) =>
            new ElementBuilder("main").MaybeAttr("class", @class);

        public static ElementBuilder Aside(string? @class = null) =>
            new ElementBuilder("aside").MaybeAttr("class", @class);
    }
}
