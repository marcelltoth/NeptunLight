using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;

namespace NeptunLight.Helpers
{
    public static class AngleSharpExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> GetPostbackData(this IDocument doc)
        {
            foreach (IHtmlFormElement form in doc.Forms)
            {
                foreach (IHtmlElement formElement in form.Elements)
                {
                    if(formElement.HasAttribute("name") && formElement.HasAttribute("value"))
                        yield return new KeyValuePair<string, string>(formElement.Attributes["name"].Value, formElement.Attributes["value"].Value);
                }
            }
        }

        public static string GetFirstLineOfText(this IHtmlElement elem)
        {
            return elem.InnerText.Split('\n').First().Trim();
        }
    }
}