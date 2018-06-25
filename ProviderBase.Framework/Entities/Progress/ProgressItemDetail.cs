using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("ProgressItemDetail_T")]
    public class ProgressItemDetail
    {
        [DataProviderResultField("ProgressItemDetailID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int ProgressItemDetailID { get; set; }

        [DataProviderResultField("ProgressItemID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ProgressItemID { get; set; }

        [DataProviderResultField("ProgressDetailID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where)]
        public int ProgressDetailID { get; set; }

        [DataProviderResultField("ExternalReference", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public string ExternalReference { get; set; }

        public ProgressItemDetail()
        {
            this.ProgressItemDetailID = 0;
            this.ProgressItemID = 0;
            this.ProgressDetailID = 0;
            this.ExternalReference = "";
        }
    }
}
