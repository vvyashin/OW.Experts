using System.Collections.Generic;
using System.Web.Mvc;
using Domain;

namespace WebUI.ViewModels.SessionHistory
{
    public class SemanticNetworkOfSessionViewModel
    {
        public IEnumerable<SelectListItem> SessionSelectList { get; set; }
        public int SessionId { get; set; }
        public SemanticNetworkReadModel SemanticNetwork { get; set; }
        public SessionOfExperts SessionOfExperts { get; set; }
    }
}