using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("WebsiteAlias_T")]
    public class WebsiteAlias
    {
        [DataProviderResultField("WebsiteAliasID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int WebsiteAliasID { get; set; }

        [DataProviderResultField("WebsiteID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int WebsiteID { get; set; }

        [DataPropertyDisplay(DataPropertyDisplayType.Description)]
        [DataProviderResultField("WebsiteAliasHost", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public string WebsiteAliasHost { get; set; }

        public WebsiteAlias()
        {
            this.WebsiteAliasID = 0;
            this.WebsiteID = 0;
            this.WebsiteAliasHost = "";
        }
    }
}
