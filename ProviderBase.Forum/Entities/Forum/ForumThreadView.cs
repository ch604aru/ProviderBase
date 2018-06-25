using ProviderBase.Data.Entities;
using System;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("ForumThreadView_T")]
    public class ForumThreadView : DataProviderPaging
    {
        [DataProviderResultField("ForumThreadViewID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int ForumThreadViewID { get; set; }

        [DataProviderResultField("CreateDate", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public DateTime CreateDate { get; set; }

        [DataProviderResultField("ForumThreadID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public int ForumThreadID { get; set; }

        [DataProviderResultField("CharacterUserID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public int CharacterUserID { get; set; }

        public ForumThreadView()
        {
            this.ForumThreadViewID = 0;
            this.ForumThreadID = 0;
            this.CharacterUserID = 0;
            this.CreateDate = DateTime.Now;
        }
    }
}
