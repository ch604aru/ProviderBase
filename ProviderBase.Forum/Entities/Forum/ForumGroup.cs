using ProviderBase.Data.Entities;
using System.Collections.Generic;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("ForumGroup_T")]
    public class ForumGroup
    {
        [DataProviderResultField("ForumGroupID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int ForumGroupID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultSet(1)]
        public List<ForumArea> ThreadAreaList { get; set; }

        public ForumGroup()
        {
            this.ForumGroupID = 0;
            this.Title = "";
            this.ThreadAreaList = new List<ForumArea>();
        }
    }
}
