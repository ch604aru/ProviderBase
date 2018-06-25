using ProviderBase.Data.Entities;
using System;
using System.Collections.Generic;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("UserRoleType_T")]
    public enum UserRoleType
    {
        Unassigned = 0,
        Registered = 1,
        Member = 2,
        Officer = 3,
        GuildMaster = 4
    }

    [DataProviderTable("User_T")]
    public class User
    {
        [DataProviderResultField("UserID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int UserID { get; set; }

        [DataProviderResultField("Username", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public string Username { get; set; }

        [DataProviderResultField("Encrypted", FieldAction.Select, FieldAction.Insert, FieldAction.Update, FieldAction.Where)]
        public string Encrypted { get; set; }

        [DataProviderResultField("CreateDate", FieldAction.Select, FieldAction.Insert)]
        public DateTime CreateDate { get; set; }

        [DataProviderResultField("ModifyDate", FieldAction.Select, FieldAction.Update)]
        public DateTime ModifyDate { get; set; }

        [DataProviderResultField("LastLogin", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public DateTime LastLogin { get; set; }

        [DataProviderResultField("UserRoleTypeID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public UserRoleType UserRoleTypeID { get; set; }

        public User()
        {
            this.UserID = 0;
            this.Username = "";
            this.Encrypted = "";
            this.CreateDate = new DateTime(1900, 1, 1);
            this.ModifyDate = new DateTime(1900, 1, 1);
            this.LastLogin = new DateTime(1900, 1, 1);
            this.UserRoleTypeID = UserRoleType.Unassigned;
        }
    }
}
