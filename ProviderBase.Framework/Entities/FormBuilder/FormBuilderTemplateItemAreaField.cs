using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("FormBuilderTemplateItemAreaFieldDisplayType_T")]
    public enum FormBuilderTemplateItemAreaFieldDisplayType
    {
        Unassigned = 0,
        Img = 1,
        Span = 2,
        InputText = 3,
        InputPassword = 4,
        InputCheckbox = 5,
        ChkEditor = 6,
        TextArea = 7,
        a = 8,
        Hidden = 9
    }

    [DataProviderTable("FormBuilderTemplateItemFieldEventType_T")]
    public enum FormBuilderTemplateItemFieldEventType
    {
        Unassigned = 0,
        OnChange = 1,
        OnClick = 2,
        OnMouseOver = 3,
        OnMouseOut = 4,
        OnKeyDown = 5,
        OnKeyUp = 6,
        OnBlur = 7
    }

    [DataProviderTable("FormBuilderTemplateItemAreaField_T")]
    public class FormBuilderTemplateItemAreaField
    {
        [DataProviderResultField("FormBuilderTemplateItemAreaFieldID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int FormBuilderTemplateItemAreaFieldID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("FormBuilderTemplateItemAreaID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int FormBuilderTemplateItemAreaID { get; set; }

        [DataProviderResultField("FormBuilderFieldID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int FormBuilderFieldID { get; set; }

        [DataProviderResultField("FormBuilderTemplateItemAreaFieldDisplayTypeID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public FormBuilderTemplateItemAreaFieldDisplayType FormBuilderTemplateItemAreaFieldDisplayTypeID { get; set; }

        [DataProviderResultField("SortOrder", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int SortOrder { get; set; }

        [DataProviderResultField("Class", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Class { get; set; }

        [DataProviderResultField("Style", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Style { get; set; }

        [DataProviderResultField("FieldName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string FieldName { get; set; }

        [DataProviderResultField("MediaID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultField("FormBuilderTemplateItemFieldEventTypeID", KeyType.ForeignKey, FieldAction.Where, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public FormBuilderTemplateItemFieldEventType FormBuilderTemplateItemFieldEventTypeID { get; set; }

        [DataProviderResultField("FieldEvent", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string FieldEvent { get; set; }

        public FormBuilderTemplateItemAreaField()
        {
            this.FormBuilderTemplateItemAreaFieldID = 0;
            this.Title = "";
            this.FormBuilderTemplateItemAreaID = 0;
            this.FormBuilderFieldID = 0;
            this.FormBuilderTemplateItemAreaFieldDisplayTypeID = 0;
            this.SortOrder = 0;
            this.Class = "";
            this.Style = "";
            this.FieldName = "";
            this.MediaID = 0;
            this.FormBuilderTemplateItemFieldEventTypeID = FormBuilderTemplateItemFieldEventType.Unassigned;
            this.FieldEvent = "";
        }
    }
}
