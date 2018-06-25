using ProviderBase.Data.Entities;
using System;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("ForumThreadMessageType_T")]
    public enum ForumThreadMessageType
    {
        Unassigned = 0,
        Header = 1,
        Reply = 2
    }

    [DataProviderTable("ForumThreadMessage_T")]
    public class ForumThreadMessage : DataProviderPaging
    {
        [DataProviderResultField("ForumThreadMessageID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int ForumThreadMessageID { get; set; }

        [DataProviderResultField("ForumThreadID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ForumThreadID { get; set; }

        [DataProviderResultField("CharacterUserID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int CharacterUserID { get; set; }

        [DataProviderResultField("CreateDate", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public DateTime CreateDate { get; set; }

        [DataProviderResultField("ModifyDate", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public DateTime ModifyDate { get; set; }

        [DataProviderResultField("MessageText", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string MessageText { get; set; }

        [DataProviderResultField("ForumThreadMessageTypeID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public ForumThreadMessageType ForumThreadMessageTypeID { get; set; }

        public ForumThreadMessage()
        {
            this.ForumThreadMessageID = 0;
            this.ForumThreadID = 0;
            this.CharacterUserID = 0;
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;
            this.MessageText = "";
            this.ForumThreadMessageTypeID = ForumThreadMessageType.Unassigned;
        }
    }
}