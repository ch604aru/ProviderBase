using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;
using System.Collections.Generic;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Forum.Entities
{
    [DataProviderTable("CharacterClassSpec_T")]
    public class CharacterClassSpec
    {
        [DataProviderResultField("CharacterClassSpecID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int CharacterClassSpecID { get; set; }

        [DataProviderResultField("MediaTypeID", KeyType.DefaultValue)]
        public MediaType MediaTypeID { get; private set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("Description", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Description { get; set; }

        [DataProviderResultField("CharacterClassID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public int CharacterClassID { get; set; }

        [DataProviderResultField("ExternalReference", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public string ExternalReference { get; set; }

        [DataProviderResultField("MediaID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int MediaID { get; set; }

        [DataProviderResultField("CharacterClassRoleID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public CharacterClassRole CharacterClassRoleID { get; set; }

        [DataProviderResultField("IsRecruiting", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public bool IsRecruiting { get; set; }

        [DataProviderResultSet(1)]
        public Media Media { get; set; }

        public CharacterClassSpec()
        {
            this.CharacterClassSpecID = 0;
            this.MediaTypeID = MediaType.CharacterClassSpec;
            this.Title = "";
            this.Description = "";
            this.CharacterClassID = 0;
            this.MediaID = 0;
            this.Media = new Media();
            this.CharacterClassRoleID = CharacterClassRole.Unassigned;
            this.ExternalReference = "";
            this.IsRecruiting = false;
        }
    }
}
