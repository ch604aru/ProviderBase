using ProviderBase.Data.Entities;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("MediaFileType_T")]
    public enum MediaFileType
    {
        Unassigned = 0,
        Image = 1,
        Video = 2
    }

    [DataProviderTable("MediaType_T")]
    public enum MediaType
    {
        Unassigned = 0,
        Progress = 1,
        CharacterClass = 2,
        CharacterClassSpec = 3,
        ProgressDetail = 4,
        ClassRoleRecruitment = 5,
        CharacterRace = 6,
        CharacterFaction = 7,
        CharacterUser = 8,
        CustomFieldItem = 9,
        CharacterSpell = 10,
        ProgressItem = 11
    }

    [DataProviderTable("Media_T")]
    public class Media
    {
        [DataProviderResultField("MediaID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int MediaID { get; set; }

        [DataProviderResultField("MediaTypeID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public MediaType MediaTypeID { get; set; }

        [DataProviderResultField("Title", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Title { get; set; }

        [DataProviderResultField("MediaLocation", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string MediaLocation { get; set; }

        [DataProviderResultField("MediaName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string MediaName { get; set; }

        [DataProviderResultField("MediaFileTypeID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public MediaFileType MediaFileTypeID { get; set; }

        public string MediaFullName
        {
            get
            {
                return this.MediaLocation + this.MediaName;
            }
        }

        [DataProviderResultField("MediaAltText", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string MediaAltText { get; set; }

        public Media()
        {
            this.MediaID = 0;
            this.MediaTypeID = MediaType.Unassigned;
            this.Title = "";
            this.MediaLocation = "";
            this.MediaName = "";
            this.MediaFileTypeID = MediaFileType.Unassigned;
            this.MediaAltText = "";
        }
    }
}
