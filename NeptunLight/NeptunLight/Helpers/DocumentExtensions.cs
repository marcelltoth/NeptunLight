using System.Collections.Generic;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;

namespace NeptunLight.Helpers
{
    public static class DocumentExtensions
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
    }
}