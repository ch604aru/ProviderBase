using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterUser_T")]
    public class CharacterUser
    {
        [DataProviderResultField("CharacterUserID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CharacterUserID { get; set; }

        [DataProviderResultField("UserID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int UserID { get; set; }

        [DataProviderResultField("CharacterName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string CharacterName { get; set; }

        [DataProviderResultField("CharacterServer", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string CharacterServer { get; set; }

        [DataProviderResultField("CharacterLevel", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CharacterLevel { get; set; }

        [DataProviderResultField("MediaTypeID", KeyType.DefaultValue)]
        public MediaType MediaType { get; private set; }

        [DataProviderResultField("MediaID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultField("CharacterClassFactionID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int CharacterClassFactionID { get; set; }

        [DataProviderResultField("CharacterClassRaceID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int CharacterClassRaceID { get; set; }

        [DataProviderResultField("CharacterClassID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int CharacterClassID { get; set; }

        [DataProviderResultField("CharacterClassSpecID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int CharacterClassSpecID { get; set; }

        [DataProviderResultField("Signature", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Signature { get; set; }

        [DataProviderResultField("ParentID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int ParentID { get; set; }

        [DataProviderResultSet(1)]
        public Media Media { get; set; }

        [DataProviderResultSet(2)]
        public CharacterClassFaction CharacterClassFaction { get; set; }

        [DataProviderResultSet(3)]
        public CharacterClassRace CharaterClassRace { get; set; }

        [DataProviderResultSet(4)]
        public CharacterClass CharacterClass { get; set; }

        [DataProviderResultSet(5)]
        public CharacterClassSpec CharacterClassSpec { get; set; }

        public CharacterUser()
        {
            this.CharacterUserID = 0;
            this.UserID = 0;
            this.CharacterName = "";
            this.CharacterServer = "";
            this.CharacterLevel = 0;
            this.MediaType = MediaType.CharacterUser;
            this.MediaID = 0;
            this.CharacterClassFactionID = 0;
            this.CharacterClassRaceID = 0;
            this.CharacterClassID = 0;
            this.CharacterClassSpecID = 0;
            this.Signature = "";
            this.ParentID = 0;
            this.Media = new Media();
            this.CharacterClassFaction = new CharacterClassFaction();
            this.CharaterClassRace = new CharacterClassRace();
            this.CharacterClass = new CharacterClass();
            this.CharacterClassSpec = new CharacterClassSpec();
        }
    }
}
