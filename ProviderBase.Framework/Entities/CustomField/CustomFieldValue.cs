using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    public class CustomFieldValue
    {
        [DataProviderResultField("", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CustomFieldValueID { get; set; }

        [DataProviderResultField("ValueText", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ValueText { get; set; }

        [DataProviderResultField("ValueInt", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ValueInt { get; set; }

        [DataProviderResultField("ValueDecimal", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public decimal ValueDecimal { get; set; }

        [DataProviderResultField("ValueBoolean", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public bool ValueBoolean { get; set; }

        [DataProviderResultField("ValueCustomFieldValueID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ValueCustomFieldValueID { get; set; }

        [DataProviderResultField("ValueDisplay", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ValueDisplay { get; set; }

        [DataProviderResultField("CustomFieldItemID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CustomFieldItemID { get; set; }

        [DataProviderResultField("", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int LinkID { get; set; }

        public CustomFieldValue()
        {
            this.CustomFieldValueID = 0;
            this.ValueText = "";
            this.ValueInt = 0;
            this.ValueDecimal = 0;
            this.ValueBoolean = false;
            this.ValueDisplay = "";
            this.CustomFieldItemID = 0;
            this.LinkID = 0;
        }
    }
}
