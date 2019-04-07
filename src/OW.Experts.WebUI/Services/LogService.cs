using System;

namespace OW.Experts.WebUI.Services
{
    public class LogService
    {
        public virtual void Log(Exception exception)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
        }
    }
}