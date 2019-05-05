using System.Collections.Generic;
using System.Web.Mvc;
using OW.Experts.WebUI.ViewModels;

namespace OW.Experts.WebUI.Infrastructure
{
    public static class WebViewPageExtensions
    {
        public static string GetErrorMessage(this WebViewPage webPage)
        {
            return (string)webPage.TempData[DataConstants.Error];
        }

        public static string GetSuccessMessage(this WebViewPage webPage)
        {
            return (string)webPage.TempData[DataConstants.Success];
        }

        public static SelectList GetTypeSelectList(this WebViewPage webPage, string selectedNotionTypeId = null)
        {
            var notionTypes = (IReadOnlyCollection<NotionTypeViewModel>)webPage.ViewBag.NotionTypes;

            return new SelectList(notionTypes, nameof(NotionTypeViewModel.Id), nameof(NotionTypeViewModel.Name), selectedNotionTypeId);
        }
    }
}