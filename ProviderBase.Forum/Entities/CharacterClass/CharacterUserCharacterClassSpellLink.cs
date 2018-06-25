using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterUserCharacterClassSpellLink_T")]
    public class CharacterUserCharacterClassSpellLink
    {
        [DataProviderResultField("CharacterUserID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CharacterUserID { get; set; }

        [DataProviderResultField("CharacterClassSpellID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CharacterClassSpellID { get; set; }

        public CharacterUserCharacterClassSpellLink()
        {
            this.CharacterUserID = 0;
            this.CharacterClassSpellID = 0;
        }
    }
}
