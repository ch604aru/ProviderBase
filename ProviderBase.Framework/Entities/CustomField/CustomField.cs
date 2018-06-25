using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("CustomField_T")]
    public class CustomField : DataProviderPaging
    {
        [DataProviderResultField("CustomFieldID", KeyType.PrimaryKey, FieldAction.Where, FieldAction.Select)]
        public int CustomFieldID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        public CustomField()
        {
            this.CustomFieldID = 0;
            this.Title = "";
        }
    }
}
