using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("ProgressDetail_T")]
    public class ProgressDetail
    {
        [DataProviderResultField("ProgressItemDetailID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int ProgressDetailID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public string Title { get; set; }

        [DataProviderResultField("Description", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public string Description { get; set; }

        [DataProviderResultField("MediaID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultField("MediaTypeID", KeyType.DefaultValue, FieldAction.Select, FieldAction.Insert)]
        public MediaType MediaTypeID { get; set; }

        public ProgressDetail()
        {
            this.ProgressDetailID = 0;
            this.Title = "";
            this.Description = "";
            this.MediaID = 0;
            this.MediaTypeID = MediaType.ProgressDetail;
        }
    }
}
