using System.Collections.Generic;
using System.Web.Mvc;

namespace WebUI.ViewModels.SessionHistory
{
    public class SelectSessionView
    {
        public IEnumerable<SelectListItem> SessionSelectList { get; set; }
        public int SessionId { get; set; }
    }
}