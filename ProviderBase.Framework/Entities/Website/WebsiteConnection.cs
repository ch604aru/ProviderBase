using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("WebsiteConnection_T")]
    public class WebsiteConnection
    {
        [DataProviderResultField("WebsiteConnectionID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int WebsiteConnectionID { get; set; }

        [DataPropertyDisplay(DataPropertyDisplayType.Description)]
        [DataProviderResultField("ConnectionStringName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ConnectionStringName { get; set; }

        [DataProviderResultField("ConnectionString", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ConnectionString { get; set; }

        public WebsiteConnection()
        {
            this.WebsiteConnectionID = 0;
            this.ConnectionString = "";
        }
    }
}
