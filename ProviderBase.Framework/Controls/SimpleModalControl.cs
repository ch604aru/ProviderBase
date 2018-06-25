using ProviderBase.Framework.Entities;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace ProviderBase.Framework.Controls
{
    public class SimpleModalControl : System.Web.UI.Control, System.Web.UI.INamingContainer
    {
        public WebsitePage WebsitePage { get; set; }

        public SimpleModalControl()
        {
            this.WebsitePage = null;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(this.Render());
        }

        private string Render()
        {
            if (string.IsNullOrEmpty(this.WebsitePage.HtmlModalTemplate) == false)
            {
                return ProviderBase.Web.Utility.GetResourceHtml(this.WebsitePage.HtmlModalTemplate);
            }
            else
            {
                return "";
            }
        }
    }
}
