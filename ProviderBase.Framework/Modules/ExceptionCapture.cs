using ProviderBase.Framework.Entities;
using ProviderBase.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ProviderBase.Framework.Modules
{
    public class ExceptionCapture : IHttpModule
    {
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.Error += new EventHandler(OnExceptionCapture);
        }

        public void OnExceptionCapture(object sender, EventArgs args)
        {
            HttpContext context = null;
            Exception exception = null;
            Tracking tracking = null;
            string errorInfo = "";

            context = HttpContext.Current;

            exception = context.Server.GetLastError();

            errorInfo = ProviderBase.Web.Utility.FormatExceptionForHtml(context, exception);

            tracking = new Tracking();

            tracking.Message = exception.Message;
            tracking.Status = -1;
            tracking.UserID = 0;
            tracking.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            tracking.AssemblyType = this.GetType().FullName;
            tracking.MethodName = exception.TargetSite.Name;
            tracking.TrackingLevelID = TrackingLevel.Error;
            tracking.TrackingTypeID = TrackingType.System;
            tracking.StackTrace = exception.StackTrace;

            if (tracking.TrackingInsertLevel())
            {
                tracking.TrackingID = ProviderBase.Data.Providers.DataProvider.Insert(tracking);
            }

            context.Response.Write(errorInfo);

            context.Server.ClearError();
        }
    }
}
