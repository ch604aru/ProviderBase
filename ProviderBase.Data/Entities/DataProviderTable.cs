using System;

namespace ProviderBase.Data.Entities
{
    public class DataProviderTable : Attribute
    {
        public string Table { get; set; }

        public DataProviderTable(string table)
        {
            this.Table = table;
        }
    }
}
