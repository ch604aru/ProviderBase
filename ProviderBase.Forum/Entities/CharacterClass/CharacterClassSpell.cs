using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterClassSpell_T")]
    public class CharacterClassSpell
    {
        [DataProviderResultField("CharacterClassSpellID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CharacterClassSpellID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("Description", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Description { get; set; }

        [DataProviderResultField("CastTime", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string CastTime { get; set; }

        [DataProviderResultField("Cooldown", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Cooldown { get; set; }

        [DataProviderResultField("MediaID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultField("MediaTypeID", KeyType.DefaultValue)]
        public MediaType MediaTypeID { get; set; }

        [DataProviderResultField("ExternalReference", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public string ExternalReference { get; set; }

        [DataProviderResultField("CharacterClassSpecID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CharacterClassSpecID { get; set; }

        public CharacterClassSpell()
        {
            this.CharacterClassSpellID = 0;
            this.Title = "";
            this.Description = "";
            this.CastTime = "";
            this.Cooldown = "";
            this.MediaID = 0;
            this.MediaTypeID = MediaType.CharacterSpell;
            this.ExternalReference = "";
            this.CharacterClassSpecID = 0;
        }
    }
}
