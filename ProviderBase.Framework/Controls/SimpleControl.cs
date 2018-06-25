using ProviderBase.Framework.Entities;
using System.Web;
using System.Web.Security;
using System;
using System.Web.UI;

namespace ProviderBase.Framework.Controls
{
    public class SimpleControl : BaseControl
    {
        protected override string Render()
        {
            string loginStatus = "Out";
            string firstValue = Utility.GetQueryValue("FirstValue", "");
            string IDValue = Utility.GetQueryValue("ID", "");
            string template = this.websitePageContent.TemplateFile;
            string hasIDValue = "";

            if (this.Context.User.Identity.IsAuthenticated && string.IsNullOrEmpty(this.Context.User.Identity.Name) == false)
            {
                loginStatus = "In";
            }

            if (string.IsNullOrEmpty(IDValue) == false)
            {
                hasIDValue = "Edit";
            }

            if (string.IsNullOrEmpty(this.websitePageContent.TemplateFile) == false)
            {
                string templateContent = "";
                TokenContainer tokenContainer = null;

                tokenContainer = ProviderBase.Data.Utility.XMLDeseralize<TokenContainer>(HttpContext.Current.Server.MapPath(@"/Resource/XML/") + this.websitePageContent.TokenFile);

                template = template.Replace("$LOGINSTATUS$", loginStatus);
                template = template.Replace("$FIRSTVALUE$", firstValue);
                template = template.Replace("$IDVALUE$", hasIDValue);

                templateContent = ProviderBase.Web.Utility.GetResourceHtml(template);

                if (tokenContainer != null && tokenContainer.TokenItemList?.Count > 0)
                {
                    foreach (TokenItem tokenItem in tokenContainer.TokenItemList)
                    {
                        string tokenItemHtml = "";

                        switch (tokenItem.TokenItemTypeID)
                        {
                            case TokenItemType.Text:
                                templateContent = templateContent.Replace(tokenItem.Token, tokenItem.Text);
                                break;

                            case TokenItemType.File:
                                tokenItemHtml = ProviderBase.Web.Utility.GetResourceHtml(tokenItem.Text);
                                templateContent = templateContent.Replace(tokenItem.Token, tokenItemHtml);
                                break;
                        }
                        
                    }
                }

                return templateContent;
            }
            else
            {
                return "";
            }
        }
    }
}
