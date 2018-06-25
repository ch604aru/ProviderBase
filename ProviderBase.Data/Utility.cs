using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using ProviderBase.Data.Entities;
using System.Data.Common;
using System.Collections;

namespace ProviderBase.Data
{
    public static class Utility
    {
        public static string GetConnectionString(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName) == true)
            {
                try
                {
                    connectionStringName = GetAppSetting("DefaultDatabase", "");
                }
                catch (Exception)
                {
                    return "";
                }
            }
            else
            {
                DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder();

                try
                {
                    connectionStringBuilder.ConnectionString = connectionStringName;

                    return connectionStringName;
                }
                catch
                {
                    // Will go to next try catch
                }
            }

            try
            {
                return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static T GetAppSetting<T>(string key, T defaultValue)
        {
            T value = defaultValue;
            string appSetting = ConfigurationManager.AppSettings.Get(key);

            if (string.IsNullOrEmpty(appSetting) == false)
            {
                value = TryParse<T>(appSetting);
            }

            return value;
        }

        public static T GetAppSettingSection<T>(string key)
        {
            return (T)ConfigurationManager.GetSection(key);
    }

        public static T MergeDictionaryLeft<T, K, V>(T primaryDictionary, params IDictionary<K, V>[] secondaryDictionaries) where T : IDictionary<K, V>, new()
        {
            T mergedDictionary = new T();

            foreach (IDictionary<K, V> sourceDictionary in (new List<IDictionary<K, V>> { primaryDictionary }).Concat(secondaryDictionaries))
            {
                foreach (KeyValuePair<K, V> pair in sourceDictionary)
                {
                    mergedDictionary[pair.Key] = pair.Value;
                }
            }
            return mergedDictionary;
        }

        public static T TryParse<T>(string inValue)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

            if (string.IsNullOrEmpty(inValue))
            {
                return default(T);
            }
            else
            {
                try
                {
                    return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, inValue);
                }
                catch
                {
                    return default(T);
                }
            }
        }

        public static object ChangeTypeUsingTypeName(object inValue, string typeName)
        {
            object returnValue = null;

            switch (typeName.ToLower())
            {
                case "string":
                    returnValue = Convert.ChangeType(inValue, typeof(string));
                    break;

                case "int":
                case "enum":
                    returnValue = Convert.ChangeType(inValue, typeof(int));
                    break;

                case "bool":
                case "boolean":
                    returnValue = Convert.ChangeType(inValue, typeof(bool));
                    break;

                case "decmal":
                    returnValue = Convert.ChangeType(inValue, typeof(decimal));
                    break;
            }

            return returnValue;
        }

        public static object ChangeType(string inValue, Type type)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);

            if (string.IsNullOrEmpty(inValue))
            {
                return "";
            }
            else
            {
                try
                {
                    return converter.ConvertFromString(null, CultureInfo.InvariantCulture, inValue);
                }
                catch
                {
                    return "";
                }
            }
        }

        public static List<int> IntFromStringList(string stringList, char delimiter)
        {
            return stringList.Split(new char[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();
        }

        public static void XMLSeralize<T>(T serializeObject, string fileName)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                xml.Serialize(sw, serializeObject);
            }
        }

        public static string XMLSeralize<T>(T serializeObject)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));

            using (StringWriter sw = new StringWriter())
            {
                xml.Serialize(sw, serializeObject);

                return sw.ToString();
            }
        }

        public static T XMLDeseralize<T>(string fileName) where T : new()
        {
            T returnObject = new T();

            if (File.Exists(fileName))
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));

                using (StreamReader sr = new StreamReader(fileName))
                {
                    returnObject = (T)xml.Deserialize(sr);
                }
            }

            return returnObject;
        }

        public static bool IsDefaultValue(Type type, object compareObject, PropertyInfo compareProperty)
        {
            object defaultObject = Activator.CreateInstance(type);
            object defaultObjectPropertyValue = compareProperty.GetValue(defaultObject);
            object compareObjectPropertyValue = compareProperty.GetValue(compareObject);

            return (compareObjectPropertyValue.Equals(defaultObjectPropertyValue));
        }

        public static string SplitCamelCase(string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        public static string StripPunctuation(string str)
        {
            return new string(str.Where(x => char.IsPunctuation(x) == false && char.IsSymbol(x) == false).ToArray());
        }

        public static string GetTemplateFileElementSingle(string templateFileContent, string elementName)
        {
            return GetTemplateFileElementSingle(templateFileContent, elementName, "");
        }

        public static string GetTemplateFileElementSingle(string templateFileContent, string elementName, string replaceString)
        {
            Regex regex = new Regex($"<{elementName}>(.*?)</{elementName}>", RegexOptions.Singleline);
            Match regexMatch = regex.Match(templateFileContent);

            if (regexMatch.Groups.Count > 0)
            {
                return regexMatch.Groups[1].ToString();
            }
            
            return "";
        }

        public static List<string> GetTemplateFileElement(string templateFileContent, string elementName)
        {
            Regex regex = new Regex($"<{elementName}>(.*?)</{elementName}>", RegexOptions.Singleline);
            MatchCollection regexMatch = regex.Matches(templateFileContent, 0);
            List<string> elementList = new List<string>();

            if (regexMatch.Count > 0)
            {
                foreach (Match match in regexMatch)
                {
                    elementList.Add(match.Groups[1].ToString());
                }
            }

            return elementList;
        }

        public static Dictionary<string, string> GetTemplateFileElementRepeat(ref string templateFileContent)
        {
            Regex regex = new Regex($"<repeat data-(.*?)</repeat>", RegexOptions.Singleline);
            Regex regexTemplate = new Regex($"objectrepeat=\"(.*?)\">", RegexOptions.Singleline);
            MatchCollection regexMatch = regex.Matches(templateFileContent, 0);
            Dictionary<string, string> elementList = new Dictionary<string, string>();

            if (regexMatch.Count > 0)
            {
                foreach (Match match in regexMatch)
                {
                    MatchCollection regexMatchTemplate = null;
                    string template = match.Groups[1].ToString();
                    regexMatchTemplate = regexTemplate.Matches(template, 0);

                    if (regexMatchTemplate?.Count > 0)
                    {
                        string objectName = regexMatchTemplate[0].Groups[1].ToString();
                        string templateTemp = match.Groups[1].ToString();

                        templateTemp = regexTemplate.Replace(templateTemp, x => x.Value.Replace(x.Value, ""));

                        elementList.Add(objectName, templateTemp);

                        templateFileContent = regex.Replace(templateFileContent, x => x.Value.Replace(x.Value, $"${objectName.ToUpper()}$"), 1);
                    }
                }
            }

            return elementList;
        }

        public static List<string> GetTemplateFileElementRepeatItem(ref string templateFileContent)
        {
            Regex regex = new Regex($"<repeatitem data-(.*?)</repeatitem>", RegexOptions.Singleline);
            Regex regexTemplate = new Regex($"objectrepeatitem=\"(.*?)\">", RegexOptions.Singleline);
            MatchCollection regexMatch = regex.Matches(templateFileContent, 0);
            List<string> elementList = new List<string>();

            if (regexMatch.Count > 0)
            {
                for (int i = 0; i < regexMatch.Count; i++)
                {
                    MatchCollection regexMatchTemplate = null;
                    string template = regexMatch[i].Groups[1].ToString();
                    regexMatchTemplate = regexTemplate.Matches(template, 0);

                    if (regexMatchTemplate?.Count > 0)
                    {
                        string templateTemp = regexMatch[i].Groups[1].ToString();

                        templateTemp = regexTemplate.Replace(templateTemp, x => x.Value.Replace(x.Value, ""));

                        elementList.Add(templateTemp);

                        templateFileContent = regex.Replace(templateFileContent, x => x.Value.Replace(x.Value, $"$REPEATITEM{i}$"), 1);
                    }
                }
            }

            return elementList;
        }

        public static void GetTemplateStartEndTag(string template, string seperator, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            if (string.IsNullOrEmpty(template) == false)
            {
                string[] templateSplit = template.Split(new string[] { seperator }, StringSplitOptions.RemoveEmptyEntries);

                if (templateSplit != null && templateSplit.Count() == 2)
                {
                    startTag = templateSplit[0];
                    endTag = templateSplit[1];
                }
            }
        }

        public static string ReplaceTemplateFileElementSingle(string templateFileContent, string elementName, string replaceString)
        {
            return Regex.Replace(templateFileContent, $"<{elementName}>(.*?)</{elementName}>", "$" + replaceString + "$", RegexOptions.Singleline);
        }

        public static string ReplaceTemplateFileElement(string templateFileContent, string elementName, string replaceString)
        {
            string pattern = $"<{elementName}>(.*?)</{elementName}>";
            int count = (templateFileContent.Length - templateFileContent.Replace($"<{elementName}>", "").Length) / $"<{elementName}>".Length;
            Regex regex = new Regex(pattern, RegexOptions.Singleline);

            for (int i = 0; i < count; i++)
            {
                templateFileContent = regex.Replace(templateFileContent, x => x.Value.Replace(x.Value, ("$" + replaceString + i + "$")), 1);
            }

            return templateFileContent;
        }

        public static string TemplateBindData(string template, object bindObject)
        {
            return TemplateBindData(template, bindObject, "");
        }

        public static string TemplateBindData(string template, object bindObject, string prefix)
        {
            return TemplateBindData(template, bindObject, prefix, null);
        }

        public static string TemplateBindData(string template, object bindObject, string prefix, List<string> propertyIgnoreDefault)
        {
            Dictionary<string, string> templateRepeatList = null;

            return TemplateBindData(template, bindObject, prefix, propertyIgnoreDefault, ref templateRepeatList);
        }

        public static string TemplateBindData(string template, object bindObject, string prefix, List<string> propertyIgnoreDefault, ref Dictionary<string, string> templateRepeatList)
        {
            string templateTemp = template;
            int indexTemp = 0;

            if (bindObject != null)
            {
                if (IsList(bindObject))
                {
                    IEnumerable bindObjectList = (IEnumerable)bindObject;
                    if (templateRepeatList?.Count > 0)
                    {
                        KeyValuePair<string, string> templateRepeatListFirst = templateRepeatListFirst = templateRepeatList.First();
                        string templateTempRepeat = "";

                        foreach (object element in bindObjectList)
                        {
                            templateTempRepeat += TemplateBindData(templateRepeatListFirst.Value, element, prefix, propertyIgnoreDefault, ref templateRepeatList);
                        }

                        templateRepeatList[templateRepeatListFirst.Key] = templateTempRepeat;
                    }
                }
                else
                {
                    PropertyInfo[] bindObjectProperties = bindObject.GetType().GetProperties();

                    if (bindObjectProperties?.Count() > 0)
                    {
                        foreach (PropertyInfo property in bindObjectProperties)
                        {
                            if (property.CanRead)
                            {
                                object propertyValue = property.GetValue(bindObject);
                                List<Type> stringableTypes = new List<Type>()
                            {
                                typeof(string)
                                ,typeof(DateTime)
                                ,typeof(decimal)
                                ,typeof(int)
                                ,typeof(long)
                                ,typeof(decimal)
                            };

                                if (propertyValue != null)
                                {
                                    if (IsList(propertyValue))
                                    {
                                        IEnumerable<object> propertyValueList = (IEnumerable<object>)propertyValue;

                                        if (propertyValueList?.Count() > 0)
                                        {
                                            if (templateRepeatList?.Count > 0)
                                            {
                                                string templateRepeat = "";
                                                string objectName = propertyValue.GetType().GetGenericArguments().Single().Name;
                                                string[] repeatItemList = new string[0];
                                                List<string> repeatItemListTemp = new List<string>();

                                                if (templateRepeatList.TryGetValue(objectName.ToUpper(), out templateRepeat))
                                                {
                                                    string templateRepeatTemp = "";

                                                    if (templateRepeat.Contains("repeatitem"))
                                                    {
                                                        repeatItemListTemp = GetTemplateFileElementRepeatItem(ref templateRepeat);
                                                        repeatItemList = new string[repeatItemListTemp.Count];
                                                    }

                                                    foreach (object objectValue in propertyValueList)
                                                    {
                                                        for (int i = 0; i < repeatItemListTemp.Count; i++)
                                                        {
                                                            string templateRepeatItemTemp = "";
                                                            indexTemp = 0;

                                                            templateRepeatItemTemp = TemplateBindDataRepeat(repeatItemListTemp[i], objectValue, propertyValue.GetType().GetGenericArguments().Single().Name, ref indexTemp, propertyIgnoreDefault);

                                                            repeatItemList[i] += (repeatItemListTemp[i] != templateRepeatItemTemp) ? templateRepeatItemTemp : "";
                                                        }

                                                        templateRepeatTemp += TemplateBindDataRepeat(templateRepeat, objectValue, objectName.ToUpper(), ref indexTemp, propertyIgnoreDefault);
                                                    }

                                                    for (int i = 0; i < repeatItemList.Count(); i++)
                                                    {
                                                        templateRepeatTemp = templateRepeatTemp.Replace($"$REPEATITEM{i}$", repeatItemList[i]);
                                                    }

                                                    templateRepeatList[objectName.ToUpper()] = templateRepeatTemp;
                                                }
                                            }

                                            templateTemp = TemplateBindData(templateTemp, propertyValueList.First(), propertyValue.GetType().GetGenericArguments().Single().Name, propertyIgnoreDefault, ref templateRepeatList);
                                        }
                                    }
                                    else if (propertyValue.GetType().IsPrimitive == false && stringableTypes.Contains(propertyValue.GetType()) == false && propertyValue is Enum == false && propertyValue is Guid == false)
                                    {
                                        templateTemp = TemplateBindData(templateTemp, propertyValue, propertyValue.GetType().Name, propertyIgnoreDefault, ref templateRepeatList);
                                    }
                                    else if (propertyValue is Enum)
                                    {
                                        templateTemp = TemplateBindEnum(templateTemp, propertyValue, prefix, propertyIgnoreDefault);
                                    }
                                    else
                                    {
                                        templateTemp = TemplateBindDataObject(templateTemp, property, propertyValue, prefix, propertyIgnoreDefault);
                                    }
                                }
                            }
                        }
                    }
                    else if (bindObject is Enum)
                    {
                        templateTemp = TemplateBindEnum(templateTemp, bindObject, prefix, propertyIgnoreDefault);
                    }
                }
            }

            return templateTemp;
        }

        public static string TemplateBindDataList<T>(string template, List<T> bindObjectListRepeat)
        {
            return TemplateBindDataList(template, bindObjectListRepeat, null, "");
        }

        public static string TemplateBindDataList<T>(string template, List<T> bindObjectListRepeat, Type typeRepeatItem, string connectionString)
        {
            if (bindObjectListRepeat?.Count > 0)
            {
                string templateRepeat = "";
                string templateRepeatTemp = "";
                string templateRepeatItem = "";
                int index = 0;
                
                templateRepeat = ProviderBase.Data.Utility.GetTemplateFileElementSingle(template, "repeat", "repeat");
                template = ProviderBase.Data.Utility.ReplaceTemplateFileElementSingle(template, "repeat", "repeat");
                templateRepeatItem = ProviderBase.Data.Utility.GetTemplateFileElementSingle(templateRepeat, "repeatitem", "repeatitem");
                templateRepeat = ProviderBase.Data.Utility.ReplaceTemplateFileElementSingle(templateRepeat, "repeatitem", "repeatitem");

                foreach (object bindObject in bindObjectListRepeat)
                {
                    templateRepeatTemp += TemplateBindDataRepeat(templateRepeat, bindObject, "", ref index, null);
                    index++;

                    if (typeRepeatItem != null && string.IsNullOrEmpty(connectionString) == false)
                    {
                        object objectRepeatItem = null;
                        string templateRepeatItemTemp = "";

                        objectRepeatItem = Activator.CreateInstance(typeRepeatItem);

                        if (objectRepeatItem != null)
                        {
                            PropertyInfo propertyInfoBindObject = null;

                            propertyInfoBindObject = GetDataProviderKeyTypeSingle(bindObject, DataProviderKeyType.PrimaryKey);

                            if (propertyInfoBindObject != null)
                            {
                                PropertyInfo propertyInfoBindObjectRepeatItem = null;

                                propertyInfoBindObjectRepeatItem = GetDataProviderField(objectRepeatItem, DataProviderKeyType.ForeignKey, propertyInfoBindObject.Name);

                                if (propertyInfoBindObjectRepeatItem != null)
                                {
                                    List<object> repeatItemList = null;

                                    propertyInfoBindObjectRepeatItem.SetValue(objectRepeatItem, propertyInfoBindObject.GetValue(bindObject));

                                    repeatItemList = ProviderBase.Data.Providers.DataProvider.Select(objectRepeatItem, connectionString);

                                    if (repeatItemList?.Count > 0)
                                    {
                                        int indexRepeatItem = 0;

                                        foreach (object bindObjectRepeatItem in repeatItemList)
                                        {
                                            templateRepeatItemTemp += TemplateBindDataRepeat(templateRepeatItem, bindObjectRepeatItem, "", ref indexRepeatItem, null);
                                            indexRepeatItem++;
                                        }
                                    }
                                }
                            }
                        }

                        templateRepeatTemp = templateRepeatTemp.Replace("$repeatitem$", templateRepeatItemTemp);
                    }
                }

                template = template.Replace("$repeat$", templateRepeatTemp);
            }

            return template;
        }

        public static string TemplateBindDataRepeat(string template, object bindObject, string prefix, ref int index, List<string> propertyIgnoreDefault)
        {
            string templateTemp = template;
            int indexTemp = index;

            if (bindObject != null && bindObject.GetType().Name.ToLower() != "List`1")
            {
                PropertyInfo[] bindObjectProperties = bindObject.GetType().GetProperties();

                if (bindObjectProperties?.Count() > 0)
                {
                    foreach (PropertyInfo property in bindObjectProperties)
                    {
                        if (property.CanRead)
                        {
                            object propertyValue = property.GetValue(bindObject);
                            List<Type> stringableTypes = new List<Type>()
                            {
                                typeof(string)
                                ,typeof(DateTime)
                                ,typeof(decimal)
                                ,typeof(int)
                                ,typeof(long)
                                ,typeof(decimal)
                            };

                            if (propertyValue != null)
                            {
                                if (IsList(propertyValue))
                                {
                                    IEnumerable<object> propertyValueList = (IEnumerable<object>)propertyValue;

                                    if (propertyValueList != null && propertyValueList.Count() > 0)
                                    {
                                        string templateRepeat = "";
                                        string[] repeatItemList = new string[0];
                                        List<string> repeatItemListTemp = new List<string>();

                                        if (templateTemp.Contains("repeatitem"))
                                        {
                                            repeatItemListTemp = GetTemplateFileElementRepeatItem(ref templateRepeat);
                                            repeatItemList = new string[repeatItemListTemp.Count];
                                        }

                                        foreach (object objectValue in propertyValueList)
                                        {
                                            string templateRepeatTemp = "";

                                            for (int i = 0; i < repeatItemListTemp.Count; i++)
                                            {
                                                string templateRepeatItemTemp = "";

                                                templateRepeatItemTemp = TemplateBindDataRepeat(repeatItemListTemp[i], objectValue, propertyValue.GetType().GetGenericArguments().Single().Name, ref indexTemp, propertyIgnoreDefault);

                                                repeatItemList[i] += (repeatItemListTemp[i] != templateRepeatItemTemp) ? templateRepeatItemTemp : "";
                                            }

                                            templateRepeatTemp += TemplateBindDataRepeat(templateTemp, objectValue, propertyValue.GetType().GetGenericArguments().Single().Name, ref indexTemp, propertyIgnoreDefault);

                                            if (templateTemp != templateRepeatTemp)
                                            {
                                                templateRepeat += templateRepeatTemp;
                                            }
                                            else
                                            {
                                                templateRepeat = templateTemp;
                                            }

                                            indexTemp++;
                                        }

                                        for (int i = 0; i < repeatItemList.Count(); i++)
                                        {
                                            templateRepeat = templateRepeat.Replace($"$REPEATITEM{i}$", repeatItemList[i]);
                                        }

                                        templateTemp = templateRepeat;
                                    }
                                }
                                else if (propertyValue.GetType().IsPrimitive == false && stringableTypes.Contains(propertyValue.GetType()) == false && propertyValue is Enum == false && propertyValue is Guid == false)
                                {
                                    templateTemp = TemplateBindDataRepeat(templateTemp, propertyValue, propertyValue.GetType().Name, ref indexTemp, propertyIgnoreDefault);
                                }
                                else
                                {
                                    templateTemp = TemplateBindDataObject(templateTemp, property, propertyValue, prefix, propertyIgnoreDefault);
                                }
                            }
                        }
                    }
                }
            }

            templateTemp = templateTemp.Replace("$INDEX$", Convert.ToString(indexTemp));
            index = indexTemp;

            return templateTemp;
        }

        public static string TemplateBindDataObject(string template, PropertyInfo property, object propertyValue, string prefix)
        {
            return TemplateBindDataObject(template, property.Name, propertyValue, prefix, null);
        }

        public static string TemplateBindDataObject(string template, PropertyInfo property, object propertyValue, string prefix, List<string> propertyIgnoreDefault)
        {
            return TemplateBindDataObject(template, property.Name, propertyValue, prefix, propertyIgnoreDefault);
        }

        public static string TemplateBindDataObject(string template, Type property, object propertyValue, string prefix)
        {
            return TemplateBindDataObject(template, property.Name, propertyValue, prefix, null);
        }

        public static string TemplateBindDataObject(string template, Type property, object propertyValue, string prefix, List<string> propertyIgnoreDefault)
        {
            return TemplateBindDataObject(template, property.Name, propertyValue, prefix, propertyIgnoreDefault);
        }

        public static string TemplateBindDataObject(string template, string propertyName, object propertyValue, string prefix)
        {
            return TemplateBindDataObject(template, propertyName, propertyValue, prefix, null);
        }

        public static string TemplateBindDataObject(string template, string propertyName, object propertyValue, string prefix, List<string> propertyIgnoreDefault)
        {
            string templateTemp = template;
            string tokenName = "";
            string propertyValueString = propertyValue.ToString();

            if (propertyIgnoreDefault != null && propertyIgnoreDefault.Contains(propertyName))
            {
                if (string.IsNullOrEmpty(propertyValueString))
                {
                    return templateTemp;
                }
            }

            if (propertyValue is Enum)
            {
                propertyValueString = Utility.SplitCamelCase(propertyValueString);
            }

            if (propertyValue.GetType() == typeof(decimal))
            {
                decimal propertyValueDecimal = 0.0M;

                if (decimal.TryParse(propertyValueString, out propertyValueDecimal))
                {
                    templateTemp = templateTemp.Replace($"${tokenName + ".ONEDP"}$", Math.Round(propertyValueDecimal, 2).ToString());
                    templateTemp = templateTemp.Replace($"${tokenName + ".TWODP"}$", Math.Round(propertyValueDecimal, 3).ToString());

                    propertyValueString = Math.Round(propertyValueDecimal, 0).ToString();
                }
            }

            tokenName += (string.IsNullOrEmpty(prefix)) ? "" : prefix.ToUpper() + "_";
            tokenName += propertyName.ToUpper();
            templateTemp = templateTemp.Replace($"${tokenName + ".TOUPPER"}$", propertyValueString.ToUpper());
            templateTemp = templateTemp.Replace($"${tokenName}$", propertyValueString);
            templateTemp = templateTemp.Replace($"${tokenName + ".TOLOWER"}$", propertyValueString.ToLower());

            return templateTemp;
        }

        public static string TemplateBindEnum(string template, object bindObject, string prefix, List<string> propertyIgnoreDefault)
        {
            Type enumType = null;

            enumType = bindObject.GetType();

            template = TemplateBindDataObject(template, enumType, bindObject.ToString(), prefix, propertyIgnoreDefault);
            template = TemplateBindDataObject(template, enumType.Name + "ID", (int)bindObject, prefix, propertyIgnoreDefault);

            if (template.Contains($"${enumType.Name.ToUpper()}SELECT$"))
            {
                Array enumValues = null;

                enumValues = Enum.GetValues(enumType);

                if (enumValues != null && enumValues.Length > 0)
                {
                    string selectOptions = "";

                    foreach (object value in enumValues)
                    {
                        string enumName = "";

                        enumName = Enum.GetName(enumType, value);

                        if (string.IsNullOrEmpty(enumName) == false)
                        {
                            string selectedOption = "";

                            selectedOption = ((int)value == (int)bindObject) ? " selected" : "";

                            selectOptions += $"<option value='{(int)value}'{selectedOption}>{SplitCamelCase(enumName)}</option>";
                        }
                    }

                    template = TemplateBindDataObject(template, enumType.Name + "SELECT", selectOptions, prefix, propertyIgnoreDefault);
                }
            }

            return template;
        }

        public static PropertyInfo GetDataPropertyDisplayType(object obj, DataPropertyDisplayType displayType)
        {
            return obj.GetType()
                .GetProperties()
                .Where(p => DataPropertyDisplay.IsDefined(p, typeof(DataPropertyDisplay)))
                .Where(p => (((DataPropertyDisplay)DataPropertyDisplay.GetCustomAttribute(
                                p, typeof(DataPropertyDisplay))).Type == displayType)).FirstOrDefault();
        }

        public static PropertyInfo GetDataProviderKeyTypeSingle(object obj, DataProviderKeyType keyType)
        {
            return obj.GetType()
                .GetProperties()
                .Where(p => DataProviderResultField.IsDefined(p, typeof(DataProviderResultField)))
                .Where(p => ((DataProviderResultField)DataProviderResultField.GetCustomAttribute(p, typeof(DataProviderResultField))).KeyType == keyType).FirstOrDefault();
        }

        public static List<PropertyInfo> GetDataProviderKeyTypeList(List<object> objectList, DataProviderKeyType keyType)
        {
            return GetDataProviderKeyTypeList(objectList, keyType, false);
        }

        public static List<PropertyInfo> GetDataProviderKeyTypeList(List<object> objectList, DataProviderKeyType keyType, bool filterValueNotSet)
        {
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();

            foreach (object obj in objectList)
            {
                List<PropertyInfo> propertyInfoListTemp = null;

                propertyInfoListTemp = GetDataProviderKeyTypeList(obj, keyType);

                if (filterValueNotSet)
                {
                    foreach (PropertyInfo primaryKey in propertyInfoListTemp)
                    {
                        int primaryKeyValueInt = 0;
                        string primaryKeyValueString = primaryKey.GetValue(obj).ToString();

                        if (int.TryParse(primaryKeyValueString, out primaryKeyValueInt))
                        {
                            if (primaryKeyValueInt == 0)
                            {
                                propertyInfoList.Add(primaryKey);
                            }
                        }
                    }
                }
                else
                {
                    propertyInfoList.AddRange(GetDataProviderKeyTypeList(obj, keyType));
                }
            }

            return propertyInfoList;
        }

        public static List<PropertyInfo> GetDataProviderKeyTypeList(object obj, DataProviderKeyType keyType)
        {
            return obj.GetType()
                .GetProperties()
                .Where(p => DataProviderResultField.IsDefined(p, typeof(DataProviderResultField)))
                .Where(p => ((DataProviderResultField)DataProviderResultField.GetCustomAttribute(p, typeof(DataProviderResultField))).KeyType == keyType).ToList();
        }

        public static PropertyInfo GetDataProviderResultFieldActionSingle(object obj, DataProviderResultFieldAction fieldAction)
        {
            return obj.GetType()
                    .GetProperties()
                    .Where(p => DataProviderResultField.IsDefined(p, typeof(DataProviderResultField)))
                    .Where(p => ((DataProviderResultField)DataProviderResultField.GetCustomAttribute(
                                    p, typeof(DataProviderResultField))).Actions.Contains(fieldAction)).FirstOrDefault();
        }

        public static List<PropertyInfo> GetDataProviderResultFieldActionList(object obj, DataProviderResultFieldAction fieldAction)
        {
            return obj.GetType()
                    .GetProperties()
                    .Where(p => DataProviderResultField.IsDefined(p, typeof(DataProviderResultField)))
                    .Where(p => ((DataProviderResultField)DataProviderResultField.GetCustomAttribute(
                                    p, typeof(DataProviderResultField))).Actions.Contains(fieldAction)).ToList();
        }

        public static PropertyInfo GetDataProviderField(object obj, string fieldName)
        {
            return obj.GetType()
                .GetProperties()
                .Where(p => DataProviderResultField.IsDefined(p, typeof(DataProviderResultField)))
                .Where(p => ((DataProviderResultField)DataProviderResultField.GetCustomAttribute(p, typeof(DataProviderResultField))).Field == fieldName).FirstOrDefault();
        }

        public static PropertyInfo GetDataProviderField(object obj, DataProviderKeyType keyType)
        {
            return obj.GetType()
                .GetProperties()
                .Where(p => DataProviderResultField.IsDefined(p, typeof(DataProviderResultField)))
                .Where(p => ((DataProviderResultField)DataProviderResultField.GetCustomAttribute(p, typeof(DataProviderResultField))).KeyType == keyType).FirstOrDefault();
        }

        public static PropertyInfo GetDataProviderField(object obj, DataProviderKeyType keyType, string fieldName)
        {
            return obj.GetType()
                            .GetProperties()
                            .Where(p => DataProviderResultField.IsDefined(p, typeof(DataProviderResultField)))
                            .Where(p => ((DataProviderResultField)DataProviderResultField.GetCustomAttribute(p, typeof(DataProviderResultField))).KeyType == keyType)
                            .Where(p => ((DataProviderResultField)DataProviderResultField.GetCustomAttribute(p, typeof(DataProviderResultField))).Field == fieldName).FirstOrDefault();
        }

        public static object GetDataProviderPrimaryKeyObject(List<object> objList, string fieldName)
        {
            foreach (object obj in objList)
            {
                PropertyInfo propertyInfo = GetDataProviderField(obj, DataProviderKeyType.PrimaryKey, fieldName);

                if (propertyInfo != null)
                {
                    return obj;
                }
            }

            return null;
        }

        public static object GetDataProviderPrimaryKeyObjectList(List<object> objList, string fieldName, int listIndex, int listCount)
        {
            foreach (object obj in objList)
            {
                if (IsList(obj))
                {
                    IList objListTemp = (IList)obj;

                    if (objListTemp != null && listIndex < objListTemp.Count && objListTemp.Count == listCount)
                    {
                        object objTemp = objListTemp[listIndex];

                        if (objTemp != null)
                        {
                            PropertyInfo propertyInfo = GetDataProviderField(objTemp, DataProviderKeyType.PrimaryKey, fieldName);

                            if (propertyInfo != null)
                            {
                                return objTemp;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static bool HasDataProviderField(object obj, string fieldName)
        {
            return obj.GetType()
                .GetProperties()
                .Where(p => DataProviderResultField.IsDefined(p, typeof(DataProviderResultField)))
                .Any(p => ((DataProviderResultField)DataProviderResultField.GetCustomAttribute(p, typeof(DataProviderResultField))).Field == fieldName);
        }

        public static bool HasDataProviderKeyType(PropertyInfo objectPropertyInfo, params DataProviderKeyType[] keyTypeList)
        {
            foreach (DataProviderKeyType keyType in keyTypeList)
            {
                if (HasDataProviderKeyType(objectPropertyInfo, keyType))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasDataProviderKeyType(PropertyInfo objectPropertyInfo, DataProviderKeyType keyType)
        {
            return (((DataProviderResultField)objectPropertyInfo.GetCustomAttribute(typeof(DataProviderResultField))).KeyType == keyType);
        }

        public static bool IsList(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj is IList &&
                   obj.GetType().IsGenericType &&
                   obj.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static bool IsDictionary(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj is IDictionary &&
                   obj.GetType().IsGenericType &&
                   obj.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }

        public static string ReplaceLast(string source, string oldValue, string newValue)
        {
            int place = source.LastIndexOf(oldValue);

            if (place == -1)
            {
                return source;
            }

            return source.Remove(place, oldValue.Length).Insert(place, newValue);
        }

        public static List<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
