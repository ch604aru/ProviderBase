using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterUserProgressDetail_T")]
    public class CharacterUserProgressDetail
    {
        [DataProviderResultField("CharacterUserProgressDetailID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CharacterUserProgressDetailID { get; set; }

        [DataProviderResultField("CharacterUserID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int CharacterUserID { get; set; }

        [DataProviderResultField("ProgressItemDetailID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ProgressItemDetailID { get; set; }

        [DataProviderResultField("ProgressCurrent", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ProgressCurrent { get; set; }

        [DataProviderResultField("ProgressTotal", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ProgressTotal { get; set; }

        [DataProviderResultField("IsComplete", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public bool IsComplete { get; set; }

        public CharacterUserProgressDetail()
        {
            this.CharacterUserProgressDetailID = 0;
            this.CharacterUserID = 0;
            this.ProgressItemDetailID = 0;
            this.ProgressCurrent = 0;
            this.ProgressTotal = 0;
            this.IsComplete = false;
        }
    }
}
