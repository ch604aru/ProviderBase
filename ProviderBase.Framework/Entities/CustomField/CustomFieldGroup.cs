using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    public class CustomFieldGroup
    {
        [DataProviderResultField("", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CustomFieldGroupID { get; set; }

        [DataProviderResultField("Description", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Description { get; set; }

        public CustomFieldGroup()
        {
            this.CustomFieldGroupID = 0;
            this.Description = "";
        }
    }
}
