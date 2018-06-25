using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    public enum WebsitePageContentType
    {
        Unassigned = 0,
        Type1 = 1,
        Type2 = 2
    }

    [DataProviderTable("WebsitePageContent_T")]
    public class WebsitePageContent
    {
        [DataProviderResultField("WebsitePageContentID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int WebsitePageContentID { get; set; }

        [DataProviderResultField("WebsitePageID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int WebsitePageID { get; set; }

        [DataProviderResultField("WebsiteID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public int WebsiteID { get; set; }

        [DataProviderResultField("WebsiteControlID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int WebsiteControlID { get; set; }

        [DataPropertyDisplay(DataPropertyDisplayType.Description)]
        [DataProviderResultField("TemplateFile", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string TemplateFile { get; set; }

        [DataProviderResultField("XMLFile", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string XMLFile { get; set; }

        [DataProviderResultField("TokenFile", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string TokenFile { get; set; }

        [DataProviderResultField("ContentLayoutID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ContentLayoutID { get; set; }

        public WebsitePageContent()
        {
            this.WebsitePageContentID = 0;
            this.WebsitePageID = 0;
            this.WebsiteID = 0;
            this.WebsiteControlID = 0;
            this.TemplateFile = "";
            this.XMLFile = "";
            this.TokenFile = "";
            this.ContentLayoutID = 0;
        }
    }
}
