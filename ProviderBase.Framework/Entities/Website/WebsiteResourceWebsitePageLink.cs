using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("WebsiteResourceWebsitePageLink_T")]
    public class WebsiteResourceWebsitePageLink
    {
        [DataProviderResultField("WebsiteResourceID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int WebsiteResourceID { get; set; }

        [DataProviderResultField("WebsitePageID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int WebsitePageID { get; set; }

        public WebsiteResourceWebsitePageLink()
        {
            this.WebsiteResourceID = 0;
            this.WebsitePageID = 0;
        }
    }
}
