using System;

namespace ProviderBase.Data.Entities
{
    public enum DataProviderResultFieldAction
    {
        Select,
        Insert,
        Update,
        Where,
        Delete
    };

    public enum DataProviderKeyType
    {
        None,
        PrimaryKey,
        ForeignKey,
        LookupKey,
        DefaultValue
    };

    public class DataProviderResultField : Attribute
    {
        public string Field { get; set; }

        public DataProviderKeyType KeyType { get; set; }

        public DataProviderResultFieldAction[] Actions { get; set; }

        public DataProviderResultField(string field, params DataProviderResultFieldAction[] actions)
        {
            this.Field = field;
            this.KeyType = DataProviderKeyType.None;
            this.Actions = actions;
        }

        public DataProviderResultField(string field, DataProviderKeyType keyType, params DataProviderResultFieldAction[] actions)
        {
            this.Field = field;
            this.KeyType = keyType;
            this.Actions = actions;
        }
    }
}
