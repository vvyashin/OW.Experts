using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace OW.Experts.WebUI.CompositionRoot.HtmlHelpers
{
    public static class MenuExtensions
    {
        public static MvcHtmlString MenuItem(
            this HtmlHelper htmlHelper,
            string text,
            string action,
            string controller)
        {
            var li = new TagBuilder("li");
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentController = routeData.GetRequiredString("controller");
            if (string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase))
                li.AddCssClass("active");
            li.InnerHtml = htmlHelper.ActionLink(text, action, controller).ToHtmlString();
            return MvcHtmlString.Create(li.ToString());
        }
    }
}