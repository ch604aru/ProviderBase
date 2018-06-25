using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("FormBuilderTemplateItemAreaType_T")]
    public enum FormBuilderTemplateItemAreaType
    {
        Unassigned = 0,
        Div = 1,
        Repeat = 2,
        RepeatItem = 3,
        Paging = 4
    }

    [DataProviderTable("FormBuilderTemplateItemArea_T")]
    public class FormBuilderTemplateItemArea
    {
        [DataProviderResultField("FormBuilderTemplateItemAreaID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int FormBuilderTemplateItemAreaID { get; set; }

        [DataProviderResultField("FormBuilderTemplateItemID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int FormBuilderTemplateItemID { get; set; }

        [DataProviderResultField("FormBuilderTemplateItemAreaTypeID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public FormBuilderTemplateItemAreaType FormBuilderTemplateItemAreaTypeID { get; set; }

        [DataProviderResultField("SortOrder", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int SortOrder { get; set; }

        [DataProviderResultField("Class", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Class { get; set; }

        [DataProviderResultField("Style", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Style { get; set; }

        [DataProviderResultField("FieldName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string FieldName { get; set; }

        [DataProviderResultField("ParentID", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int ParentID { get; set; }

        [DataProviderResultField("Object", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public string Object { get; set; }

        public FormBuilderTemplateItemArea()
        {
            this.FormBuilderTemplateItemAreaID = 0;
            this.FormBuilderTemplateItemID = 0;
            this.FormBuilderTemplateItemAreaTypeID = FormBuilderTemplateItemAreaType.Unassigned;
            this.SortOrder = 0;
            this.Class = "";
            this.Style = "";
            this.FieldName = "";
            this.ParentID = 0;
            this.Object = "";
        }
    }
}
