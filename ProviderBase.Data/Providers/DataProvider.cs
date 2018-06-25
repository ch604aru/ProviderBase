using Dapper;
using ProviderBase.Data.Entities;
using System;
using System.Collections.Generic;

namespace ProviderBase.Data.Providers
{
    public class DataProvider : DataProviderBase
    {
        // Select Link
        public static List<T> SelectLink<T, T2>(T selectItem, T2 joinItem, string linkTableName, string connectionStringName = "") where T : new() where T2 : new()
        {
            return ExecuteDataProviderActionSelectLink<T, T2>(connectionStringName, selectItem, joinItem, linkTableName);
        }

        public static List<object> SelectLink(object selectItem, object joinItem, string linkTableName, string connectionStringName = "")
        {
            return ExecuteDataProviderActionSelectLink(connectionStringName, selectItem, joinItem, linkTableName);
        }

        // Select
        public static T SelectSingleUsingDefault<T>(T selectObject, string connectionStringName = "", params string[] whereParameterIgnoreDefault) where T : new()
        {
            return ExecuteDataProviderActionSelectSingleUsingDefault<T>(connectionStringName, selectObject, whereParameterIgnoreDefault);
        }

        public static object SelectSingleUsingDefault(object selectObject, string connectionStringName = "", params string[] whereParameterIgnoreDefault)
        {
            return ExecuteDataProviderActionSelectSingleUsingDefault(connectionStringName, selectObject, whereParameterIgnoreDefault);
        }

        public static T SelectSingleUsingDefaultFull<T>(T selectObject, string connectionStringName = "", params string[] whereParameterIgnoreDefault) where T : new()
        {
            return ExecuteDataProviderActionSelectSingleUsingDefaultFull<T>(connectionStringName, selectObject, whereParameterIgnoreDefault);
        }

        public static object SelectSingleUsingDefaultFull(object selectObject, string connectionStringName = "", params string[] whereParameterIgnoreDefault)
        {
            return ExecuteDataProviderActionSelectSingleUsingDefaultFull(connectionStringName, selectObject, whereParameterIgnoreDefault);
        }

        public static T SelectSingleOrDefault<T>(T selectObject, string connectionStringName = "", params string[] whereParameterIncludeDefault) where T : new()
        {
            return ExecuteDataProviderActionSelectSingleOrDefault<T>(connectionStringName, selectObject, whereParameterIncludeDefault);
        }

        public static object SelectSingleOrDefault(object selectObject, string connectionStringName = "", params string[] whereParameterIncludeDefault)
        {
            return ExecuteDataProviderActionSelectSingleOrDefault(connectionStringName, selectObject, whereParameterIncludeDefault);
        }

        public static T SelectSingleOrDefaultFull<T>(T selectObject, string connectionStringName = "", params string[] whereParameterIncludeDefault) where T : new()
        {
            return ExecuteDataProviderActionSelectSingleOrDefaultFull<T>(connectionStringName, selectObject, whereParameterIncludeDefault);
        }

        public static object SelectSingleOrDefaultFull(object selectObject, string connectionStringName = "", params string[] whereParameterIncludeDefault)
        {
            return ExecuteDataProviderActionSelectSingleOrDefaultFull(connectionStringName, selectObject, whereParameterIncludeDefault);
        }

        public static T SelectSingle<T>(T selectObject, string connectionStringName = "") where T : new()
        {
            return ExecuteDataProviderActionSelectSingle<T>(connectionStringName, selectObject);
        }

        public static object SelectSingle(object selectObject, string connectionStringName = "")
        {
            return ExecuteDataProviderActionSelectSingle(connectionStringName, selectObject);
        }

        public static T SelectSingleFull<T>(T selectObject, string connectionStringName = "", DataProviderResultFilter paging = null) where T : new()
        {
            return ExecuteDataProviderActionSelectSingleFull<T>(connectionStringName, selectObject, paging);
        }

        public static object SelectSingleFull(object selectObject, string connectionStringName = "", DataProviderResultFilter paging = null)
        {
            return ExecuteDataProviderActionSelectSingleFull(connectionStringName, selectObject, paging);
        }

        public static List<T> SelectUsingDefault<T>(T selectObject, string connectionStringName = "", params string[] whereParameterIgnoreDefault) where T : new()
        {
            return ExecuteDataProviderActionSelectUsingDefault<T>(connectionStringName, selectObject, whereParameterIgnoreDefault);
        }

        public static List<object> SelectUsingDefault(object selectObject, string connectionStringName = "", params string[] whereParameterIgnoreDefault)
        {
            return ExecuteDataProviderActionSelectUsingDefault(connectionStringName, selectObject, whereParameterIgnoreDefault);
        }

        public static List<T> SelectOrDefault<T>(T selectObject, string connectionStringName = "", params string[] whereParameterIncludeDefault) where T : new()
        {
            return SelectOrDefault<T>(selectObject, connectionStringName, null, whereParameterIncludeDefault);
        }

        public static List<T> SelectOrDefault<T>(T selectObject, string connectionStringName = "", DataProviderResultFilter paging = null, params string[] whereParameterIncludeDefault) where T : new()
        {
            return ExecuteDataProviderActionSelectOrDefault<T>(connectionStringName, selectObject, whereParameterIncludeDefault, paging);
        }

        public static List<object> SelectOrDefault(object selectObject, string connectionStringName = "", params string[] whereParameterIncludeDefault)
        {
            return SelectOrDefault(selectObject, connectionStringName, null, whereParameterIncludeDefault);
        }

        public static List<object> SelectOrDefault(object selectObject, string connectionStringName = "", DataProviderResultFilter paging = null, params string[] whereParameterIncludeDefault)
        {
            return ExecuteDataProviderActionSelectOrDefault(connectionStringName, selectObject, whereParameterIncludeDefault, paging);
        }

        public static List<T> Select<T>(T selectObject, string connectionStringName = "", DataProviderResultFilter paging = null) where T : new()
        {
            return ExecuteDataProviderActionSelect<T>(connectionStringName, selectObject, paging);
        }

        public static List<object> Select(object selectObject, string connectionStringName = "", DataProviderResultFilter paging = null)
        {
            return ExecuteDataProviderActionSelect(connectionStringName, selectObject, paging);
        }

        public static List<T> SelectAll<T>(T selectObject, string connectionStringName = "", DataProviderResultFilter paging = null) where T : new()
        {
            return ExecuteDataProviderActionSelectAll<T>(connectionStringName, selectObject, paging);
        }

        public static List<object> SeletAll(object selectObject, string connectionStringName = "")
        {
            return ExecuteDataProviderActionSelectAll(connectionStringName, selectObject);
        }

        public static int SelectCount<T1>(T1 selectObject, string connectionStringName = "") where T1 : new()
        {
            return ExecuteDataProviderActionSelectCount<T1>(connectionStringName, selectObject, null, null, true);
        }

        public static int SelectCount<T1, T2>(T1 selectObject, string connectionStringName = "") where T1 : new() where T2 : new()
        {
            return ExecuteDataProviderActionSelectCount<T1, T2>(connectionStringName, selectObject, null, null, true);
        }

        public static T2 SelectNewest<T1, T2>(T1 selectObject, string connectionStringName = "") where T1 : new() where T2 : new()
        {
            return ExecuteDataProviderActionSelectNewest<T1, T2>(connectionStringName, selectObject, null, null, true);
        }

        // Insert
        public static int Insert<T>(T insertObject, string connectionStringName = "") where T : new()
        {
            return ExecuteDataProviderActionInsert(connectionStringName, insertObject);
        }

        public static int Insert(object insertObject, string connectionStringName = "")
        {
            return ExecuteDataProviderActionInsert(connectionStringName, insertObject);
        }

        public static int InsertOverrideTableField<T>(T insertObject, string connectionStringName = "", string tableName = "", Dictionary<string, string> fieldConvertList = null) where T : new()
        {
            return ExecuteDataProviderActionInsert(connectionStringName, insertObject, tableName, fieldConvertList);
        }

        public static int InsertOverrideTableField(object insertObject, string connectionStringName = "", string tableName = "", Dictionary<string, string> fieldConvertList = null)
        {
            return ExecuteDataProviderActionInsert(connectionStringName, insertObject, tableName, fieldConvertList);
        }

        // Update
        public static void Update<T>(T updateObject, string connectionStringName = "") where T : new()
        {
            ExecuteDataProviderActionUpdate(connectionStringName, updateObject);
        }

        public static void Update(object updateObject, string connectionStringName = "")
        {
            ExecuteDataProviderActionUpdate(connectionStringName, updateObject);
        }

        // Delete
        public static void Delete<T>(T deleteObject, string connectionStringName = "") where T : new()
        {
            ExecuteDataProviderActionDelete(connectionStringName, deleteObject);
        }

        public static void Delete(object deleteObject, string connectionStringName = "")
        {
            ExecuteDataProviderActionDelete(connectionStringName, deleteObject);
        }
    }
}
