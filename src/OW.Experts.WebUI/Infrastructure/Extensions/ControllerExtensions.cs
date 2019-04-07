using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebUI.ViewModels;

namespace WebUI.Infrastructure
{
    public static class ControllerExtensions
    {
        public static void Fail(this ControllerBase controller, string failMessage)
        {
            controller.TempData[DataConstants.Error] = failMessage;
        }

        public static void Success(this ControllerBase controller, string successMessage)
        {
            controller.TempData[DataConstants.Success] = successMessage;
        }

        public static void PopulateNotionTypes(this ControllerBase controller, IReadOnlyCollection<NotionTypeViewModel> notionTypes)
        {
            controller.ViewBag.NotionTypes = notionTypes;
        }
    }
}
