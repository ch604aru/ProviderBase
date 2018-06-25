using ProviderBase.Framework.Entities;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace ProviderBase.Framework.Controls
{
    public abstract class BaseControl : System.Web.UI.Control, System.Web.UI.INamingContainer
    {
        public User User { get; set; }

        public WebsitePageContent websitePageContent { get; set; }

        public BaseControl()
        {
            this.User = new User();
            this.websitePageContent = new WebsitePageContent();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(this.Render());
        }

        protected abstract string Render();
    }
}
