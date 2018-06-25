using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("Progress_T")]
    public class Progress
    {
        [DataProviderResultField("ProgressID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int ProgressID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public string Title { get; set; }

        [DataProviderResultField("Description", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public string Description { get; set; }

        [DataProviderResultField("MediaTypeID", KeyType.DefaultValue, FieldAction.Select, FieldAction.Insert)]
        public MediaType MediaTypeID { get; set; }

        [DataProviderResultField("MediaID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultField("ExternalReference", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public string ExternalReference { get; set; }

        public Progress()
        {
            this.ProgressID = 0;
            this.Title = "";
            this.Description = "";
            this.MediaTypeID = MediaType.Progress;
            this.MediaID = 0;
            this.ExternalReference = "";
        }
    }
}
