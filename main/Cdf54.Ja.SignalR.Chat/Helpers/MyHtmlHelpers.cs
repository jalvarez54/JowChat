using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace JA.UTILS.Helpers
{
    public static class MyHtmlHelpers
    {
        // http://www.itorian.com/2012/10/html-helper-for-image-htmlimage.html
        public static MvcHtmlString Image(this HtmlHelper helper, string imgClass, string imgSrc, string imgTitle, string imgAlt, string imgBorder)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("class", imgClass);
            builder.MergeAttribute("src", imgSrc);
            builder.MergeAttribute("title", imgTitle);
            builder.MergeAttribute("alt", imgAlt);
            builder.MergeAttribute("border", imgBorder);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }
    }

}