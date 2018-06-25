using ProviderBase.Framework.Entities;
using System.Web;
using System;
using System.Collections.Generic;

namespace ProviderBase.Framework.Controls
{
    public class MenuControl : BaseControl
    {
        protected override string Render()
        {
            MenuItemContainer menuItemContainerItem = null;
            string pageURL = "";
            string template = "";

            menuItemContainerItem = ProviderBase.Data.Utility.XMLDeseralize<MenuItemContainer>(HttpContext.Current.Server.MapPath(@"/Resource/XML/") + this.websitePageContent.XMLFile);
            pageURL = this.Context.Request.CurrentExecutionFilePath.Replace(".aspx", "");

            if (menuItemContainerItem != null && menuItemContainerItem.MenuItemList.Count > 0)
            {
                template += "<ul";
                template += (string.IsNullOrEmpty(menuItemContainerItem.CSSClass) == false) ? $" class=\"{menuItemContainerItem.CSSClass}\"" : "";
                template += ">";

                foreach (MenuItem menuItem in menuItemContainerItem.MenuItemList)
                {
                    if ((int)this.User.UserRoleTypeID < menuItem.UserRoleTypeID)
                    {
                        break;
                    }

                    template += "<li";

                    if (menuItem.Link == pageURL || (menuItem.Link == "/" && pageURL.ToLower() == "/default"))
                    {
                        template += (string.IsNullOrEmpty(menuItem.CSSClassSelected) == false) ? $" class=\"{menuItem.CSSClassSelected}\"" : "";
                    }
                    else
                    {
                        template += (string.IsNullOrEmpty(menuItem.CSSClass) == false) ? $" class=\"{menuItem.CSSClass}\"" : "";
                    }

                    template += (string.IsNullOrEmpty(menuItem.Tooltip) == false) ? $" tooltip=\"{menuItem.Tooltip}\"" : "";
                    template += (string.IsNullOrEmpty(menuItem.OnClick) == false) ? $" OnClick=\"{menuItem.OnClick}\"" : "";
                    template += (string.IsNullOrEmpty(menuItem.OnMouseEnter) == false) ? $" OnMouseEnter=\"{menuItem.OnMouseEnter}\"" : "";
                    template += (string.IsNullOrEmpty(menuItem.OnMouseLeave) == false) ? $" OnMouseleave=\"{menuItem.OnMouseLeave}\"" : "";
                    template += (string.IsNullOrEmpty(menuItem.OnMouseMove) == false) ? $" OnMouseMove=\"{menuItem.OnMouseMove}\"" : "";
                    template += ">";

                    template += (string.IsNullOrEmpty(menuItem.Link) == false) ? $"<a href=\"{menuItem.Link}\">" : "";

                    template += (string.IsNullOrEmpty(menuItem.DisplayText) == false) ? $"<span>{menuItem.DisplayText.ToUpper()}</span>" : "";

                    template += (string.IsNullOrEmpty(menuItem.DisplayImage) == false) ? $"<img src=\"{menuItem.DisplayImage}\"/>" : "";

                    template += (string.IsNullOrEmpty(menuItem.Link) == false) ? "</a>" : "";

                    template += "</li>";

                    
                }

                template += "</ul>";
            }

            return template;
        }
    }
}
