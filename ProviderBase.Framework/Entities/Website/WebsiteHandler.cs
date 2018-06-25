using ProviderBase.Data.Entities;
using System.Collections.Generic;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("WebsiteHandler_T")]
    public class WebsiteHandler
    {
        [DataProviderResultField("WebsiteHandlerID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int WebsiteHandlerID { get; set; }

        [DataProviderResultField("WebsiteID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public int WebsiteID { get; set; }

        [DataProviderResultField("HandlerName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string HandlerName { get; set; }

        [DataProviderResultField("HandlerURL", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string HandlerURL { get; set; }

        [DataProviderResultField("Authenticate", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public bool Authenticate { get; set; }

        public WebsiteHandler()
        {
            this.WebsiteHandlerID = 0;
            this.WebsiteID = 0;
            this.HandlerName = "";
            this.HandlerURL = "";
            this.Authenticate = false;
        }
    }
}
