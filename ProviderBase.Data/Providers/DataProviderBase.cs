using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using ProviderBase.Data.Entities;
using System.Linq;
using System;
using System.Collections;
using Dapper;

namespace ProviderBase.Data.Providers
{
    public abstract class DataProviderBase
    {
        public enum DataProviderAction
        {
            Select,
            Insert,
            Update,
            Where,
            Delete
        };


        // Select Link
        protected static List<T> ExecuteDataProviderActionSelectLink<T, T2>(string connectionStringName, T actionItem, T2 joinItem, string linkTableName) where T : new() where T2 : new()
        {
            string tableName = "";
            string joinTableName = "";
            string tablePrimaryKey = "";
            string joinPrimaryKey = "";
            List<PropertyInfo> wherePropertyFieldList = null;
            List<PropertyInfo> selectPropertyFieldList = null;
            List<T> returnItems = new List<T>();

            selectPropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Select);

            // .ToList() - Possibly composite key
            wherePropertyFieldList = Utility.GetDataProviderKeyTypeList(joinItem, DataProviderKeyType.PrimaryKey);

            tablePrimaryKey = Utility.GetDataProviderKeyTypeSingle(joinItem, DataProviderKeyType.PrimaryKey).GetCustomAttribute<DataProviderResultField>().Field;

            joinPrimaryKey = Utility.GetDataProviderKeyTypeSingle(actionItem, DataProviderKeyType.PrimaryKey).GetCustomAttribute<DataProviderResultField>().Field;

            tableName = joinItem.GetType().GetCustomAttribute<DataProviderTable>().Table;
            joinTableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;

            if (selectPropertyFieldList != null && selectPropertyFieldList.Count > 0 && wherePropertyFieldList != null && wherePropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false
               && joinTableName != null && string.IsNullOrEmpty(joinTableName) == false && tablePrimaryKey != null && string.IsNullOrEmpty(tablePrimaryKey) == false && joinPrimaryKey != null && string.IsNullOrEmpty(joinPrimaryKey) == false)
            {
                string query = "";
                string queryFields = "";
                string whereFields = "";
                string selectPropertyField = "";
                string wherePropertyField = "";
                object wherePropertyValue = null;
                object wherePropertyValueDefault = null;
                bool selectAdded = false;
                bool whereAdded = false;
                DynamicParameters sqlParameters = null;
                object compareItem = Activator.CreateInstance(joinItem.GetType());

                sqlParameters = new DynamicParameters();

                // Select parameters
                for (int i = 0; i < selectPropertyFieldList.Count(); i++)
                {
                    selectPropertyField = selectPropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;

                    if (string.IsNullOrEmpty(selectPropertyField) == false)
                    {
                        queryFields += (selectAdded) ? ", " : "";

                        queryFields += "C.[" + selectPropertyField + "]";

                        selectAdded = true;
                    }
                }


                // Add where based on primary key
                for (int i = 0; i < wherePropertyFieldList.Count; i++)
                {
                    wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    wherePropertyValue = wherePropertyFieldList[i].GetValue(joinItem);
                    wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                    if ((wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                    {
                        whereFields += (whereAdded) ? " AND " : "";

                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                        sqlParameters.Add("@" + wherePropertyField, wherePropertyValue);
                        whereAdded = true;
                    }
                }

                // Else add where based on where fields
                if (whereAdded == false)
                {
                    wherePropertyFieldList = Utility.GetDataProviderResultFieldActionList(joinItem, DataProviderResultFieldAction.Where);

                    if (wherePropertyFieldList != null && wherePropertyFieldList.Count > 0)
                    {
                        for (int i = 0; i < wherePropertyFieldList.Count; i++)
                        {
                            wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                            wherePropertyValue = wherePropertyFieldList[i].GetValue(joinItem);
                            wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                            if ((wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                            {
                                whereFields += (whereAdded) ? " AND " : "";

                                whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                                sqlParameters.Add("@" + wherePropertyField, wherePropertyValue);
                                whereAdded = true;
                            }
                        }
                    }
                }

                query = $"SELECT {queryFields} FROM [dbo].[{tableName}] a"
                        + $" INNER JOIN {linkTableName} B ON A.{tablePrimaryKey} = B.{tablePrimaryKey}"
                        + $" INNER JOIN {joinTableName} C ON B.{joinPrimaryKey} = C.{joinPrimaryKey}";

                if (whereAdded)
                {
                    query += $" WHERE {whereFields}";
                }

                if (selectAdded)
                {
                    returnItems = ExecuteReader<T>(connectionStringName, query, sqlParameters);
                }
            }
            else
            {
                ExecuteDataProviderActionException(selectPropertyFieldList, wherePropertyFieldList, tableName);
            }

            return returnItems;
        }

        protected static List<object> ExecuteDataProviderActionSelectLink(string connectionStringName, object actionItem, object joinItem, string linkTableName)
        {
            string tableName = "";
            string joinTableName = "";
            string tablePrimaryKey = "";
            string joinPrimaryKey = "";
            List<PropertyInfo> wherePropertyFieldList = null;
            List<PropertyInfo> selectPropertyFieldList = null;
            List<object> returnItems = new List<object>();

            selectPropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Select);

            // .ToList() - Possibly composite key
            wherePropertyFieldList = Utility.GetDataProviderKeyTypeList(joinItem, DataProviderKeyType.PrimaryKey);

            tablePrimaryKey = Utility.GetDataProviderKeyTypeSingle(joinItem, DataProviderKeyType.PrimaryKey).GetCustomAttribute<DataProviderResultField>().Field;

            joinPrimaryKey = Utility.GetDataProviderKeyTypeSingle(actionItem, DataProviderKeyType.PrimaryKey).GetCustomAttribute<DataProviderResultField>().Field;

            tableName = joinItem.GetType().GetCustomAttribute<DataProviderTable>().Table;
            joinTableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;

            if (selectPropertyFieldList != null && selectPropertyFieldList.Count > 0 && wherePropertyFieldList != null && wherePropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false
               && joinTableName != null && string.IsNullOrEmpty(joinTableName) == false && tablePrimaryKey != null && string.IsNullOrEmpty(tablePrimaryKey) == false && joinPrimaryKey != null && string.IsNullOrEmpty(joinPrimaryKey) == false)
            {
                string query = "";
                string queryFields = "";
                string whereFields = "";
                string selectPropertyField = "";
                string wherePropertyField = "";
                object wherePropertyValue = null;
                object wherePropertyValueDefault = null;
                bool selectAdded = false;
                bool whereAdded = false;
                DynamicParameters sqlParameters = null;
                object compareItem = Activator.CreateInstance(joinItem.GetType());

                sqlParameters = new DynamicParameters();

                // Select parameters
                for (int i = 0; i < selectPropertyFieldList.Count(); i++)
                {
                    selectPropertyField = selectPropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;

                    if (string.IsNullOrEmpty(selectPropertyField) == false)
                    {
                        queryFields += (selectAdded) ? ", " : "";

                        queryFields += "C.[" + selectPropertyField + "]";

                        selectAdded = true;
                    }
                }


                // Add where based on primary key
                for (int i = 0; i < wherePropertyFieldList.Count; i++)
                {
                    wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    wherePropertyValue = wherePropertyFieldList[i].GetValue(joinItem);
                    wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                    if ((wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                    {
                        whereFields += (whereAdded) ? " AND " : "";

                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                        sqlParameters.Add("@" + wherePropertyField, wherePropertyValue);
                        whereAdded = true;
                    }
                }

                // Else add where based on where fields
                if (whereAdded == false)
                {
                    wherePropertyFieldList = Utility.GetDataProviderResultFieldActionList(joinItem, DataProviderResultFieldAction.Where);

                    if (wherePropertyFieldList != null && wherePropertyFieldList.Count > 0)
                    {
                        for (int i = 0; i < wherePropertyFieldList.Count; i++)
                        {
                            wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                            wherePropertyValue = wherePropertyFieldList[i].GetValue(joinItem);
                            wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                            if ((wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                            {
                                whereFields += (whereAdded) ? " AND " : "";

                                whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                                sqlParameters.Add("@" + wherePropertyField, wherePropertyValue);
                                whereAdded = true;
                            }
                        }
                    }
                }

                query = $"SELECT {queryFields} FROM [dbo].[{tableName}] A"
                        + $" INNER JOIN {linkTableName} B ON A.{tablePrimaryKey} = B.{tablePrimaryKey}"
                        + $" INNER JOIN {joinTableName} C ON B.{joinPrimaryKey} = C.{joinPrimaryKey}";

                if (whereAdded)
                {
                    query += $" WHERE {whereFields}";
                }

                if (selectAdded)
                {
                    returnItems = ExecuteReader(connectionStringName, actionItem.GetType(), query, sqlParameters);
                }
            }
            else
            {
                ExecuteDataProviderActionException(selectPropertyFieldList, wherePropertyFieldList, tableName);
            }

            return returnItems;
        }

        // Select Overload Methods
        protected static T ExecuteDataProviderActionSelectSingle<T>(string connectionStringName, T actionItem) where T : new()
        {
            return ExecuteDataProviderActionSelectSingle<T>(connectionStringName, actionItem, null, null, false, true, null);
        }

        protected static object ExecuteDataProviderActionSelectSingle(string connectionStringName, object actionItem)
        {
            return ExecuteDataProviderActionSelectSingle(connectionStringName, actionItem, null, null, false, true, null);
        }

        protected static T ExecuteDataProviderActionSelectSingleFull<T>(string connectionStringName, T actionItem, DataProviderResultFilter paging) where T : new()
        {
            return ExecuteDataProviderActionSelectSingle<T>(connectionStringName, actionItem, null, null, true, true, paging);
        }

        protected static object ExecuteDataProviderActionSelectSingleFull(string connectionStringName, object actionItem, DataProviderResultFilter paging)
        {
            return ExecuteDataProviderActionSelectSingle(connectionStringName, actionItem, null, null, true, true, paging);
        }

        protected static T ExecuteDataProviderActionSelectSingleUsingDefault<T>(string connectionStringName, T actionItem, params string[] whereParameterIgnoreDefault) where T : new()
        {
            return ExecuteDataProviderActionSelectSingle<T>(connectionStringName, actionItem, null, whereParameterIgnoreDefault, false, true, null);
        }

        protected static object ExecuteDataProviderActionSelectSingleUsingDefault(string connectionStringName, object actionItem, params string[] whereParameterIgnoreDefault)
        {
            return ExecuteDataProviderActionSelectSingle(connectionStringName, actionItem, null, whereParameterIgnoreDefault, false, true, null);
        }

        protected static T ExecuteDataProviderActionSelectSingleUsingDefaultFull<T>(string connectionStringName, T actionItem, params string[] whereParameterIgnoreDefault) where T : new()
        {
            return ExecuteDataProviderActionSelectSingle<T>(connectionStringName, actionItem, null, whereParameterIgnoreDefault, true, true, null);
        }

        protected static object ExecuteDataProviderActionSelectSingleUsingDefaultFull(string connectionStringName, object actionItem, params string[] whereParameterIgnoreDefault)
        {
            return ExecuteDataProviderActionSelectSingle(connectionStringName, actionItem, null, whereParameterIgnoreDefault, true, true, null);
        }

        protected static T ExecuteDataProviderActionSelectSingleOrDefault<T>(string connectionStringName, T actionItem, params string[] whereParameterIncludeDefault) where T : new()
        {
            return ExecuteDataProviderActionSelectSingle<T>(connectionStringName, actionItem, whereParameterIncludeDefault, null, false, true, null);
        }

        protected static object ExecuteDataProviderActionSelectSingleOrDefault(string connectionStringName, object actionItem, params string[] whereParameterIncludeDefault)
        {
            return ExecuteDataProviderActionSelectSingle(connectionStringName, actionItem, whereParameterIncludeDefault, null, false, true, null);
        }

        protected static T ExecuteDataProviderActionSelectSingleOrDefaultFull<T>(string connectionStringName, T actionItem, params string[] whereParameterIncludeDefault) where T : new()
        {
            return ExecuteDataProviderActionSelectSingle<T>(connectionStringName, actionItem, whereParameterIncludeDefault, null, true, true, null);
        }

        protected static object ExecuteDataProviderActionSelectSingleOrDefaultFull(string connectionStringName, object actionItem, params string[] whereParameterIncludeDefault)
        {
            return ExecuteDataProviderActionSelectSingle(connectionStringName, actionItem, whereParameterIncludeDefault, null, true, true, null);
        }

        protected static List<T> ExecuteDataProviderActionSelect<T>(string connectionStringName, T actionItem, DataProviderResultFilter paging) where T : new()
        {
            return ExecuteDataProviderActionSelect<T>(connectionStringName, actionItem, null, null, true, paging);
        }

        protected static List<object> ExecuteDataProviderActionSelect(string connectionStringName, object actionItem, DataProviderResultFilter paging)
        {
            return ExecuteDataProviderActionSelect(connectionStringName, actionItem, null, null, true, paging);
        }

        protected static List<T> ExecuteDataProviderActionSelectAll<T>(string connectionStringName, T actionItem, DataProviderResultFilter paging) where T : new()
        {
            return ExecuteDataProviderActionSelect<T>(connectionStringName, actionItem, null, null, false, paging);
        }

        protected static List<object> ExecuteDataProviderActionSelectAll(string connectionStringName, object actionItem)
        {
            return ExecuteDataProviderActionSelect(connectionStringName, actionItem, null, null, false, null);
        }

        protected static List<T> ExecuteDataProviderActionSelectUsingDefault<T>(string connectionStringName, T actionItem, params string[] whereParameterIgnoreDefault) where T : new()
        {
            return ExecuteDataProviderActionSelect<T>(connectionStringName, actionItem, null, whereParameterIgnoreDefault, true, null);
        }

        protected static List<object> ExecuteDataProviderActionSelectUsingDefault(string connectionStringName, object actionItem, params string[] whereParameterIgnoreDefault)
        {
            return ExecuteDataProviderActionSelect(connectionStringName, actionItem, null, whereParameterIgnoreDefault, true, null);
        }

        protected static List<T> ExecuteDataProviderActionSelectOrDefault<T>(string connectionStringName, T actionItem, string[] whereParameterIncludeDefault, DataProviderResultFilter paging) where T : new()
        {
            return ExecuteDataProviderActionSelect<T>(connectionStringName, actionItem, whereParameterIncludeDefault, null, true, paging);
        }

        protected static List<object> ExecuteDataProviderActionSelectOrDefault(string connectionStringName, object actionItem, string[] whereParameterIncludeDefault, DataProviderResultFilter paging)
        {
            return ExecuteDataProviderActionSelect(connectionStringName, actionItem, whereParameterIncludeDefault, null, true, paging);
        }

        // Select Main Methods
        protected static T ExecuteDataProviderActionSelectSingle<T>(string connectionStringName, T actionItem, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, bool resultSetSelect, bool whereRequired, DataProviderResultFilter paging) where T : new()
        {
            string query = "";
            string queryWhere = "";
            string queryFrom = "";
            bool resultSetContinue = resultSetSelect;
            int readerResultSet = 1;
            PropertyInfo resultSetPropertyInfo = null;
            DynamicParameters sqlParameters = null;
            DataProviderResultFilterItem filterItem = null;
            T returnItem = new T();

            sqlParameters = new DynamicParameters();

            if (paging != null)
            {
                filterItem = paging.GetFilterItem(0);
            }

            query += GetQuery(actionItem, whereParameterIncludeDefault, whereParameterIgnoreDefault, ref queryFrom, ref queryWhere, ref sqlParameters, filterItem);
            query += ";" + Environment.NewLine;

            while (resultSetContinue)
            {
                resultSetPropertyInfo = actionItem.GetType()
                               .GetProperties()
                               .Where(p => DataProviderResultSet.IsDefined(p, typeof(DataProviderResultSet)))
                               .FirstOrDefault(p => ((DataProviderResultSet)DataProviderResultSet.GetCustomAttribute(
                                                p, typeof(DataProviderResultSet))).ResultSetID == readerResultSet);

                if (resultSetPropertyInfo == null)
                {
                    resultSetContinue = false;
                }
                else
                {
                    object resultSetObject = null;

                    resultSetObject = Activator.CreateInstance(resultSetPropertyInfo.PropertyType);

                    if (resultSetObject != null)
                    {
                        if (paging != null)
                        {
                            filterItem = paging.GetFilterItem(readerResultSet);
                        }

                        if (string.IsNullOrEmpty(resultSetPropertyInfo.GetCustomAttribute<DataProviderResultSet>().LinkTable) == false)
                        {
                            if (resultSetObject.GetType().IsGenericType && resultSetObject is IEnumerable)
                            {
                                resultSetObject = (Object)Activator.CreateInstance(resultSetObject.GetType().GetGenericArguments()[0]);
                            }

                            if (resultSetObject != null)
                            {
                                string linkTable = resultSetPropertyInfo.GetCustomAttribute<DataProviderResultSet>().LinkTable;
                                query += GetQueryLink(resultSetObject, actionItem, linkTable, whereParameterIncludeDefault, whereParameterIgnoreDefault, queryFrom, queryWhere, ref sqlParameters, filterItem);
                                query += ";" + Environment.NewLine;
                            }
                        }
                        else if (resultSetObject.GetType().IsGenericType && resultSetObject is IEnumerable)
                        {
                            resultSetObject = (Object)Activator.CreateInstance(resultSetObject.GetType().GetGenericArguments()[0]);

                            if (resultSetObject != null)
                            {
                                query += GetQueryJoin(resultSetObject, actionItem, whereParameterIncludeDefault, whereParameterIgnoreDefault, queryFrom, queryWhere, ref sqlParameters, filterItem);
                                query += ";" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            query += GetQueryJoin(resultSetObject, null, whereParameterIncludeDefault, whereParameterIgnoreDefault, queryFrom, queryWhere, ref sqlParameters, filterItem);
                            query += ";" + Environment.NewLine;
                        }
                    }
                }

                readerResultSet++;
            }

            if (string.IsNullOrEmpty(query) == false && (whereRequired == false || string.IsNullOrEmpty(queryWhere) == false))
            {
                returnItem = ExecuteReaderSingleFull<T>(connectionStringName, query, sqlParameters);
            }

            return returnItem;
        }

        protected static object ExecuteDataProviderActionSelectSingle(string connectionStringName, object actionItem, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, bool resultSetSelect, bool whereRequired, DataProviderResultFilter paging)
        {
            string query = "";
            string queryWhere = "";
            string queryFrom = "";
            bool resultSetContinue = resultSetSelect;
            int readerResultSet = 1;
            PropertyInfo resultSetPropertyInfo = null;
            DynamicParameters sqlParameters = null;
            DataProviderResultFilterItem filterItem = null;
            object returnItem = new object();

            sqlParameters = new DynamicParameters();

            if (paging != null)
            {
                filterItem = paging.GetFilterItem(0);
            }

            query += GetQuery(actionItem, whereParameterIncludeDefault, whereParameterIgnoreDefault, ref queryFrom, ref queryWhere, ref sqlParameters, filterItem);
            query += ";" + Environment.NewLine;

            while (resultSetContinue)
            {
                resultSetPropertyInfo = actionItem.GetType()
                               .GetProperties()
                               .Where(p => DataProviderResultSet.IsDefined(p, typeof(DataProviderResultSet)))
                               .FirstOrDefault(p => ((DataProviderResultSet)DataProviderResultSet.GetCustomAttribute(
                                                p, typeof(DataProviderResultSet))).ResultSetID == readerResultSet);

                if (resultSetPropertyInfo == null)
                {
                    resultSetContinue = false;
                }
                else
                {
                    object resultSetObject = null;

                    resultSetObject = Activator.CreateInstance(resultSetPropertyInfo.PropertyType);

                    if (resultSetObject != null)
                    {
                        if (paging != null)
                        {
                            filterItem = paging.GetFilterItem(readerResultSet);
                        }

                        if (string.IsNullOrEmpty(resultSetPropertyInfo.GetCustomAttribute<DataProviderResultSet>().LinkTable) == false)
                        {
                            if (resultSetObject.GetType().IsGenericType && resultSetObject is IEnumerable)
                            {
                                resultSetObject = (Object)Activator.CreateInstance(resultSetObject.GetType().GetGenericArguments()[0]);
                            }

                            if (resultSetObject != null)
                            {
                                string linkTable = resultSetPropertyInfo.GetCustomAttribute<DataProviderResultSet>().LinkTable;
                                query += GetQueryLink(resultSetObject, actionItem, linkTable, whereParameterIncludeDefault, whereParameterIgnoreDefault, queryFrom, queryWhere, ref sqlParameters, filterItem);
                                query += ";" + Environment.NewLine;
                            }
                        }
                        else if (resultSetObject.GetType().IsGenericType && resultSetObject is IEnumerable)
                        {
                            resultSetObject = (Object)Activator.CreateInstance(resultSetObject.GetType().GetGenericArguments()[0]);

                            if (resultSetObject != null)
                            {
                                query += GetQueryJoin(resultSetObject, actionItem, whereParameterIncludeDefault, whereParameterIgnoreDefault, queryFrom, queryWhere, ref sqlParameters, filterItem);
                                query += ";" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            query += GetQueryJoin(resultSetObject, actionItem, whereParameterIncludeDefault, whereParameterIgnoreDefault, queryFrom, queryWhere, ref sqlParameters, filterItem);
                            query += ";" + Environment.NewLine;
                        }
                    }
                }

                readerResultSet++;
            }

            if (string.IsNullOrEmpty(query) == false && (whereRequired == false || string.IsNullOrEmpty(queryWhere) == false))
            {
                returnItem = ExecuteReaderSingleFull(connectionStringName, actionItem.GetType(), query, sqlParameters);
            }

            return returnItem;
        }

        protected static List<T> ExecuteDataProviderActionSelect<T>(string connectionStringName, T actionItem, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, bool whereRequired, DataProviderResultFilter paging) where T : new()
        {
            string query = "";
            string queryWhere = "";
            string queryFrom = "";
            DynamicParameters sqlParameters = null;
            DataProviderResultFilterItem filterItem = null;
            List<T> returnItems = new List<T>();

            sqlParameters = new DynamicParameters();

            if (paging != null)
            {
                filterItem = paging.GetFilterItem(typeof(T));
            }

            query += GetQuery(actionItem, whereParameterIncludeDefault, whereParameterIgnoreDefault, ref queryFrom, ref queryWhere, ref sqlParameters, filterItem);
            query += ";" + Environment.NewLine;

            if (string.IsNullOrEmpty(query) == false && (whereRequired == false || string.IsNullOrEmpty(queryWhere) == false))
            {
                returnItems = ExecuteReader<T>(connectionStringName, query, sqlParameters);
            }

            return returnItems;
        }

        protected static List<object> ExecuteDataProviderActionSelect(string connectionStringName, object actionItem, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, bool whereRequired, DataProviderResultFilter paging)
        {
            string query = "";
            string queryWhere = "";
            string queryFrom = "";
            DynamicParameters sqlParameters = null;
            DataProviderResultFilterItem filterItem = null;
            List<object> returnItem = new List<object>();

            sqlParameters = new DynamicParameters();

            if (paging != null)
            {
                filterItem = paging.GetFilterItem(actionItem.GetType());
            }

            query += GetQuery(actionItem, whereParameterIncludeDefault, whereParameterIgnoreDefault, ref queryFrom, ref queryWhere, ref sqlParameters, filterItem);
            query += ";" + Environment.NewLine;

            if (string.IsNullOrEmpty(query) == false && (whereRequired == false || string.IsNullOrEmpty(queryWhere) == false))
            {
                returnItem = ExecuteReader(connectionStringName, actionItem.GetType(), query, sqlParameters);
            }

            return returnItem;
        }

        protected static int ExecuteDataProviderActionSelectCount<T1>(string connectionStringName, T1 actionItem, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, bool whereRequired) where T1 : new()
        {
            string query = "";
            string queryWhere = "";
            string queryFrom = "";
            DynamicParameters sqlParameters = null;
            int returnItem = 0;

            sqlParameters = new DynamicParameters();

            query += GetQueryCount(actionItem, null, whereParameterIncludeDefault, whereParameterIgnoreDefault, queryFrom, ref queryWhere, ref sqlParameters);
            query += ";" + Environment.NewLine;

            if (string.IsNullOrEmpty(query) == false && (whereRequired == false || string.IsNullOrEmpty(queryWhere) == false))
            {
                returnItem = ExecuteReaderSingle<int>(connectionStringName, query, sqlParameters);
            }

            return returnItem;
        }

        protected static int ExecuteDataProviderActionSelectCount<T1, T2>(string connectionStringName, T1 actionItem, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, bool whereRequired) where T1 : new() where T2 : new()
        {
            string query = "";
            string queryWhere = "";
            string queryFrom = "";
            DynamicParameters sqlParameters = null;
            int returnItem = 0;
            T2 parentItem = new T2();

            sqlParameters = new DynamicParameters();

            query += GetQueryCount(actionItem, parentItem,  whereParameterIncludeDefault, whereParameterIgnoreDefault, queryFrom, ref queryWhere, ref sqlParameters);
            query += ";" + Environment.NewLine;

            if (string.IsNullOrEmpty(query) == false && (whereRequired == false || string.IsNullOrEmpty(queryWhere) == false))
            {
                returnItem = ExecuteReaderSingle<int>(connectionStringName, query, sqlParameters);
            }

            return returnItem;
        }

        protected static T2 ExecuteDataProviderActionSelectNewest<T1, T2>(string connectionStringName, T1 actionItem, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, bool whereRequired) where T1 : new() where T2 : new()
        {
            string query = "";
            string queryWhere = "";
            string queryFrom = "";
            DynamicParameters sqlParameters = null;
            T2 returnItem = new T2();
            T2 parentItem = new T2();

            sqlParameters = new DynamicParameters();

            query += GetQueryNewest(actionItem, parentItem, whereParameterIncludeDefault, whereParameterIgnoreDefault, queryFrom, ref queryWhere, ref sqlParameters);
            query += ";" + Environment.NewLine;

            if (string.IsNullOrEmpty(query) == false && (whereRequired == false || string.IsNullOrEmpty(queryWhere) == false))
            {
                returnItem = ExecuteReaderSingle<T2>(connectionStringName, query, sqlParameters);
            }

            return returnItem;
        }


        // Insert
        protected static int ExecuteDataProviderActionInsert<T>(string connectionStringName, T actionItem) where T : new()
        {
            return ExecuteDataProviderActionInsert<T>(connectionStringName, actionItem, "", null);
        }

        protected static int ExecuteDataProviderActionInsert(string connectionStringName, object actionItem)
        {
            return ExecuteDataProviderActionInsert(connectionStringName, actionItem, "", null);
        }

        protected static int ExecuteDataProviderActionInsert<T>(string connectionStringName, T actionItem, string tableName, Dictionary<string, string> fieldConvertList) where T : new()
        {
            decimal insertID = 0;
            List<PropertyInfo> insertPropertyFieldList = null;

            insertPropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Insert);

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;
            }

            if (insertPropertyFieldList != null && insertPropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false)
            {
                string query = "";
                string queryFields = "";
                string selectPropertyField = "";
                object selectPropertyValue = "";
                string queryFieldValues = "";
                bool selectAdded = false;
                DynamicParameters sqlParameters = null;

                sqlParameters = new DynamicParameters();

                for (int i = 0; i < insertPropertyFieldList.Count; i++)
                {
                    selectPropertyField = insertPropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    selectPropertyValue = insertPropertyFieldList[i].GetValue(actionItem);

                    queryFields += (selectAdded) ? "," : "";
                    queryFieldValues += (selectAdded) ? "," : "";

                    if (fieldConvertList?.Count > 0)
                    {
                        if (fieldConvertList.ContainsKey(insertPropertyFieldList[i].Name))
                        {
                            fieldConvertList.TryGetValue(insertPropertyFieldList[i].Name, out selectPropertyField);
                        }
                    }

                    queryFields += $"[{selectPropertyField}]";
                    queryFieldValues += $"@{selectPropertyField}";
                    sqlParameters.Add(selectPropertyField, selectPropertyValue);

                    selectAdded = true;
                }

                query = $"INSERT INTO [dbo].[{tableName}] ({queryFields}) VALUES ({queryFieldValues}); SELECT SCOPE_IDENTITY()";

                if (selectAdded)
                {
                    insertID = ExecuteScalar(connectionStringName, query, sqlParameters);
                }
            }
            else
            {
                ExecuteDataProviderActionException(insertPropertyFieldList, tableName);
            }

            return (int)insertID;
        }

        protected static int ExecuteDataProviderActionInsert(string connectionStringName, object actionItem, string tableName, Dictionary<string, string> fieldConvertList)
        {
            decimal insertID = 0;
            List<PropertyInfo> insertPropertyFieldList = null;

            insertPropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Insert);

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;
            }

            if (insertPropertyFieldList != null && insertPropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false)
            {
                string query = "";
                string queryFields = "";
                string selectPropertyField = "";
                object selectPropertyValue = "";
                string queryFieldValues = "";
                bool selectAdded = false;
                DynamicParameters sqlParameters = null;

                sqlParameters = new DynamicParameters();

                for (int i = 0; i < insertPropertyFieldList.Count; i++)
                {
                    selectPropertyField = insertPropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    selectPropertyValue = insertPropertyFieldList[i].GetValue(actionItem);

                    queryFields += (selectAdded) ? "," : "";
                    queryFieldValues += (selectAdded) ? "," : "";

                    if (fieldConvertList?.Count > 0)
                    {
                        fieldConvertList.TryGetValue(selectPropertyField, out selectPropertyField);
                    }

                    queryFields += $"[{selectPropertyField}]";
                    queryFieldValues += $"@{selectPropertyField}";
                    sqlParameters.Add(selectPropertyField, selectPropertyValue);

                    selectAdded = true;
                }

                query = $"INSERT INTO [dbo].[{tableName}] ({queryFields}) VALUES ({queryFieldValues}); SELECT SCOPE_IDENTITY()";

                if (selectAdded)
                {
                    insertID = ExecuteScalar(connectionStringName, query, sqlParameters);
                }
            }
            else
            {
                ExecuteDataProviderActionException(insertPropertyFieldList, tableName);
            }

            return (int)insertID;
        }


        // Update
        protected static void ExecuteDataProviderActionUpdate<T>(string connectionStringName, T actionItem) where T : new()
        {
            string tableName = "";
            List<PropertyInfo> selectPropertyFieldList = null;
            List<PropertyInfo> wherePropertyFieldList = null;

            selectPropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Update);

            // .ToList() - Possibly composite key
            wherePropertyFieldList = Utility.GetDataProviderKeyTypeList(actionItem, DataProviderKeyType.PrimaryKey);

            tableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;

            if (selectPropertyFieldList != null && selectPropertyFieldList.Count > 0 && wherePropertyFieldList != null && wherePropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false)
            {
                string query = "";
                string whereFields = "";
                string wherePropertyField = "";
                object wherePropertyValue = null;
                object wherePropertyValueDefault = null;
                string setFields = "";
                string setPropertyField = "";
                object setPropertyValue = null;
                bool selectAdded = false;
                bool whereAdded = false;
                DynamicParameters sqlParameters = null;
                object compareItem = Activator.CreateInstance(actionItem.GetType());

                sqlParameters = new DynamicParameters();

                for (int i = 0; i < selectPropertyFieldList.Count; i++)
                {
                    setPropertyField = selectPropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    setPropertyValue = selectPropertyFieldList[i].GetValue(actionItem);

                    setFields += (selectAdded) ? "," : "";

                    setFields += $"[{setPropertyField}] = @{setPropertyField}";
                    sqlParameters.Add(setPropertyField, setPropertyValue);

                    selectAdded = true;
                }

                for (int i = 0; i < wherePropertyFieldList.Count; i++)
                {
                    wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                    wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                    if (wherePropertyValue.Equals(wherePropertyValueDefault) == false && string.IsNullOrEmpty(wherePropertyField) == false)
                    {
                        whereFields += (whereAdded) ? "," : "";

                        whereFields += $"[{wherePropertyField}] = @{wherePropertyField}";
                        sqlParameters.Add(wherePropertyField, wherePropertyValue);

                        whereAdded = true;
                    }
                }

                query = $"UPDATE [dbo].[{tableName}] SET {setFields} WHERE {whereFields}";

                // Update must have a WHERE statement, otherwise too dangerous!
                if (selectAdded && whereAdded)
                {
                    ExecuteNonQuery(connectionStringName, query, sqlParameters);
                }
            }
            else
            {
                ExecuteDataProviderActionException(selectPropertyFieldList, wherePropertyFieldList, tableName);
            }
        }

        protected static void ExecuteDataProviderActionUpdate(string connectionStringName, object actionItem)
        {
            string tableName = "";
            List<PropertyInfo> selectPropertyFieldList = null;
            List<PropertyInfo> wherePropertyFieldList = null;

            selectPropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Update);

            // .ToList() - Possibly composite key
            wherePropertyFieldList = Utility.GetDataProviderKeyTypeList(actionItem, DataProviderKeyType.PrimaryKey);

            tableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;

            if (selectPropertyFieldList != null && selectPropertyFieldList.Count > 0 && wherePropertyFieldList != null && wherePropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false)
            {
                string query = "";
                string whereFields = "";
                string wherePropertyField = "";
                object wherePropertyValue = null;
                object wherePropertyValueDefault = null;
                string setFields = "";
                string setPropertyField = "";
                object setPropertyValue = null;
                bool selectAdded = false;
                bool whereAdded = false;
                DynamicParameters sqlParameters = null;
                object compareItem = Activator.CreateInstance(actionItem.GetType());

                sqlParameters = new DynamicParameters();

                for (int i = 0; i < selectPropertyFieldList.Count; i++)
                {
                    setPropertyField = selectPropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    setPropertyValue = selectPropertyFieldList[i].GetValue(actionItem);

                    setFields += (selectAdded) ? "," : "";

                    setFields += $"[{setPropertyField}] = @{setPropertyField}";
                    sqlParameters.Add(setPropertyField, setPropertyValue);

                    selectAdded = true;
                }

                for (int i = 0; i < wherePropertyFieldList.Count; i++)
                {
                    wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                    wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                    if (wherePropertyValue.Equals(wherePropertyValueDefault) == false && string.IsNullOrEmpty(wherePropertyField) == false)
                    {
                        whereFields += (whereAdded) ? "," : "";

                        whereFields += $"[{wherePropertyField}] = @{wherePropertyField}";
                        sqlParameters.Add(wherePropertyField, wherePropertyValue);

                        whereAdded = true;
                    }
                }

                query = $"UPDATE [dbo].[{tableName}] SET {setFields} WHERE {whereFields}";

                // Update must have a WHERE statement, otherwise too dangerous!
                if (selectAdded && whereAdded)
                {
                    ExecuteNonQuery(connectionStringName, query, sqlParameters);
                }
            }
            else
            {
                ExecuteDataProviderActionException(selectPropertyFieldList, wherePropertyFieldList, tableName);
            }
        }

        // Delete
        protected static void ExecuteDataProviderActionDelete<T>(string connectionStringName, T actionItem) where T : new()
        {
            string tableName = "";
            List<PropertyInfo> wherePropertyFieldList = null;

            // .ToList() - Possibly composite key
            wherePropertyFieldList = Utility.GetDataProviderKeyTypeList(actionItem, DataProviderKeyType.PrimaryKey);

            tableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;

            if (wherePropertyFieldList != null && wherePropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false)
            {
                string query = "";
                string whereFields = "";
                string wherePropertyField = "";
                object wherePropertyValue = null;
                object wherePropertyValueDefault = null;
                bool whereAdded = false;
                DynamicParameters sqlParameters = null;
                object compareItem = Activator.CreateInstance(actionItem.GetType());

                sqlParameters = new DynamicParameters();

                for (int i = 0; i < wherePropertyFieldList.Count; i++)
                {
                    wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                    wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                    if (wherePropertyValue.Equals(wherePropertyValueDefault) == false && string.IsNullOrEmpty(wherePropertyField) == false)
                    {
                        whereFields += (whereAdded) ? " AND " : "";

                        whereFields += $"[{wherePropertyField}] = @{wherePropertyField}";
                        sqlParameters.Add(wherePropertyField, wherePropertyValue);

                        whereAdded = true;
                    }
                }

                query = $"DELETE FROM [dbo].[{tableName}] WHERE {whereFields}";

                // Delete must have a WHERE statement, otherwise too dangerous!
                if (whereAdded)
                {
                    ExecuteNonQuery(connectionStringName, query, sqlParameters);
                }
            }
            else
            {
                ExecuteDataProviderActionException(wherePropertyFieldList, tableName);
            }
        }

        protected static void ExecuteDataProviderActionDelete(string connectionStringName, object actionItem)
        {
            string tableName = "";
            List<PropertyInfo> wherePropertyFieldList = null;

            // .ToList() - Possibly composite key
            wherePropertyFieldList = Utility.GetDataProviderKeyTypeList(actionItem, DataProviderKeyType.PrimaryKey);

            tableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;

            if (wherePropertyFieldList != null && wherePropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false)
            {
                string query = "";
                string whereFields = "";
                string wherePropertyField = "";
                object wherePropertyValue = null;
                object wherePropertyValueDefault = null;
                bool whereAdded = false;
                DynamicParameters sqlParameters = null;
                object compareItem = Activator.CreateInstance(actionItem.GetType());

                sqlParameters = new DynamicParameters();

                for (int i = 0; i < wherePropertyFieldList.Count; i++)
                {
                    wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                    wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                    if (wherePropertyValue.Equals(wherePropertyValueDefault) == false && string.IsNullOrEmpty(wherePropertyField) == false)
                    {
                        whereFields += (whereAdded) ? " AND " : "";

                        whereFields += $"[{wherePropertyField}] = @{wherePropertyField}";
                        sqlParameters.Add(wherePropertyField, wherePropertyValue);

                        whereAdded = true;
                    }
                }

                query = $"DELETE FROM [dbo].[{tableName}] WHERE {whereFields}";

                // Delete must have a WHERE statement, otherwise too dangerous!
                if (whereAdded)
                {
                    ExecuteNonQuery(connectionStringName, query, sqlParameters);
                }
            }
            else
            {
                ExecuteDataProviderActionException(wherePropertyFieldList, tableName);
            }
        }

        // Execute query
        protected static T ExecuteStoredProcedureReaderSingle<T>(string connectionStringName, string storedProcedureName) where T : new()
        {
            return ExecuteStoredProcedureReaderSingle<T>(connectionStringName, storedProcedureName, null);
        }

        protected static T ExecuteStoredProcedureReaderSingle<T>(string connectionStringName, string storedProcedureName, DynamicParameters storedProcedureParameters) where T : new()
        {
            T returnItem = new T();

            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                connection.Open();

                using (SqlMapper.GridReader reader = connection.QueryMultiple(storedProcedureName, storedProcedureParameters, commandType: CommandType.StoredProcedure))
                {
                    returnItem = ReaderBindDataObject<T>(reader, returnItem);
                }

                connection.Close();
            }

            return returnItem;
        }

        protected static object ExecuteStoredProcedureReaderSingle(string connectionStringName, string storedProcedureName)
        {
            return ExecuteStoredProcedureReaderSingle(connectionStringName, storedProcedureName, null);
        }

        protected static object ExecuteStoredProcedureReaderSingle(string connectionStringName, string storedProcedureName, DynamicParameters storedProcedureParameters)
        {
            object returnItem = new object();

            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                connection.Open();

                using (SqlMapper.GridReader reader = connection.QueryMultiple(storedProcedureName, storedProcedureParameters, commandType: CommandType.StoredProcedure))
                {
                    returnItem = ReaderBindDataObject(reader, returnItem);
                }

                connection.Close();
            }

            return returnItem;
        }

        protected static List<T> ExecuteStoredProcedureReader<T>(string connectionStringName, string storedProcedureName) where T : new()
        {
            return ExecuteStoredProcedureReader<T>(connectionStringName, storedProcedureName, null);
        }

        protected static List<T> ExecuteStoredProcedureReader<T>(string connectionStringName, string storedProcedureName, DynamicParameters storedProcedureParameters) where T : new()
        {
            List<T> returnItems = new List<T>();

            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                connection.Open();

                using (SqlMapper.GridReader reader = connection.QueryMultiple(storedProcedureName, storedProcedureParameters, commandType: CommandType.StoredProcedure))
                {
                    returnItems = ReaderBindDataObject<T>(reader, returnItems);
                }

                connection.Close();
            }

            return returnItems;
        }

        protected static List<object> ExecuteStoredProcedureReader(string connectionStringName, string storedProcedureName)
        {
            return ExecuteStoredProcedureReader(connectionStringName, storedProcedureName, null);
        }

        protected static List<object> ExecuteStoredProcedureReader(string connectionStringName, string storedProcedureName, DynamicParameters storedProcedureParameters)
        {
            List<object> returnItems = new List<object>();

            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                connection.Open();

                using (SqlMapper.GridReader reader = connection.QueryMultiple(storedProcedureName, storedProcedureParameters, commandType: CommandType.StoredProcedure))
                {
                    returnItems = ReaderBindDataObject(reader, returnItems);
                }

                connection.Close();
            }

            return returnItems;
        }

        protected static void ExecuteStoredProcedure(string connectionStringName, string storedProcedureName)
        {
            ExecuteStoredProcedure(connectionStringName, storedProcedureName, null);
        }

        protected static void ExecuteStoredProcedure(string connectionStringName, string storedProcedureName, DynamicParameters storedProcedureParameters)
        {
            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                try
                {
                    connection.Open();

                    connection.Execute(storedProcedureName, storedProcedureParameters, commandType: CommandType.StoredProcedure);

                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + Environment.NewLine + "Stored Procedure: " + storedProcedureName, ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        protected static T ExecuteReaderSingle<T>(string connectionStringName, string query) where T : new()
        {
            return ExecuteReaderSingle<T>(connectionStringName, query, null);
        }

        protected static T ExecuteReaderSingle<T>(string connectionStringName, string query, DynamicParameters sqlParameters) where T : new()
        {
            T returnItem = new T();

            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                try
                {
                    connection.Open();

                    returnItem = connection.Query<T>(query, sqlParameters).FirstOrDefault();

                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + Environment.NewLine + "Query: " + query, ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return (returnItem == null) ? new T() : returnItem;
        }

        protected static object ExecuteReaderSingle(string connectionStringName, Type type, string query)
        {
            return ExecuteReaderSingle(connectionStringName, type, query, null);
        }

        protected static object ExecuteReaderSingle(string connectionStringName, Type type, string query, DynamicParameters sqlParameters)
        {
            object returnItem = new object();

            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                try
                {
                    connection.Open();

                    returnItem = connection.Query(type, query, sqlParameters).FirstOrDefault();

                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + Environment.NewLine + "Query: " + query, ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return (returnItem == null) ? new object() : returnItem;
        }

        protected static List<T> ExecuteReader<T>(string connectionStringName, string query) where T : new()
        {
            return ExecuteReader<T>(connectionStringName, query, null);
        }

        protected static List<T> ExecuteReader<T>(string connectionStringName, string query, DynamicParameters sqlParameters) where T : new()
        {
            List<T> returnItems = new List<T>();

            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                try
                {
                    connection.Open();

                    returnItems = connection.Query<T>(query, sqlParameters).ToList();

                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + Environment.NewLine + "Query: " + query, ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return returnItems;
        }

        protected static List<object> ExecuteReader(string connectionStringName, Type type, string query)
        {
            return ExecuteReader(connectionStringName, type, query, null);
        }

        protected static List<object> ExecuteReader(string connectionStringName, Type type, string query, DynamicParameters sqlParameters)
        {
            List<object> returnItems = new List<object>();

            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                try
                {
                    connection.Open();

                    returnItems = connection.Query(type, query, sqlParameters).ToList();

                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + Environment.NewLine + "Query: " + query, ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return returnItems;
        }

        protected static T ExecuteReaderSingleFull<T>(string connectionStringName, string query) where T : new()
        {
            return ExecuteReaderSingleFull<T>(connectionStringName, query, null);
        }

        protected static T ExecuteReaderSingleFull<T>(string connectionStringName, string query, DynamicParameters sqlParameters) where T : new()
        {
            T returnItem = new T();

            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                connection.Open();

                using (SqlMapper.GridReader reader = connection.QueryMultiple(query, sqlParameters))
                {
                    returnItem = ReaderBindDataObject<T>(reader, returnItem);
                }

                connection.Close();
            }

            return returnItem;
        }

        protected static object ExecuteReaderSingleFull(string connectionStringName, Type type, string query)
        {
            return ExecuteReaderSingleFull(connectionStringName, type, query, null);
        }

        protected static object ExecuteReaderSingleFull(string connectionStringName, Type type, string query, DynamicParameters sqlParameters)
        {
            object returnItem = Activator.CreateInstance(type);

            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                connection.Open();

                using (SqlMapper.GridReader reader = connection.QueryMultiple(query, sqlParameters))
                {
                    returnItem = ReaderBindDataObject(reader, returnItem);
                }

                connection.Close();
            }

            return returnItem;
        }

        protected static decimal ExecuteScalar(string connectionStringName, string query)
        {
            return ExecuteScalar(connectionStringName, query, null);
        }

        protected static decimal ExecuteScalar(string connectionStringName, string query, DynamicParameters sqlParameters)
        {
            decimal insertID = 0;

            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                try
                {
                    object insertIDTemp = 0;

                    connection.Open();

                    insertIDTemp = connection.ExecuteScalar(query, sqlParameters);

                    if (insertIDTemp != null)
                    {
                        insertID = (decimal)insertIDTemp;
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + Environment.NewLine + "Query: " + query, ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return insertID;
        }

        protected static void ExecuteNonQuery(string connectionStringName, string query)
        {
            ExecuteNonQuery(connectionStringName, query, null);
        }

        protected static void ExecuteNonQuery(string connectionStringName, string query, DynamicParameters sqlParameters)
        {
            using (SqlConnection connection = new SqlConnection(Utility.GetConnectionString(connectionStringName)))
            {
                try
                {
                    connection.Open();

                    connection.Execute(query, sqlParameters);

                    connection.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + Environment.NewLine + "Query: " + query, ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        // Reader
        private static T ReaderBindDataObject<T>(SqlMapper.GridReader reader, T item)
        {
            T returnItem = item;

            int readerResultSet = 0;

            while (reader.IsConsumed == false)
            {
                Object resultSetObject = null;
                PropertyInfo resultSetPropertyInfo = null;
                DataProviderResultFilterItem filterItem = null;

                if (readerResultSet > 0)
                {
                    if (returnItem != null)
                    {
                        if (resultSetObject == null)
                        {
                            resultSetPropertyInfo = returnItem.GetType()
                                       .GetProperties()
                                       .Where(p => DataProviderResultSet.IsDefined(p, typeof(DataProviderResultSet)))
                                       .FirstOrDefault(p => ((DataProviderResultSet)DataProviderResultSet.GetCustomAttribute(
                                                        p, typeof(DataProviderResultSet))).ResultSetID == readerResultSet);

                            resultSetObject = Activator.CreateInstance(resultSetPropertyInfo.PropertyType);

                            if (resultSetObject.GetType().IsGenericType && resultSetObject is IEnumerable)
                            {
                                IList resultsetObjectList = null;

                                resultsetObjectList = (IList)Activator.CreateInstance(resultSetObject.GetType());
                                resultSetObject = (Object)Activator.CreateInstance(resultSetObject.GetType().GetGenericArguments()[0]);

                                resultSetObject = reader.Read(resultSetObject.GetType()).ToList();

                                foreach (var resultItem in (IEnumerable)resultSetObject)
                                {
                                    resultsetObjectList.Add(resultItem);
                                }

                                resultSetPropertyInfo.SetValue(returnItem, resultsetObjectList);
                            }
                            else
                            {
                                resultSetObject = reader.Read(resultSetObject.GetType()).FirstOrDefault();
                                resultSetPropertyInfo.SetValue(returnItem, resultSetObject);
                            }
                        }
                    }
                    else
                    {
                        // No results, returnItem = null & we've read the first result set
                        return returnItem;
                    }
                }
                else
                {
                    returnItem = reader.Read<T>().FirstOrDefault();
                }

                readerResultSet++;
            }

            return returnItem;
        }

        private static object ReaderBindDataObject(SqlMapper.GridReader reader, object item)
        {
            object returnItem = item;

            int readerResultSet = 0;

            while (reader.IsConsumed == false)
            {
                Object resultSetObject = null;
                PropertyInfo resultSetPropertyInfo = null;

                if (readerResultSet > 0)
                {
                    if (returnItem != null)
                    {
                        if (resultSetObject == null)
                        {
                            resultSetPropertyInfo = returnItem.GetType()
                                       .GetProperties()
                                       .Where(p => DataProviderResultSet.IsDefined(p, typeof(DataProviderResultSet)))
                                       .FirstOrDefault(p => ((DataProviderResultSet)DataProviderResultSet.GetCustomAttribute(
                                                        p, typeof(DataProviderResultSet))).ResultSetID == readerResultSet);

                            resultSetObject = Activator.CreateInstance(resultSetPropertyInfo.PropertyType);

                            if (resultSetObject.GetType().IsGenericType && resultSetObject is IEnumerable)
                            {
                                IList resultsetObjectList = null;

                                resultsetObjectList = (IList)Activator.CreateInstance(resultSetObject.GetType());
                                resultSetObject = (Object)Activator.CreateInstance(resultSetObject.GetType().GetGenericArguments()[0]);

                                resultSetObject = reader.Read(resultSetObject.GetType()).ToList();

                                foreach (var resultItem in (IEnumerable)resultSetObject)
                                {
                                    resultsetObjectList.Add(resultItem);
                                }

                                resultSetPropertyInfo.SetValue(returnItem, resultsetObjectList);
                            }
                            else
                            {
                                resultSetObject = reader.Read(resultSetObject.GetType()).FirstOrDefault();
                                resultSetPropertyInfo.SetValue(returnItem, resultSetObject);
                            }
                        }
                    }
                    else
                    {
                        // No results, returnItem = null & we've read the first result set
                        return returnItem;
                    }
                }
                else
                {
                    returnItem = reader.Read(item.GetType()).FirstOrDefault();
                }

                readerResultSet++;
            }

            return returnItem;
        }

        private static List<T> ReaderBindDataObject<T>(SqlMapper.GridReader reader, List<T> item) where T : new()
        {
            List<T> returnItems = item;

            int readerResultSet = 0;

            while (reader.IsConsumed == false)
            {
                Object resultSetObject = null;
                PropertyInfo resultSetPropertyInfo = null;

                if (readerResultSet > 0)
                {
                    if (returnItems != null)
                    {
                        if (resultSetObject == null)
                        {
                            resultSetPropertyInfo = returnItems.GetType()
                                       .GetProperties()
                                       .Where(p => DataProviderResultSet.IsDefined(p, typeof(DataProviderResultSet)))
                                       .FirstOrDefault(p => ((DataProviderResultSet)DataProviderResultSet.GetCustomAttribute(
                                                        p, typeof(DataProviderResultSet))).ResultSetID == readerResultSet);

                            resultSetObject = Activator.CreateInstance(resultSetPropertyInfo.PropertyType);

                            if (resultSetObject.GetType().IsGenericType && resultSetObject is IEnumerable)
                            {
                                IList resultsetObjectList = null;

                                resultsetObjectList = (IList)Activator.CreateInstance(resultSetObject.GetType());
                                resultSetObject = (Object)Activator.CreateInstance(resultSetObject.GetType().GetGenericArguments()[0]);

                                resultSetObject = reader.Read(resultSetObject.GetType()).ToList();

                                foreach (var resultItem in (IEnumerable)resultSetObject)
                                {
                                    resultsetObjectList.Add(resultItem);
                                }

                                resultSetPropertyInfo.SetValue(returnItems, resultsetObjectList);
                            }
                            else
                            {
                                resultSetObject = reader.Read(resultSetObject.GetType()).FirstOrDefault();
                                resultSetPropertyInfo.SetValue(returnItems, resultSetObject);
                            }
                        }
                    }
                    else
                    {
                        // No results, returnItem = null & we've read the first result set
                        return returnItems;
                    }
                }
                else
                {
                    returnItems = reader.Read<T>().ToList();
                }

                readerResultSet++;
            }

            return returnItems;
        }

        private static List<object> ReaderBindDataObject(SqlMapper.GridReader reader, List<object> item)
        {
            List<object> returnItems = item;

            int readerResultSet = 0;

            while (reader.IsConsumed == false)
            {
                Object resultSetObject = null;
                PropertyInfo resultSetPropertyInfo = null;

                if (readerResultSet > 0)
                {
                    if (returnItems != null)
                    {
                        if (resultSetObject == null)
                        {
                            resultSetPropertyInfo = returnItems.GetType()
                                       .GetProperties()
                                       .Where(p => DataProviderResultSet.IsDefined(p, typeof(DataProviderResultSet)))
                                       .FirstOrDefault(p => ((DataProviderResultSet)DataProviderResultSet.GetCustomAttribute(
                                                        p, typeof(DataProviderResultSet))).ResultSetID == readerResultSet);

                            resultSetObject = Activator.CreateInstance(resultSetPropertyInfo.PropertyType);

                            if (resultSetObject.GetType().IsGenericType && resultSetObject is IEnumerable)
                            {
                                IList resultsetObjectList = null;

                                resultsetObjectList = (IList)Activator.CreateInstance(resultSetObject.GetType());
                                resultSetObject = (Object)Activator.CreateInstance(resultSetObject.GetType().GetGenericArguments()[0]);

                                resultSetObject = reader.Read(resultSetObject.GetType()).ToList();

                                foreach (var resultItem in (IEnumerable)resultSetObject)
                                {
                                    resultsetObjectList.Add(resultItem);
                                }

                                resultSetPropertyInfo.SetValue(returnItems, resultsetObjectList);
                            }
                            else
                            {
                                resultSetObject = reader.Read(resultSetObject.GetType()).FirstOrDefault();
                                resultSetPropertyInfo.SetValue(returnItems, resultSetObject);
                            }
                        }
                    }
                    else
                    {
                        // No results, returnItem = null & we've read the first result set
                        return returnItems;
                    }
                }
                else
                {
                    returnItems = reader.Read(returnItems.GetType()).ToList();
                }

                readerResultSet++;
            }

            return returnItems;
        }

        // Exception
        private static void ExecuteDataProviderActionException(List<PropertyInfo> selectPropertyFieldList, string tableName)
        {
            if (selectPropertyFieldList == null || selectPropertyFieldList.Count == 0)
            {
                throw new Exception("No DataProviderResultFieldAction.Select object properties found");
            }
            else if (string.IsNullOrEmpty(tableName))
            {
                throw new Exception("No table name supplied");
            }
        }

        private static void ExecuteDataProviderActionException(List<PropertyInfo> selectPropertyFieldList, List<PropertyInfo> wherePropertyFieldList, string tableName)
        {
            ExecuteDataProviderActionException(selectPropertyFieldList, tableName);

            if (wherePropertyFieldList == null || wherePropertyFieldList.Count == 0)
            {
                throw new Exception("No DataProviderResultFieldAction.Where object properties found");
            }
        }

        // Query Builder
        private static string GetQuery(object actionItem, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, ref string fromClause, ref string whereClause, ref DynamicParameters sqlParameters, DataProviderResultFilterItem filterItem)
        {
            string query = "";
            string tableName = "";
            List<PropertyInfo> wherePropertyFieldList = null;
            List<PropertyInfo> selectPropertyFieldList = null;

            selectPropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Select);

            // .ToList() - Possibly composite key
            wherePropertyFieldList = Utility.GetDataProviderKeyTypeList(actionItem, DataProviderKeyType.PrimaryKey);

            tableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;

            if (selectPropertyFieldList != null && selectPropertyFieldList.Count > 0 && wherePropertyFieldList != null && wherePropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false)
            {
                string queryFields = "";
                string whereFields = "";
                string orderFields = "";
                string wherePropertyField = "";
                object wherePropertyValue = null;
                object wherePropertyValueDefault = null;
                bool selectAdded = false;
                bool whereAdded = false;
                bool orderAddded = false;
                object compareItem = Activator.CreateInstance(actionItem.GetType());

                for (int i = 0; i < selectPropertyFieldList.Count(); i++)
                {
                    string selectPropertyField = "";

                    selectPropertyField = selectPropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;

                    if (string.IsNullOrEmpty(selectPropertyField) == false)
                    {
                        queryFields += (selectAdded) ? ", " : "";

                        queryFields += $"A.[{selectPropertyField}]";

                        selectAdded = true;
                    }
                }

                if (string.IsNullOrEmpty(filterItem?.OrderField) == false)
                {
                    if (Utility.HasDataProviderField(actionItem, filterItem.OrderField))
                    {
                        orderFields += $"ORDER BY {filterItem.OrderField} {filterItem.OrderFieldDirectionString}";
                        orderAddded = true;
                    }
                }

                // Add where based on primary key
                for (int i = 0; i < wherePropertyFieldList.Count; i++)
                {
                    bool includeNestedOr = false;
                    bool nestedWhereAdded = false;

                    wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                    wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                    if (whereParameterIncludeDefault != null && whereParameterIncludeDefault.Count() > 0 && whereParameterIncludeDefault.Contains(wherePropertyField))
                    {
                        includeNestedOr = true;
                    }

                    if (whereParameterIgnoreDefault != null && whereParameterIgnoreDefault.Count() > 0 && whereParameterIgnoreDefault.Contains(wherePropertyField)
                    || (wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                    {
                        whereFields += (whereAdded) ? " AND (" : "(";
                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                        sqlParameters.Add($"@{wherePropertyField}", wherePropertyValue);
                        whereFields += (includeNestedOr) ? "" : ")";

                        whereAdded = true;
                        nestedWhereAdded = true;
                    }

                    if (includeNestedOr)
                    {
                        whereFields += (nestedWhereAdded) ? " OR " : "(";

                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}OR";
                        sqlParameters.Add($"@{wherePropertyField}OR", wherePropertyValue);
                        whereFields += ")";

                        whereAdded = true;
                    }

                    if (orderAddded == false)
                    {
                        orderFields += $"ORDER BY {wherePropertyField}";
                        orderAddded = true;
                    }
                }

                // Else add where based on where fields
                if (whereAdded == false)
                {
                    wherePropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Where);

                    for (int i = 0; i < wherePropertyFieldList.Count; i++)
                    {
                        bool includeNestedOr = false;
                        bool nestedWhereAdded = false;

                        wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                        wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                        wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                        if (whereParameterIncludeDefault != null && whereParameterIncludeDefault.Count() > 0 && whereParameterIncludeDefault.Contains(wherePropertyField))
                        {
                            includeNestedOr = true;
                        }

                        if (whereParameterIgnoreDefault != null && whereParameterIgnoreDefault.Count() > 0 && whereParameterIgnoreDefault.Contains(wherePropertyField)
                        || (wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                        {
                            whereFields += (whereAdded) ? " AND (" : "(";
                            whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                            sqlParameters.Add($"@{wherePropertyField}", wherePropertyValue);
                            whereFields += (includeNestedOr) ? "" : ")";

                            whereAdded = true;
                            nestedWhereAdded = true;
                        }

                        if (includeNestedOr)
                        {
                            whereFields += (nestedWhereAdded) ? " OR " : (whereAdded) ? " AND (" : "(";


                            whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}OR";
                            sqlParameters.Add($"@{wherePropertyField}OR", wherePropertyValueDefault);
                            whereFields += ")";

                            whereAdded = true;
                        }
                    }
                }

                if (string.IsNullOrEmpty(filterItem?.FilterField) == false)
                {
                    if (Utility.HasDataProviderField(actionItem, filterItem.FilterField))
                    {
                        whereFields += (whereAdded) ? " AND (" : "(";
                        whereFields += $"{filterItem.FilterField} LIKE '%' + @FilterFieldValue + '%'";
                        whereFields += ")";

                        sqlParameters.Add("@FilterFieldValue", filterItem.FilterFieldValue);

                        whereAdded = true;
                    }
                }

                query = $"SELECT {queryFields}";

                if (string.IsNullOrEmpty(fromClause))
                {
                    fromClause = $" FROM [dbo].[{tableName}] A";
                    query += fromClause;
                }

                if (string.IsNullOrEmpty(whereClause) && whereAdded)
                {
                    whereClause = $" WHERE {whereFields}";
                    query += whereClause;
                }
                else
                {
                    query += (string.IsNullOrEmpty(whereClause)) ? whereClause : "";
                    query += (whereAdded) ? $" AND {whereFields}" : "";
                }

                if (filterItem?.PageSize > 0 && filterItem?.PageCurrent > 0)
                {
                    query = $"WITH [ResultData] AS ({query}), [ResultCount] AS (SELECT COUNT(*) AS [PageTotalCount]";
                    query += ", @PageSize AS [PageSize]";
                    query += ", @PageCurrent AS [PageCurrent]";
                    query += "FROM [ResultData])";
                    query += $"SELECT * FROM [ResultData], [ResultCount] {orderFields} OFFSET @PageSize * (@PageCurrent - 1) ROWS FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE)";

                    sqlParameters.Add("@PageSize", filterItem.PageSize);
                    sqlParameters.Add("@PageCurrent", filterItem.PageCurrent);
                }
                else if (string.IsNullOrEmpty(orderFields) == false)
                {
                    query += $" {orderFields}";
                }
            }

            return query;
        }

        private static string GetQueryJoin(object actionItem, object parentItem, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, string fromClause, string whereClause, ref DynamicParameters sqlParameters, DataProviderResultFilterItem filterItem)
        {
            string query = "";
            string tableName = "";
            List<PropertyInfo> wherePropertyFieldList = null;
            List<PropertyInfo> selectPropertyFieldList = null;

            selectPropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Select);

            // .ToList() - Possibly composite key
            wherePropertyFieldList = Utility.GetDataProviderKeyTypeList(actionItem, DataProviderKeyType.PrimaryKey);

            tableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;

            if (selectPropertyFieldList != null && selectPropertyFieldList.Count > 0 && wherePropertyFieldList != null && wherePropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false)
            {
                string queryFields = "";
                string whereFields = "";
                string orderFields = "";
                string innerJoinFields = "";
                string wherePropertyField = "";
                object wherePropertyValue = null;
                object wherePropertyValueDefault = null;
                bool selectAdded = false;
                bool whereAdded = false;
                bool orderAdded = false;
                object compareItem = Activator.CreateInstance(actionItem.GetType());

                for (int i = 0; i < selectPropertyFieldList.Count(); i++)
                {
                    string selectPropertyField = "";

                    selectPropertyField = selectPropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;

                    if (string.IsNullOrEmpty(selectPropertyField) == false)
                    {
                        queryFields += (selectAdded) ? ", " : "";

                        queryFields += $"B.[{selectPropertyField}]";

                        selectAdded = true;
                    }
                }

                if (string.IsNullOrEmpty(filterItem?.OrderField) == false)
                {
                    if (Utility.HasDataProviderField(actionItem, filterItem.OrderField))
                    {
                        orderFields += $"ORDER BY {filterItem.OrderField} {filterItem.OrderFieldDirectionString}";
                        orderAdded = true;
                    }
                }

                // Add where based on primary key
                for (int i = 0; i < wherePropertyFieldList.Count; i++)
                {
                    bool includeNestedOr = false;
                    bool nestedWhereAdded = false;

                    wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                    wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                    if (whereParameterIncludeDefault != null && whereParameterIncludeDefault.Count() > 0 && whereParameterIncludeDefault.Contains(wherePropertyField))
                    {
                        includeNestedOr = true;
                    }

                    if (whereParameterIgnoreDefault != null && whereParameterIgnoreDefault.Count() > 0 && whereParameterIgnoreDefault.Contains(wherePropertyField)
                    || (wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                    {
                        whereFields += (whereAdded) ? " AND (" : "(";
                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                        sqlParameters.Add($"@{wherePropertyField}", wherePropertyValue);
                        whereFields += (includeNestedOr) ? "" : ")";

                        whereAdded = true;
                        nestedWhereAdded = true;
                    }

                    if (includeNestedOr)
                    {
                        whereFields += (nestedWhereAdded) ? " OR " : "(";

                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}OR";
                        sqlParameters.Add($"@{wherePropertyField}OR", wherePropertyValue);
                        whereFields += ")";

                        whereAdded = true;
                    }

                    if (parentItem == null)
                    {
                        if (Utility.HasDataProviderField(actionItem, wherePropertyField))
                        {
                            innerJoinFields += $"(A.[{wherePropertyField}] = B.[{wherePropertyField}])";
                        }
                    }

                    if (orderAdded == false)
                    {
                        orderFields += $"ORDER BY {wherePropertyField}";
                        orderAdded = true;
                    }
                }

                if (parentItem != null)
                {
                    List<PropertyInfo> innerJoinPropertyFieldList = Utility.GetDataProviderKeyTypeList(actionItem, DataProviderKeyType.ForeignKey);

                    foreach (PropertyInfo innerJoinPropertyField in innerJoinPropertyFieldList)
                    {
                        string innerJoinField = innerJoinPropertyField.GetCustomAttribute<DataProviderResultField>().Field;

                        if (Utility.HasDataProviderField(parentItem, innerJoinField))
                        {
                            innerJoinFields += $"(A.[{innerJoinField}] = B.[{innerJoinField}])";
                            break;
                        }
                    }
                }

                // Else add where based on where fields
                if (whereAdded == false)
                {
                    wherePropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Where);

                    for (int i = 0; i < wherePropertyFieldList.Count; i++)
                    {
                        bool includeNestedOr = false;
                        bool nestedWhereAdded = false;

                        wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                        wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                        wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                        if (whereParameterIncludeDefault != null && whereParameterIncludeDefault.Count() > 0 && whereParameterIncludeDefault.Contains(wherePropertyField))
                        {
                            includeNestedOr = true;
                        }

                        if (whereParameterIgnoreDefault != null && whereParameterIgnoreDefault.Count() > 0 && whereParameterIgnoreDefault.Contains(wherePropertyField)
                        || (wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                        {
                            whereFields += (whereAdded) ? " AND (" : "(";
                            whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                            sqlParameters.Add($"@{wherePropertyField}", wherePropertyValue);
                            whereFields += (includeNestedOr) ? "" : ")";

                            whereAdded = true;
                            nestedWhereAdded = true;
                        }

                        if (includeNestedOr)
                        {
                            whereFields += (whereAdded) ? " AND " : "(";
                            whereFields += (nestedWhereAdded) ? " OR " : "(";
                            whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}OR";
                            sqlParameters.Add($"@{wherePropertyField}OR", wherePropertyValueDefault);
                            whereFields += ")";

                            whereAdded = true;
                        }
                    }
                }

                if (string.IsNullOrEmpty(filterItem?.FilterField) == false)
                {
                    if (Utility.HasDataProviderField(actionItem, filterItem.FilterField))
                    {
                        whereFields += (whereAdded) ? " AND (" : "(";
                        whereFields += $"{filterItem.FilterField} LIKE '%' + @FilterFieldValue + '%'";
                        whereFields += ")";

                        sqlParameters.Add("@FilterFieldValue", filterItem.FilterFieldValue);

                        whereAdded = true;
                    }
                }

                query = $"SELECT {queryFields}";

                if (string.IsNullOrEmpty(fromClause))
                {
                    query += $" FROM [dbo].[{tableName}] A";
                }
                else
                {
                    query += fromClause;
                    query += $" INNER JOIN [dbo].{tableName} B ON {innerJoinFields}";
                }


                if (string.IsNullOrEmpty(whereClause) && whereAdded)
                {
                    whereClause = $" WHERE {whereFields}";
                    query += whereClause;
                }
                else
                {
                    query += (string.IsNullOrEmpty(whereClause)) ? "" : whereClause;
                    query += (whereAdded) ? $" AND {whereFields}" : "";
                }

                if (filterItem?.PageSize > 0 && filterItem?.PageCurrent > 0)
                {
                    query = $"WITH [ResultData] AS ({query}), [ResultCount] AS (SELECT COUNT(*) AS [PageTotalCount]";
                    query += ", @PageSize AS [PageSize]";
                    query += ", @PageCurrent AS [PageCurrent]";
                    query += "FROM [ResultData])";
                    query += $"SELECT * FROM [ResultData], [ResultCount] {orderFields} OFFSET @PageSize * (@PageCurrent - 1) ROWS FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE)";

                    sqlParameters.Add("@PageSize", filterItem.PageSize);
                    sqlParameters.Add("@PageCurrent", filterItem.PageCurrent);
                }
                else if (string.IsNullOrEmpty(orderFields) == false)
                {
                    query += $" {orderFields}";
                }
            }

            return query;
        }

        private static string GetQueryLink(object actionItem, object parentItem, string linkTableName, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, string fromClause, string whereClause, ref DynamicParameters sqlParameters, DataProviderResultFilterItem filterItem)
        {
            string query = "";
            string tableName = "";
            List<PropertyInfo> wherePropertyFieldList = null;
            List<PropertyInfo> selectPropertyFieldList = null;

            selectPropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Select);

            // .ToList() - Possibly composite key
            wherePropertyFieldList = Utility.GetDataProviderKeyTypeList(actionItem, DataProviderKeyType.PrimaryKey);

            tableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;

            if (selectPropertyFieldList != null && selectPropertyFieldList.Count > 0 && wherePropertyFieldList != null && wherePropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false)
            {
                string queryFields = "";
                string whereFields = "";
                string orderFields = "";
                string innerJoin = "";
                string wherePropertyField = "";
                object wherePropertyValue = null;
                object wherePropertyValueDefault = null;
                bool selectAdded = false;
                bool whereAdded = false;
                bool orderAdded = false;
                object compareItem = Activator.CreateInstance(actionItem.GetType());

                for (int i = 0; i < selectPropertyFieldList.Count(); i++)
                {
                    string selectPropertyField = "";

                    selectPropertyField = selectPropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;

                    if (string.IsNullOrEmpty(selectPropertyField) == false)
                    {
                        queryFields += (selectAdded) ? ", " : "";

                        queryFields += $"C.[{selectPropertyField}]";

                        selectAdded = true;
                    }
                }

                if (string.IsNullOrEmpty(filterItem?.OrderField) == false)
                {
                    if (Utility.HasDataProviderField(actionItem, filterItem.OrderField))
                    {
                        orderFields += $"ORDER BY {filterItem.OrderField} {filterItem.OrderFieldDirectionString}";
                        orderAdded = true;
                    }
                }

                // Add where based on primary key
                for (int i = 0; i < wherePropertyFieldList.Count; i++)
                {
                    bool includeNestedOr = false;
                    bool nestedWhereAdded = false;

                    wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                    wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                    if (whereParameterIncludeDefault != null && whereParameterIncludeDefault.Count() > 0 && whereParameterIncludeDefault.Contains(wherePropertyField))
                    {
                        includeNestedOr = true;
                    }

                    if (whereParameterIgnoreDefault != null && whereParameterIgnoreDefault.Count() > 0 && whereParameterIgnoreDefault.Contains(wherePropertyField)
                    || (wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                    {
                        whereFields += (whereAdded) ? " AND (" : "(";
                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                        sqlParameters.Add($"@{wherePropertyField}", wherePropertyValue);
                        whereFields += (includeNestedOr) ? "" : ")";

                        whereAdded = true;
                        nestedWhereAdded = true;
                    }

                    if (includeNestedOr)
                    {
                        whereFields += (nestedWhereAdded) ? " OR " : "(";
                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}OR";
                        sqlParameters.Add($"@{wherePropertyField}OR", wherePropertyValue);
                        whereFields += ")";

                        whereAdded = true;
                    }

                    if (orderAdded == false)
                    {
                        orderFields += $"ORDER BY {wherePropertyField}";
                        orderAdded = true;
                    }
                }

                if (parentItem != null)
                {
                    List<PropertyInfo> innerJoinPropertyFieldList = Utility.GetDataProviderKeyTypeList(parentItem, DataProviderKeyType.PrimaryKey);

                    foreach (PropertyInfo innerJoinPropertyField in innerJoinPropertyFieldList)
                    {
                        string innerJoinField = innerJoinPropertyField.GetCustomAttribute<DataProviderResultField>().Field;

                        if (Utility.HasDataProviderField(parentItem, innerJoinField))
                        {
                            innerJoin += $" INNER JOIN {linkTableName} B ON (B.{innerJoinField} = A.{innerJoinField})";
                        }
                    }

                    if (actionItem != null)
                    {
                        innerJoinPropertyFieldList = Utility.GetDataProviderKeyTypeList(actionItem, DataProviderKeyType.PrimaryKey);

                        foreach (PropertyInfo innerJoinPropertyField in innerJoinPropertyFieldList)
                        {
                            string innerJoinField = innerJoinPropertyField.GetCustomAttribute<DataProviderResultField>().Field;

                            if (Utility.HasDataProviderField(actionItem, innerJoinField))
                            {
                                innerJoin += $" INNER JOIN {tableName} C ON (C.{innerJoinField} = B.{innerJoinField})";
                            }
                        }
                    }
                }

                // Else add where based on where fields
                if (whereAdded == false)
                {
                    wherePropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Where);

                    for (int i = 0; i < wherePropertyFieldList.Count; i++)
                    {
                        bool includeNestedOr = false;

                        wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                        wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                        wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                        if (whereParameterIncludeDefault != null && whereParameterIncludeDefault.Count() > 0 && whereParameterIncludeDefault.Contains(wherePropertyField))
                        {
                            includeNestedOr = true;
                        }

                        if (whereParameterIgnoreDefault != null && whereParameterIgnoreDefault.Count() > 0 && whereParameterIgnoreDefault.Contains(wherePropertyField)
                        || (wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                        {
                            whereFields += (whereAdded) ? " AND (" : "(";
                            whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                            sqlParameters.Add($"@{wherePropertyField}", wherePropertyValue);
                            whereFields += (includeNestedOr) ? "" : ")";

                            whereAdded = true;
                        }

                        if (orderAdded == false)
                        {
                            orderFields += $"ORDER BY {wherePropertyField}";
                            orderAdded = true;
                        }
                    }
                }

                if (string.IsNullOrEmpty(filterItem?.FilterField) == false)
                {
                    if (Utility.HasDataProviderField(actionItem, filterItem.FilterField))
                    {
                        whereFields += (whereAdded) ? " AND (" : "(";
                        whereFields += $"{filterItem.FilterField} LIKE '%' + @FilterFieldValue + '%'";
                        whereFields += ")";

                        sqlParameters.Add("@FilterFieldValue", filterItem.FilterFieldValue);

                        whereAdded = true;
                    }
                }

                query = $"SELECT {queryFields}";

                if (string.IsNullOrEmpty(fromClause))
                {
                    query += $" FROM [dbo].[{tableName}] A";
                }
                else
                {
                    query += fromClause;
                    query += innerJoin;
                }

                if (string.IsNullOrEmpty(whereClause) && whereAdded)
                {
                    whereClause = $" WHERE {whereFields}";
                    query += whereClause;
                }
                else
                {
                    query += (string.IsNullOrEmpty(whereClause)) ? "" : whereClause;
                    query += (whereAdded) ? $" AND {whereFields}" : "";
                }

                if (filterItem?.PageSize > 0 && filterItem?.PageCurrent > 0)
                {
                    query = $"WITH [ResultData] AS ({query}), [ResultCount] AS (SELECT COUNT(*) AS [PageTotalCount]";
                    query += ", @PageSize AS [PageSize]";
                    query += ", @PageCurrent AS [PageCurrent]";
                    query += "FROM [ResultData])";
                    query += $"SELECT * FROM [ResultData], [ResultCount] {orderFields} OFFSET @PageSize * (@PageCurrent - 1) ROWS FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE)";

                    sqlParameters.Add("@PageSize", filterItem.PageSize);
                    sqlParameters.Add("@PageCurrent", filterItem.PageCurrent);
                }
                else if (string.IsNullOrEmpty(orderFields) == false)
                {
                    query += $" {orderFields}";
                }
            }

            return query;
        }

        private static string GetQueryCount(object actionItem, object parentItem, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, string fromClause, ref string whereClause, ref DynamicParameters sqlParameters)
        {
            string query = "";
            string tableName = "";
            List<PropertyInfo> wherePropertyFieldList = null;
            List<PropertyInfo> selectPropertyFieldList = null;

            if (parentItem != null)
            {
                selectPropertyFieldList = Utility.GetDataProviderResultFieldActionList(parentItem, DataProviderResultFieldAction.Select);
            }
            else
            {
                selectPropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Select);
            }

            // .ToList() - Possibly composite key
            wherePropertyFieldList = Utility.GetDataProviderKeyTypeList(actionItem, DataProviderKeyType.PrimaryKey);

            tableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;

            if (selectPropertyFieldList != null && selectPropertyFieldList.Count > 0 && wherePropertyFieldList != null && wherePropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false)
            {
                string queryFields = "";
                string whereFields = "";
                string innerJoinFields = "";
                string wherePropertyField = "";
                object wherePropertyValue = null;
                object wherePropertyValueDefault = null;
                int parentTableAlias = 64;
                bool whereAdded = false;
                object compareItem = Activator.CreateInstance(actionItem.GetType());

                if (parentItem != null)
                {
                    ParentItemJoin(actionItem, parentItem, ref parentTableAlias, ref innerJoinFields);
                }
                else
                {
                    parentTableAlias += 1;
                }

                if (selectPropertyFieldList?.Count > 0)
                {
                    string selectPropertyField = "";

                    selectPropertyField = selectPropertyFieldList[0].GetCustomAttribute<DataProviderResultField>().Field;

                    queryFields += $"COUNT({Convert.ToChar(parentTableAlias)}.[{selectPropertyField}])";
                }

                // Add where based on primary key
                for (int i = 0; i < wherePropertyFieldList.Count; i++)
                {
                    bool includeNestedOr = false;
                    bool nestedWhereAdded = false;

                    wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                    wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                    if (whereParameterIncludeDefault != null && whereParameterIncludeDefault.Count() > 0 && whereParameterIncludeDefault.Contains(wherePropertyField))
                    {
                        includeNestedOr = true;
                    }

                    if (whereParameterIgnoreDefault != null && whereParameterIgnoreDefault.Count() > 0 && whereParameterIgnoreDefault.Contains(wherePropertyField)
                    || (wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                    {
                        whereFields += (whereAdded) ? " AND (" : "(";
                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                        sqlParameters.Add($"@{wherePropertyField}", wherePropertyValue);
                        whereFields += (includeNestedOr) ? "" : ")";

                        whereAdded = true;
                        nestedWhereAdded = true;
                    }

                    if (includeNestedOr)
                    {
                        whereFields += (nestedWhereAdded) ? " OR " : "(";

                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}OR";
                        sqlParameters.Add($"@{wherePropertyField}OR", wherePropertyValue);
                        whereFields += ")";

                        whereAdded = true;
                    }
                }

                // Else add where based on where fields
                if (whereAdded == false)
                {
                    wherePropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Where);

                    for (int i = 0; i < wherePropertyFieldList.Count; i++)
                    {
                        bool includeNestedOr = false;
                        bool nestedWhereAdded = false;

                        wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                        wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                        wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                        if (whereParameterIncludeDefault != null && whereParameterIncludeDefault.Count() > 0 && whereParameterIncludeDefault.Contains(wherePropertyField))
                        {
                            includeNestedOr = true;
                        }

                        if (whereParameterIgnoreDefault != null && whereParameterIgnoreDefault.Count() > 0 && whereParameterIgnoreDefault.Contains(wherePropertyField)
                        || (wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                        {
                            whereFields += (whereAdded) ? " AND (" : "(";
                            whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                            sqlParameters.Add($"@{wherePropertyField}", wherePropertyValue);
                            whereFields += (includeNestedOr) ? "" : ")";

                            whereAdded = true;
                            nestedWhereAdded = true;
                        }

                        if (includeNestedOr)
                        {
                            whereFields += (whereAdded) ? " AND " : "(";
                            whereFields += (nestedWhereAdded) ? " OR " : "(";
                            whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}OR";
                            sqlParameters.Add($"@{wherePropertyField}OR", wherePropertyValueDefault);
                            whereFields += ")";

                            whereAdded = true;
                        }
                    }
                }

                query = $"SELECT {queryFields}";

                if (string.IsNullOrEmpty(fromClause))
                {
                    query += $" FROM [dbo].[{tableName}] A";

                    if (string.IsNullOrEmpty(innerJoinFields) == false)
                    {
                        query += innerJoinFields;
                    }
                }
                else
                {
                    query += fromClause;
                    query += innerJoinFields;
                }


                if (string.IsNullOrEmpty(whereClause) && whereAdded)
                {
                    whereClause = $" WHERE {whereFields}";
                    query += whereClause;
                }
                else
                {
                    query += (string.IsNullOrEmpty(whereClause)) ? "" : whereClause;
                    query += (whereAdded) ? $" AND {whereFields}" : "";
                }
            }

            return query;
        }

        private static string GetQueryNewest(object actionItem, object parentItem, string[] whereParameterIncludeDefault, string[] whereParameterIgnoreDefault, string fromClause, ref string whereClause, ref DynamicParameters sqlParameters)
        {
            string query = "";
            string tableName = "";
            List<PropertyInfo> wherePropertyFieldList = null;
            List<PropertyInfo> selectPropertyFieldList = null;
            List<PropertyInfo> orderByPropertyFieldList = null;

            selectPropertyFieldList = Utility.GetDataProviderResultFieldActionList(parentItem, DataProviderResultFieldAction.Select);

            // .ToList() - Possibly composite key
            wherePropertyFieldList = Utility.GetDataProviderKeyTypeList(actionItem, DataProviderKeyType.PrimaryKey);

            orderByPropertyFieldList = Utility.GetDataProviderKeyTypeList(parentItem, DataProviderKeyType.PrimaryKey);

            tableName = actionItem.GetType().GetCustomAttribute<DataProviderTable>().Table;

            if (selectPropertyFieldList != null && selectPropertyFieldList.Count > 0 && wherePropertyFieldList != null && wherePropertyFieldList.Count > 0 && string.IsNullOrEmpty(tableName) == false)
            {
                string queryFields = "";
                string whereFields = "";
                string orderByFields = "";
                string innerJoinFields = "";
                string wherePropertyField = "";
                object wherePropertyValue = null;
                object wherePropertyValueDefault = null;
                int parentTableAlias = 64;
                bool whereAdded = false;
                bool orderByAdded = false;
                bool selectAdded = false;
                object compareItem = Activator.CreateInstance(actionItem.GetType());

                if (parentItem != null)
                {
                     ParentItemJoin(actionItem, parentItem, ref parentTableAlias, ref innerJoinFields);
                }

                for (int i = 0; i < selectPropertyFieldList.Count(); i++)
                {
                    string selectPropertyField = "";

                    selectPropertyField = selectPropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;

                    if (string.IsNullOrEmpty(selectPropertyField) == false)
                    {
                        queryFields += (selectAdded) ? ", " : "";

                        queryFields += $"{Convert.ToChar(parentTableAlias)}.[{selectPropertyField}]";

                        selectAdded = true;
                    }
                }

                // Add where based on primary key
                for (int i = 0; i < wherePropertyFieldList.Count; i++)
                {
                    bool includeNestedOr = false;
                    bool nestedWhereAdded = false;

                    wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                    wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                    wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                    if (whereParameterIncludeDefault != null && whereParameterIncludeDefault.Count() > 0 && whereParameterIncludeDefault.Contains(wherePropertyField))
                    {
                        includeNestedOr = true;
                    }

                    if (whereParameterIgnoreDefault != null && whereParameterIgnoreDefault.Count() > 0 && whereParameterIgnoreDefault.Contains(wherePropertyField)
                    || (wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                    {
                        whereFields += (whereAdded) ? " AND (" : "(";
                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                        sqlParameters.Add($"@{wherePropertyField}", wherePropertyValue);
                        whereFields += (includeNestedOr) ? "" : ")";

                        whereAdded = true;
                        nestedWhereAdded = true;
                    }

                    if (includeNestedOr)
                    {
                        whereFields += (nestedWhereAdded) ? " OR " : "(";

                        whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}OR";
                        sqlParameters.Add($"@{wherePropertyField}OR", wherePropertyValue);
                        whereFields += ")";

                        whereAdded = true;
                    }
                }

                // Else add where based on where fields
                if (whereAdded == false)
                {
                    wherePropertyFieldList = Utility.GetDataProviderResultFieldActionList(actionItem, DataProviderResultFieldAction.Where);

                    for (int i = 0; i < wherePropertyFieldList.Count; i++)
                    {
                        bool includeNestedOr = false;
                        bool nestedWhereAdded = false;

                        wherePropertyField = wherePropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;
                        wherePropertyValue = wherePropertyFieldList[i].GetValue(actionItem);
                        wherePropertyValueDefault = wherePropertyFieldList[i].GetValue(compareItem);

                        if (whereParameterIncludeDefault != null && whereParameterIncludeDefault.Count() > 0 && whereParameterIncludeDefault.Contains(wherePropertyField))
                        {
                            includeNestedOr = true;
                        }

                        if (whereParameterIgnoreDefault != null && whereParameterIgnoreDefault.Count() > 0 && whereParameterIgnoreDefault.Contains(wherePropertyField)
                        || (wherePropertyValue.Equals(wherePropertyValueDefault) == false) && string.IsNullOrEmpty(wherePropertyField) == false)
                        {
                            whereFields += (whereAdded) ? " AND (" : "(";
                            whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}";
                            sqlParameters.Add($"@{wherePropertyField}", wherePropertyValue);
                            whereFields += (includeNestedOr) ? "" : ")";

                            whereAdded = true;
                            nestedWhereAdded = true;
                        }

                        if (includeNestedOr)
                        {
                            whereFields += (whereAdded) ? " AND " : "(";
                            whereFields += (nestedWhereAdded) ? " OR " : "(";
                            whereFields += $"A.[{wherePropertyField}] = @{wherePropertyField}OR";
                            sqlParameters.Add($"@{wherePropertyField}OR", wherePropertyValueDefault);
                            whereFields += ")";

                            whereAdded = true;
                        }
                    }
                }

                for (int i = 0; i < orderByPropertyFieldList.Count; i++)
                {
                    string orderByPropertyField = "";

                    orderByPropertyField = orderByPropertyFieldList[i].GetCustomAttribute<DataProviderResultField>().Field;

                    if (string.IsNullOrEmpty(orderByPropertyField) == false)
                    {
                        orderByFields += (orderByAdded) ? ", " : "";
                        orderByFields += $"{Convert.ToChar(parentTableAlias)}.[{orderByPropertyField}] DESC";

                        orderByAdded = true;
                    }
                }

                query = $"SELECT TOP 1 {queryFields}";

                if (string.IsNullOrEmpty(fromClause))
                {
                    query += $" FROM [dbo].[{tableName}] A";

                    if (string.IsNullOrEmpty(innerJoinFields) == false)
                    {
                        query += innerJoinFields;
                    }
                }
                else
                {
                    query += fromClause;
                    query += innerJoinFields;
                }

                if (string.IsNullOrEmpty(whereClause) && whereAdded)
                {
                    whereClause = $" WHERE {whereFields}";
                    query += whereClause;
                }
                else
                {
                    query += (string.IsNullOrEmpty(whereClause)) ? "" : whereClause;
                    query += (whereAdded) ? $" AND {whereFields}" : "";
                }

                if (orderByAdded)
                {
                    query += $" ORDER BY {orderByFields}";
                }
            }

            return query;
        }

        private static bool ParentItemJoin(object actionItem, object parentItem, ref int tableAlias, ref string innerJoinFields)
        {
            return ParentItemJoin(actionItem, parentItem, ref tableAlias, ref innerJoinFields, 0);
        }

        private static bool ParentItemJoin(object actionItem, object parentItem, ref int tableAlias, ref string innerJoinFields, int tableNameAliasOffset)
        {
            bool success = false;

            if (parentItem != null && actionItem != null)
            {
                int readerResultSet = 1;
                bool resultSetContinue = true;
                PropertyInfo resultSetPropertyInfo = null;
                Type parentItemType = null;

                parentItemType = parentItem.GetType();

                while (resultSetContinue)
                {
                    resultSetPropertyInfo = actionItem.GetType()
                               .GetProperties()
                               .Where(p => DataProviderResultSet.IsDefined(p, typeof(DataProviderResultSet)))
                               .FirstOrDefault(p => ((DataProviderResultSet)DataProviderResultSet.GetCustomAttribute(
                                                p, typeof(DataProviderResultSet))).ResultSetID == readerResultSet);

                    if (resultSetPropertyInfo == null)
                    {
                        resultSetContinue = false;
                    }
                    else
                    {
                        object resultSetObject = null;

                        resultSetObject = Activator.CreateInstance(resultSetPropertyInfo.PropertyType);

                        if (resultSetObject.GetType().IsGenericType && resultSetObject is IEnumerable)
                        {
                            resultSetObject = (Object)Activator.CreateInstance(resultSetObject.GetType().GetGenericArguments()[0]);
                        }

                        if (resultSetObject.GetType() == parentItemType)
                        {
                            List<PropertyInfo> innerJoinPropertyFieldList = Utility.GetDataProviderKeyTypeList(resultSetObject, DataProviderKeyType.ForeignKey);

                            foreach (PropertyInfo innerJoinPropertyField in innerJoinPropertyFieldList)
                            {
                                string innerJoinField = innerJoinPropertyField.GetCustomAttribute<DataProviderResultField>().Field;

                                if (Utility.HasDataProviderField(resultSetObject, innerJoinField))
                                {
                                    string tableName = resultSetObject.GetType().GetCustomAttribute<DataProviderTable>().Table;
                                    char tableNameAliasAction = Convert.ToChar(65 + tableNameAliasOffset);
                                    char tableNameAliasParent = Convert.ToChar(65 + tableNameAliasOffset + 1);

                                    innerJoinFields += $" INNER JOIN [{tableName}] {tableNameAliasParent} ON ({tableNameAliasAction}.[{innerJoinField}] = {tableNameAliasParent}.[{innerJoinField}])";

                                    tableAlias = (tableNameAliasParent > tableAlias) ? tableNameAliasParent : tableAlias;
                                    success = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (ParentItemJoin(resultSetObject, parentItem, ref tableAlias, ref innerJoinFields, tableNameAliasOffset + 1))
                            {
                                string innerJoinFieldsTemp = "";

                                ParentItemJoin(actionItem, resultSetObject, ref tableAlias, ref innerJoinFieldsTemp, tableNameAliasOffset);

                                innerJoinFields = innerJoinFieldsTemp + innerJoinFields;
                            }
                        }
                    }

                    readerResultSet++;
                }
            }

            return success;
        }
    }
}

// TODO: SQL Bulk Copy