using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("WebsiteControl_T")]
    public class WebsiteControl
    {
        [DataProviderResultField("WebsiteControlID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int WebsiteControlID { get; set; }

        [DataPropertyDisplay(DataPropertyDisplayType.Description)]
        [DataProviderResultField("WebsiteControlName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string WebsiteControlName { get; set; }

        [DataProviderResultField("AssemblyName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string AssemblyName { get; set; }

        [DataProviderResultField("AssemblyType", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string AssemblyType { get; set; }

        public string AssemblyFullName
        {
            get
            {
                return this.AssemblyName + "," + this.AssemblyType;
            }
        }

        public WebsiteControl()
        {
            this.WebsiteControlID = 0;
            this.WebsiteControlName = "";
            this.AssemblyName = "";
            this.AssemblyType = "";
        }
    }
}
