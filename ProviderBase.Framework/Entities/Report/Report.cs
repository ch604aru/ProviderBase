using ProviderBase.Data.Entities;
using System;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("Report_T")]
    public class Report
    {
        [DataProviderResultField("ReportID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int ReportID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("SortOrder", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int SortOrder { get; set; }

        [DataProviderResultField("ParentID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ParentID { get; set; }

        [DataProviderResultField("GUID", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public Guid GUID { get; set; }

        [DataProviderResultField("Class", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Class { get; set; }

        [DataProviderResultField("Style", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Style { get; set; }

        [DataProviderResultField("QueryID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int QueryID { get; set; }

        public Report()
        {
            this.ReportID = 0;
            this.Title = "";
            this.SortOrder = 0;
            this.ParentID = 0;
            this.GUID = new Guid();
            this.Class = "";
            this.Style = "";
            this.QueryID = 0;
        }
    }
}
