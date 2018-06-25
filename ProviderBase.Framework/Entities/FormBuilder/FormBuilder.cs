using ProviderBase.Data.Entities;
using System;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("FormBuilderSubmitType_T")]
    public enum FormBuilderSubmitType
    {
        Unassigned = 0,
        Insert = 1,
        Update = 2,
        Delete = 3
    }

    public enum FormBuilderDisplayType
    {
        Unassigned = 0,
        Display = 1,
        Edit = 2
    }

    [DataProviderTable("FormBuilder_T")]
    public class FormBuilder
    {
        [DataProviderResultField("FormBuilderID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int FormBuilderID { get; set; }

        [DataProviderResultField("CreateDate", FieldAction.Select, FieldAction.Insert)]
        public DateTime CreateDate { get; set; }

        [DataProviderResultField("ModifyDate", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public DateTime ModifyDate { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("FormBuilderSubmitTypeID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public FormBuilderSubmitType FormBuilderSubmitTypeID { get; set; }

        [DataProviderResultField("SortOrder", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int SortOrder { get; set; }

        [DataProviderResultField("ParentID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ParentID { get; set; }

        [DataProviderResultField("GUID", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public Guid GUID { get; set; }

        [DataProviderResultField("FormBuilderDisplayTypeID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public FormBuilderDisplayType FormBuilderDisplayTypeID { get; set; }

        public FormBuilder()
        {
            this.FormBuilderID = 0;
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;
            this.Title = "";
            this.FormBuilderSubmitTypeID = FormBuilderSubmitType.Unassigned;
            this.SortOrder = 0;
            this.ParentID = 0;
            this.GUID = new Guid();
            this.FormBuilderDisplayTypeID = FormBuilderDisplayType.Unassigned;
        }
    }
}
