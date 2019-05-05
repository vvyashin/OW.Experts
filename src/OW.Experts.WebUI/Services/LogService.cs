using System;
using Elmah;

namespace OW.Experts.WebUI.Services
{
    public class LogService
    {
        public virtual void Log(Exception exception)
        {
            ErrorSignal.FromCurrentContext().Raise(exception);
        }
    }
}