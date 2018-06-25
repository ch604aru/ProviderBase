using System;
using System.Web.UI;
using ProviderBase.Framework.Providers;
using System.Web.UI.HtmlControls;
using System.IO;
using ProviderBase.Data.Providers;
using ProviderBase.Framework.Entities;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using FiftyOne.Foundation.Mobile.Detection.Factories;
using FiftyOne.Foundation.Mobile.Detection;
using FiftyOne.Foundation.Mobile.Detection.Entities;

namespace ProviderBase.Framework.Controls
{
    public class LayoutControl : Control, INamingContainer
    {
        private Website Website { get; set; }

        private WebsitePage WebsitePage { get; set; }

        public override Page Page { get; set; }

        private string CurrentPage { get; set; }

        public LayoutControl()
        {
            this.WebsitePage = null;
            this.Page = null;
        }

        public override ControlCollection Controls
        {
            get
            {
                this.EnsureChildControls();
                return base.Controls;
            }
        }

        public override void DataBind()
        {
            base.OnDataBinding(EventArgs.Empty);
            this.Controls.Clear();
            this.ClearChildViewState();
            this.TrackViewState();

            this.CreateControlHierarchy();

            this.ChildControlsCreated = true;
            base.DataBind();
        }

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this.CreateControlHierarchy();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.Controls.Count == 0)
            {
                return;
            }

            this.RenderChildren(writer);
        }

        protected override void OnLoad(EventArgs e)
        {
            string currentWebsiteHost = "";
            WebsitePage websitePageItem = null;
            WebsitePageSizeType websitePageSizeType = WebsitePageSizeType.Desktop;
            Provider fiftyOneDegreesMobileProvider = null;
            
            this.Page = (Page)this.Context.Handler;

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
                    return;
                }
            }

            this.CurrentPage = this.Page.AppRelativeVirtualPath.Replace(".aspx", "").Replace("~/", "");

            fiftyOneDegreesMobileProvider = new Provider(StreamFactory.Create(this.Context.Server.MapPath("App_Data/51Degrees.dat")));

            if (fiftyOneDegreesMobileProvider != null)
            {
                Match fiftyOneDegreesMatch = null;
                
                fiftyOneDegreesMatch = fiftyOneDegreesMobileProvider.Match(this.Context.Request.UserAgent);

                if (fiftyOneDegreesMatch != null)
                {
                    Values fiftyOneDegreesValuesIsMobile = null;

                    fiftyOneDegreesValuesIsMobile = fiftyOneDegreesMatch["IsMobile"];

                    if (fiftyOneDegreesValuesIsMobile != null)
                    {
                        bool isMobile = false;

                        bool.TryParse(fiftyOneDegreesValuesIsMobile.ToString(), out isMobile);

                        websitePageSizeType = (isMobile) ? WebsitePageSizeType.Mobile : WebsitePageSizeType.Desktop;
                    }
                }
            }

            websitePageItem = DataProvider.SelectSingle(new WebsitePage()
            {
                PageName = this.CurrentPage,
                WebsiteID = this.Website.WebsiteID,
                WebsitePageSizeTypeID = websitePageSizeType
            }, this.Website.WebsiteConnection.ConnectionString);

            if (websitePageItem != null && websitePageItem.WebsitePageID > 0)
            {
                this.WebsitePage = DataProvider.SelectSingleOrDefaultFull(new WebsitePage()
                {
                    PageName = this.CurrentPage,
                    WebsiteID = this.Website.WebsiteID,
                    WebsitePageContentTypeID = websitePageItem.WebsitePageContentTypeID,
                    WebsitePageSizeTypeID = websitePageSizeType
                }, this.Website.WebsiteConnection.ConnectionString, "PageName");

                this.LoadPageHeader();
            }
            else
            {
                throw new Exception("No page data found");
            }
        }

        private void CreateControlHierarchy()
        {
            BaseControl control = null;
            Control controlLayout = null;
            Tracking tracking = new Tracking();
            WebsitePage websitePageItem = null;

            try
            {
                if (this.WebsitePage != null)
                {
                    if (string.IsNullOrEmpty(this.WebsitePage.PageName))
                    {
                        websitePageItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<WebsitePage>(new WebsitePage()
                        {
                            PageName = this.CurrentPage,
                            WebsiteID = this.Website.WebsiteID
                        }, this.Website.WebsiteConnection.ConnectionString);
                    }
                    else
                    {
                        websitePageItem = this.WebsitePage;
                    }

                    this.Page.Header.Title = websitePageItem.PageTitle;

                    if (string.IsNullOrEmpty(websitePageItem.PageLayout) == false)
                    {
                        controlLayout = this.Page.LoadControl(@"/Resource/Layouts/" + websitePageItem.PageLayout);

                        foreach (WebsitePageContent pageContent in this.WebsitePage.PageContent)
                        {
                            WebsiteControl websiteControl = null;
                            Control controlLayoutContent = null;

                            websiteControl = DataProvider.SelectSingle(new WebsiteControl()
                            {
                                WebsiteControlID = pageContent.WebsiteControlID
                            }, this.Website.WebsiteConnection.ConnectionString);

                            controlLayoutContent = controlLayout.FindControl("ProviderBaseFramework_" + pageContent.ContentLayoutID);

                            if (controlLayoutContent != null)
                            {
                                try
                                {
                                    control = (BaseControl)Activator.CreateInstance(websiteControl.AssemblyName, websiteControl.AssemblyType).Unwrap();
                                }
                                catch (Exception ex)
                                {
                                    HtmlGenericControl genericControl = new HtmlGenericControl("div");
                                    genericControl.InnerText = "No control found";
                                    controlLayoutContent.Controls.Add(genericControl);

                                    tracking = new Tracking();

                                    tracking.Message = ex.Message;
                                    tracking.Status = -1;
                                    tracking.UserID = 0;
                                    tracking.AssemblyName = "ProviderBase.Framework";
                                    tracking.AssemblyType = "ProviderBase.Framework.Entities.ContentControl";
                                    tracking.MethodName = ex.TargetSite.Name;
                                    tracking.TrackingLevelID = TrackingLevel.Error;
                                    tracking.TrackingTypeID = TrackingType.System;
                                    tracking.StackTrace = ex.StackTrace;

                                    if (tracking.TrackingInsertLevel())
                                    {
                                        tracking.TrackingID = ProviderBase.Data.Providers.DataProvider.Insert(tracking, this.Website.WebsiteConnection.ConnectionString);
                                    }
                                }

                                control.websitePageContent = pageContent;

                                controlLayoutContent.Controls.Add(control);
                            }

                            tracking = new Tracking();
                            tracking.Message = websiteControl.AssemblyFullName + " complete";
                            tracking.Status = 1;
                            tracking.UserID = 0;
                            tracking.AssemblyName = "ProviderBase.Framework";
                            tracking.AssemblyType = "ProviderBase.Framework.Entities.ContentControl";
                            tracking.MethodName = "Load";
                            tracking.TrackingLevelID = TrackingLevel.Success;
                            tracking.TrackingTypeID = TrackingType.System;

                            if (tracking.TrackingInsertLevel())
                            {
                                tracking.TrackingID = ProviderBase.Data.Providers.DataProvider.Insert(tracking, this.Website.WebsiteConnection.ConnectionString);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracking = new Tracking();

                tracking.Message = ex.Message;
                tracking.Status = -1;
                tracking.UserID = 0;
                tracking.AssemblyName = "ProviderBase.Framework";
                tracking.AssemblyType = "ProviderBase.Framework.Entities.ContentControl";
                tracking.MethodName = ex.TargetSite.Name;
                tracking.TrackingLevelID = TrackingLevel.Error;
                tracking.TrackingTypeID = TrackingType.System;
                tracking.StackTrace = ex.StackTrace;

                if (tracking.TrackingInsertLevel())
                {
                    tracking.TrackingID = ProviderBase.Data.Providers.DataProvider.Insert(tracking, this.Website.WebsiteConnection.ConnectionString);
                }
            }

            if (controlLayout != null)
            {
                this.Controls.Add(controlLayout);
            }

        }

        private void LoadPageHeader()
        {
            bool minifyMode = false;
            List<WebsiteResource> websiteResourceLoaded = new List<WebsiteResource>();
            List<WebsiteResource> websiteResourceNotLoaded = new List<WebsiteResource>();

            minifyMode = this.Website.GetSettingValue<bool>("MinifyMode", false);

            this.WebsitePage.Resource = this.WebsitePage.Resource.OrderBy(x => x.ResourceType).ToList();

            foreach (WebsiteResource resource in this.WebsitePage.Resource)
            {
                LoadResourceDependency(resource, ref websiteResourceLoaded, ref websiteResourceNotLoaded, minifyMode, false);
            }

            while (websiteResourceNotLoaded.Count > 0)
            {
                for (int i = (websiteResourceNotLoaded.Count - 1); i >= 0; i--)
                {
                    LoadResourceDependency(websiteResourceNotLoaded[i], ref websiteResourceLoaded, ref websiteResourceNotLoaded, minifyMode, true);
                }
            }

            base.Page.Header.Controls.Add(new LiteralControl(Environment.NewLine));
        }

        private void LoadResourceDependency(WebsiteResource websiteResource, ref List<WebsiteResource> websiteResourceLoaded, ref List<WebsiteResource> websiteResourceNotLoaded, bool minifyMode, bool secondPass)
        {
            List<WebsiteResource> resourceLoaded = websiteResourceLoaded;
            List<WebsiteResource> resourceNotLoaded = websiteResourceNotLoaded;

            WebsiteResource resourceTemp = DataProvider.SelectSingleFull(new WebsiteResource()
            {
                WebsiteResourceID = websiteResource.WebsiteResourceID
            }, this.Website.WebsiteConnection.ConnectionString);

            if ((resourceTemp.WebsiteResourceDependency == null || resourceTemp.WebsiteResourceDependency.Count == 0)
                || (resourceTemp.WebsiteResourceDependency.Exists(x => resourceLoaded.Any(y => y.WebsiteResourceID == x.DependencyWebsiteResourceID))))
            {
                // Load
                LoadResource(resourceTemp, minifyMode);

                websiteResourceLoaded.Add(websiteResource);

                if (websiteResourceNotLoaded.Exists(x => x.WebsiteResourceID == websiteResource.WebsiteResourceID))
                {
                    websiteResourceNotLoaded.Remove(websiteResourceNotLoaded.Single(x => x.WebsiteResourceID == resourceTemp.WebsiteResourceID));
                }
            }
            else if (secondPass == true && resourceTemp.WebsiteResourceDependency.Exists(x => resourceNotLoaded.Any(y => y.WebsiteResourceID == x.DependencyWebsiteResourceID)) == false)
            {
                bool test = resourceTemp.WebsiteResourceDependency.Exists(x => resourceNotLoaded.Any(y => y.WebsiteResourceID == x.DependencyWebsiteResourceID));

                List<WebsiteResourceDependency> websiteResourceNotAvailable = resourceTemp.WebsiteResourceDependency.Where(x => resourceNotLoaded.Any(y => y.WebsiteResourceID != x.DependencyWebsiteResourceID)).ToList<WebsiteResourceDependency>();

                foreach (WebsiteResourceDependency dependency in websiteResourceNotAvailable)
                {
                    resourceTemp = DataProvider.SelectSingle(new WebsiteResource()
                    {
                        WebsiteResourceID = dependency.DependencyWebsiteResourceID
                    }, this.Website.WebsiteConnection.ConnectionString);

                    // Load
                    LoadResource(resourceTemp, minifyMode);

                    websiteResourceLoaded.Add(resourceTemp);

                    if (websiteResourceNotLoaded.Exists(x => x.WebsiteResourceID == resourceTemp.WebsiteResourceID))
                    {
                        websiteResourceNotLoaded.Remove(websiteResourceNotLoaded.Single(x => x.WebsiteResourceID == resourceTemp.WebsiteResourceID));
                    }
                }
            }
            else if (websiteResourceNotLoaded.Exists(x => x.WebsiteResourceID == websiteResource.WebsiteResourceID) == false)
            {
                // Not load
                websiteResourceNotLoaded.Add(websiteResource);
            }
        }

        private void LoadResource(WebsiteResource resource, bool minifyMode)
        {
            long version = 0;

            base.Page.Header.Controls.Add(new LiteralControl(Environment.NewLine));
            
            version = (long)(File.GetLastWriteTimeUtc(Context.Server.MapPath(resource.ResourceFullName)) - new DateTime(1970, 1, 1)).TotalMilliseconds;

            if (resource.ResourceType == "text/javascript")
            {
                HtmlGenericControl scriptControl = new HtmlGenericControl("script");

                if (minifyMode && resource.ResourceName.Contains("min") == false)
                {
                    if (File.Exists(Context.Server.MapPath(resource.ResourceFullName.Replace(".js", ".min.js"))))
                    {
                        resource.ResourceName = resource.ResourceName.Replace(".js", ".min.js");

                        scriptControl.Attributes["type"] = resource.ResourceType;
                        scriptControl.Attributes["src"] = resource.ResourceFullName + "?v=" + version;

                        this.Page.Header.Controls.Add(scriptControl);
                    }
                }
                else
                {
                    if (File.Exists(Context.Server.MapPath(resource.ResourceFullName)))
                    {
                        scriptControl.Attributes["type"] = resource.ResourceType;
                        scriptControl.Attributes["src"] = resource.ResourceFullName + "?v=" + version;

                        this.Page.Header.Controls.Add(scriptControl);
                    }
                }
            }
            else if (resource.ResourceType == "text/css")
            {
                HtmlLink scriptControl = new HtmlLink();

                if (minifyMode && resource.ResourceName.Contains("min") == false)
                {
                    if (File.Exists(Context.Server.MapPath(resource.ResourceFullName.Replace(".css", ".min.css"))))
                    {
                        resource.ResourceName = resource.ResourceName.Replace(".css", ".min.css");

                        scriptControl.Attributes["type"] = resource.ResourceType;
                        scriptControl.Attributes["href"] = resource.ResourceFullName + "?v=" + version;
                        scriptControl.Attributes["rel"] = "stylesheet";

                        this.Page.Header.Controls.Add(scriptControl);
                    }
                }
                else
                {
                    if (File.Exists(Context.Server.MapPath(resource.ResourceFullName)))
                    {
                        scriptControl.Attributes["type"] = resource.ResourceType;
                        scriptControl.Attributes["href"] = resource.ResourceFullName + "?v=" + version;
                        scriptControl.Attributes["rel"] = "stylesheet";

                        this.Page.Header.Controls.Add(scriptControl);
                    }
                }
            }
        }
    }
}
