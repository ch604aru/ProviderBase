namespace ProviderBase.Framework.Entities
{
    public enum TokenItemType
    {
        Unassigned = 0,
        Text = 1,
        File = 2
    }

    public class TokenItem
    {
        public string Text { get; set; }

        public string Token { get; set; }

        public int UserRoleTypeID { get; set; }

        public int UserRoleLevel { get; set; }

        public TokenItemType TokenItemTypeID { get; set; }

        public TokenItem()
        {
            this.Text = "";
            this.Token = "";
            this.UserRoleTypeID = 0;
            this.UserRoleLevel = 0;
            this.TokenItemTypeID = TokenItemType.Unassigned;
        }
    }
}
