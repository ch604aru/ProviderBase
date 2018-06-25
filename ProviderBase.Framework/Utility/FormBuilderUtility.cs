using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProviderBase.Framework
{
    public enum FormBuilderUtilityMode
    {
        Unassigned = 0,
        Form = 1,
        Designer = 2
    }

    public class FormBuilderUtility
    {
        private FormBuilder FormBuilder { get; set; }

        private Website Website { get; set; }

        private Dictionary<string, string> HiddenFieldList { get; set; }

        private FormBuilderUtilityMode FormBuilderUtilityMode { get; set; }

        private string FormBuilderTemplateItemStart { get; set; }

        private string FormBuilderTemplateItemEnd { get; set; }

        private string FormBuilderTemplateItemAreaStart { get; set; }

        private string FormBuilderTemplateItemAreaEnd { get; set; }

        private string FormBuilderTemplateItemAreaFieldStart { get; set; }

        private string FormBuilderTemplateItemAreaFieldEnd { get; set; }

        private string FormBuilderTemplateHidden { get; set; }

        public FormBuilderUtility(Website website, int formBuilderID)
        {
            this.Website = website;
            this.FormBuilder = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilder>(new FormBuilder()
            {
                FormBuilderID = formBuilderID
            }, website.WebsiteConnection.ConnectionString);
            this.HiddenFieldList = new Dictionary<string, string>();
        }

        public FormBuilderUtility(Website website, string formBuilderGUID)
        {
            this.Website = website;
            this.FormBuilder = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilder>(new FormBuilder()
            {
                GUID = new Guid(formBuilderGUID)
            }, website.WebsiteConnection.ConnectionString);
            this.HiddenFieldList = new Dictionary<string, string>();
        }

        public FormBuilderUtility(Website website, FormBuilder formBuilder)
        {
            this.Website = website;
            this.FormBuilder = formBuilder;
            this.HiddenFieldList = new Dictionary<string, string>();
        }

        public string Render()
        {
            string formTemplate = "";

            if (this.FormBuilder?.FormBuilderID > 0)
            {
                FormBuilderTemplate formBuilderTemplate = null;

                // Template
                formBuilderTemplate = Data.Providers.DataProvider.SelectSingle<FormBuilderTemplate>(new FormBuilderTemplate()
                {
                    FormBuilderID = this.FormBuilder.FormBuilderID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderTemplate?.FormBuilderTemplateID > 0)
                {
                    formTemplate = this.GetTemplateFile(formBuilderTemplate);

                    if (string.IsNullOrEmpty(formTemplate))
                    {
                        string formBuilderTemplateStartTag = "";
                        string formBuilderTemplateEndTag = "";
                        DataProviderResultFilter formBuilderTemplateItemPaging = null;
                        List<FormBuilderTemplateItem> formBuilderTemplateItemList = null;

                        this.DrawElement(formBuilderTemplate, out formBuilderTemplateStartTag, out formBuilderTemplateEndTag);
                        formTemplate += formBuilderTemplateStartTag;

                        if (this.FormBuilderUtilityMode == FormBuilderUtilityMode.Designer)
                        {
                            formTemplate += $"<div class=\"formbuilder-edit-designer-item-drop js-item-drop-toggle\" onclick=\"ProviderBaseAdmin.FormBuilderTemplateItemCreate({formBuilderTemplate.FormBuilderTemplateID});\">Item</div>";
                        }

                        formBuilderTemplateItemPaging = new DataProviderResultFilter();
                        formBuilderTemplateItemPaging.SetOrder(typeof(FormBuilderTemplateItem), "SortOrder", OrderFieldDirection.Ascending);

                        // Template Item
                        formBuilderTemplateItemList = ProviderBase.Data.Providers.DataProvider.Select<FormBuilderTemplateItem>(new FormBuilderTemplateItem()
                        {
                            FormBuilderTemplateID = formBuilderTemplate.FormBuilderTemplateID
                        }, this.Website.WebsiteConnection.ConnectionString, formBuilderTemplateItemPaging);

                        if (formBuilderTemplateItemList?.Count > 0)
                        {
                            foreach (FormBuilderTemplateItem formBuilderTemplateItem in formBuilderTemplateItemList)
                            {
                                string formBuilderTemplateItemStartTag = "";
                                string formBuilderTemplateItemEndTag = "";
                                DataProviderResultFilter formBuilderTemplateItemAreaPaging = null;
                                List<FormBuilderTemplateItemArea> formBuilderTemplateItemAreaList = null;

                                this.DrawElement(formBuilderTemplateItem, out formBuilderTemplateItemStartTag, out formBuilderTemplateItemEndTag);
                                formTemplate += formBuilderTemplateItemStartTag;

                                formBuilderTemplateItemAreaPaging = new DataProviderResultFilter();
                                formBuilderTemplateItemAreaPaging.SetOrder(typeof(FormBuilderTemplateItemArea), "SortOrder", OrderFieldDirection.Ascending);

                                // Template Item Area
                                formBuilderTemplateItemAreaList = ProviderBase.Data.Providers.DataProvider.SelectOrDefault<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                                {
                                    FormBuilderTemplateItemID = formBuilderTemplateItem.FormBuilderTemplateItemID
                                }, this.Website.WebsiteConnection.ConnectionString, formBuilderTemplateItemAreaPaging, "ParentID");

                                if (formBuilderTemplateItemAreaList?.Count > 0)
                                {
                                    foreach (FormBuilderTemplateItemArea formBuilderTemplateItemArea in formBuilderTemplateItemAreaList)
                                    {
                                        string formBuilderTemplateItemAreaStartTag = "";
                                        string formBuilderTemplateItemAreaEndTag = "";
                                        bool addFieldToHidden = false;

                                        this.DrawElement(formBuilderTemplateItemArea, out formBuilderTemplateItemAreaStartTag, out formBuilderTemplateItemAreaEndTag, out addFieldToHidden);
                                        formTemplate += formBuilderTemplateItemAreaStartTag;

                                        formTemplate += this.DrawElement(formBuilderTemplateItemArea, addFieldToHidden, 0);

                                        // Parent ID Recursive
                                        formTemplate += this.DrawElement(formBuilderTemplateItemArea.FormBuilderTemplateItemAreaID, addFieldToHidden, 0);

                                        formTemplate += formBuilderTemplateItemAreaEndTag;
                                    }
                                }

                                formTemplate += formBuilderTemplateItemEndTag;
                            }
                        }
                        else if (this.FormBuilderUtilityMode != FormBuilderUtilityMode.Designer)
                        {
                            formTemplate = "Unable to load form items";
                        }

                        formTemplate += DrawElementHidden(formBuilderTemplate);

                        formTemplate += formBuilderTemplateEndTag;

                        this.SaveTemplateFile(formBuilderTemplate, formTemplate);
                    }
                }
                else
                {
                    formTemplate = "Unable to load form";
                }
            }
            else
            {
                formTemplate = "Unable to load form";
            }

            return formTemplate;
        }

        public string Render(object bindObject)
        {
            return this.Render(bindObject, null);
        }

        public string Render(object bindObject, User user)
        {
            string template = "";
            Dictionary<string, string> templateRepeatList = null;

            template = this.Render();
            templateRepeatList = ProviderBase.Data.Utility.GetTemplateFileElementRepeat(ref template);

            // ******************************** //
            // if list loop through list and do the below
            // if single object (not full) check for object types and select them
            //      bind these to the template if they are included as a <repeat>
            //      within this object bind further if included as a <repeatitem>
            // if complex object (all values set, ie full) then just bind to TemplateBindData
            
            // Maybe if object has provider base attributes then split up template and bind seperately (calling select methods)
            // Else send to bind method to handle the complex (already set?) object

            // Change FormBuilderTemplateItemArea.Object to TableDefinitionID
            // ******************************** //

            template = ProviderBase.Data.Utility.TemplateBindData(template, bindObject, "", null, ref templateRepeatList);
            
            foreach (KeyValuePair<string, string> pair in templateRepeatList)
            {
                template = template.Replace($"${pair.Key.ToUpper()}$", pair.Value);
            }

            if (user?.UserID > 0)
            {
                template = ProviderBase.Data.Utility.ReplaceTemplateFileElement(template, "loggedout", "");
                template = template.Replace("<loggedin>", "");
                template = template.Replace("</loggedin>", "");
            }
            else
            {
                template = ProviderBase.Data.Utility.ReplaceTemplateFileElement(template, "loggedin", "");
                template = template.Replace("<loggedout>", "");
                template = template.Replace("</loggedout>", "");
            }

            return template;
        }

        public string RenderDesigner()
        {
            string templateNameDesignerItem = "ProviderBase/ProviderBaseAdminFormBuilderEditDesignerItem.htm";
            string templateNameDesignerItemArea = "ProviderBase/ProviderBaseAdminFormBuilderEditDesignerItemArea.htm";
            string templateNameDesignerItemAreaField = "ProviderBase/ProviderBaseAdminFormBuilderEditDesignerItemAreaField.htm";
            string templateNameDesignerHidden = "ProviderBase/ProviderBaseAdminFormBuilderEditDesignerHidden.htm";
            string formBuilderTemplateItem = "";
            string formBuilderTemplateItemArea = "";
            string formBuilderTemplateItemAreaField = "";
            string template = "";
            string startTagTemp = "";
            string endTagTemp = "";

            this.FormBuilderUtilityMode = FormBuilderUtilityMode.Designer;

            formBuilderTemplateItem = ProviderBase.Web.Utility.GetReturnResourceHtml(templateNameDesignerItem);
            ProviderBase.Data.Utility.GetTemplateStartEndTag(formBuilderTemplateItem, "$DATA$", out startTagTemp, out endTagTemp);
            this.FormBuilderTemplateItemStart = startTagTemp;
            this.FormBuilderTemplateItemEnd = endTagTemp;

            formBuilderTemplateItemArea = ProviderBase.Web.Utility.GetReturnResourceHtml(templateNameDesignerItemArea);
            ProviderBase.Data.Utility.GetTemplateStartEndTag(formBuilderTemplateItemArea, "$DATA$", out startTagTemp, out endTagTemp);
            this.FormBuilderTemplateItemAreaStart = startTagTemp;
            this.FormBuilderTemplateItemAreaEnd = endTagTemp;

            formBuilderTemplateItemAreaField = ProviderBase.Web.Utility.GetReturnResourceHtml(templateNameDesignerItemAreaField);
            ProviderBase.Data.Utility.GetTemplateStartEndTag(formBuilderTemplateItemAreaField, "$DATA$", out startTagTemp, out endTagTemp);
            this.FormBuilderTemplateItemAreaFieldStart = startTagTemp;
            this.FormBuilderTemplateItemAreaFieldEnd = endTagTemp;

            this.FormBuilderTemplateHidden = ProviderBase.Web.Utility.GetReturnResourceHtml(templateNameDesignerHidden);

            template = Render();

            return template;
        }

        private void DrawElement(FormBuilderTemplate formBuilderTemplate, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            if (this.FormBuilderUtilityMode != FormBuilderUtilityMode.Designer)
            {
                if (formBuilderTemplate?.FormBuilderTemplateID > 0)
                {
                    switch (formBuilderTemplate.FormBuilderTemplateTypeID)
                    {
                        case FormBuilderTemplateType.Form:
                            startTag += "<form";
                            startTag += (string.IsNullOrEmpty(formBuilderTemplate.FormMethod)) ? "" : $" method=\"{formBuilderTemplate.FormMethod}\"";
                            startTag += (string.IsNullOrEmpty(formBuilderTemplate.FormAction)) ? "" : $" action=\"{formBuilderTemplate.FormAction}\"";
                            startTag += ">";

                            endTag += (string.IsNullOrEmpty(formBuilderTemplate.FormAction)) ? "" : $"<button type=\"submit\" name=\"Action\" value=\"Preview\" class=\"floatleft button\">Submit</button>";
                            endTag += "</form>";
                            break;

                        case FormBuilderTemplateType.Div:
                            startTag += "<div>";

                            endTag += "</div>";
                            break;
                    }

                    if (string.IsNullOrEmpty(formBuilderTemplate.Title) == false)
                    {
                        startTag += "<div";
                        startTag += (string.IsNullOrEmpty(formBuilderTemplate.Class)) ? ">" : $" class=\"{formBuilderTemplate.Class}\">";
                        startTag += formBuilderTemplate.Title;
                        startTag += "</div>";
                    }
                }
            }
        }

        private void DrawElement(FormBuilderTemplateItem formBuilderTemplateItem, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            if (this.FormBuilderUtilityMode == FormBuilderUtilityMode.Designer)
            {
                startTag = ProviderBase.Data.Utility.TemplateBindData(this.FormBuilderTemplateItemStart, formBuilderTemplateItem);
                endTag = ProviderBase.Data.Utility.TemplateBindData(this.FormBuilderTemplateItemEnd, formBuilderTemplateItem);
            }
            else
            {
                if (formBuilderTemplateItem?.FormBuilderTemplateItemID > 0)
                {
                    if (string.IsNullOrEmpty(formBuilderTemplateItem.Title) == false)
                    {
                        startTag += "<div>";
                        startTag = startTag.Replace(">", (string.IsNullOrEmpty(formBuilderTemplateItem.Class) ? ">" : $" class=\"{formBuilderTemplateItem.Class}\">"));
                        startTag += (string.IsNullOrEmpty(formBuilderTemplateItem.Title)) ? "" : formBuilderTemplateItem.Title;
                        startTag += "</div>";
                    }
                }
            }
        }

        private void DrawElement(FormBuilderTemplateItemArea formBuilderTemplateItemArea, out string startTag, out string endTag, out bool addFieldToHidden)
        {
            startTag = "";
            endTag = "";
            addFieldToHidden = false;

            if (this.FormBuilderUtilityMode == FormBuilderUtilityMode.Designer)
            {
                startTag = ProviderBase.Data.Utility.TemplateBindData(this.FormBuilderTemplateItemAreaStart, formBuilderTemplateItemArea);
                endTag = ProviderBase.Data.Utility.TemplateBindData(this.FormBuilderTemplateItemAreaEnd, formBuilderTemplateItemArea);
            }
            else
            {
                if (formBuilderTemplateItemArea?.FormBuilderTemplateItemAreaID > 0)
                {
                    this.DrawElement(formBuilderTemplateItemArea.FormBuilderTemplateItemAreaTypeID, formBuilderTemplateItemArea.Object, out startTag, out endTag, out addFieldToHidden);
                    startTag = Data.Utility.ReplaceLast(startTag, ">", (string.IsNullOrEmpty(formBuilderTemplateItemArea.Class) ? ">" : $" class=\"{formBuilderTemplateItemArea.Class}\">"));
                    startTag = Data.Utility.ReplaceLast(startTag, ">", (string.IsNullOrEmpty(formBuilderTemplateItemArea.Style) ? ">" : $" style=\"{formBuilderTemplateItemArea.Style}\">"));
                    startTag = Data.Utility.ReplaceLast(startTag, ">", (string.IsNullOrEmpty(formBuilderTemplateItemArea.FieldName) ? ">" : $" id=\"{formBuilderTemplateItemArea.FieldName}\">"));
                    startTag = Data.Utility.ReplaceLast(startTag, ">", (string.IsNullOrEmpty(formBuilderTemplateItemArea.FieldName) ? ">" : $" name=\"{formBuilderTemplateItemArea.FieldName}\">"));
                }
            }
        }

        private void DrawElement(FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            if (formBuilderTemplateItemAreaField?.FormBuilderTemplateItemAreaFieldID > 0)
            {
                startTag += "<div>";
                startTag = startTag.Replace(">", (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.Class) ? ">" : $" class=\"{formBuilderTemplateItemAreaField.Class}\">"));
                startTag = startTag.Replace(">", (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.Style) ? ">" : $" style=\"{formBuilderTemplateItemAreaField.Style}\">"));

                startTag += (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.Title)) ? "" : $"<span>{formBuilderTemplateItemAreaField.Title}</span>";

                endTag += "</div>";
            }
        }

        private void DrawElement(FormBuilderField formBuilderField, out string startTag, out string endTag, FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField, bool addFieldToHidden, int fieldRepeatIndex)
        {
            string elementEvent = "";

            startTag = "";
            endTag = "";

            if (this.FormBuilderUtilityMode == FormBuilderUtilityMode.Designer)
            {
                // Filter out records without a field attached
                if (formBuilderField?.FormBuilderFieldID > 0)
                {
                    startTag = this.FormBuilderTemplateItemAreaFieldStart;
                    endTag = this.FormBuilderTemplateItemAreaFieldEnd;

                    startTag = ProviderBase.Data.Utility.TemplateBindData(startTag, formBuilderTemplateItemAreaField, "", new List<string>() { "FieldName" });
                    startTag = ProviderBase.Data.Utility.TemplateBindData(startTag, formBuilderField, "", new List<string>() { "FieldName" });

                    endTag = ProviderBase.Data.Utility.TemplateBindData(endTag, formBuilderTemplateItemAreaField, "", new List<string>() { "FieldName" });
                    endTag = ProviderBase.Data.Utility.TemplateBindData(endTag, formBuilderField, "", new List<string>() { "FieldName" });
                }
            }
            else
            {
                this.DrawElement(formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldDisplayTypeID, out startTag, out endTag);
            }

            if (formBuilderField?.TableDefinitionFieldID > 0)
            {
                TableDefinitionField tableDefinitionField = null;

                tableDefinitionField = ProviderBase.Data.Providers.DataProvider.SelectSingle<TableDefinitionField>(new TableDefinitionField()
                {
                    TableDefinitionFieldID = formBuilderField.TableDefinitionFieldID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (tableDefinitionField?.TableDefinitionFieldID > 0)
                {
                    string fieldName = "";

                    fieldName = (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.FieldName)) ? tableDefinitionField.FieldName : formBuilderTemplateItemAreaField.FieldName;

                    if (this.FormBuilderUtilityMode == FormBuilderUtilityMode.Designer)
                    {
                        startTag = ProviderBase.Data.Utility.TemplateBindData(startTag, tableDefinitionField);

                        endTag = ProviderBase.Data.Utility.TemplateBindData(endTag, tableDefinitionField);
                    }
                    else
                    {
                        startTag = startTag.Replace(">", (string.IsNullOrEmpty(tableDefinitionField.FieldName) ? ">" : $" id=\"{tableDefinitionField.FieldName}\">"));
                        startTag = startTag.Replace("\">", (addFieldToHidden) ? $"_$INDEX$\">" : "\">");
                        startTag = startTag.Replace("\">", (addFieldToHidden) ? $"_{fieldRepeatIndex}\">" : "\">");
                        startTag = startTag.Replace(">", (string.IsNullOrEmpty(tableDefinitionField.FieldName) ? ">" : $" name=\"{tableDefinitionField.FieldName}\">"));
                        startTag = startTag.Replace("\">", (addFieldToHidden) ? $"_$INDEX$\">" : "\">");
                        startTag = startTag.Replace("\">", (addFieldToHidden) ? $"_{fieldRepeatIndex}\">" : "\">");

                        switch (formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldDisplayTypeID)
                        {
                            case FormBuilderTemplateItemAreaFieldDisplayType.Img:
                                startTag = startTag.Replace(">", (string.IsNullOrEmpty(tableDefinitionField.FieldName) ? ">" : $" src=\"{fieldName}\">"));
                                break;

                            case FormBuilderTemplateItemAreaFieldDisplayType.InputCheckbox:
                                endTag = endTag.Replace(">", (string.IsNullOrEmpty(tableDefinitionField.FieldName) ? ">" : $" for=\"{tableDefinitionField.FieldName}\">"));
                                break;

                            case FormBuilderTemplateItemAreaFieldDisplayType.a:
                                startTag = startTag.Replace(">", (string.IsNullOrEmpty(tableDefinitionField.FieldName) ? ">" : $" href=\"{fieldName}\">"));
                                break;
                        }
                    }

                    if (addFieldToHidden)
                    {
                        if (this.HiddenFieldList.ContainsKey(tableDefinitionField.FieldName) == false)
                        {
                            this.HiddenFieldList.Add(tableDefinitionField.FieldName, "ISLIST");
                        }
                    }
                }
            }
            else if (formBuilderField?.CustomFieldItemID > 0)
            {
                CustomFieldItem customFieldItem = null;

                customFieldItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<CustomFieldItem>(new CustomFieldItem()
                {
                    CustomFieldItemID = formBuilderField.CustomFieldItemID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (customFieldItem?.CustomFieldID > 0)
                {
                    string fieldName = "";

                    fieldName = (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.FieldName)) ? customFieldItem.FieldName : formBuilderTemplateItemAreaField.FieldName;

                    if (this.FormBuilderUtilityMode == FormBuilderUtilityMode.Designer)
                    {
                        startTag = ProviderBase.Data.Utility.TemplateBindData(startTag, customFieldItem);

                        endTag = ProviderBase.Data.Utility.TemplateBindData(endTag, customFieldItem);
                    }
                    else
                    {
                        startTag = startTag.Replace(">", (string.IsNullOrEmpty(customFieldItem.FieldName) ? ">" : $" id=\"{customFieldItem.FieldName}\">"));
                        startTag = startTag.Replace("\">", (addFieldToHidden) ? $"_$INDEX$\">" : "\">");
                        startTag = startTag.Replace(">", (string.IsNullOrEmpty(customFieldItem.FieldName) ? ">" : $" name=\"{customFieldItem.FieldName}\">"));
                        startTag = startTag.Replace("\">", (addFieldToHidden) ? $"_$INDEX$\">" : "\">");

                        switch (formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldDisplayTypeID)
                        {
                            case FormBuilderTemplateItemAreaFieldDisplayType.Img:
                                startTag = startTag.Replace(">", (string.IsNullOrEmpty(customFieldItem.FieldName) ? ">" : $" src=\"{fieldName}\">"));
                                break;

                            case FormBuilderTemplateItemAreaFieldDisplayType.InputCheckbox:
                                endTag = endTag.Replace(">", (string.IsNullOrEmpty(customFieldItem.FieldName) ? ">" : $" for=\"{customFieldItem.FieldName}\">"));
                                break;

                            case FormBuilderTemplateItemAreaFieldDisplayType.a:
                                startTag = startTag.Replace(">", (string.IsNullOrEmpty(customFieldItem.FieldName) ? ">" : $" href=\"{fieldName}\">"));
                                break;
                        }
                    }

                    if (addFieldToHidden)
                    {
                        if (this.HiddenFieldList.ContainsKey(customFieldItem.FieldName) == false)
                        {
                            this.HiddenFieldList.Add(customFieldItem.FieldName, "ISLIST");
                        }
                    }
                }
            }
            else if (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.FieldName) == false)
            {
                if (this.FormBuilderUtilityMode != FormBuilderUtilityMode.Designer)
                {
                    switch (formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldDisplayTypeID)
                    {
                        case FormBuilderTemplateItemAreaFieldDisplayType.Img:
                            startTag = startTag.Replace(">", (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.FieldName) ? ">" : $" src=\"{formBuilderTemplateItemAreaField.FieldName}\">"));
                            break;

                        case FormBuilderTemplateItemAreaFieldDisplayType.a:
                            startTag = startTag.Replace(">", (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.FieldName) ? ">" : $" href=\"{formBuilderTemplateItemAreaField.FieldName}\">"));
                            break;
                    }
                }
            }

            if (this.FormBuilderUtilityMode != FormBuilderUtilityMode.Designer)
            {
                this.DrawElement(formBuilderTemplateItemAreaField.FormBuilderTemplateItemFieldEventTypeID, out elementEvent);
                elementEvent = (string.IsNullOrEmpty(elementEvent)) ? "" : elementEvent.Replace("\"\"", $"\"{formBuilderTemplateItemAreaField.FieldEvent}\"");
                startTag = startTag.Replace(">", (string.IsNullOrEmpty(elementEvent) ? ">" : $"{elementEvent}>"));
            }
        }

        private void DrawElement(Media media, out string startTag, out string endTag, FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField)
        {
            string elementEvent = "";

            startTag = "";
            endTag = "";

            if (media?.MediaID > 0)
            {
                if (this.FormBuilderUtilityMode == FormBuilderUtilityMode.Designer)
                {
                    startTag = this.FormBuilderTemplateItemAreaFieldStart;
                    endTag = this.FormBuilderTemplateItemAreaFieldEnd;

                    startTag = startTag.Replace("$FIELDNAME$", media.Title);

                    startTag = ProviderBase.Data.Utility.TemplateBindData(startTag, formBuilderTemplateItemAreaField);
                    endTag = ProviderBase.Data.Utility.TemplateBindData(endTag, formBuilderTemplateItemAreaField);
                }
                else
                {
                    this.DrawElement(formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldDisplayTypeID, out startTag, out endTag);

                    startTag = startTag.Replace(">", (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.FieldName) ? ">" : $" id=\"{formBuilderTemplateItemAreaField.FieldName}\">"));
                    startTag = startTag.Replace(">", (string.IsNullOrEmpty(media.MediaFullName) ? ">" : $" src=\"{media.MediaFullName}\">"));

                    this.DrawElement(formBuilderTemplateItemAreaField.FormBuilderTemplateItemFieldEventTypeID, out elementEvent);
                    elementEvent = (string.IsNullOrEmpty(elementEvent)) ? "" : elementEvent.Replace("\"\"", $"\"{formBuilderTemplateItemAreaField.FieldEvent}\"");
                    startTag = startTag.Replace(">", (string.IsNullOrEmpty(elementEvent) ? ">" : $"{elementEvent}>"));
                }
            }
        }

        private void DrawElement(FormBuilderTemplateItemAreaType formBuilderTemplateItemAreaType, string objectName, out string startTag, out string endTag, out bool addFieldToHidden)
        {
            startTag = "";
            endTag = "";
            addFieldToHidden = false;

            switch (formBuilderTemplateItemAreaType)
            {
                case FormBuilderTemplateItemAreaType.Div:
                    startTag = "<div>";
                    endTag = "</div>";
                    addFieldToHidden = false;
                    break;

                case FormBuilderTemplateItemAreaType.Repeat:
                    startTag = $"<repeat data-objectrepeat=\"{objectName.ToUpper()}\">";
                    endTag = "</repeat>";
                    addFieldToHidden = true;
                    break;

                case FormBuilderTemplateItemAreaType.RepeatItem:
                    startTag = $"<repeatitem data-objectrepeatitem=\"{objectName.ToUpper()}\">";
                    endTag = "</repeatitem>";
                    addFieldToHidden = true;
                    break;

                case FormBuilderTemplateItemAreaType.Paging:
                    startTag = "$PAGING$";
                    endTag = "";
                    addFieldToHidden = false;
                    break;

                case FormBuilderTemplateItemAreaType.Unassigned:
                default:
                    startTag = "";
                    endTag = "";
                    addFieldToHidden = false;
                    break;
            }
        }

        private void DrawElement(FormBuilderTemplateItemAreaFieldDisplayType formBuilderTemplateItemAreaFieldDisplayType, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            switch (formBuilderTemplateItemAreaFieldDisplayType)
            {
                case FormBuilderTemplateItemAreaFieldDisplayType.Img:
                    startTag = "<img>";
                    endTag = "";
                    break;

                case FormBuilderTemplateItemAreaFieldDisplayType.Span:
                    startTag = "<span>";
                    endTag = "</span>";
                    break;

                case FormBuilderTemplateItemAreaFieldDisplayType.InputText:
                    startTag = "<input type=\"text\">";
                    endTag = "";
                    break;

                case FormBuilderTemplateItemAreaFieldDisplayType.InputPassword:
                    startTag = "<input type=\"password\">";
                    endTag = "";
                    break;

                case FormBuilderTemplateItemAreaFieldDisplayType.InputCheckbox:
                    startTag = "<input type=\"checkbox\" class=\"hide\">";
                    endTag = "<label>";
                    break;

                case FormBuilderTemplateItemAreaFieldDisplayType.ChkEditor:
                case FormBuilderTemplateItemAreaFieldDisplayType.TextArea:
                    startTag = "<textarea>";
                    endTag = "</textarea>";
                    break;

                case FormBuilderTemplateItemAreaFieldDisplayType.a:
                    startTag = "<a>";
                    endTag = "</a>";
                    break;

                case FormBuilderTemplateItemAreaFieldDisplayType.Hidden:
                    startTag = "<input type=\"hidden\">";
                    endTag = "";
                    break;

                case FormBuilderTemplateItemAreaFieldDisplayType.Unassigned:
                default:
                    startTag = "";
                    endTag = "";
                    break;
            }
        }

        private void DrawElement(FormBuilderTemplateItemFieldEventType formBuilderTemplateItemFieldEventType, out string elementEvent)
        {
            elementEvent = "";

            switch (formBuilderTemplateItemFieldEventType)
            {
                case FormBuilderTemplateItemFieldEventType.OnChange:
                    elementEvent = "OnChange=\"\"";
                    break;

                case FormBuilderTemplateItemFieldEventType.OnClick:
                    elementEvent = "OnClick=\"\"";
                    break;

                case FormBuilderTemplateItemFieldEventType.OnMouseOver:
                    elementEvent = "OnMouseOver=\"\"";
                    break;

                case FormBuilderTemplateItemFieldEventType.OnMouseOut:
                    elementEvent = "OnMouseOut=\"\"";
                    break;

                case FormBuilderTemplateItemFieldEventType.OnKeyDown:
                    elementEvent = "OnKeyDown=\"\"";
                    break;

                case FormBuilderTemplateItemFieldEventType.OnKeyUp:
                    elementEvent = "OnKeyUp=\"\"";
                    break;

                case FormBuilderTemplateItemFieldEventType.OnBlur:
                    elementEvent = "OnBlur=\"\"";
                    break;

                case FormBuilderTemplateItemFieldEventType.Unassigned:
                default:
                    elementEvent = "";
                    break;
            }
        }

        private string DrawElement(FormBuilderTemplateItemArea formBuilderTemplateItemArea, bool addFieldToHidden, int fieldRepeatIndex)
        {
            string formTemplate = "";
            DataProviderResultFilter formBuilderTemplateItemAreaFieldPaging = null;
            List<FormBuilderTemplateItemAreaField> formBuilderTemplateItemAreaFieldList = null;

            formBuilderTemplateItemAreaFieldPaging = new DataProviderResultFilter();
            formBuilderTemplateItemAreaFieldPaging.SetOrder(typeof(FormBuilderTemplateItemAreaField), "SortOrder", OrderFieldDirection.Ascending);

            // Template Item Area Field
            formBuilderTemplateItemAreaFieldList = ProviderBase.Data.Providers.DataProvider.Select<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
            {
                FormBuilderTemplateItemAreaID = formBuilderTemplateItemArea.FormBuilderTemplateItemAreaID
            }, this.Website.WebsiteConnection.ConnectionString, formBuilderTemplateItemAreaFieldPaging);

            if (formBuilderTemplateItemAreaFieldList?.Count > 0)
            {
                List<FormBuilderTemplateItemAreaFieldDisplayType> displayTypes = new List<FormBuilderTemplateItemAreaFieldDisplayType>()
                {
                    FormBuilderTemplateItemAreaFieldDisplayType.Span,
                    FormBuilderTemplateItemAreaFieldDisplayType.ChkEditor,
                    FormBuilderTemplateItemAreaFieldDisplayType.TextArea,
                    FormBuilderTemplateItemAreaFieldDisplayType.a
                };

                List<FormBuilderTemplateItemAreaFieldDisplayType> valueTypes = new List<FormBuilderTemplateItemAreaFieldDisplayType>()
                {
                    FormBuilderTemplateItemAreaFieldDisplayType.InputText,
                    FormBuilderTemplateItemAreaFieldDisplayType.InputPassword,
                    FormBuilderTemplateItemAreaFieldDisplayType.Hidden,
                    FormBuilderTemplateItemAreaFieldDisplayType.InputCheckbox
                };

                foreach (FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField in formBuilderTemplateItemAreaFieldList)
                {
                    string formBuilderTemplateItemAreaFieldStartTag = "";
                    string formBuilderTemplateItemAreaFieldEndTag = "";

                    if (this.FormBuilderUtilityMode != FormBuilderUtilityMode.Designer)
                    {
                        this.DrawElement(formBuilderTemplateItemAreaField, out formBuilderTemplateItemAreaFieldStartTag, out formBuilderTemplateItemAreaFieldEndTag);
                        formTemplate += formBuilderTemplateItemAreaFieldStartTag;
                    }

                    if (formBuilderTemplateItemAreaField.FormBuilderFieldID > 0)
                    {
                        FormBuilderField formBuilderItemField = null;

                        // Form Builder Item Field
                        formBuilderItemField = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderField>(new FormBuilderField()
                        {
                            FormBuilderFieldID = formBuilderTemplateItemAreaField.FormBuilderFieldID
                        }, this.Website.WebsiteConnection.ConnectionString);

                        if (formBuilderItemField?.FormBuilderFieldID > 0)
                        {
                            string formBuilderItemFieldStartTag = "";
                            string formBuilderItemFieldEndTag = "";
                            string fieldValue = "";

                            this.DrawElement(formBuilderItemField, out formBuilderItemFieldStartTag, out formBuilderItemFieldEndTag, formBuilderTemplateItemAreaField, addFieldToHidden, fieldRepeatIndex);

                            if (this.FormBuilderUtilityMode != FormBuilderUtilityMode.Designer)
                            {
                                if (this.FormBuilder.FormBuilderDisplayTypeID == FormBuilderDisplayType.Display)
                                {
                                    if (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.FieldName) == false)
                                    {
                                        fieldValue = $"{formBuilderTemplateItemAreaField.FieldName.ToUpper()}";
                                    }
                                    else if (formBuilderItemField.TableDefinitionFieldID > 0)
                                    {
                                        TableDefinitionField tableDefinitionField = null;

                                        tableDefinitionField = ProviderBase.Data.Providers.DataProvider.SelectSingle<TableDefinitionField>(new TableDefinitionField()
                                        {
                                            TableDefinitionFieldID = formBuilderItemField.TableDefinitionFieldID
                                        }, this.Website.WebsiteConnection.ConnectionString);

                                        if (tableDefinitionField?.TableDefinitionFieldID > 0)
                                        {
                                            fieldValue = $"${tableDefinitionField.FieldName.ToUpper()}$";
                                        }
                                    }
                                    else if (formBuilderItemField.CustomFieldItemID > 0)
                                    {
                                        CustomFieldItem customFieldItem = null;

                                        customFieldItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<CustomFieldItem>(new CustomFieldItem()
                                        {
                                            CustomFieldItemID = formBuilderItemField.CustomFieldItemID
                                        }, this.Website.WebsiteConnection.ConnectionString);

                                        if (customFieldItem?.CustomFieldID > 0)
                                        {
                                            fieldValue = $"${customFieldItem.FieldName.ToUpper()}$";
                                        }
                                    }

                                    if (displayTypes.Contains(formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldDisplayTypeID))
                                    {
                                        formBuilderItemFieldStartTag += fieldValue;
                                    }
                                    else if (valueTypes.Contains(formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldDisplayTypeID))
                                    {
                                        formBuilderItemFieldStartTag = formBuilderItemFieldStartTag.Replace(">", (string.IsNullOrEmpty(fieldValue) ? ">" : $" value=\"{fieldValue}\">"));
                                    }
                                }
                            }

                            formBuilderItemFieldStartTag += formBuilderItemFieldEndTag;

                            formTemplate += formBuilderItemFieldStartTag;
                        }
                    }
                    else if (formBuilderTemplateItemAreaField?.MediaID > 0)
                    {
                        Media media = null;

                        media = ProviderBase.Data.Providers.DataProvider.SelectSingle<Media>(new Media()
                        {
                            MediaID = formBuilderTemplateItemAreaField.MediaID
                        }, this.Website.WebsiteConnection.ConnectionString);

                        if (media?.MediaID > 0)
                        {
                            string formBuilderItemFieldStartTag = "";
                            string formBuilderItemFieldEndTag = "";

                            this.DrawElement(media, out formBuilderItemFieldStartTag, out formBuilderItemFieldEndTag, formBuilderTemplateItemAreaField);

                            formTemplate += formBuilderItemFieldStartTag;
                            formTemplate += formBuilderItemFieldEndTag;
                        }
                    }
                    else
                    {
                        string formBuilderItemFieldStartTag = "";
                        string formBuilderItemFieldEndTag = "";

                        this.DrawElement(new FormBuilderField(), out formBuilderItemFieldStartTag, out formBuilderItemFieldEndTag, formBuilderTemplateItemAreaField, addFieldToHidden, fieldRepeatIndex);

                        formTemplate += formBuilderItemFieldStartTag;

                        if (this.FormBuilderUtilityMode != FormBuilderUtilityMode.Designer)
                        {
                            if (displayTypes.Contains(formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldDisplayTypeID))
                            {
                                formTemplate += (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.FieldName)) ? "" : $"{formBuilderTemplateItemAreaField.FieldName}";
                            }
                            else if (valueTypes.Contains(formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldDisplayTypeID))
                            {
                                formTemplate = formTemplate.Replace(">", (string.IsNullOrEmpty(formBuilderTemplateItemAreaField.FieldName) ? ">" : $" value=\"{formBuilderTemplateItemAreaField.FieldName}\">"));
                            }
                        }

                        formTemplate += formBuilderItemFieldEndTag;
                    }

                    formTemplate += formBuilderTemplateItemAreaFieldEndTag;


                    if (valueTypes.Contains(formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldDisplayTypeID))
                    {
                        fieldRepeatIndex++;
                    }
                }
            }

            return formTemplate;
        }

        private string DrawElement(int formBuilderTemplateItemAreaID, bool addFieldToHidden, int fieldRepeatIndex)
        {
            string formTemplate = "";

            if (formBuilderTemplateItemAreaID > 0)
            {
                DataProviderResultFilter formBuilderTemplateItemAreaParentPaging = null;
                List<FormBuilderTemplateItemArea> formBuilderTemplateItemAreaParentList = null;

                formBuilderTemplateItemAreaParentPaging = new DataProviderResultFilter();
                formBuilderTemplateItemAreaParentPaging.SetOrder(typeof(FormBuilderTemplateItemArea), "SortOrder", OrderFieldDirection.Ascending);

                // Template Item Area Field Parent ID
                formBuilderTemplateItemAreaParentList = ProviderBase.Data.Providers.DataProvider.Select<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                {
                    ParentID = formBuilderTemplateItemAreaID
                }, this.Website.WebsiteConnection.ConnectionString, formBuilderTemplateItemAreaParentPaging);

                if (formBuilderTemplateItemAreaParentList?.Count > 0)
                {
                    foreach (FormBuilderTemplateItemArea formBuilderTemplateItemAreaParent in formBuilderTemplateItemAreaParentList)
                    {
                        string formBuilderTemplateItemAreaParentStartTag = "";
                        string formBuilderTemplateItemAreaParentEndTag = "";
                        bool addFieldToHiddenTemp = false;

                        this.DrawElement(formBuilderTemplateItemAreaParent, out formBuilderTemplateItemAreaParentStartTag, out formBuilderTemplateItemAreaParentEndTag, out addFieldToHiddenTemp);

                        addFieldToHiddenTemp = (addFieldToHidden == true || addFieldToHiddenTemp == true);

                        formTemplate += formBuilderTemplateItemAreaParentStartTag;

                        formTemplate += this.DrawElement(formBuilderTemplateItemAreaParent, addFieldToHiddenTemp, fieldRepeatIndex);

                        formTemplate += this.DrawElement(formBuilderTemplateItemAreaParent.FormBuilderTemplateItemAreaID, addFieldToHiddenTemp, fieldRepeatIndex);

                        formTemplate += formBuilderTemplateItemAreaParentEndTag;

                        fieldRepeatIndex++;
                    }
                }
            }

            return formTemplate;
        }

        private string DrawElementHidden(FormBuilderTemplate formBuilder)
        {
            string element = "";

            if (this.FormBuilderUtilityMode == FormBuilderUtilityMode.Designer)
            {
                string elementTemp = "";

                elementTemp = this.FormBuilderTemplateHidden.Replace("$KEY$", "FormBuilder_GUID");
                elementTemp = elementTemp.Replace("$VALUE$", this.FormBuilder.GUID.ToString());

                element += elementTemp;

                if (this.HiddenFieldList?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> pair in this.HiddenFieldList)
                    {
                        elementTemp = this.FormBuilderTemplateHidden.Replace("$KEY$", pair.Key);
                        elementTemp = this.FormBuilderTemplateHidden.Replace("$VALUE$", pair.Value);

                        element += elementTemp;
                    }
                }
            }
            else
            {
                element += $"<input type=\"hidden\" id=\"FormBuilder_GUID\" name=\"FormBuilder_GUID\" value=\"{this.FormBuilder.GUID}\" />";

                if (this.HiddenFieldList?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> pair in this.HiddenFieldList)
                    {
                        element += $"<input type=\"hidden\" id=\"{pair.Key}\" name=\"{pair.Key}\" value=\"{pair.Value}\" />";
                    }
                }
            }

            return element;
        }

        private string GetTemplateFile(FormBuilderTemplate formBuilderTemplate)
        {
            if (this.FormBuilderUtilityMode == FormBuilderUtilityMode.Designer)
            {
                return "";
            }
            else
            {
                string template = "";
                string filePath = "";

                filePath = this.Website.WebsiteTemplateFolderPath + formBuilderTemplate.TemplateFileName;

                if (File.Exists(ProviderBase.Web.Utility.GetResourcePath(filePath)))
                {
                    DateTime fileWriteTime = new DateTime(1900, 1, 1);

                    fileWriteTime = File.GetLastWriteTime(ProviderBase.Web.Utility.GetResourcePath(filePath));

                    if (fileWriteTime > formBuilderTemplate.ModifyDate)
                    {
                        template = ProviderBase.Web.Utility.GetResourceHtml(filePath);
                    }
                }

                return template;
            }
        }

        private void SaveTemplateFile(FormBuilderTemplate formBuilderTemplate, string template)
        {
            if (this.FormBuilderUtilityMode != FormBuilderUtilityMode.Designer)
            {
                string filePath = "";

                filePath = this.Website.WebsiteTemplateFolderPath + formBuilderTemplate.TemplateFileName;

                if (File.Exists(ProviderBase.Web.Utility.GetResourcePath(filePath)))
                {
                    DateTime fileWriteTime = new DateTime(1900, 1, 1);

                    fileWriteTime = File.GetLastWriteTime(filePath);

                    if (fileWriteTime > formBuilderTemplate.ModifyDate)
                    {
                        return;
                    }
                }

                ProviderBase.Web.Utility.SaveResourceHtml(filePath, template);
            }
        }
    }
}