using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;
using System.Collections.Generic;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterClassRole_T")]
    public enum CharacterClassRole
    {
        Unassigned = 0,
        Tank = 1,
        DPS = 2,
        Healer = 3
    }


    [DataProviderTable("CharacterClass_T")]
    public class CharacterClass
    {
        [DataProviderResultField("CharacterClassID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CharacterClassID { get; set; }

        [DataProviderResultField("MediaTypeID", KeyType.DefaultValue, FieldAction.Select, FieldAction.Insert)]
        public MediaType MediaTypeID { get; private set; }

        [DataProviderResultField("Description", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public string Description { get; set; }

        [DataProviderResultField("ExternalReference", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public string ExternalReference { get; set; }

        [DataProviderResultField("MediaID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultSet(1)]
        public Media Media { get; set; }

        [DataProviderResultSet(2)]
        public List<CharacterClassSpec> CharacterClassSpec { get; set; }

        public CharacterClass()
        {
            this.CharacterClassID = 0;
            this.MediaTypeID = MediaType.CharacterClass;
            this.Description = "";
            this.ExternalReference = "";
            this.Media = new Media();
            this.CharacterClassSpec = new List<CharacterClassSpec>();
        }
    }
}
