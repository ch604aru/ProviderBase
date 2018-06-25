using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("ForumGroupUserRoleLink_T")]
    public class ForumGroupUserRoleLink
    {
        [DataProviderResultField("UserRoleTypeID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public UserRoleType UserRoleTypeID { get; set; }

        [DataProviderResultField("UserRoleLevelID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public int UserRoleLevelID { get; set; }

        [DataProviderResultField("ForumGroupID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public int ForumGroupID { get; set; }

        public ForumGroupUserRoleLink()
        {
            this.UserRoleTypeID = UserRoleType.Unassigned;
            this.UserRoleLevelID = 0;
            this.ForumGroupID = 0;
        }
    }
}
