using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using ProviderBase.Framework.Providers;
using ProviderBase.Data.Providers;
using ProviderBase.Framework.Entities;

namespace ProviderBase.Framework.Handlers
{
    public class BaseHandler : IHttpAsyncHandler
    {
        private Tracking Tracking { get; set; }
        public bool IsReusable { get { return false; } }
        protected HttpContext Context { get; set; }
        protected AjaxResult AjaxResult { get; set; }
        protected string CommandRequest { get; set; }
        protected Dictionary<string, string> RequestParameters { get; set; }
        protected User User { get; set; }
        protected Website Website { get; set; }


        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callBack, object extraData)
        {
            ProviderFramework providerFramework = new ProviderFramework();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string currentWebsiteHost = "";
            int userID = 0;

            this.Tracking = new Tracking();
            this.Tracking.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            this.Tracking.AssemblyType = this.GetType().FullName;

            this.AjaxResult = new AjaxResult(callBack, context, extraData);
            this.Context = context;

            currentWebsiteHost = this.Context.Request.Url.Authority;

            this.Website = DataProvider.SelectSingleFull<Website>(new Website()
            {
                WebsiteHost = currentWebsiteHost
            });

            if (this.Website == null || this.Website.WebsiteID == 0)
            {
                WebsiteAlias websiteAlias = DataProvider.SelectSingle<WebsiteAlias>(new WebsiteAlias()
                {
                    WebsiteAliasHost = currentWebsiteHost
                });

                if (websiteAlias != null && websiteAlias.WebsiteAliasID > 0)
                {
                    this.Website = DataProvider.SelectSingleFull<Website>(new Website()
                    {
                        WebsiteID = websiteAlias.WebsiteID
                    });
                }
                else
                {
                    callBack(this.AjaxResult);
                    return this.AjaxResult;
                }
            }

            this.RequestParameters = ProviderBase.Framework.Utility.GetQueryAndFormValues();
            this.CommandRequest = ProviderBase.Framework.Utility.GetQueryValue<string>("Command", "");

            if (Int32.TryParse(this.Context.User.Identity.Name, out userID) == true)
            {
                this.User = ProviderBase.Data.Providers.DataProvider.SelectSingle<User>(new User()
                {
                    UserID = userID
                }, this.Website.WebsiteConnection.ConnectionString);
            }
            else
            {
                this.User = new User();
            }

            this.Tracking.MethodName = CommandRequest;
            this.Tracking.UserID = this.User.UserID;
            this.Tracking.RequestData = serializer.Serialize(ProviderBase.Framework.Utility.GetQueryAndFormValues());

            this.ProcessRequest(context);

            callBack(this.AjaxResult);

            return this.AjaxResult;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            string serializedAjaxResult = "";
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            serializedAjaxResult = serializer.Serialize(this.AjaxResult);

            this.Tracking.Status = 1;
            this.Tracking.Message = CommandRequest + " request completed";
            this.Tracking.ReturnData = serializedAjaxResult;
            this.Tracking.TrackingLevelID = TrackingLevel.Success;
            this.Tracking.TrackingTypeID = TrackingType.User;

            if (this.Tracking.TrackingInsertLevel())
            {
                this.Tracking.TrackingID = ProviderBase.Data.Providers.DataProvider.Insert(this.Tracking, this.Website.WebsiteConnection.ConnectionString);
            }

            Context.Response.Write(serializedAjaxResult);
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            Type thisType = this.GetType();
            MethodInfo methodRequest = thisType.GetMethod(this.CommandRequest);

            if (methodRequest != null)
            {
                try
                {
                    methodRequest.Invoke(this, null);

                    this.AjaxResult.Action = ProviderBase.Framework.Utility.GetFormValue<string>("Action", "");
                }
                catch (Exception ex)
                {
                    string errorInfo = "";
                    Tracking tracking = null;

                    errorInfo = ProviderBase.Web.Utility.FormatExceptionForHtml(this.Context, ex);

                    tracking = new Tracking();

                    tracking.Message = ex.Message;
                    tracking.Status = -1;
                    tracking.UserID = 0;
                    tracking.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                    tracking.AssemblyType = this.GetType().FullName;
                    tracking.MethodName = ex.TargetSite.Name;
                    tracking.TrackingLevelID = TrackingLevel.Error;
                    tracking.TrackingTypeID = TrackingType.System;
                    tracking.StackTrace = ex.StackTrace;

                    if (tracking.TrackingInsertLevel())
                    {
                        tracking.TrackingID = ProviderBase.Data.Providers.DataProvider.Insert(tracking, this.Website.WebsiteConnection.ConnectionString);
                    }

                    this.AjaxResult.Data.Add(errorInfo);
                    this.AjaxResult.Message = "Error: " + ex.Message;
                    this.AjaxResult.Status = AjaxResultStatus.Exception;
                }
            }
            else
            {
                this.AjaxResult.Data.Add(ProviderBase.Web.Utility.FormatExceptionForHtml(Context, new Exception("Command not found")));
                this.AjaxResult.Message = "Command not found";
                this.AjaxResult.Status = AjaxResultStatus.Exception;
            }
        }
    }
}
