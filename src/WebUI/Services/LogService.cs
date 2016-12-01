using System;

namespace WebUI.Services
{
    public class LogService
    {
        public virtual void Log(Exception exception)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
        }
    }
}