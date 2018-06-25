using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("WebsiteSetting_T")]
    public class WebsiteSetting
    {
        [DataProviderResultField("WebsiteSettingID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int WebsiteSettingID { get; set; }

        [DataProviderResultField("WebsiteID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public int WebsiteID { get; set; }

        [DataPropertyDisplay(DataPropertyDisplayType.Description)]
        [DataProviderResultField("WebsiteSettingName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string WebsiteSettingName { get; set; }

        [DataProviderResultField("WebsiteSettingValue", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string WebsiteSettingValue { get; set; }

        public WebsiteSetting()
        {
            this.WebsiteSettingID = 0;
            this.WebsiteSettingName = "";
            this.WebsiteSettingValue = "";
        }
    }
}
