using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterUserCustomFieldGroup_T")]
    public class CharacterUserCustomFieldGroup
    {
        [DataProviderResultField("CharacterUserCustomFieldGroupID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CharacterUserCustomFieldGroupID { get; set; }

        [DataProviderResultField("Description", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Description { get; set; }

        public CharacterUserCustomFieldGroup()
        {
            this.CharacterUserCustomFieldGroupID = 0;
            this.Description = "";
        }
    }
}
