using ProviderBase.Data.Entities;
using System;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("ReportFIeldEventType_T")]
    public enum ReportFIeldEventType
    {
        Unassigned = 0,
        OnClick = 1,
        OnMouseOver = 2,
        OnMouseOut = 3,
        a = 4
    }

    [DataProviderTable("ReportField_T")]
    public class ReportField
    {
        [DataProviderResultField("ReportFieldID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int ReportFieldID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("TableDefinitionFieldID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int TableDefinitionFieldID { get; set; }

        [DataProviderResultField("CustomFieldItemID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int CustomFieldItemID { get; set; }

        [DataProviderResultField("ReportID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public int ReportID { get; set; }

        [DataProviderResultField("SortOrder", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int SortOrder { get; set; }

        [DataProviderResultField("Class", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Class { get; set; }

        [DataProviderResultField("Style", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Style { get; set; }

        [DataProviderResultField("MediaID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultField("FieldName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string FieldName { get; set; }

        [DataProviderResultField("ReportFIeldEventTypeID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public ReportFIeldEventType ReportFIeldEventTypeID { get; set; }

        [DataProviderResultField("FieldEvent", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string FieldEvent { get; set; }

        [DataProviderResultField("ContainerWidth", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ContainerWidth { get; set; }

        public ReportField()
        {
            this.ReportFieldID = 0;
            this.Title = "";
            this.TableDefinitionFieldID = 0;
            this.CustomFieldItemID = 0;
            this.ReportID = 0;
            this.SortOrder = 0;
            this.Class = "";
            this.Style = "";
            this.MediaID = 0;
            this.FieldName = "";
            this.ReportFIeldEventTypeID = ReportFIeldEventType.Unassigned;
            this.FieldEvent = "";
            this.ContainerWidth = 0;
        }
    }
}
