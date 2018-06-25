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

namespace ProviderBase.Framework.Controls
{
    public class ModalControl : Control, INamingContainer
    {
        private Website Website { get; set; }

        private List<WebsitePage> WebsitePageList { get; set; }

        public override Page Page { get; set; }

        public ModalControl()
        {
            this.WebsitePageList = null;
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
            string currentPage = "";
            string currentWebsiteHost = "";

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

            currentPage = this.Page.AppRelativeVirtualPath.Replace(".aspx", "").Replace("~/", "");
            this.WebsitePageList = DataProvider.SelectOrDefault<WebsitePage>(new WebsitePage()
            {
                PageName = currentPage,
                WebsiteID = this.Website.WebsiteID
            }, this.Website.WebsiteConnection.ConnectionString, "PageName");
        }

        private void CreateControlHierarchy()
        {
            WebsitePage websitePage = null;
            Control controlLayout = null;
            Control controlLayoutContent = null;
            string currentPage = "";

            currentPage = this.Page.AppRelativeVirtualPath.Replace(".aspx", "").Replace("~/", "");
            websitePage = DataProvider.SelectSingle<WebsitePage>(new WebsitePage()
            {
                PageName = currentPage,
                WebsiteID = this.Website.WebsiteID
            }, this.Website.WebsiteConnection.ConnectionString);

            if (string.IsNullOrEmpty(websitePage.ModalLayout) == false)
            {
                controlLayout = this.Page.LoadControl(@"/Resource/Layouts/" + websitePage.ModalLayout);

                controlLayoutContent = controlLayout.FindControl("ProviderBaseFramework_Modal");

                if (controlLayoutContent != null)
                {
                    List<string> loadedTemplates = null;

                    loadedTemplates = new List<string>();

                    foreach (WebsitePage websitePageItem in this.WebsitePageList)
                    {
                        if (string.IsNullOrEmpty(websitePageItem.HtmlModalTemplate) == false && loadedTemplates.Contains(websitePageItem.HtmlModalTemplate) == false)
                        {
                            SimpleModalControl simpleModalControl = null;

                            simpleModalControl = new SimpleModalControl();
                            simpleModalControl.WebsitePage = websitePageItem;

                            controlLayoutContent.Controls.Add(simpleModalControl);

                            loadedTemplates.Add(websitePageItem.HtmlModalTemplate);
                        }
                    }

                    this.Controls.Add(controlLayoutContent);
                }
            }
        }
    }
}
