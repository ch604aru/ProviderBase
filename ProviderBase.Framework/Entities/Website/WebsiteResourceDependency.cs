using ProviderBase.Data.Entities;
namespace ProviderBase.Framework.Entities
{
    using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
    using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

    [DataProviderTable("WebsiteResourceDependency_T")]
    public class WebsiteResourceDependency
    {
        [DataProviderResultField("WebsiteResourceDependencyID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int WebsiteResourceDependencyID { get; set; }

        [DataProviderResultField("WebsiteResourceID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int WebsiteResourceID { get; set; }

        [DataProviderResultField("DependencyWebsiteResourceID", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int DependencyWebsiteResourceID { get; set; }

        public WebsiteResourceDependency()
        {
            this.WebsiteResourceDependencyID = 0;
            this.WebsiteResourceID = 0;
            this.DependencyWebsiteResourceID = 0;
        }
    }
}
