using System;

namespace ProviderBase.Data.Entities
{
    public class DataProviderResultSet : Attribute
    {
        public int ResultSetID { get; set; }

        public string LinkTable { get; set; }

        public DataProviderResultSet(int resultSet)
        {
            this.ResultSetID = resultSet;
            this.LinkTable = "";
        }

        public DataProviderResultSet(int resultSet, string linkTable)
        {
            this.ResultSetID = resultSet;
            this.LinkTable = linkTable;
        }
    }
}
