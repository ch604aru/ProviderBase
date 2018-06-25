using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;
using System;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("FormBuilderTemplateType")]
    public enum FormBuilderTemplateType
    {
        Unassigned = 0,
        Form = 1,
        Div = 2
    }

    [DataProviderTable("FormBuilderTemplate_T")]
    public class FormBuilderTemplate
    {
        [DataProviderResultField("FormBuilderTemplateID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int FormBuilderTemplateID { get; set; }

        [DataProviderResultField("CreateDate", FieldAction.Select, FieldAction.Insert)]
        public DateTime CreateDate { get; set; }

        [DataProviderResultField("ModifyDate", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public DateTime ModifyDate { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("TemplateFileName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string TemplateFileName { get; set; }

        [DataProviderResultField("FormBuilderID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int FormBuilderID { get; set; }

        [DataProviderResultField("Class", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Class { get; set; }

        [DataProviderResultField("Style", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Style { get; set; }

        [DataProviderResultField("FormAction", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string FormAction { get; set; }

        [DataProviderResultField("FormMethod", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string FormMethod { get; set; }

        [DataProviderResultField("FormName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string FormName { get; set; }

        [DataProviderResultField("FormBuilderTemplateTypeID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public FormBuilderTemplateType FormBuilderTemplateTypeID { get; set; }

        public FormBuilderTemplate()
        {
            this.FormBuilderTemplateID = 0;
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;
            this.Title = "";
            this.TemplateFileName = "";
            this.FormBuilderID = 0;
            this.Class = "";
            this.Style = "";
            this.FormAction = "";
            this.FormMethod = "";
            this.FormName = "";
            this.FormBuilderTemplateTypeID = FormBuilderTemplateType.Unassigned;
        }
    }
}
