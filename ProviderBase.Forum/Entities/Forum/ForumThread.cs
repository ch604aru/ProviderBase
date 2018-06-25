using ProviderBase.Data.Entities;
using System;
using System.Collections.Generic;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("ForumThreadType_T")]
    public enum ForumThreadType
    {
        Unassigned = 0,
        General = 1,
        Sticky = 2,
        Locked = 3,
        StickyLocked = 4,
        Announcement = 5
    }

    [DataProviderTable("ForumThread_T")]
    public class ForumThread : DataProviderPaging
    {
        [DataProviderResultField("ForumThreadID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int ForumThreadID { get; set; }

        [DataProviderResultField("ForumAreaID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ForumAreaID { get; set; }

        [DataProviderResultField("CharacterUserID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int CharacterUserID { get; set; }

        [DataProviderResultField("CreateDate", FieldAction.Select, FieldAction.Insert)]
        public DateTime CreateDate { get; set; }

        [DataProviderResultField("ModifyDate", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public DateTime ModifyDate { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("ForumThreadTypeID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public ForumThreadType ForumThreadTypeID { get; set; }

        [DataProviderResultSet(1)]
        public List<ForumThreadMessage> ThreadMessageList { get; set; }

        [DataProviderResultSet(2)]
        public List<ForumThreadView> ForumThreadView { get; set; }

        public ForumThread()
        {
            this.ForumThreadID = 0;
            this.ForumAreaID = 0;
            this.CharacterUserID = 0;
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;
            this.Title = "";
            this.ForumThreadTypeID = 0;
            this.ThreadMessageList = new List<ForumThreadMessage>();
            this.ForumThreadView = new List<ForumThreadView>();
        }
    }
}
