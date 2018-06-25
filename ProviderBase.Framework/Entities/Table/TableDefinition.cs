using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("TableDefinition_T")]
    public class TableDefinition : DataProviderPaging
    {
        [DataProviderResultField("TableDefinitionID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int TableDefinitionID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("TableName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string TableName { get; set; }

        [DataProviderResultField("ObjectName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ObjectName { get; set; }

        [DataProviderResultField("AssemblyName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string AssemblyName { get; set; }

        [DataProviderResultField("AssemblyType", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string AssemblyType { get; set; }

        public string AssemblyFullName
        {
            get
            {
                return this.AssemblyType + ", " + this.AssemblyName;
            }
        }

        public TableDefinition()
        {
            this.TableDefinitionID = 0;
            this.Title = "";
            this.TableName = "";
            this.ObjectName = "";
            this.AssemblyName = "";
            this.AssemblyType = "";
        }
    }
}
