namespace ProviderBase.Framework.Entities
{
    public class MenuItem
    {
        public string DisplayText { get; set; }

        public string DisplayImage { get; set; }

        public string CSSClass { get; set; }

        public string CSSClassSelected { get; set; }

        public string Link { get; set; }

        public string Tooltip { get; set; }

        public int UserRoleTypeID { get; set; }

        public string OnClick { get; set; }

        public string OnMouseEnter { get; set; }

        public string OnMouseLeave { get; set; }

        public string OnMouseMove { get; set; }

        public MenuItem()
        {
            this.DisplayText = "";
            this.DisplayImage = "";
            this.CSSClass = "";
            this.CSSClassSelected = "";
            this.Link = "";
            this.Tooltip = "";
            this.UserRoleTypeID = 0;
            this.OnClick = "";
            this.OnMouseEnter = "";
            this.OnMouseLeave = "";
            this.OnMouseMove = "";
        }
    }
}
