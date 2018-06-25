using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("FormBuilderTemplateItem_T")]
    public class FormBuilderTemplateItem
    {
        [DataProviderResultField("FormBuilderTemplateItemID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int FormBuilderTemplateItemID { get; set; }

        [DataProviderResultField("FormBuilderTemplateID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int FormBuilderTemplateID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("Class", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Class { get; set; }

        [DataProviderResultField("Style", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Style { get; set; }

        [DataProviderResultField("SortOrder", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public int SortOrder { get; set; }

        public FormBuilderTemplateItem()
        {
            this.FormBuilderTemplateItemID = 0;
            this.FormBuilderTemplateID = 0;
            this.Title = "";
            this.Class = "";
            this.Style = "";
            this.SortOrder = 0;
        }
    }
}
