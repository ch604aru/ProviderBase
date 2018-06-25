using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Reflection;

namespace ProviderBase.Framework
{
    public static class Utility
    {
        public static Dictionary<string, string> GetQueryAndFormValues()
        {
            Dictionary<string, string> queryValues = null;
            Dictionary<string, string> formValues = null;

            formValues = Utility.GetFormValues();
            queryValues = Utility.GetQueryValues();

            if (formValues.Count > 0 && queryValues.Count > 0)
            {
                Dictionary<string, string> queryAndFormValues = new Dictionary<string, string>();

                queryAndFormValues = ProviderBase.Data.Utility.MergeDictionaryLeft(formValues, queryValues);

                return queryAndFormValues;
            }
            else if (formValues.Count > 0 && queryValues.Count == 0)
            {
                return formValues;
            }
            else
            {
                return queryValues;
            }
        }

        public static T GetFormValue<T>(string key, T defaultValue)
        {
            HttpContext context = HttpContext.Current;
            string[] keyValue = context.Request.Form.GetValues(key);

            if (keyValue != null && keyValue.Length > 0)
            {
                if (defaultValue is Enum)
                {
                    string enumStringValue = WebUtility.HtmlDecode(keyValue[0]);
                    int enumValue = 0;

                    if (int.TryParse(enumStringValue, out enumValue))
                    {
                        return (T)Enum.ToObject(typeof(T), enumValue);
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
                else
                {
                    return (T)Convert.ChangeType(WebUtility.HtmlDecode(keyValue[0]), typeof(T));
                }
            }
            else
            {
                return defaultValue;
            }
        }

        public static Guid GetFormValue(string key, Guid defaultValue)
        {
            HttpContext context = HttpContext.Current;
            string[] keyValue = context.Request.Form.GetValues(key);

            if (keyValue != null && keyValue.Length > 0)
            {
                return new Guid(keyValue[0]);
            }
            else
            {
                return defaultValue;
            }
        }

        public static T GetQueryValue<T>(string key, T defaultValue)
        {
            HttpContext context = HttpContext.Current;
            string[] keyValue = context.Request.QueryString.GetValues(key);

            if (keyValue != null && keyValue.Length > 0)
            {
                return (T)Convert.ChangeType(WebUtility.HtmlDecode(keyValue[0]), typeof(T));
            }
            else
            {
                return defaultValue;
            }
        }

        public static Dictionary<string, string> GetFormValues()
        {
            return GetFormValues("");
        }

        public static Dictionary<string, string> GetFormValues(string prefix)
        {
            HttpContext context = HttpContext.Current;
            Dictionary<string, string> formValues = new Dictionary<string, string>();

            foreach (string key in context.Request.Form.AllKeys)
            {
                if (key.StartsWith(prefix))
                {
                    formValues.Add(key, WebUtility.HtmlDecode(context.Request.Form[key]));
                }
            }

            return formValues;
        }

        public static Dictionary<string, string> GetQueryValues()
        {
            HttpContext context = HttpContext.Current;
            Dictionary<string, string> formValues = new Dictionary<string, string>();

            foreach (string key in context.Request.QueryString.AllKeys)
            {
                formValues.Add(key, WebUtility.HtmlDecode(context.Request.QueryString[key]));
            }

            return formValues;
        }

        public static object BindFormValues(object obj)
        {
            return BindFormValues(obj, "");
        }

        public static object BindFormValues(object obj, string prefix)
        {
            return BindFormValues(obj, prefix, DataProviderKeyType.None);
        }

        public static T BindFormValues<T>(T obj, string prefix, DataProviderKeyType ignoreKeyType)
        {
            Dictionary<string, string> formValues = null;
            
            formValues = GetFormValues(prefix);

            if (formValues?.Count > 0)
            {
                List<PropertyInfo> objPropertyInfo = null;

                objPropertyInfo = obj.GetType().GetProperties().ToList();

                if (objPropertyInfo?.Count > 0)
                {
                    foreach (PropertyInfo property in objPropertyInfo)
                    {
                        object objProperty = null;

                        objProperty = property.GetValue(obj);

                        if (ProviderBase.Data.Utility.HasDataProviderKeyType(property, ignoreKeyType) == false)
                        {
                            string objValue = "";

                            if (formValues.TryGetValue(prefix + property.Name, out objValue))
                            {
                                if (property.PropertyType.IsEnum)
                                {
                                    int objValueInt = 0;

                                    objValueInt = ProviderBase.Data.Utility.TryParse<int>(objValue);

                                    property.SetValue(obj, objValueInt);
                                }
                                else
                                {
                                    object value = null;

                                    value = ProviderBase.Data.Utility.ChangeType(objValue, property.PropertyType);

                                    property.SetValue(obj, value);
                                }
                            }
                        }
                    }
                }
            }

            return obj;
        }

        public static string ToQueryString(NameValueCollection collection)
        {
            var array = (from key in collection.AllKeys
                         from value in collection.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value))).ToArray();
            return string.Join("&", array);
        }

        public static string GeneratePaging(PagingItem pagingItem, int pageCurrent, int pageTotal)
        {
            string template = "";
            
            if (pagingItem != null && pageTotal > 1)
            {
                string templateButton = "";
                string templateUpperBand = "";
                string templateLowerBand = "";
                int templateUpperLoop = 0;
                int templateLowerLoop = 0;

                template += "<div class=\"paging\">";

                templateButton += "<button type=\"submit\" name=\"PageCurrent\"";
                templateButton += (string.IsNullOrEmpty(pagingItem.PagingItemCSSClass) == false) ? $" class=\"{pagingItem.PagingItemCSSClass}\"" : "";

                if (pagingItem.IncludeFirst && pageCurrent > 1)
                {
                    template += templateButton;
                    template += $"value=\"{1}\"> << </button>";
                }

                if (pagingItem.IncludePrevious && pageCurrent > 1)
                {
                    template += templateButton;
                    template += $"value=\"{pageCurrent - 1}\"> < </button>";
                }

                if (pagingItem.PagingLowerBand > 0)
                {
                    string pagingLowerBandTemp = "";

                    for (int i = 1; i < pagingItem.PagingLowerBand + 1; i++)
                    {
                        int newPage = (pageCurrent - i);
                        string templateTemp = "";

                        if (newPage >= 1)
                        {
                            templateTemp += templateButton;
                            templateTemp += $"value=\"{newPage}\"> {newPage} </button>";

                            pagingLowerBandTemp = templateTemp + pagingLowerBandTemp;
                        }
                        else
                        {
                            templateLowerLoop++;
                        }
                    }

                    template += pagingLowerBandTemp;
                }

                template += "<button type=\"submit\" name=\"Paging\"";
                template += (string.IsNullOrEmpty(pagingItem.PagingItemCSSClass) == false) ? $" class=\"{pagingItem.PagingItemCSSClass}" : "";
                template += (string.IsNullOrEmpty(pagingItem.pagingItemSelectedCSSClass) == false) ? $" {pagingItem.pagingItemSelectedCSSClass}\"" : "\"";
                template += $"value=\"{pageCurrent}\"> {pageCurrent} </button>";

                if (pagingItem.PagingUpperBand > 0)
                {
                    for (int i = 1; i < pagingItem.PagingUpperBand + 1; i++)
                    {
                        int newPage = (pageCurrent + i);

                        if (newPage <= pageTotal)
                        {
                            templateUpperBand += templateButton;
                            templateUpperBand += $"value=\"{newPage}\"> {newPage} </button>";
                        }
                        else
                        {
                            templateUpperLoop++;
                        }
                    }
                }

                if (pagingItem.LoopPaging)
                {
                    for (int i = 1; i < templateLowerLoop + 1; i++)
                    {
                        int newPage = ((pageCurrent - pagingItem.PagingLowerBand) - i);

                        if (newPage > 1)
                        {
                            templateUpperBand += templateButton;
                            templateUpperBand += $"value=\"{i}\"> {i} </button>";
                        }
                    }

                    for (int i = 1; i < templateUpperLoop + 1; i++)
                    {
                        int newPage = ((pageCurrent + pagingItem.PagingUpperBand) + i);

                        if (newPage < pageTotal && newPage != pageCurrent)
                        {
                            string temp = "";

                            temp += templateButton;
                            temp += $"value=\"{i}\"> {i} </button>";

                            templateLowerBand = temp + templateLowerBand;
                        }
                    }
                }

                template += templateLowerBand;
                template += templateUpperBand;

                if (pagingItem.IncludeNext && pageCurrent < pageTotal)
                {
                    template += templateButton;
                    template += $"value=\"{pageCurrent + 1}\"> > </button>";
                }

                if (pagingItem.IncludeLast && pageCurrent < pageTotal)
                {
                    template += templateButton;
                    template += $"value=\"{pageTotal}\"> >> </button>";
                }

                template += "</div>";
            }

            return template;
        }

        public static string GetRedirect(HttpContext currentContext, RedirectReason redirectReason)
        {
            string redirectURL = "";

            if (redirectReason > 0)
            {
                switch (redirectReason)
                {
                    case RedirectReason.PageNotFound:
                    case RedirectReason.InvalidPermission:
                        redirectURL += "/Oops/" + redirectReason.ToString();
                        break;
                }
            }

            return redirectURL;
        }

        public static void Redirect(HttpContext currentContext, RedirectReason redirectReason)
        {
            currentContext.Response.Redirect(Utility.GetRedirect(currentContext, redirectReason));
        }
    }
}
