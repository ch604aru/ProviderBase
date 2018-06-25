using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;

namespace ProviderBase.Framework
{
    public class TemplateFile
    {
        private string TemplateFileContentRaw { get; set; }

        private string TemplateFileContentRepeatRaw { get; set; }

        private List<string> TemplateFileContentRepeatItemRaw { get; set; }

        private string TemplateFileContentWorking { get; set; }

        private string TemplateFileContentRepeatWorking { get; set; }

        private string TemplateFileContentRepeatItemWorking { get; set; }

        public TemplateFile()
        {
            this.TemplateFileContentRaw = "";
            this.TemplateFileContentRepeatRaw = "";
            this.TemplateFileContentRepeatItemRaw = new List<string>();
        }

        public TemplateFile(string templateFileName)
        {
            this.TemplateFileContentRaw = ProviderBase.Web.Utility.GetResourceHtml(templateFileName);
            this.TemplateFileContentRepeatRaw = ProviderBase.Data.Utility.GetTemplateFileElementSingle(this.TemplateFileContentRaw, "repeat");
            this.TemplateFileContentRaw = ProviderBase.Data.Utility.ReplaceTemplateFileElementSingle(this.TemplateFileContentRaw, "repeat", "$REPEAT$");
            this.TemplateFileContentRepeatItemRaw = ProviderBase.Data.Utility.GetTemplateFileElement(this.TemplateFileContentRepeatRaw, "repeatitem");
            this.TemplateFileContentRepeatRaw = ProviderBase.Data.Utility.ReplaceTemplateFileElement(this.TemplateFileContentRepeatRaw, "repeatitem", "$REPEATITEM$");
        }

        public TemplateFile(string templateFileName, string folderName)
        {
            this.TemplateFileContentRaw = ProviderBase.Web.Utility.GetResourceHtml(folderName + "/" + templateFileName);
            this.TemplateFileContentRepeatRaw = ProviderBase.Data.Utility.GetTemplateFileElementSingle(this.TemplateFileContentRaw, "repeat");
            this.TemplateFileContentRaw = ProviderBase.Data.Utility.ReplaceTemplateFileElementSingle(this.TemplateFileContentRaw, "repeat", "REPEAT");
            this.TemplateFileContentRepeatItemRaw = ProviderBase.Data.Utility.GetTemplateFileElement(this.TemplateFileContentRepeatRaw, "repeatitem");
            this.TemplateFileContentRepeatRaw = ProviderBase.Data.Utility.ReplaceTemplateFileElement(this.TemplateFileContentRepeatRaw, "repeatitem", "REPEATITEM");
        }

        public TemplateFile(FormBuilderUtility formBuilder)
        {
            string template = "";

            template = formBuilder.Render();

            this.TemplateFileContentRaw = template;
            this.TemplateFileContentRepeatRaw = ProviderBase.Data.Utility.GetTemplateFileElementSingle(this.TemplateFileContentRaw, "repeat");
            this.TemplateFileContentRaw = ProviderBase.Data.Utility.ReplaceTemplateFileElementSingle(this.TemplateFileContentRaw, "repeat", "REPEAT");
            this.TemplateFileContentRepeatItemRaw = ProviderBase.Data.Utility.GetTemplateFileElement(this.TemplateFileContentRepeatRaw, "repeatitem");
            this.TemplateFileContentRepeatRaw = ProviderBase.Data.Utility.ReplaceTemplateFileElement(this.TemplateFileContentRepeatRaw, "repeatitem", "REPEATITEM");
        }

        public void BindTemplateFileContent(object bindObject)
        {
            this.BindTemplate(bindObject, null, null);
        }

        public void BindTemplateFileContent(object bindObject, string prefix)
        {
            if (bindObject != null && string.IsNullOrEmpty(this.TemplateFileContentRaw) == false)
            {
                this.TemplateFileContentWorking = ProviderBase.Data.Utility.TemplateBindData(this.TemplateFileContentRaw, bindObject, prefix);
            }
        }

        public void BindTemplateFileContentRepeat(object bindObjectRepeat)
        {
            this.BindTemplate(null, bindObjectRepeat, null);
        }

        public void BindTemplateFileContentRepeat(object bindObject, object bindObjectRepeat)
        {
            this.BindTemplate(bindObject, bindObjectRepeat, null);
        }

        public void BindTemplateFileContentRepeatItem(object bindObjectRepeatItem)
        {
            List<object> bindObjectRepeatList = new List<object>();
            bindObjectRepeatList.Add(bindObjectRepeatItem);

            this.BindTemplate(null, null, bindObjectRepeatList);
        }

        public void BindTemplateFileContentRepeatItem(List<object> bindObjectRepeatList)
        {
            this.BindTemplate(null, null, bindObjectRepeatList);
        }

        public void BindTemplateFileContentRepeatItem(object bindObjectRepeat, object bindObjectRepeatItem)
        {
            List<object> bindObjectRepeatList = new List<object>();
            bindObjectRepeatList.Add(bindObjectRepeatItem);

            this.BindTemplate(null, bindObjectRepeat, bindObjectRepeatList);
        }

        public void BindTemplateFileContentRepeatItem<T>(object bindObjectRepeat, List<T> bindObjectRepeatItemList)
        {
            this.BindTemplate(null, bindObjectRepeat, bindObjectRepeatItemList);
        }

        public void BindTemplateFileContentRepeatItem(object bindObject, object bindObjectRepeat, object bindObjectRepeatItem)
        {
            List<object> bindObjectRepeatList = new List<object>();
            bindObjectRepeatList.Add(bindObjectRepeatItem);

            this.BindTemplate(bindObject, bindObjectRepeat, bindObjectRepeatList);
        }

        public void BindTemplateFileContentRepeatItem<T>(object bindObject, object bindObjectRepeat, List<T> bindObjectRepeatItemList)
        {
            this.BindTemplate(bindObject, bindObjectRepeat, bindObjectRepeatItemList);
        }

        public void BindTemplateFileContentSingle(string keyName, object value)
        {
            this.TemplateFileContentWorking = this.TemplateFileContentWorking.Replace(keyName, Convert.ToString(value));
        }

        public void BindTemplateFileContentRepeatSingle(string keyName, object value)
        {
            this.TemplateFileContentRepeatWorking = this.TemplateFileContentRepeatWorking.Replace(keyName, Convert.ToString(value));
        }

        public void BindTemplateFileContentRepeatItemSingle(string keyName, object value)
        {
            this.TemplateFileContentRepeatItemWorking = this.TemplateFileContentRepeatItemWorking.Replace(keyName, Convert.ToString(value));
        }

        public void TemplateFinaliseRepeat()
        {
            string templateTemp = "";

            if (string.IsNullOrEmpty(this.TemplateFileContentWorking))
            {
                templateTemp = this.TemplateFileContentRaw;
                templateTemp = templateTemp.Replace("$REPEAT$", this.TemplateFileContentRepeatWorking);
            }
            else
            {
                templateTemp = this.TemplateFileContentRepeatWorking;
            }

            templateTemp = templateTemp.Replace("$REPEATITEM0$", this.TemplateFileContentRepeatItemWorking);

            this.TemplateFileContentRepeatWorking = "";
            this.TemplateFileContentRepeatItemWorking = "";

            this.TemplateFileContentWorking += templateTemp;
        }

        public string TemplateFinalise(User user)
        {
            string templateTemp = "";

            if (string.IsNullOrEmpty(TemplateFileContentWorking) == false)
            {
                templateTemp = this.TemplateFileContentWorking;
            }
            else
            {
                templateTemp = this.TemplateFileContentRaw;
            }

            templateTemp = templateTemp.Replace("$REPEAT$", this.TemplateFileContentRepeatWorking);
            templateTemp = templateTemp.Replace("$REPEATITEM0$", this.TemplateFileContentRepeatItemWorking);

            if (user?.UserID > 0)
            {
                templateTemp = ProviderBase.Data.Utility.ReplaceTemplateFileElement(templateTemp, "loggedout", "");
                templateTemp = templateTemp.Replace("<loggedin>", "");
                templateTemp = templateTemp.Replace("</loggedin>", "");
            }
            else
            {
                templateTemp = ProviderBase.Data.Utility.ReplaceTemplateFileElement(templateTemp, "loggedin", "");
                templateTemp = templateTemp.Replace("<loggedout>", "");
                templateTemp = templateTemp.Replace("</loggedout>", "");
            }

            return templateTemp;
        }

        private void BindTemplate(object bindObject, object bindObjectRepeat, List<object> bindObjectRepeatList)
        {
            if (bindObject != null && string.IsNullOrEmpty(this.TemplateFileContentRaw) == false)
            {
                if (string.IsNullOrEmpty(TemplateFileContentWorking))
                {
                    this.TemplateFileContentWorking = ProviderBase.Data.Utility.TemplateBindData(this.TemplateFileContentRaw, bindObject);
                }
                else
                {
                    this.TemplateFileContentWorking = ProviderBase.Data.Utility.TemplateBindData(this.TemplateFileContentWorking, bindObject);
                }
            }

            if (bindObjectRepeat != null)
            {
                string templateTemp = "";

                templateTemp = ProviderBase.Data.Utility.TemplateBindData(TemplateFileContentRepeatRaw, bindObjectRepeat);

                this.TemplateFileContentRepeatWorking += templateTemp;
            }

            if (bindObjectRepeatList?.Count > 0 && this.TemplateFileContentRepeatItemRaw?.Count > 0)
            {
                string templateTemp = "";

                templateTemp = TemplateFileContentRepeatItemRaw[0];

                foreach (object bindObjectRepeatItem in bindObjectRepeatList)
                {
                    templateTemp = ProviderBase.Data.Utility.TemplateBindData(templateTemp, bindObjectRepeatItem);
                }

                this.TemplateFileContentRepeatItemWorking += templateTemp;
            }
        }

        private void BindTemplate<T>(object bindObject, object bindObjectRepeat, List<T> bindObjectRepeatItemList)
        {
            if (bindObject != null && string.IsNullOrEmpty(this.TemplateFileContentRaw) == false)
            {
                this.TemplateFileContentWorking = ProviderBase.Data.Utility.TemplateBindData(this.TemplateFileContentRaw, bindObject);
            }

            if (bindObjectRepeat != null && string.IsNullOrEmpty(this.TemplateFileContentRepeatRaw) == false)
            {
                string templateTemp = "";

                templateTemp = ProviderBase.Data.Utility.TemplateBindData(TemplateFileContentRepeatRaw, bindObjectRepeat);

                if (bindObjectRepeatItemList != null && this.TemplateFileContentRepeatItemRaw?.Count > 0)
                {
                    string[] templateTempRepeatItemList = new string[this.TemplateFileContentRepeatItemRaw.Count];

                    foreach (T bindObjectRepeatItem in bindObjectRepeatItemList)
                    {
                        for (int i = 0; i < this.TemplateFileContentRepeatItemRaw.Count; i++)
                        {
                            string templateTempRepeatItem = "";

                            templateTempRepeatItem += ProviderBase.Data.Utility.TemplateBindData(this.TemplateFileContentRepeatItemRaw[i], bindObjectRepeatItem);

                            if (templateTempRepeatItemList[i] == null)
                            {
                                templateTempRepeatItemList[i] = templateTempRepeatItem;
                            }
                            else
                            {
                                templateTempRepeatItemList[i] += templateTempRepeatItem;
                            }
                        }   
                    }

                    for (int i = 0; i < templateTempRepeatItemList.Count(); i++)
                    {
                        templateTemp = templateTemp.Replace("$REPEATITEM" + i + "$", templateTempRepeatItemList[i]);
                    }
                }

                this.TemplateFileContentRepeatWorking += templateTemp;
            }
        }
    }
}
