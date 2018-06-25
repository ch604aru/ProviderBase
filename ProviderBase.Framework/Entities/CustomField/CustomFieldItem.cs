using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("CustomFieldItemType_T")]
    public enum CustomFieldItemType
    {
        Unassigned = 0,
        Text = 1,
        Int = 2,
        Decimal = 3,
        Boolean = 4,
        CustomFieldValue = 5
    }

    [DataProviderTable("CustomFieldItemDisplayType_T")]
    public enum CustomFieldItemDisplayType
    {
        Unassigned = 0,
        Img = 1,
        Span = 2,
        InputText = 3,
        InputPassword = 4,
        InputCheckbox = 5,
        chkEditor = 6,
        TextArea = 7
    }

    [DataProviderTable("CustomFieldItem_T")]
    public class CustomFieldItem
    {
        [DataProviderResultField("CustomFieldItemID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CustomFieldItemID { get; set; }

        [DataProviderResultField("MediaTypeID", KeyType.DefaultValue)]
        public MediaType MediaTypeID { get; set; }

        [DataProviderResultField("FieldName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string FieldName { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("CustomFieldItemTypeID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public CustomFieldItemType CustomFieldItemTypeID { get; set; }

        [DataProviderResultField("CustomFieldItemDisplayTypeID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public CustomFieldItemDisplayType CustomFieldItemDisplayTypeID { get; set; }

        [DataProviderResultField("MediaID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultField("CustomFieldID", KeyType.ForeignKey, FieldAction.Where, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int CustomFieldID { get; set; }

        [DataProviderResultField("TableDefinitionID", KeyType.ForeignKey, FieldAction.Where, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int TableDefinitionID { get; set; }

        public CustomFieldItem()
        {
            this.CustomFieldItemID = 0;
            this.MediaTypeID = MediaType.CustomFieldItem;
            this.FieldName = "";
            this.Title = "";
            this.CustomFieldItemTypeID = CustomFieldItemType.Unassigned;
            this.CustomFieldItemDisplayTypeID = CustomFieldItemDisplayType.Unassigned;
            this.MediaID = 0;
            this.CustomFieldID = 0;
            this.TableDefinitionID = 0;
        }
    }
}
