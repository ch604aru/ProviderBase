using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    class CustomFieldLink
    {
        [DataProviderResultField("", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CustomFieldGroupID { get; set; }

        [DataProviderResultField("CustomFieldID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CustomFieldID { get; set; }

        public CustomFieldLink()
        {
            this.CustomFieldGroupID = 0;
            this.CustomFieldID = 0;
        }
    }
}
