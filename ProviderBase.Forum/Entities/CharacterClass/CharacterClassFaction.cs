using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;
using System.Collections.Generic;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterClassFaction_T")]
    public class CharacterClassFaction
    {
        [DataProviderResultField("CharacterClassFactionID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CharacterClassFactionID { get; set; }

        [DataProviderResultField("MediaTypeID", KeyType.DefaultValue)]
        public MediaType MediaTypeID { get; private set; }

        [DataProviderResultField("Description", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Description { get; set; }

        [DataProviderResultField("ExternalReference", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public string ExternalReference { get; set; }

        [DataProviderResultField("MediaID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultSet(1)]
        public Media Media { get; set; }

        public List<CharacterClassRace> CharacterRaceList { get; set; }

        public CharacterClassFaction()
        {
            this.CharacterClassFactionID = 0;
            this.MediaTypeID = MediaType.CharacterFaction;
            this.Description = "";
            this.ExternalReference = "";
            this.Media = new Media();
            this.CharacterRaceList = new List<CharacterClassRace>();
        }
    }
}
