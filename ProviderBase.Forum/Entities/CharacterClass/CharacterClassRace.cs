using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;
using System.Collections.Generic;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterClassRace_T")]
    public class CharacterClassRace
    {
        [DataProviderResultField("CharacterClassRaceID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CharacterClassRaceID { get; set; }

        [DataProviderResultField("MediaTypeID", KeyType.DefaultValue)]
        public MediaType MediaTypeID { get; private set; }

        [DataProviderResultField("Description", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Description { get; set; }

        [DataProviderResultField("CharacterClassFactionID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CharacterClassFactionID { get; set; }

        [DataProviderResultField("ExternalReference", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public string ExternalReference { get; set; }

        [DataProviderResultField("MediaID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultSet(1)]
        public Media Media { get; set; }

        [DataProviderResultSet(2)]
        public List<CharacterClass> CharacterClassList { get; set; }

        public CharacterClassRace()
        {
            this.CharacterClassRaceID = 0;
            this.MediaTypeID = MediaType.CharacterRace;
            this.Description = "";
            this.CharacterClassFactionID = 0;
            this.ExternalReference = "";
            this.Media = new Media();
            this.CharacterClassList = new List<CharacterClass>();
        }
    }
}
