using ProviderBase.Data.Providers;
using ProviderBase.Framework.Entities;
using ProviderBase.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ProviderBase.Framework.Modules
{
    public class URLRewriter : IHttpModule
    {
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.URLRewrite);
        }

        protected void URLRewrite(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            Website website = null;
            
            string currentWebsiteHost = "";

            currentWebsiteHost = app.Request.Url.Authority;

            website = DataProvider.SelectSingle<Website>(new Website()
            {
                WebsiteHost = currentWebsiteHost
            });

            if (website == null || website.WebsiteID == 0)
            {
                WebsiteAlias websiteAlias = DataProvider.SelectSingle<WebsiteAlias>(new WebsiteAlias()
                {
                    WebsiteAliasHost = currentWebsiteHost
                });

                if (websiteAlias != null && websiteAlias.WebsiteAliasID > 0)
                {
                    website = DataProvider.SelectSingleFull<Website>(new Website()
                    {
                        WebsiteID = websiteAlias.WebsiteID
                    });
                }
                else
                {
                    return;
                }
            }

            string pageURL = "";
            string[] pageURLSplit = null;
            List<WebsitePage> websitePageList = null;

            websitePageList = DataProvider.Select<WebsitePage>(new WebsitePage()
            {
                WebsiteID = website.WebsiteID
            }, website.WebsiteConnection.ConnectionString);

            pageURLSplit = app.Request.CurrentExecutionFilePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (pageURLSplit?.Count() > 0)
            {
                pageURL = pageURLSplit[0];
            }
            else
            {
                pageURL = "Default.aspx";
            }

            if (websitePageList.Exists(x => x.PageName.ToLower() == pageURL.ToLower()) == false)
            {
                return;
            }

            WebsitePage websitePage = null;

            websitePage = websitePageList.Where(x => x.PageName.ToLower() == pageURL.ToLower()).FirstOrDefault<WebsitePage>();

            if (websitePage != null && websitePage.WebsitePageID > 0)
            {
                string pageURLPath = "";

                pageURLPath = "/" + pageURL + ".aspx";

                if (pageURLSplit.Count() > 1)
                {
                    pageURLPath += "?FirstValue=" + pageURLSplit[1];

                    if (app.Request.QueryString.Count > 0)
                    {
                        pageURLPath += "&" + Utility.ToQueryString(app.Request.QueryString);
                    }
                }

                app.Context.RewritePath(pageURLPath);
            }
            else
            {
                app.Response.Redirect("/");
            }
        }
    }
}
