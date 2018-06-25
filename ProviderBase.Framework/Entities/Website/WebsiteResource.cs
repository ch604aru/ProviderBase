using ProviderBase.Data.Entities;
using System.Collections.Generic;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("WebsiteResource_T")]
    public class WebsiteResource
    {
        [DataProviderResultField("WebsiteResourceID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int WebsiteResourceID { get; set; }

        [DataProviderResultField("ResourceLocation", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ResourceLocation { get; set; }

        [DataPropertyDisplay(DataPropertyDisplayType.Description)]
        [DataProviderResultField("ResourceName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ResourceName { get; set; }

        [DataProviderResultField("ResourceType", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ResourceType { get; set; }

        [DataProviderResultSet(1)]
        public List<WebsiteResourceDependency> WebsiteResourceDependency { get; set; }

        public string ResourceFullName
        {
            get
            {
                return this.ResourceLocation + this.ResourceName;
            }
        }

        public WebsiteResource()
        {
            this.WebsiteResourceID = 0;
            this.ResourceLocation = "";
            this.ResourceName = "";
            this.ResourceType = "";
        }
    }
}
