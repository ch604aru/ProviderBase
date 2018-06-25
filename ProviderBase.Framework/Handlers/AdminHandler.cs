using ProviderBase.Data.Entities;
using ProviderBase.Data.Providers;
using ProviderBase.Framework.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProviderBase.Framework.Handlers
{
    public enum FormBuilderTemplateItemAreaFieldMode
    {
        Unassigned = 0,
        Table = 1,
        Object = 2,
        Custom = 3
    }

    public class AdminHandler : BaseHandler
    {
        public void AdminIndexGet()
        {
            string reportGUID = "";

            reportGUID = ProviderBase.Framework.Utility.GetFormValue<string>("ReportGUID", "f205cde4-ddb0-4920-9ae6-2cfaba164d45");

            if (string.IsNullOrEmpty(reportGUID) == false)
            {
                List<FormBuilder> formBuilderList = null;

                formBuilderList = DataProvider.SelectAll(new FormBuilder(), this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderList?.Count > 0)
                {
                    ReportUtility reportUtility = null;
                    string templateFinal = "";

                    reportUtility = new ReportUtility(this.Website, reportGUID);

                    templateFinal = reportUtility.Render(formBuilderList);

                    this.AjaxResult.Data.Add(templateFinal);
                    this.AjaxResult.Message = "Get Form Builder success";
                    this.AjaxResult.Status = AjaxResultStatus.Success;
                }
                else
                {
                    this.AjaxResult.Message = "No Form Builder found";
                    this.AjaxResult.Status = AjaxResultStatus.Unassigned;
                }
            }
            else
            {
                this.AjaxResult.Message = "No report found";
                this.AjaxResult.Status = AjaxResultStatus.Unassigned;
            }
        }

        public void FormBuilderEditItemGet()
        {
            string templateNameArea = "ProviderBase/ProviderBaseAdminFormBuilderEditItem.htm";
            string template = "";

            template = ProviderBase.Web.Utility.GetResourceHtml(templateNameArea);

            if (string.IsNullOrEmpty(template) == false)
            {
                this.AjaxResult.Data.Add(template);
                this.AjaxResult.Message = "Get Form Builder Edit Area Success";
                this.AjaxResult.Status = AjaxResultStatus.Success;
            }
            else
            {
                this.AjaxResult.Message = "No Template found";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void FormBuilderEditItemAreaGet()
        {
            string templateNameItemArea = "ProviderBase/ProviderBaseAdminFormBuilderEditItemArea.htm";
            string template = "";

            template = ProviderBase.Web.Utility.GetResourceHtml(templateNameItemArea);

            if (string.IsNullOrEmpty(template) == false)
            {
                string templateItemAreaRepeat = "";
                string templateRepeatTemp = "";
                List<FormBuilderTemplateItemAreaType> formBuilderTemplateItemAreaTypeList = null;

                formBuilderTemplateItemAreaTypeList = ProviderBase.Data.Utility.GetEnumValues<FormBuilderTemplateItemAreaType>();

                templateItemAreaRepeat = ProviderBase.Data.Utility.GetTemplateFileElementSingle(template, "repeat", "repeat");
                template = ProviderBase.Data.Utility.ReplaceTemplateFileElementSingle(template, "repeat", "repeat");

                foreach (FormBuilderTemplateItemAreaType formBuilderTemplateItemAreaType in formBuilderTemplateItemAreaTypeList)
                {
                    templateRepeatTemp += ProviderBase.Data.Utility.TemplateBindData(templateItemAreaRepeat, formBuilderTemplateItemAreaType);
                }

                template = template.Replace("$repeat$", templateRepeatTemp);

                this.AjaxResult.Data.Add(template);
                this.AjaxResult.Message = "Get Form Builder Edit Area Success";
                this.AjaxResult.Status = AjaxResultStatus.Success;
            }
            else
            {
                this.AjaxResult.Message = "No Template found";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void FormBuilderEditDesignerGet()
        {
            Guid formBuilderGuid = ProviderBase.Framework.Utility.GetFormValue("FormBuilder_GUID", Guid.Empty);

            if (formBuilderGuid != Guid.Empty)
            {
                FormBuilder formBuilder = null;

                formBuilder = ProviderBase.Data.Providers.DataProvider.SelectSingle(new FormBuilder()
                {
                    GUID = formBuilderGuid
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilder?.FormBuilderID > 0)
                {

                    string template = "";
                    FormBuilderUtility formBuilderUtility = null;

                    formBuilderUtility = new FormBuilderUtility(this.Website, formBuilder);

                    template = formBuilderUtility.RenderDesigner();

                    this.AjaxResult.Data.Add(template);
                    this.AjaxResult.Message = "Get Form Builder Edit Designer Success";
                    this.AjaxResult.Status = AjaxResultStatus.Success;
                }
                else
                {
                    this.AjaxResult.Message = "No Form Builder Found";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
            else
            {
                this.AjaxResult.Message = "No Form Builder GUID Supplied";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void FormBuilderEditFieldTableGet()
        {
            string templateNameArea = "ProviderBase/ProviderBaseAdminFormBuilderEditFieldTable.htm";
            string template = "";

            template = ProviderBase.Web.Utility.GetResourceHtml(templateNameArea);

            if (string.IsNullOrEmpty(template) == false)
            {
                int pageCurrent = ProviderBase.Framework.Utility.GetFormValue<int>("PageCurrent", 1);
                int pageSize = ProviderBase.Framework.Utility.GetFormValue<int>("PageSize", 20);
                string search = ProviderBase.Framework.Utility.GetFormValue<string>("Search", "");
                List<TableDefinition> tableDefinitionList = null;
                DataProviderResultFilter tableDefinitionPaging = null;
                PagingItem pagingItem = null;

                pagingItem = ProviderBase.Data.Utility.XMLDeseralize<PagingItem>(this.Context.Server.MapPath(@"/Resource/XML/") + "Conquest/ConquestGamingPaging.xml");

                tableDefinitionPaging = new DataProviderResultFilter();

                tableDefinitionPaging.SetPaging(typeof(TableDefinition), pageCurrent, pageSize);
                tableDefinitionPaging.SetFilter(typeof(TableDefinition), "Title", search);

                tableDefinitionList = ProviderBase.Data.Providers.DataProvider.SelectAll<TableDefinition>(new TableDefinition()
                {

                }, this.Website.WebsiteConnection.ConnectionString, tableDefinitionPaging);

                template = ProviderBase.Data.Utility.TemplateBindDataList<TableDefinition>(template, tableDefinitionList, typeof(TableDefinitionField), this.Website.WebsiteConnection.ConnectionString);

                template = template.Replace("$PAGING$", Utility.GeneratePaging(pagingItem, tableDefinitionList[0].PageCurrent, tableDefinitionList[0].PageTotal));
                template = template.Replace("$SEARCH$", search);

                this.AjaxResult.Data.Add(template);
                this.AjaxResult.Message = "Get Form Builder Edit Field Success";
                this.AjaxResult.Status = AjaxResultStatus.Success;
            }
            else
            {
                this.AjaxResult.Message = "No Template found";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void FormBuilderEditFieldObjectGet()
        {
            /*
            string templateNameArea = "ProviderBase/ProviderBaseAdminFormBuilderEditField.htm";
            string template = "";

            template = ProviderBase.Web.Utility.GetResourceHtml(templateNameArea);

            if (string.IsNullOrEmpty(template) == false)
            {
                int pageCurrent = ProviderBase.Framework.Utility.GetFormValue<int>("PageCurrent", 1);
                int pageSize = ProviderBase.Framework.Utility.GetFormValue<int>("PageSize", 20);
                string search = ProviderBase.Framework.Utility.GetFormValue<string>("Search", "");
                List<ObjectDefinition> objectDefinitionList = null;
                DataProviderResultFilter objectDefinitionPaging = null;
                PagingItem pagingItem = null;

                pagingItem = ProviderBase.Data.Utility.XMLDeseralize<PagingItem>(this.Context.Server.MapPath(@"/Resource/XML/") + "Conquest/ConquestGamingPaging.xml");

                objectDefinitionPaging = new DataProviderResultFilter();

                objectDefinitionPaging.SetPaging(typeof(TableDefinition), pageCurrent, pageSize);
                objectDefinitionPaging.SetFilter(typeof(TableDefinition), "Title", search);

                objectDefinitionList = ProviderBase.Data.Providers.DataProvider.SelectAll<ObjectDefinition>(new ObjectDefinition()
                {

                }, this.Website.WebsiteConnection.ConnectionString, objectDefinitionPaging);

                template = ProviderBase.Data.Utility.TemplateBindDataList<ObjectDefinition>(template, objectDefinitionList, typeof(ObjectDefinition), this.Website.WebsiteConnection.ConnectionString);

                template = template.Replace("$PAGING$", Utility.GeneratePaging(pagingItem, objectDefinitionList[0].PageCurrent, objectDefinitionList[0].PageTotal));
                template = template.Replace("$SEARCH$", search);

                this.AjaxResult.Data.Add(template);
                this.AjaxResult.Message = "Get Form Builder Edit Field Success";
                this.AjaxResult.Status = AjaxResultStatus.Success;
            }
            else
            {
                this.AjaxResult.Message = "No Template found";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
            */

            this.AjaxResult.Message = "NOT YET IMPLEMENTED";
            this.AjaxResult.Status = AjaxResultStatus.Success;
        }

        public void FormBuilderEditFieldCustomGet()
        {
            string templateNameArea = "ProviderBase/ProviderBaseAdminFormBuilderEditFieldCustom.htm";
            string template = "";

            template = ProviderBase.Web.Utility.GetResourceHtml(templateNameArea);

            if (string.IsNullOrEmpty(template) == false)
            {
                int pageCurrent = ProviderBase.Framework.Utility.GetFormValue<int>("PageCurrent", 1);
                int pageSize = ProviderBase.Framework.Utility.GetFormValue<int>("PageSize", 20);
                string search = ProviderBase.Framework.Utility.GetFormValue<string>("Search", "");
                List<CustomField> customFieldList = null;
                DataProviderResultFilter customFieldPaging = null;
                PagingItem pagingItem = null;

                pagingItem = ProviderBase.Data.Utility.XMLDeseralize<PagingItem>(this.Context.Server.MapPath(@"/Resource/XML/") + "Conquest/ConquestGamingPaging.xml");

                customFieldPaging = new DataProviderResultFilter();

                customFieldPaging.SetPaging(typeof(TableDefinition), pageCurrent, pageSize);
                customFieldPaging.SetFilter(typeof(TableDefinition), "Title", search);

                customFieldList = ProviderBase.Data.Providers.DataProvider.SelectAll<CustomField>(new CustomField()
                {

                }, this.Website.WebsiteConnection.ConnectionString, customFieldPaging);

                template = ProviderBase.Data.Utility.TemplateBindDataList<CustomField>(template, customFieldList, typeof(CustomFieldItem), this.Website.WebsiteConnection.ConnectionString);

                template = template.Replace("$PAGING$", Utility.GeneratePaging(pagingItem, customFieldList[0].PageCurrent, customFieldList[0].PageTotal));
                template = template.Replace("$SEARCH$", search);

                this.AjaxResult.Data.Add(template);
                this.AjaxResult.Message = "Get Form Builder Edit Field Success";
                this.AjaxResult.Status = AjaxResultStatus.Success;
            }
            else
            {
                this.AjaxResult.Message = "No Template found";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void FormBuilderTemplateItemCreate()
        {
            int formBuilderTemplateID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateID", 0);

            if (formBuilderTemplateID > 0)
            {
                FormBuilderTemplateItem formBuilderTemplateItem = null;
                int formBuilderTemplateItemCount = 0;

                formBuilderTemplateItemCount = ProviderBase.Data.Providers.DataProvider.SelectCount<FormBuilderTemplateItem>(new FormBuilderTemplateItem()
                {
                    FormBuilderTemplateID = formBuilderTemplateID
                }, this.Website.WebsiteConnection.ConnectionString);

                formBuilderTemplateItem = new FormBuilderTemplateItem()
                {
                    FormBuilderTemplateID = formBuilderTemplateID,
                    SortOrder = (formBuilderTemplateItemCount + 1),
                };

                formBuilderTemplateItem.FormBuilderTemplateItemID = ProviderBase.Data.Providers.DataProvider.Insert<FormBuilderTemplateItem>(formBuilderTemplateItem, this.Website.WebsiteConnection.ConnectionString);

                FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItem));

                this.AjaxResult.Status = AjaxResultStatus.Success;
                this.AjaxResult.Message = "Form Builder Template Item Create Success";
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No Form Builder Template ID supplied";
            }
        }

        public void FormBuilderTemplateItemDelete()
        {
            int formBuilderTemplateItemID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemID", 0);

            if (formBuilderTemplateItemID > 0)
            {
                FormBuilderTemplateItem formBuilderTemplateItem = null;
                List<FormBuilderTemplateItemArea> formBuilderTemplateItemAreaList = null;

                formBuilderTemplateItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItem>(new FormBuilderTemplateItem()
                {
                    FormBuilderTemplateItemID = formBuilderTemplateItemID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderTemplateItem?.FormBuilderTemplateItemID > 0)
                {
                    formBuilderTemplateItemAreaList = ProviderBase.Data.Providers.DataProvider.Select<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                    {
                        FormBuilderTemplateItemID = formBuilderTemplateItem.FormBuilderTemplateItemID
                    }, this.Website.WebsiteConnection.ConnectionString);

                    foreach (FormBuilderTemplateItemArea formBuilderTemplateItemArea in formBuilderTemplateItemAreaList)
                    {
                        this.FormBuilderTemplateItemAreaDelete(formBuilderTemplateItemArea.FormBuilderTemplateItemAreaID);
                    }

                    ProviderBase.Data.Providers.DataProvider.Delete<FormBuilderTemplateItem>(formBuilderTemplateItem, this.Website.WebsiteConnection.ConnectionString);

                    FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItem));

                    this.AjaxResult.Status = AjaxResultStatus.Success;
                    this.AjaxResult.Message = "Form Builder Template Item Create Success";
                }
                else
                {
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                    this.AjaxResult.Message = "Form Builder Template Item Not Found";
                }
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No Form Builder Template ID supplied";
            }
        }

        public void FormBuilderTemplateItemMoveUp()
        {
            int formBuilderTemplateItemID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemID", 0);

            if (formBuilderTemplateItemID > 0)
            {
                FormBuilderTemplateItem formBuilderTemplateItem = null;

                formBuilderTemplateItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItem>(new FormBuilderTemplateItem()
                {
                    FormBuilderTemplateItemID = formBuilderTemplateItemID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderTemplateItem?.FormBuilderTemplateItemID > 0)
                {
                    FormBuilderTemplateItem formBuilderTemplateItemTemp = null;

                    formBuilderTemplateItemTemp = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItem>(new FormBuilderTemplateItem()
                    {
                        FormBuilderTemplateID = formBuilderTemplateItem.FormBuilderTemplateID,
                        SortOrder = (formBuilderTemplateItem.SortOrder - 1)
                    }, this.Website.WebsiteConnection.ConnectionString);

                    if (formBuilderTemplateItemTemp?.FormBuilderTemplateItemID > 0)
                    {
                        formBuilderTemplateItemTemp.SortOrder += 1;
                        ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItem>(formBuilderTemplateItemTemp, this.Website.WebsiteConnection.ConnectionString);
                    }

                    formBuilderTemplateItem.SortOrder -= 1;
                    ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItem>(formBuilderTemplateItem, this.Website.WebsiteConnection.ConnectionString);

                    FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItem));

                    this.AjaxResult.Status = AjaxResultStatus.Success;
                    this.AjaxResult.Message = "Form Builder Template Item Create Success";
                }
                else
                {
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                    this.AjaxResult.Message = "Could not find Form Builder Template Item";
                }
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No Form builder Template ID supplied";
            }
        }

        public void FormBuilderTemplateItemMoveDown()
        {
            int formBuilderTemplateItemID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemID", 0);

            if (formBuilderTemplateItemID > 0)
            {
                FormBuilderTemplateItem formBuilderTemplateItem = null;

                formBuilderTemplateItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItem>(new FormBuilderTemplateItem()
                {
                    FormBuilderTemplateItemID = formBuilderTemplateItemID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderTemplateItem?.FormBuilderTemplateItemID > 0)
                {
                    FormBuilderTemplateItem formBuilderTemplateItemTemp = null;

                    formBuilderTemplateItemTemp = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItem>(new FormBuilderTemplateItem()
                    {
                        FormBuilderTemplateID = formBuilderTemplateItem.FormBuilderTemplateID,
                        SortOrder = (formBuilderTemplateItem.SortOrder + 1)
                    }, this.Website.WebsiteConnection.ConnectionString);

                    if (formBuilderTemplateItemTemp?.FormBuilderTemplateItemID > 0)
                    {
                        formBuilderTemplateItemTemp.SortOrder -= 1;
                        ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItem>(formBuilderTemplateItemTemp, this.Website.WebsiteConnection.ConnectionString);
                    }

                    formBuilderTemplateItem.SortOrder += 1;
                    ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItem>(formBuilderTemplateItem, this.Website.WebsiteConnection.ConnectionString);

                    FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItem));

                    this.AjaxResult.Status = AjaxResultStatus.Success;
                    this.AjaxResult.Message = "Form Builder Template Item Create Success";
                }
                else
                {
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                    this.AjaxResult.Message = "Could not find Form Builder Template Item";
                }
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No Form builder Template ID supplied";
            }
        }

        public void FormBuilderTemplateItemEdit()
        {
            int formBuilderTemplateItemID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemID", 0);

            if (formBuilderTemplateItemID > 0)
            {
                string templateNameItemArea = "ProviderBase/ProviderBaseAdminFormBuilderTemplateItemEdit.htm";
                string template = "";

                template = ProviderBase.Web.Utility.GetResourceHtml(templateNameItemArea);

                if (string.IsNullOrEmpty(template) == false)
                {
                    FormBuilderTemplateItem formBuilderTemplateItem = null;

                    formBuilderTemplateItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItem>(new FormBuilderTemplateItem()
                    {
                        FormBuilderTemplateItemID = formBuilderTemplateItemID
                    }, this.Website.WebsiteConnection.ConnectionString);

                    if (formBuilderTemplateItem?.FormBuilderTemplateItemID > 0)
                    {
                        template = ProviderBase.Data.Utility.TemplateBindData(template, formBuilderTemplateItem);

                        this.AjaxResult.Data.Add(template);
                        this.AjaxResult.Status = AjaxResultStatus.Success;
                        this.AjaxResult.Message = "Form Builder Template Item Edit Success";
                    }
                }
                else
                {
                    this.AjaxResult.Message = "No Template found";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
            else
            {
                this.AjaxResult.Message = "No Form Builder Template Item ID found";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void FormBuilderTemplateItemSave()
        {
            int formBuilderTemplateItemID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItem_FormBuilderTemplateItemID", 0);

            if (formBuilderTemplateItemID > 0)
            {
                FormBuilderTemplateItem formBuilderTemplateItem = null;

                formBuilderTemplateItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItem>(new FormBuilderTemplateItem()
                {
                    FormBuilderTemplateItemID = formBuilderTemplateItemID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderTemplateItem?.FormBuilderTemplateItemID > 0)
                {
                    formBuilderTemplateItem = ProviderBase.Framework.Utility.BindFormValues<FormBuilderTemplateItem>(formBuilderTemplateItem, "FormBuilderTemplateItem_", DataProviderKeyType.PrimaryKey);

                    ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItem>(formBuilderTemplateItem, this.Website.WebsiteConnection.ConnectionString);

                    FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItem));

                    this.AjaxResult.Message = "Form Builder Template Item Save Success";
                    this.AjaxResult.Status = AjaxResultStatus.Success;
                }
                else
                {
                    this.AjaxResult.Message = "No Form Builder Template Item found";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
            else
            {
                this.AjaxResult.Message = "No Form Builder Template Item ID found";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void FormBuilderTemplateItemAreaCreate()
        {
            int formBuilderTemplateItemID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemID", 0);
            int formBuilderTemplateItemAreaID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaID", 0);
            int formBuilderTemplateItemAreaTypeID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaTypeID", 0);

            if (formBuilderTemplateItemAreaTypeID > 0)
            {
                if (formBuilderTemplateItemID > 0)
                {
                    FormBuilderTemplateItemArea formBuilderTemplateItemArea = null;
                    int formBuilderTemplateItemAreaCount = 0;

                    formBuilderTemplateItemAreaCount = ProviderBase.Data.Providers.DataProvider.SelectCount<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                    {
                        FormBuilderTemplateItemID = formBuilderTemplateItemID,
                        ParentID = formBuilderTemplateItemAreaID
                    }, this.Website.WebsiteConnection.ConnectionString);

                    formBuilderTemplateItemArea = new FormBuilderTemplateItemArea()
                    {
                        FormBuilderTemplateItemID = formBuilderTemplateItemID,
                        FormBuilderTemplateItemAreaTypeID = (FormBuilderTemplateItemAreaType)formBuilderTemplateItemAreaTypeID,
                        SortOrder = (formBuilderTemplateItemAreaCount + 1),
                        ParentID = formBuilderTemplateItemAreaID
                    };

                    formBuilderTemplateItemArea.FormBuilderTemplateItemAreaID = ProviderBase.Data.Providers.DataProvider.Insert<FormBuilderTemplateItemArea>(formBuilderTemplateItemArea, this.Website.WebsiteConnection.ConnectionString);

                    FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItemArea));

                    this.AjaxResult.Status = AjaxResultStatus.Success;
                    this.AjaxResult.Message = "Form Builder Template Item Area Create Success";
                }
                else
                {
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                    this.AjaxResult.Message = "No Form Builder Template Item ID supplied";
                }
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No Form Builder Template Item Area Type ID supplied";
            }
        }

        public void FormBuilderTemplateItemAreaDelete()
        {
            int formBuilderTemplateItemAreaID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaID", 0);

            if (formBuilderTemplateItemAreaID > 0)
            {
                FormBuilderTemplateItemArea formBuilderTemplateItemArea = null;

                formBuilderTemplateItemArea = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                {
                    FormBuilderTemplateItemAreaID = formBuilderTemplateItemAreaID
                }, this.Website.WebsiteConnection.ConnectionString);

                this.FormBuilderTemplateItemAreaDelete(formBuilderTemplateItemAreaID);

                FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItemArea));

                this.AjaxResult.Status = AjaxResultStatus.Success;
                this.AjaxResult.Message = "Form Builder Template Item Area Create Success";
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No Form Builder Template Item Area Type ID supplied";
            }
        }

        public void FormBuilderTemplateItemAreaMoveUp()
        {
            int formBuilderTemplateItemAreaID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaID", 0);

            if (formBuilderTemplateItemAreaID > 0)
            {
                FormBuilderTemplateItemArea formBuilderTemplateItemArea = null;

                formBuilderTemplateItemArea = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                {
                    FormBuilderTemplateItemAreaID = formBuilderTemplateItemAreaID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderTemplateItemArea?.FormBuilderTemplateItemAreaID > 0)
                {
                    FormBuilderTemplateItemArea formBuilderTemplateItemAreaTemp = null;

                    formBuilderTemplateItemAreaTemp = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                    {
                        FormBuilderTemplateItemID = formBuilderTemplateItemArea.FormBuilderTemplateItemID,
                        SortOrder = (formBuilderTemplateItemArea.SortOrder - 1)
                    }, this.Website.WebsiteConnection.ConnectionString);

                    if (formBuilderTemplateItemAreaTemp?.FormBuilderTemplateItemAreaID > 0)
                    {
                        formBuilderTemplateItemAreaTemp.SortOrder += 1;
                        ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItemArea>(formBuilderTemplateItemAreaTemp, this.Website.WebsiteConnection.ConnectionString);
                    }

                    formBuilderTemplateItemArea.SortOrder -= 1;
                    ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItemArea>(formBuilderTemplateItemArea, this.Website.WebsiteConnection.ConnectionString);

                    FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItemArea));

                    this.AjaxResult.Status = AjaxResultStatus.Success;
                    this.AjaxResult.Message = "Form Builder Template Item Create Success";
                }
                else
                {
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                    this.AjaxResult.Message = "Could not find Form Builder Template Item";
                }
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No Form Builder Template ID supplied";
            }
        }

        public void FormBuilderTemplateItemAreaMoveDown()
        {
            int formBuilderTemplateItemAreaID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaID", 0);

            if (formBuilderTemplateItemAreaID > 0)
            {
                FormBuilderTemplateItemArea formBuilderTemplateItemArea = null;

                formBuilderTemplateItemArea = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                {
                    FormBuilderTemplateItemAreaID = formBuilderTemplateItemAreaID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderTemplateItemArea?.FormBuilderTemplateItemID > 0)
                {
                    FormBuilderTemplateItemArea formBuilderTemplateItemAreaTemp = null;

                    formBuilderTemplateItemAreaTemp = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                    {
                        FormBuilderTemplateItemID = formBuilderTemplateItemArea.FormBuilderTemplateItemID,
                        SortOrder = (formBuilderTemplateItemArea.SortOrder + 1)
                    }, this.Website.WebsiteConnection.ConnectionString);

                    if (formBuilderTemplateItemAreaTemp?.FormBuilderTemplateItemID > 0)
                    {
                        formBuilderTemplateItemAreaTemp.SortOrder -= 1;
                        ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItemArea>(formBuilderTemplateItemAreaTemp, this.Website.WebsiteConnection.ConnectionString);
                    }

                    formBuilderTemplateItemArea.SortOrder += 1;
                    ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItemArea>(formBuilderTemplateItemArea, this.Website.WebsiteConnection.ConnectionString);

                    FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItemArea));

                    this.AjaxResult.Status = AjaxResultStatus.Success;
                    this.AjaxResult.Message = "Form Builder Template Item Create Success";
                }
                else
                {
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                    this.AjaxResult.Message = "Could not find Form Builder Template Item";
                }
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No Form Builder Template ID supplied";
            }
        }

        public void FormBuilderTemplateItemAreaEdit()
        {
            int formBuilderTemplateItemAreaID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaID", 0);

            if (formBuilderTemplateItemAreaID > 0)
            {
                string templateNameItemArea = "ProviderBase/ProviderBaseAdminFormBuilderTemplateItemAreaEdit.htm";
                string template = "";

                template = ProviderBase.Web.Utility.GetResourceHtml(templateNameItemArea);

                if (string.IsNullOrEmpty(template) == false)
                {
                    FormBuilderTemplateItemArea formBuilderTemplateItemArea = null;

                    formBuilderTemplateItemArea = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                    {
                        FormBuilderTemplateItemAreaID = formBuilderTemplateItemAreaID
                    }, this.Website.WebsiteConnection.ConnectionString);

                    if (formBuilderTemplateItemArea?.FormBuilderTemplateItemAreaID > 0)
                    {
                        template = ProviderBase.Data.Utility.TemplateBindData(template, formBuilderTemplateItemArea);

                        this.AjaxResult.Data.Add(template);
                        this.AjaxResult.Status = AjaxResultStatus.Success;
                        this.AjaxResult.Message = "Form Builder Template Item Edit Success";
                    }
                }
                else
                {
                    this.AjaxResult.Message = "No Template found";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
            else
            {
                this.AjaxResult.Message = "No Form Builder Template Item Area ID found";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void FormBuilderTemplateItemAreaSave()
        {
            int formBuilderTemplateItemAreaID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemArea_FormBuilderTemplateItemAreaID", 0);

            if (formBuilderTemplateItemAreaID > 0)
            {
                FormBuilderTemplateItemArea formBuilderTemplateItemArea = null;

                formBuilderTemplateItemArea = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                {
                    FormBuilderTemplateItemAreaID = formBuilderTemplateItemAreaID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderTemplateItemArea?.FormBuilderTemplateItemAreaID > 0)
                {
                    formBuilderTemplateItemArea = ProviderBase.Framework.Utility.BindFormValues<FormBuilderTemplateItemArea>(formBuilderTemplateItemArea, "FormBuilderTemplateItemArea_", DataProviderKeyType.PrimaryKey);

                    ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItemArea>(formBuilderTemplateItemArea, this.Website.WebsiteConnection.ConnectionString);

                    FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItemArea));

                    this.AjaxResult.Message = "Form Builder Template Item Area Save Success";
                    this.AjaxResult.Status = AjaxResultStatus.Success;
                }
                else
                {
                    this.AjaxResult.Message = "No Form Builder Template Item Area found";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
            else
            {
                this.AjaxResult.Message = "No Form Builder Template Item Area ID found";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void FormBuilderTemplateItemAreaFieldCreate()
        {
            int formBuilderTemplateItemAreaID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaID", 0);
            int formBuilderTemplateItemAreaFieldID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaFieldID", 0); // parentID + 1
            string formBuilderTableDefinitionFieldIDString = ProviderBase.Framework.Utility.GetFormValue<string>("FormBuilderTableDefinitionFieldID", "");
            FormBuilderTemplateItemAreaFieldMode formBuilderTemplateItemAreaFieldMode = ProviderBase.Framework.Utility.GetFormValue<FormBuilderTemplateItemAreaFieldMode>("FormBuilderTemplateItemAreaFieldMode", FormBuilderTemplateItemAreaFieldMode.Unassigned);

            if (formBuilderTemplateItemAreaID > 0)
            {
                List<int> formBuilderTableDefinitionFieldIDList = null;

                formBuilderTableDefinitionFieldIDList = ProviderBase.Data.Utility.IntFromStringList(formBuilderTableDefinitionFieldIDString, ',');

                if (formBuilderTableDefinitionFieldIDList?.Count > 0)
                {
                    FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaFieldSortOrder = null;

                    if (formBuilderTemplateItemAreaFieldID > 0)
                    {
                        formBuilderTemplateItemAreaFieldSortOrder = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
                        {
                            FormBuilderTemplateItemAreaFieldID = formBuilderTemplateItemAreaFieldID
                        }, this.Website.WebsiteConnection.ConnectionString);
                    }
                    else
                    {
                        formBuilderTemplateItemAreaFieldSortOrder = new FormBuilderTemplateItemAreaField();
                    }

                    for (int i = 0; i < formBuilderTableDefinitionFieldIDList.Count; i++)
                    {
                        FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField = null;
                        FormBuilderField formBuilderField = null;

                        formBuilderField = new FormBuilderField()
                        {
                            Title = "",
                            FormBuilderFieldValidationID = 0
                        };

                        switch (formBuilderTemplateItemAreaFieldMode)
                        {
                            case FormBuilderTemplateItemAreaFieldMode.Table:
                                formBuilderField.TableDefinitionFieldID = formBuilderTableDefinitionFieldIDList[i];
                                break;

                            case FormBuilderTemplateItemAreaFieldMode.Object:
                                formBuilderField.ObjectDefinitionID = formBuilderTableDefinitionFieldIDList[i];
                                break;

                            case FormBuilderTemplateItemAreaFieldMode.Custom:
                                formBuilderField.CustomFieldItemID = formBuilderTableDefinitionFieldIDList[i];
                                break;
                        }

                        formBuilderField.FormBuilderFieldID = ProviderBase.Data.Providers.DataProvider.Insert<FormBuilderField>(formBuilderField, this.Website.WebsiteConnection.ConnectionString);

                        formBuilderTemplateItemAreaField = new FormBuilderTemplateItemAreaField()
                        {
                            Title = "",
                            FormBuilderTemplateItemAreaID = formBuilderTemplateItemAreaID,
                            FormBuilderFieldID = formBuilderField.FormBuilderFieldID,
                            FormBuilderTemplateItemAreaFieldDisplayTypeID = FormBuilderTemplateItemAreaFieldDisplayType.Unassigned,
                            SortOrder = ((formBuilderTemplateItemAreaFieldSortOrder.SortOrder + i) + 1),
                            Class = "",
                            Style = "",
                            FieldName = "",
                            MediaID = 0,
                            FormBuilderTemplateItemFieldEventTypeID = FormBuilderTemplateItemFieldEventType.Unassigned,
                            FieldEvent = ""
                        };

                        if (formBuilderTemplateItemAreaFieldSortOrder?.FormBuilderTemplateItemAreaFieldID > 0)
                        {
                            FormBuilderTemplateItemAreaFieldSort(formBuilderTemplateItemAreaID, ((formBuilderTemplateItemAreaFieldSortOrder.SortOrder + i) + 1));
                        }

                        formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldID = ProviderBase.Data.Providers.DataProvider.Insert<FormBuilderTemplateItemAreaField>(formBuilderTemplateItemAreaField, this.Website.WebsiteConnection.ConnectionString);

                        if (i == formBuilderTableDefinitionFieldIDList.Count)
                        {
                            FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItemAreaField));
                        }
                    }

                    this.AjaxResult.Status = AjaxResultStatus.Success;
                    this.AjaxResult.Message = "Form Builder Template Item Area Field Create Success";
                }
                else
                {
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                    this.AjaxResult.Message = "No Form Builder Table Definition Field ID supplied";
                }
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No Form Builder Template Item Area ID supplied";
            }
        }

        public void FormBuilderTemplateItemAreaFieldDelete()
        {
            int formBuilderTemplateItemAreaFieldID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaFieldID", 0);

            if (formBuilderTemplateItemAreaFieldID > 0)
            {
                FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField = null;

                formBuilderTemplateItemAreaField = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
                {
                    FormBuilderTemplateItemAreaFieldID = formBuilderTemplateItemAreaFieldID
                }, this.Website.WebsiteConnection.ConnectionString);

                this.FormBuilderTemplateItemAreaFieldDelete(formBuilderTemplateItemAreaFieldID);

                FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItemAreaField));

                this.AjaxResult.Status = AjaxResultStatus.Success;
                this.AjaxResult.Message = "FormBuilder Template Item Area Create Success";
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No FormBuilder Template Item Area Type ID supplied";
            }
        }

        public void FormBuilderTemplateItemAreaFieldMoveUp()
        {
            int formBuilderTemplateItemAreaFieldID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaFieldID", 0);

            if (formBuilderTemplateItemAreaFieldID > 0)
            {
                FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField = null;

                formBuilderTemplateItemAreaField = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
                {
                    FormBuilderTemplateItemAreaFieldID = formBuilderTemplateItemAreaFieldID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderTemplateItemAreaField?.FormBuilderTemplateItemAreaFieldID > 0)
                {
                    FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaFieldTemp = null;

                    formBuilderTemplateItemAreaFieldTemp = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
                    {
                        FormBuilderTemplateItemAreaID = formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaID,
                        SortOrder = (formBuilderTemplateItemAreaField.SortOrder - 1)
                    }, this.Website.WebsiteConnection.ConnectionString);

                    if (formBuilderTemplateItemAreaFieldTemp?.FormBuilderTemplateItemAreaID > 0)
                    {
                        formBuilderTemplateItemAreaFieldTemp.SortOrder += 1;
                        ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItemAreaField>(formBuilderTemplateItemAreaFieldTemp, this.Website.WebsiteConnection.ConnectionString);
                    }

                    formBuilderTemplateItemAreaField.SortOrder -= 1;
                    ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItemAreaField>(formBuilderTemplateItemAreaField, this.Website.WebsiteConnection.ConnectionString);

                    FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItemAreaField));

                    this.AjaxResult.Status = AjaxResultStatus.Success;
                    this.AjaxResult.Message = "Form Builder Template Item Area Field Move Up Success";
                }
                else
                {
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                    this.AjaxResult.Message = "Could not find Form Builder Template Item Area Field";
                }
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No Form Builder Template Item Area Field ID supplied";
            }
        }

        public void FormBuilderTemplateItemAreaFieldMoveDown()
        {
            int formBuilderTemplateItemAreaFieldID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaFieldID", 0);

            if (formBuilderTemplateItemAreaFieldID > 0)
            {
                FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField = null;

                formBuilderTemplateItemAreaField = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
                {
                    FormBuilderTemplateItemAreaFieldID = formBuilderTemplateItemAreaFieldID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderTemplateItemAreaField?.FormBuilderTemplateItemAreaFieldID > 0)
                {
                    FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaFieldTemp = null;

                    formBuilderTemplateItemAreaFieldTemp = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
                    {
                        FormBuilderTemplateItemAreaID = formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaID,
                        SortOrder = (formBuilderTemplateItemAreaField.SortOrder + 1)
                    }, this.Website.WebsiteConnection.ConnectionString);

                    if (formBuilderTemplateItemAreaFieldTemp?.FormBuilderTemplateItemAreaID > 0)
                    {
                        formBuilderTemplateItemAreaFieldTemp.SortOrder -= 1;
                        ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItemAreaField>(formBuilderTemplateItemAreaFieldTemp, this.Website.WebsiteConnection.ConnectionString);
                    }

                    formBuilderTemplateItemAreaField.SortOrder += 1;
                    ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItemAreaField>(formBuilderTemplateItemAreaField, this.Website.WebsiteConnection.ConnectionString);

                    FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItemAreaField));

                    this.AjaxResult.Status = AjaxResultStatus.Success;
                    this.AjaxResult.Message = "Form Builder Template Item Area Field Move Up Success";
                }
                else
                {
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                    this.AjaxResult.Message = "Could not find Form Builder Template Item Area Field";
                }
            }
            else
            {
                this.AjaxResult.Status = AjaxResultStatus.Failed;
                this.AjaxResult.Message = "No Form Builder Template Item Area Field ID supplied";
            }
        }

        public void FormBuilderTemplateItemAreaFieldEdit()
        {
            int formBuilderTemplateItemAreaFieldID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaFieldID", 0);

            if (formBuilderTemplateItemAreaFieldID > 0)
            {
                string templateNameItemArea = "ProviderBase/ProviderBaseAdminFormBuilderTemplateItemAreaFieldEdit.htm";
                string template = "";

                template = ProviderBase.Web.Utility.GetResourceHtml(templateNameItemArea);

                if (string.IsNullOrEmpty(template) == false)
                {
                    FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField = null;

                    formBuilderTemplateItemAreaField = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
                    {
                        FormBuilderTemplateItemAreaFieldID = formBuilderTemplateItemAreaFieldID
                    }, this.Website.WebsiteConnection.ConnectionString);

                    if (formBuilderTemplateItemAreaField?.FormBuilderTemplateItemAreaFieldID > 0)
                    {
                        template = ProviderBase.Data.Utility.TemplateBindData(template, formBuilderTemplateItemAreaField);

                        this.AjaxResult.Data.Add(template);
                        this.AjaxResult.Status = AjaxResultStatus.Success;
                        this.AjaxResult.Message = "Form Builder Template Item Edit Success";
                    }
                }
                else
                {
                    this.AjaxResult.Message = "No Template found";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
            else
            {
                this.AjaxResult.Message = "No Form Builder Template Item Area Field ID found";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void FormBuilderTemplateItemAreaFieldSave()
        {
            int formBuilderTemplateItemAreaFieldID = ProviderBase.Framework.Utility.GetFormValue<int>("FormBuilderTemplateItemAreaField_FormBuilderTemplateItemAreaFieldID", 0);

            if (formBuilderTemplateItemAreaFieldID > 0)
            {
                FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField = null;

                formBuilderTemplateItemAreaField = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
                {
                    FormBuilderTemplateItemAreaFieldID = formBuilderTemplateItemAreaFieldID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilderTemplateItemAreaField?.FormBuilderTemplateItemAreaFieldID > 0)
                {
                    formBuilderTemplateItemAreaField = ProviderBase.Framework.Utility.BindFormValues<FormBuilderTemplateItemAreaField>(formBuilderTemplateItemAreaField, "FormBuilderTemplateItemAreaField_", DataProviderKeyType.PrimaryKey);

                    ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItemAreaField>(formBuilderTemplateItemAreaField, this.Website.WebsiteConnection.ConnectionString);

                    FormBuilderUpdateDate(FormBuilderGet(formBuilderTemplateItemAreaField));

                    this.AjaxResult.Message = "Form Builder Template Item Area Field Save Success";
                    this.AjaxResult.Status = AjaxResultStatus.Success;
                }
                else
                {
                    this.AjaxResult.Message = "No Form Builder Template Item Area found";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
            else
            {
                this.AjaxResult.Message = "No Form Builder Template Item Area ID found";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void ReportBuilderGet()
        {

        }

        public void QueryBuilderGet()
        {

        }

        private void FormBuilderUpdateDate(FormBuilderTemplate formBuilderTemplate)
        {
            if (formBuilderTemplate?.FormBuilderTemplateID > 0)
            {
                formBuilderTemplate.ModifyDate = DateTime.Now;

                ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplate>(formBuilderTemplate, this.Website.WebsiteConnection.ConnectionString);
            }
        }

        private FormBuilderTemplate FormBuilderGet(FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField)
        {
            if (formBuilderTemplateItemAreaField?.FormBuilderTemplateItemAreaFieldID > 0)
            {
                FormBuilderTemplateItemArea formBuilderTemplateItemArea = null;

                formBuilderTemplateItemArea = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                {
                    FormBuilderTemplateItemAreaID = formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaID
                }, this.Website.WebsiteConnection.ConnectionString);

                return FormBuilderGet(formBuilderTemplateItemArea);
            }
            else
            {
                return new FormBuilderTemplate();
            }
        }

        private FormBuilderTemplate FormBuilderGet(FormBuilderTemplateItemArea formBuilderTemplateItemArea)
        {
            if (formBuilderTemplateItemArea?.FormBuilderTemplateItemAreaID > 0)
            {
                FormBuilderTemplateItem formBuilderTemplateItem = null;

                formBuilderTemplateItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItem>(new FormBuilderTemplateItem()
                {
                    FormBuilderTemplateItemID = formBuilderTemplateItemArea.FormBuilderTemplateItemID
                }, this.Website.WebsiteConnection.ConnectionString);

                return FormBuilderGet(formBuilderTemplateItem);
            }
            else
            {
                return new FormBuilderTemplate();
            }
        }

        private FormBuilderTemplate FormBuilderGet(FormBuilderTemplateItem formBuilderTemplateItem)
        {
            if (formBuilderTemplateItem?.FormBuilderTemplateItemID > 0)
            {
                FormBuilderTemplate formBuilderTemplate = null;

                formBuilderTemplate = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplate>(new FormBuilderTemplate()
                {
                    FormBuilderTemplateID = formBuilderTemplateItem.FormBuilderTemplateID
                }, this.Website.WebsiteConnection.ConnectionString);

                return formBuilderTemplate;
            }
            else
            {
                return new FormBuilderTemplate();
            }
        }

        private void FormBuilderTemplateItemAreaDelete(int formBuilderTemplateItemAreaID)
        {
            if (formBuilderTemplateItemAreaID > 0)
            {
                FormBuilderTemplateItemArea formBuilderTemplateItemArea = null;
                List<FormBuilderTemplateItemAreaField> formBuilderTemplateItemAreaFieldList = null;

                formBuilderTemplateItemArea = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemArea>(new FormBuilderTemplateItemArea()
                {
                    FormBuilderTemplateItemAreaID = formBuilderTemplateItemAreaID
                }, this.Website.WebsiteConnection.ConnectionString);

                formBuilderTemplateItemAreaFieldList = ProviderBase.Data.Providers.DataProvider.Select<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
                {
                    FormBuilderTemplateItemAreaID = formBuilderTemplateItemArea.FormBuilderTemplateItemAreaID
                }, this.Website.WebsiteConnection.ConnectionString);

                foreach (FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField in formBuilderTemplateItemAreaFieldList)
                {
                    FormBuilderTemplateItemAreaFieldDelete(formBuilderTemplateItemAreaField.FormBuilderTemplateItemAreaFieldID);
                }

                ProviderBase.Data.Providers.DataProvider.Delete<FormBuilderTemplateItemArea>(formBuilderTemplateItemArea, this.Website.WebsiteConnection.ConnectionString);
            }
        }

        private void FormBuilderTemplateItemAreaFieldDelete(int formBuilderTemplateItemAreaFieldID)
        {
            FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField = null;
            FormBuilderField formBuilderField = null;

            formBuilderTemplateItemAreaField = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
            {
                FormBuilderTemplateItemAreaFieldID = formBuilderTemplateItemAreaFieldID
            }, this.Website.WebsiteConnection.ConnectionString);

            formBuilderField = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderField>(new FormBuilderField()
            {
                FormBuilderFieldID = formBuilderTemplateItemAreaField.FormBuilderFieldID
            }, this.Website.WebsiteConnection.ConnectionString);

            ProviderBase.Data.Providers.DataProvider.Delete<FormBuilderField>(formBuilderField, this.Website.WebsiteConnection.ConnectionString);
            ProviderBase.Data.Providers.DataProvider.Delete<FormBuilderTemplateItemAreaField>(formBuilderTemplateItemAreaField, this.Website.WebsiteConnection.ConnectionString);
        }

        private void FormBuilderTemplateItemAreaFieldSort(int formBuilderTemplateItemAreaID, int targetSortOrder)
        {
            FormBuilderTemplateItemAreaField formBuilderTemplateItemAreaField = null;

            formBuilderTemplateItemAreaField = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilderTemplateItemAreaField>(new FormBuilderTemplateItemAreaField()
            {
                FormBuilderTemplateItemAreaID = formBuilderTemplateItemAreaID,
                SortOrder = targetSortOrder
            }, this.Website.WebsiteConnection.ConnectionString);

            if (formBuilderTemplateItemAreaField?.FormBuilderTemplateItemAreaFieldID > 0)
            {
                FormBuilderTemplateItemAreaFieldSort(formBuilderTemplateItemAreaID, (targetSortOrder + 1));

                formBuilderTemplateItemAreaField.SortOrder++;

                ProviderBase.Data.Providers.DataProvider.Update<FormBuilderTemplateItemAreaField>(formBuilderTemplateItemAreaField, this.Website.WebsiteConnection.ConnectionString);
            }
        }
    }
}
