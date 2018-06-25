using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("FormBuilderField_T")]
    public class FormBuilderField
    {
        [DataProviderResultField("FormBuilderFieldID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int FormBuilderFieldID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("TableDefinitionFieldID", KeyType.ForeignKey, FieldAction.Where, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int TableDefinitionFieldID { get; set; }

        [DataProviderResultField("CustomFieldItemID", KeyType.ForeignKey, FieldAction.Where, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int CustomFieldItemID { get; set; }

        [DataProviderResultField("ObjectDefinitionID", KeyType.ForeignKey, FieldAction.Where, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ObjectDefinitionID { get; set; }

        [DataProviderResultField("FormBuilderID", KeyType.ForeignKey, FieldAction.Where, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int FormBuilderID { get; set; }

        [DataProviderResultField("FormBuilderFieldValidationID", KeyType.ForeignKey, FieldAction.Where, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int FormBuilderFieldValidationID { get; set; }

        public FormBuilderField()
        {
            this.FormBuilderFieldID = 0;
            this.Title = "";
            this.TableDefinitionFieldID = 0;
            this.CustomFieldItemID = 0;
            this.ObjectDefinitionID = 0;
            this.FormBuilderID = 0;
            this.FormBuilderFieldValidationID = 0;
        }
    }
}
