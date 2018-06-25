using ProviderBase.Data.Entities;
using System.Collections.Generic;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("WebsitePage_T")]
    public enum WebsitePageSizeType
    {
        Unassigned = 0,
        Desktop = 1,
        Mobile = 2,
        Tablet = 3
    }

    [DataProviderTable("WebsitePage_T")]
    public class WebsitePage
    {
        [DataProviderResultField("WebsitePageID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int WebsitePageID { get; set; }

        [DataPropertyDisplay(DataPropertyDisplayType.Description)]
        [DataProviderResultField("PageName", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public string PageName { get; set; }

        [DataProviderResultField("PageTitle", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string PageTitle { get; set; }

        [DataProviderResultField("PageLayout", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string PageLayout { get; set; }

        [DataProviderResultField("WebsiteID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public int WebsiteID { get; set; }

        [DataProviderResultField("WebsitePageContentTypeID", KeyType.LookupKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public WebsitePageContentType WebsitePageContentTypeID { get; set; }

        [DataProviderResultField("ModalLayout", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ModalLayout { get; set; }

        [DataProviderResultField("HtmlModalTemplate", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string HtmlModalTemplate { get; set; }

        [DataProviderResultField("Authenticate", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public bool Authenticate { get; set; }

        [DataProviderResultField("WebsitePageSizeTypeID", KeyType.LookupKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public WebsitePageSizeType WebsitePageSizeTypeID { get; set; }

        [DataProviderResultSet(1, "WebsiteResourceWebsitePageLink_T")]
        public List<WebsiteResource> Resource { get; set; }

        [DataProviderResultSet(2)]
        public List<WebsitePageContent> PageContent { get; set; }

        public WebsitePage()
        {
            this.WebsitePageID = 0;
            this.PageName = "";
            this.PageTitle = "";
            this.PageLayout = "";
            this.WebsiteID = 0;
            this.WebsitePageContentTypeID = WebsitePageContentType.Unassigned;
            this.ModalLayout = "";
            this.HtmlModalTemplate = "";
            this.Authenticate = false;
            this.WebsitePageSizeTypeID = WebsitePageSizeType.Unassigned;
            this.Resource = new List<WebsiteResource>();
            this.PageContent = new List<WebsitePageContent>();
        }
    }
}
