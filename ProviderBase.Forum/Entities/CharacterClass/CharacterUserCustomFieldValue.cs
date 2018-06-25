using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterUserCustomFieldValue_T")]
    public class CharacterUserCustomFieldValue
    {
        [DataProviderResultField("CharacterUserCustomFieldValueID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CharacterUserCustomFieldValueID { get; set; }

        [DataProviderResultField("ValueText", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ValueText { get; set; }

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

        [DataProviderResultField("CharacterUserID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CharacterUserID { get; set; }

        public CharacterUserCustomFieldValue()
        {
            this.CharacterUserCustomFieldValueID = 0;
            this.ValueText = "";
            this.ValueDecimal = 0;
            this.ValueBoolean = false;
            this.ValueDisplay = "";
            this.CustomFieldItemID = 0;
            this.CharacterUserID = 0;
        }
    }
}
