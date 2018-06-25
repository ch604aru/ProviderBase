using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterClassTalent_T")]
    public class CharacterClassTalent
    {
        [DataProviderResultField("CharacterClassTalentID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CharacterClassTalentID { get; set; }

        [DataProviderResultField("TalentTier", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int TalentTier { get; set; }

        [DataProviderResultField("TalentColumn", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int TalentColumn { get; set; }

        [DataProviderResultField("CharacterClassSpellID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CharacterClassSpellID { get; set; }

        [DataProviderResultField("CharacterClassSpecID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CharacterClassSpecID { get; set; }

        public CharacterClassTalent()
        {
            this.CharacterClassTalentID = 0;
            this.TalentTier = 0;
            this.TalentColumn = 0;
            this.CharacterClassSpellID = 0;
            this.CharacterClassSpecID = 0;
        }
    }
}
