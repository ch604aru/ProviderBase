using ProviderBase.Data.Entities;
using System.Collections.Generic;
using System.Linq;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("Website_T")]
    public class Website
    {

        [DataProviderResultField("WebsiteID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int WebsiteID { get; set; }

        [DataProviderResultField("WebsiteHost", FieldAction.Select, FieldAction.Where, FieldAction.Insert, FieldAction.Update)]
        public string WebsiteHost { get; set; }

        [DataPropertyDisplay(DataPropertyDisplayType.Description)]
        [DataProviderResultField("WebsiteName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string WebsiteName { get; set; }

        [DataProviderResultField("WebsiteConnectionID", KeyType.ForeignKey, FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int WebsiteConnectionID { get; set; }

        [DataProviderResultField("WebsiteTemplatePrefix", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string WebsiteTemplatePrefix { get; set; }

        [DataProviderResultField("WebsiteTemplateFolder", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string WebsiteTemplateFolder { get; set; }

        public string WebsiteTemplateFolderPath
        {
            get
            {
                if (this.WebsiteTemplateFolder.EndsWith("/"))
                {
                    return this.WebsiteTemplateFolder;
                }
                else
                {
                    return this.WebsiteTemplateFolder + "/";
                }
            }
        }

        [DataProviderResultSet(1)]
        public List<WebsiteSetting> WebsiteSettingList { get; set; }

        [DataProviderResultSet(2)]
        public WebsiteConnection WebsiteConnection { get; set; }

        public Website()
        {
            this.WebsiteID = 0;
            this.WebsiteHost = "";
            this.WebsiteName = "";
            this.WebsiteConnectionID = 0;
            this.WebsiteTemplatePrefix = "";
            this.WebsiteTemplateFolder = "";
            this.WebsiteSettingList = new List<WebsiteSetting>();
            this.WebsiteConnection = new WebsiteConnection();
        }

        public WebsiteSetting GetSetting(string websiteSettingName)
        {
            if (this.WebsiteSettingList != null && this.WebsiteSettingList.Count > 0)
            {
                WebsiteSetting websiteSetting = null;

                websiteSetting = this.WebsiteSettingList.Where(x => x.WebsiteSettingName.ToLower() == websiteSettingName.ToLower()).SingleOrDefault<WebsiteSetting>();

                if (websiteSetting != null)
                {
                    return websiteSetting;
                }
                else
                {
                    return new WebsiteSetting();
                }
            }
            else
            {
                return new WebsiteSetting();
            }
        }

        public T GetSettingValue<T>(string websiteSettingName, T defaultValue)
        {
            T value = defaultValue;
            WebsiteSetting websiteSetting = null;

            websiteSetting = this.GetSetting(websiteSettingName);

            if (websiteSetting.WebsiteSettingID > 0 && string.IsNullOrEmpty(websiteSetting.WebsiteSettingValue) == false)
            {
                return ProviderBase.Data.Utility.TryParse<T>(websiteSetting.WebsiteSettingValue);
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
