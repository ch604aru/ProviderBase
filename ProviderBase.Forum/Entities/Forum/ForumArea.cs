using ProviderBase.Data.Entities;
using System.Collections.Generic;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("ForumArea_T")]
    public class ForumArea
    {
        [DataProviderResultField("ForumAreaID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int ForumAreaID { get; set; }

        [DataProviderResultField("ForumGroupID", KeyType.ForeignKey, FieldAction.Where, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ForumGroupID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("Description", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Description { get; set; }

        [DataProviderResultSet(1)]
        public List<ForumThread> ThreadList { get; set; }

        public ForumArea()
        {
            this.ForumAreaID = 0;
            this.ForumGroupID = 0;
            this.Title = "";
            this.Description = "";
            this.ThreadList = new List<ForumThread>();
        }
    }
}
