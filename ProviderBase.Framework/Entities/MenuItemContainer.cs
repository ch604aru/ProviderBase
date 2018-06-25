using System.Collections.Generic;

namespace ProviderBase.Framework.Entities
{
    public class MenuItemContainer
    {
        public string CSSClass { get; set; }

        public List<MenuItem> MenuItemList { get; set; }

        public MenuItemContainer()
        {
            this.CSSClass = "";
            this.MenuItemList = new List<MenuItem>();
        }
    }
}
