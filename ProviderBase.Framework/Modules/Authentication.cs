using ProviderBase.Data.Providers;
using ProviderBase.Framework.Entities;
using ProviderBase.Framework.Handlers;
using ProviderBase.Framework.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace ProviderBase.Framework.Modules
{
    public class Authentication : IHttpModule
    {
        private Website Website { get; set; }

        private List<WebsitePage> WebsitePageList { get; set; }

        private List<WebsiteHandler> WebsiteHandlerList { get; set; }

        public bool PageRequest { get; set; }

        public void Dispose()
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += new EventHandler(this.AuthenticateRequest);
            context.EndRequest += new EventHandler(this.EndRequest);
        }

        protected void AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;

            string currentWebsiteHost = "";

            currentWebsiteHost = app.Request.Url.Authority;

            this.Website = DataProvider.SelectSingle<Website>(new Website()
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
                    return;
                }
            }

            string pageURL = "";
            string handlerURL = "";
            string[] pageURLSplit = null;

            this.WebsitePageList = DataProvider.Select<WebsitePage>(new WebsitePage()
            {
                WebsiteID = this.Website.WebsiteID
            }, this.Website.WebsiteConnection.ConnectionString);

            pageURLSplit = app.Request.CurrentExecutionFilePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (pageURLSplit?.Count() > 0)
            {
                pageURL = pageURLSplit[0];
            }
            else
            {
                pageURL = "Default.aspx";
            }

            pageURL = pageURL.Replace(".aspx", "");
            handlerURL = (pageURL.Contains('.')) ? "." + pageURL.Split('.').Last() : "";

            this.WebsiteHandlerList = DataProvider.Select<WebsiteHandler>(new WebsiteHandler()
            {
                WebsiteID = this.Website.WebsiteID
            }, this.Website.WebsiteConnection.ConnectionString);

            if (this.WebsitePageList.Exists(x => x.PageName.ToLower() == pageURL.ToLower()) == false
                && this.WebsiteHandlerList.Exists(x => x.HandlerURL.ToLower() == handlerURL.ToLower()) == false)
            {
                return;
            }
            else
            {
                bool releventRequest = false;
                bool authenticateRequest = true;

                WebsitePage websitePage = null;

                websitePage = this.WebsitePageList.Where(x => x.PageName.ToLower() == pageURL.ToLower()).FirstOrDefault<WebsitePage>();

                if (websitePage?.WebsitePageID > 0)
                {
                    releventRequest = true;
                    authenticateRequest = websitePage.Authenticate;
                    this.PageRequest = true;
                }
                else
                {
                    WebsiteHandler websiteHandler = null;

                    websiteHandler = this.WebsiteHandlerList.Where(x => x.HandlerURL.ToLower() == handlerURL.ToLower()).FirstOrDefault<WebsiteHandler>();

                    if (websiteHandler?.WebsiteHandlerID > 0)
                    {
                        releventRequest = true;
                        authenticateRequest = websiteHandler.Authenticate;
                        this.PageRequest = false;
                    }
                }

                if (releventRequest)
                {
                    // websitePage
                    if (app.Request.Cookies["ProviderBaseAuthentication"] != null)
                    {
                        string providerBaseAuthenticationHmac = "";
                        string providerBaseAuthenticationToken = "";

                        if (app.Request.Cookies["ProviderBaseAuthentication"]["Hmac"] != null)
                        {
                            providerBaseAuthenticationHmac = app.Request.Cookies["ProviderBaseAuthentication"]["Hmac"];
                        }

                        if (app.Request.Cookies["ProviderBaseAuthentication"]["Token"] != null)
                        {
                            providerBaseAuthenticationToken = app.Request.Cookies["ProviderBaseAuthentication"]["Token"];
                        }

                        if (string.IsNullOrEmpty(providerBaseAuthenticationHmac) == false && string.IsNullOrEmpty(providerBaseAuthenticationToken) == false)
                        {
                            string token = "";

                            token = ProviderBase.Data.Entities.Encryption.DecryptString(providerBaseAuthenticationToken);

                            if (string.IsNullOrEmpty(token) == false)
                            {
                                string hmac = "";

                                hmac = ProviderBase.Data.Entities.Encryption.HashHMAC(token);

                                if (hmac == providerBaseAuthenticationHmac)
                                {
                                    if (token.Contains("|"))
                                    {
                                        string user = "";
                                        string expiry = "";
                                        string[] tokenSplit = token.Split('|');
                                        long expiryMilliseconds = 0;

                                        user = tokenSplit[0];
                                        expiry = tokenSplit[1];

                                        if (long.TryParse(expiry, out expiryMilliseconds))
                                        {
                                            DateTime expiryDate = new DateTime(1970, 1, 1);

                                            expiryDate.AddMilliseconds(expiryMilliseconds);

                                            if (expiryDate < DateTime.Now)
                                            {
                                                GenericIdentity identitiy = new GenericIdentity(user);
                                                SetPrinciple(new GenericPrincipal(identitiy, null));

                                                // Add 20 minutes timeout
                                                token = user + "|" + (long)(DateTime.Now.AddMinutes(20) - new DateTime(1970, 1, 1)).TotalMilliseconds;
                                                hmac = ProviderBase.Data.Entities.Encryption.HashHMAC(token);
                                                token = ProviderBase.Data.Entities.Encryption.EncryptString(token);

                                                app.Response.Cookies["ProviderBaseAuthentication"]["Hmac"] = hmac;
                                                app.Response.Cookies["ProviderBaseAuthentication"]["Token"] = token;

                                                return; // Authorised
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (authenticateRequest)
                    {
                        app.Response.StatusCode = 401; // Unauthorised
                    }
                }
                else
                {
                    app.Response.StatusCode = 401; // Unauthorised
                }
            }
        }

        protected void EndRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;

            if (app.Response.StatusCode == 401)
            {
                if (PageRequest)
                {
                    if (this.WebsitePageList.Exists(x => x.PageName.ToLower() == "login"))
                    {
                        app.Response.Redirect("/Login");
                    }
                    else
                    {
                        app.Response.Redirect("/");
                    }
                }
            }
        }

        private void SetPrinciple(IPrincipal principle)
        {
            Thread.CurrentPrincipal = principle;

            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principle;
            }
        }
    }
}
