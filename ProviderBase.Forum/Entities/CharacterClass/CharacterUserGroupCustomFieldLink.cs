using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterUserGroupCustomFieldLink_T")]
    public class CharacterUserGroupCustomFieldLink
    {
        [DataProviderResultField("CharacterUserCustomFieldGroupID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CharacterUserCustomFieldGroupID { get; set; }

        [DataProviderResultField("CustomFieldID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CustomFieldID { get; set; }

        public CharacterUserGroupCustomFieldLink()
        {
            this.CharacterUserCustomFieldGroupID = 0;
            this.CustomFieldID = 0;
        }
    }
}
