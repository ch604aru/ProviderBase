using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProviderBase.Framework.Handlers
{
    public class FormBuilderHandler : BaseHandler
    {
        public void FormBuilderGet()
        {
            string formBuilderGUID = ProviderBase.Framework.Utility.GetFormValue<string>("FormBuilder_GUID", "");

            if (string.IsNullOrEmpty(formBuilderGUID) == false)
            {
                FormBuilderUtility formBuilderUtility = null;

                formBuilderUtility = new FormBuilderUtility(this.Website, formBuilderGUID);

                this.AjaxResult.Data.Add(formBuilderUtility.Render());
                this.AjaxResult.Message = "Get form builder success";
                this.AjaxResult.Status = AjaxResultStatus.Success;
            }
            else
            {
                this.AjaxResult.Message = "No form builder GUID supplied";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }

        public void FormBuilderSubmit()
        {
            string formBuilderGUID = "";

            formBuilderGUID = ProviderBase.Framework.Utility.GetFormValue<string>("FormBuilder_GUID", "");

            if (string.IsNullOrEmpty(formBuilderGUID) == false)
            {
                FormBuilder formBuilder = null;

                formBuilder = ProviderBase.Data.Providers.DataProvider.SelectSingle<FormBuilder>(new FormBuilder()
                {
                    GUID = new Guid(formBuilderGUID)
                }, this.Website.WebsiteConnection.ConnectionString);

                if (formBuilder?.FormBuilderID > 0)
                {
                    bool fieldSet = false;
                    int customFieldGroup = 0;
                    List<object> formBuilderSubmitObjectList = null;
                    List<CustomFieldValue> customFieldValueList = null;

                    customFieldValueList = new List<CustomFieldValue>();
                    formBuilderSubmitObjectList = new List<object>();

                    customFieldGroup = ProviderBase.Framework.Utility.GetFormValue<int>("CustomFieldGroupID", 0);

                            List<FormBuilderField> formBuilderFieldList = null;

                            formBuilderFieldList = ProviderBase.Data.Providers.DataProvider.Select<FormBuilderField>(new FormBuilderField()
                            {
                                FormBuilderID = formBuilder.FormBuilderID
                            }, this.Website.WebsiteConnection.ConnectionString);

                    if (formBuilderFieldList?.Count > 0)
                    {
                        List<string> listFieldProcessed = null;

                        listFieldProcessed = new List<string>();

                        foreach (FormBuilderField formBuilderField in formBuilderFieldList)
                        {
                            if (formBuilderField.TableDefinitionFieldID > 0)
                            {
                                TableDefinition tableDefinition = null;
                                TableDefinitionField tableDefinitionField = null;
                                object formBuilderSubmitObject = null;
                                object formFieldValue = null;
                                bool processField = true;

                                tableDefinitionField = ProviderBase.Data.Providers.DataProvider.SelectSingle<TableDefinitionField>(new TableDefinitionField()
                                {
                                    TableDefinitionFieldID = formBuilderField.TableDefinitionFieldID
                                }, this.Website.WebsiteConnection.ConnectionString);

                                tableDefinition = ProviderBase.Data.Providers.DataProvider.SelectSingle<TableDefinition>(new TableDefinition()
                                {
                                    TableDefinitionID = tableDefinitionField.TableDefinitionID
                                }, this.Website.WebsiteConnection.ConnectionString);

                                if (tableDefinitionField?.TableDefinitionFieldID > 0)
                                {
                                    formFieldValue = ProviderBase.Framework.Utility.GetFormValue<object>(tableDefinitionField.FieldName, null);

                                    if (formFieldValue.ToString().ToLower() == "islist" && listFieldProcessed.Contains(tableDefinitionField.FieldName))
                                    {
                                        // Already processed this object list in one go
                                        // (may be on page mulitple times, it's a list)
                                        processField = false;
                                    }
                                }

                                if (processField)
                                {
                                    if (tableDefinition?.TableDefinitionID > 0)
                                    {
                                        Type submitObjectType = null;

                                        submitObjectType = Type.GetType(tableDefinition.AssemblyFullName);

                                        formBuilderSubmitObject = formBuilderSubmitObjectList.Where(x => x.GetType().Equals(submitObjectType)).SingleOrDefault<object>();

                                        if (formBuilderSubmitObject == null && formFieldValue.ToString().ToLower() == "islist")
                                        {
                                            List<object> listSubmitObject = null;

                                            listSubmitObject = formBuilderSubmitObjectList.Where(x => (x.GetType().IsGenericType)).ToList();

                                            formBuilderSubmitObject = listSubmitObject.Where(x => x.GetType().GetGenericArguments().Single().Equals(submitObjectType)).SingleOrDefault<object>();
                                        }

                                        if (formBuilderSubmitObject == null)
                                        {
                                            // If object still null
                                            formBuilderSubmitObject = Activator.CreateInstance(tableDefinition.AssemblyName, tableDefinition.AssemblyType).Unwrap();
                                        }
                                        else
                                        {
                                            formBuilderSubmitObjectList.Remove(formBuilderSubmitObject);
                                        }
                                    }

                                    if (tableDefinitionField?.TableDefinitionFieldID > 0)
                                    {
                                        IList formBuilderSubmitObjectListRepeat = null;

                                        object formBuilderSubmitObjectItem = null;
                                        int index = 0;
                                        int repeatIndex = 0;
                                        int repeatItemIndex = 0;
                                        bool isList = false;
                                        bool isListRetry = false;

                                        do
                                        {
                                            object formFieldValueItem = null;

                                            if (formFieldValue != null)
                                            {
                                                if (formFieldValue.ToString().ToLower() == "islist")
                                                {
                                                    string fieldName = "";

                                                    if (ProviderBase.Data.Utility.IsList(formBuilderSubmitObject) == false)
                                                    {
                                                        formBuilderSubmitObjectItem = Activator.CreateInstance(tableDefinition.AssemblyName, tableDefinition.AssemblyType).Unwrap();

                                                        if (formBuilderSubmitObjectListRepeat == null)
                                                        {
                                                            Type listType = null;

                                                            listType = typeof(List<>).MakeGenericType(formBuilderSubmitObject.GetType());
                                                            formBuilderSubmitObjectListRepeat = (IList)Activator.CreateInstance(listType);
                                                        }
                                                    }
                                                    else if (formBuilderSubmitObjectListRepeat == null)
                                                    {
                                                        formBuilderSubmitObjectListRepeat = (IList)formBuilderSubmitObject;
                                                        formBuilderSubmitObjectItem = formBuilderSubmitObjectListRepeat[index];
                                                    }

                                                    fieldName = tableDefinitionField.FieldName;
                                                    fieldName = fieldName + "_" + repeatIndex;

                                                    formFieldValueItem = ProviderBase.Framework.Utility.GetFormValue<object>(fieldName, null);

                                                    if (formFieldValueItem == null)
                                                    {
                                                        fieldName = fieldName + "_" + repeatItemIndex;

                                                        formFieldValueItem = ProviderBase.Framework.Utility.GetFormValue<object>(fieldName, null);

                                                        if (formFieldValueItem != null)
                                                        {
                                                            repeatIndex++;

                                                            isListRetry = false;
                                                        }
                                                        else
                                                        {
                                                            repeatIndex = 0;
                                                            repeatItemIndex++;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        repeatIndex++;
                                                    }

                                                    isList = true;
                                                }
                                                else
                                                {
                                                    formFieldValueItem = formFieldValue;
                                                    formBuilderSubmitObjectItem = formBuilderSubmitObject;

                                                    isList = false;
                                                }

                                                if (formFieldValueItem != null)
                                                {
                                                    formFieldValueItem = ProviderBase.Data.Utility.ChangeTypeUsingTypeName(formFieldValueItem, tableDefinitionField.ObjectPropertyType);

                                                    if (formFieldValueItem != null)
                                                    {
                                                        PropertyInfo propertyInfo = null;

                                                        propertyInfo = ProviderBase.Data.Utility.GetDataProviderField(formBuilderSubmitObjectItem, tableDefinitionField.ObjectPropertyName);

                                                        if (propertyInfo != null)
                                                        {
                                                            propertyInfo.SetValue(formBuilderSubmitObjectItem, formFieldValueItem);

                                                            fieldSet = true;
                                                        }

                                                        if (tableDefinitionField.ObjectPropertyName.ToLower() == "externalreference")
                                                        {
                                                            formBuilderSubmitObjectItem = ProviderBase.Data.Providers.DataProvider.SelectSingle(formBuilderSubmitObjectItem, this.Website.WebsiteConnection.ConnectionString);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // No value found
                                                    formBuilderSubmitObjectItem = null;

                                                    if (isListRetry)
                                                    {
                                                        formBuilderSubmitObjectItem = null;

                                                        isList = false;
                                                    }
                                                    else
                                                    {
                                                        isListRetry = true;
                                                    }
                                                }
                                            }

                                            index++;

                                            if (formBuilderSubmitObjectListRepeat != null && formBuilderSubmitObjectItem != null)
                                            {
                                                formBuilderSubmitObjectListRepeat.Add(formBuilderSubmitObjectItem);
                                            }
                                        } while (isList);

                                        if (formBuilderSubmitObjectListRepeat != null)
                                        {
                                            formBuilderSubmitObject = formBuilderSubmitObjectListRepeat;
                                        }
                                        else
                                        {
                                            formBuilderSubmitObject = formBuilderSubmitObjectItem;
                                        }
                                    }

                                    if (formBuilderSubmitObject != null)
                                    {
                                        formBuilderSubmitObjectList.Add(formBuilderSubmitObject);
                                    }

                                    if (formFieldValue.ToString().ToLower() == "islist")
                                    {
                                        listFieldProcessed.Add(tableDefinitionField.FieldName);
                                    }
                                }
                            }
                            else if (formBuilderField.CustomFieldItemID > 0)
                            {
                                CustomFieldItem customFieldItem = null;
                                CustomFieldValue customFieldValue = null;

                                customFieldItem = ProviderBase.Data.Providers.DataProvider.SelectSingle<CustomFieldItem>(new CustomFieldItem()
                                {
                                    CustomFieldItemID = formBuilderField.CustomFieldItemID
                                }, this.Website.WebsiteConnection.ConnectionString);

                                if (customFieldItem?.CustomFieldItemID > 0)
                                {
                                    string fieldValue = "";

                                    fieldValue = ProviderBase.Framework.Utility.GetFormValue<string>(customFieldItem.FieldName, "");

                                    customFieldValue = new CustomFieldValue();

                                    switch (customFieldItem.CustomFieldItemTypeID)
                                    {
                                        case CustomFieldItemType.Text:
                                        case CustomFieldItemType.Unassigned:
                                        default:
                                            customFieldValue.ValueText = fieldValue;
                                            break;

                                        case CustomFieldItemType.Int:
                                            customFieldValue.ValueInt = ProviderBase.Data.Utility.TryParse<int>(fieldValue);
                                            break;

                                        case CustomFieldItemType.Decimal:
                                            customFieldValue.ValueDecimal = ProviderBase.Data.Utility.TryParse<decimal>(fieldValue);
                                            break;

                                        case CustomFieldItemType.Boolean:
                                            customFieldValue.ValueBoolean = ProviderBase.Data.Utility.TryParse<bool>(fieldValue);
                                            break;

                                        case CustomFieldItemType.CustomFieldValue:
                                            throw new NotImplementedException();
                                    }

                                    customFieldValue.ValueDisplay = fieldValue;
                                    customFieldValue.CustomFieldItemID = customFieldItem.CustomFieldItemID;

                                    customFieldValueList.Add(customFieldValue);
                                }
                            }
                        }
                    }

                    if (fieldSet)
                    {
                        bool actionTaken = true;
                        List<PropertyInfo> primaryKeyList = null;
                        List<int> processedObjectIndex = null;

                        processedObjectIndex = new List<int>();

                        primaryKeyList = ProviderBase.Data.Utility.GetDataProviderKeyTypeList(formBuilderSubmitObjectList, DataProviderKeyType.PrimaryKey, true);

                        while (actionTaken)
                        {
                            actionTaken = false;

                            // Object
                            for (int i = 0; i < formBuilderSubmitObjectList.Count; i++)
                            {
                                if (processedObjectIndex.Contains(i) == false)
                                {
                                    int listIndex = -1;
                                    int listCount = -1;
                                    bool isList = false;

                                    do
                                    {
                                        List<PropertyInfo> foreignKeySubmitObject = null;
                                        object formBuilderSubmitObject = null;

                                        if (ProviderBase.Data.Utility.IsList(formBuilderSubmitObjectList[i]))
                                        {
                                            IList formBuilderSubmitObjectTempList = null;

                                            isList = true;
                                            listIndex++;

                                            formBuilderSubmitObjectTempList = (IList)formBuilderSubmitObjectList[i];

                                            if (formBuilderSubmitObjectTempList != null && listIndex < formBuilderSubmitObjectTempList.Count)
                                            {
                                                formBuilderSubmitObject = formBuilderSubmitObjectTempList[listIndex];

                                                listCount = formBuilderSubmitObjectTempList.Count;
                                            }
                                        }
                                        else
                                        {
                                            formBuilderSubmitObject = formBuilderSubmitObjectList[i];
                                        }

                                        if (formBuilderSubmitObject != null)
                                        {
                                            bool processObject = true;

                                            foreignKeySubmitObject = ProviderBase.Data.Utility.GetDataProviderKeyTypeList(formBuilderSubmitObject, DataProviderKeyType.ForeignKey);

                                            if (primaryKeyList.Exists(x => foreignKeySubmitObject.Any(y => y.Name.Equals(x.Name))) == false)
                                            {
                                                PropertyInfo propertyInfoPrimaryKey = null;
                                                int primaryKey = 0;

                                                propertyInfoPrimaryKey = ProviderBase.Data.Utility.GetDataProviderKeyTypeSingle(formBuilderSubmitObject, DataProviderKeyType.PrimaryKey);

                                                if (propertyInfoPrimaryKey != null)
                                                {
                                                    int primaryKeyTemp = 0;

                                                    primaryKeyTemp = (int)propertyInfoPrimaryKey.GetValue(formBuilderSubmitObject);

                                                    if (primaryKeyTemp != 0)
                                                    {
                                                        // If object has primary key (non link object) must be 0
                                                        processObject = false;
                                                    }
                                                }

                                                if (processObject)
                                                {
                                                    foreach (PropertyInfo foreignKey in foreignKeySubmitObject)
                                                    {
                                                        object primaryKeyObject = null;

                                                        primaryKeyObject = ProviderBase.Data.Utility.GetDataProviderPrimaryKeyObject(formBuilderSubmitObjectList, foreignKey.Name);

                                                        if (primaryKeyObject == null && isList)
                                                        {
                                                            primaryKeyObject = ProviderBase.Data.Utility.GetDataProviderPrimaryKeyObjectList(formBuilderSubmitObjectList, foreignKey.Name, listIndex, listCount);
                                                        }

                                                        if (primaryKeyObject != null)
                                                        {
                                                            object primaryKeyValue = null;
                                                            PropertyInfo primaryKeyPropertyInfo = null;

                                                            primaryKeyPropertyInfo = ProviderBase.Data.Utility.GetDataProviderKeyTypeSingle(primaryKeyObject, DataProviderKeyType.PrimaryKey);

                                                            if (primaryKeyPropertyInfo != null)
                                                            {
                                                                primaryKeyValue = primaryKeyPropertyInfo.GetValue(primaryKeyObject);

                                                                foreignKey.SetValue(formBuilderSubmitObject, primaryKeyValue);
                                                            }
                                                        }
                                                    }

                                                    primaryKey = ProviderBase.Data.Providers.DataProvider.Insert(formBuilderSubmitObject, this.Website.WebsiteConnection.ConnectionString);

                                                    if (propertyInfoPrimaryKey != null)
                                                    {
                                                        propertyInfoPrimaryKey.SetValue(formBuilderSubmitObject, primaryKey);
                                                    }

                                                    primaryKeyList.Remove(propertyInfoPrimaryKey);
                                                    processedObjectIndex.Add(i);

                                                    actionTaken = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            isList = false;
                                        }
                                    } while (isList);
                                }
                            }
                        }

                        // Custom Field
                        foreach (CustomFieldValue customFieldValue in customFieldValueList)
                        {
                            if (string.IsNullOrEmpty(customFieldValue.ValueDisplay) == false)
                            {
                                CustomFieldItem customFieldItem = null;
                                TableDefinition tableDefinition = null;
                                Type submitObjectType = null;
                                object formBuilderSubmitObject = null;
                                Dictionary<string, string> fieldConvertList = null;

                                customFieldItem = Data.Providers.DataProvider.SelectSingle<CustomFieldItem>(new CustomFieldItem()
                                {
                                    CustomFieldItemID = customFieldValue.CustomFieldItemID
                                }, this.Website.WebsiteConnection.ConnectionString);

                                tableDefinition = Data.Providers.DataProvider.SelectSingle<TableDefinition>(new TableDefinition()
                                {
                                    TableDefinitionID = customFieldItem.TableDefinitionID
                                }, this.Website.WebsiteConnection.ConnectionString);

                                submitObjectType = Type.GetType(tableDefinition.AssemblyFullName);

                                formBuilderSubmitObject = formBuilderSubmitObjectList.Where(x => x.GetType().Equals(submitObjectType)).SingleOrDefault<object>();

                                if (formBuilderSubmitObject != null)
                                {
                                    PropertyInfo propertyInfoPrimaryKey = null;

                                    propertyInfoPrimaryKey = ProviderBase.Data.Utility.GetDataProviderField(formBuilderSubmitObject, DataProviderKeyType.PrimaryKey);

                                    if (propertyInfoPrimaryKey != null)
                                    {
                                        object primaryKeyObject = null;

                                        primaryKeyObject = propertyInfoPrimaryKey.GetValue(formBuilderSubmitObject);

                                        customFieldValue.LinkID = (int)primaryKeyObject;

                                        fieldConvertList = new Dictionary<string, string>();
                                        fieldConvertList.Add("CustomFieldValueID", tableDefinition.ObjectName + "CustomFieldValueID");
                                        fieldConvertList.Add("LinkID", tableDefinition.ObjectName + "ID");

                                        customFieldValue.CustomFieldValueID = ProviderBase.Data.Providers.DataProvider.InsertOverrideTableField<CustomFieldValue>(customFieldValue, this.Website.WebsiteConnection.ConnectionString, tableDefinition.ObjectName + "CustomFieldValue_T", fieldConvertList);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                AjaxResult.Message = "No FormBuilder_GUID supplied";
                AjaxResult.Status = AjaxResultStatus.Failed;
            }
        }
    }
}
