using System;
using Domain;

namespace WebUI.ViewModels.Admin
{
    public class SessionViewModel
    {
        public string BaseNotion { get; set; }

        public DateTime StartTime { get; set; }

        public SessionPhase CurrentPhase { get; set; }
    }
}