using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("TableDefinitionField_T")]
    public class TableDefinitionField
    {
        [DataProviderResultField("TableDefinitionFieldID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int TableDefinitionFieldID { get; set; }

        [DataProviderResultField("FieldName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string FieldName { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("TableColumnName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string TableColumnName { get; set; }

        [DataProviderResultField("ObjectPropertyName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ObjectPropertyName { get; set; }

        [DataProviderResultField("ObjectPropertyType", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ObjectPropertyType { get; set; }

        [DataProviderResultField("TableDefinitionID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int TableDefinitionID { get; set; }

        [DataProviderResultField("ForeignTableDefinitionFieldID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ForeignTableDefinitionFieldID { get; set; }

        public TableDefinitionField()
        {
            this.TableDefinitionFieldID = 0;
            this.FieldName = "";
            this.Title = "";
            this.TableColumnName = "";
            this.ObjectPropertyName = "";
            this.ObjectPropertyType = "";
            this.TableDefinitionID = 0;
            this.ForeignTableDefinitionFieldID = 0;
        }
    }
}
