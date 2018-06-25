using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("ProgressItem_T")]
    public class ProgressItem
    {
        [DataProviderResultField("ProgressItemID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int ProgressItemID { get; set; }

        [DataProviderResultField("ProgressID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ProgressID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public string Title { get; set; }

        [DataProviderResultField("Description", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public string Description { get; set; }

        [DataProviderResultField("MediaID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultField("MediaTypeID", KeyType.DefaultValue, FieldAction.Select, FieldAction.Insert)]
        public MediaType MediaTypeID { get; set; }

        [DataProviderResultField("ExternalReference", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public string ExternalReference { get; set; }

        public ProgressItem()
        {
            this.ProgressItemID = 0;
            this.ProgressID = 0;
            this.Title = "";
            this.Description = "";
            this.MediaID = 0;
            this.MediaTypeID = MediaType.ProgressItem;
            this.ExternalReference = "";
        }
    }
}
