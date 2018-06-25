using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProviderBase.Data.Entities
{
    public class DataProviderResultFilter
    {
        private List<DataProviderResultFilterItem> FilterList { get; set; }

        public void SetPaging(Type actionType, int pageCurrent, int pageSize)
        {
            this.SetFilterItem(actionType, actionType, pageCurrent, pageSize, "", "", "", OrderFieldDirection.Unassigned);
        }

        public void SetPaging(Type actionType, Type pagingType, int pageCurrent, int pageSize)
        {
            this.SetFilterItem(actionType, pagingType, pageCurrent, pageSize, "", "", "", OrderFieldDirection.Unassigned);
        }

        public void SetFilter(Type actionType, string filterField, string filterFieldValue)
        {
            this.SetFilterItem(actionType, actionType, 0, 0, filterField, filterFieldValue, "", OrderFieldDirection.Unassigned);
        }

        public void SetFilter(Type actionType, Type pagingType, string filterField, string filterFieldValue)
        {
            this.SetFilterItem(actionType, pagingType, 0, 0, filterField, filterFieldValue, "", OrderFieldDirection.Unassigned);
        }

        public void SetOrder(Type actionType, string orderField, OrderFieldDirection orderFieldDirection)
        {
            this.SetFilterItem(actionType, actionType, 0, 0, "", "", orderField, orderFieldDirection);
        }

        public void SetOrder(Type actionType, Type pagingType, string orderField, OrderFieldDirection orderFieldDirection)
        {
            this.SetFilterItem(actionType, pagingType, 0, 0, "", "", orderField, orderFieldDirection);
        }

        public void SetFilterItem(Type actionType, Type pagingType, int pageCurrent, int pageSize, string filterField, string filterFieldValue, string orderField, OrderFieldDirection orderFieldDirection)
        {
            DataProviderResultFilterItem dataProviderPagingItem = null;

            dataProviderPagingItem = this.GetFilterItem(pagingType);

            if (dataProviderPagingItem == null)
            {
                PropertyInfo resultSet = null;

                dataProviderPagingItem = new DataProviderResultFilterItem();

                resultSet = actionType.GetProperties()
                                .Where(p => DataProviderResultSet.IsDefined(p, typeof(DataProviderResultSet)))
                                .Where(p => p.PropertyType == pagingType).FirstOrDefault();

                if (resultSet != null)
                {
                    dataProviderPagingItem.DataProviderResultSetID = resultSet.GetCustomAttribute<DataProviderResultSet>().ResultSetID;
                }

                dataProviderPagingItem.DataProviderResultType = pagingType;
            }

            dataProviderPagingItem.PageCurrent = (pageCurrent > 0) ? pageCurrent : dataProviderPagingItem.PageCurrent;
            dataProviderPagingItem.PageSize = (pageSize > 0) ? pageSize : dataProviderPagingItem.PageSize;
            dataProviderPagingItem.FilterField = (string.IsNullOrEmpty(filterField) == false) ? filterField : dataProviderPagingItem.FilterField;
            dataProviderPagingItem.FilterFieldValue = (string.IsNullOrEmpty(filterFieldValue) == false) ? filterFieldValue : dataProviderPagingItem.FilterFieldValue;
            dataProviderPagingItem.OrderField = (string.IsNullOrEmpty(orderField) == false) ? orderField : dataProviderPagingItem.OrderField;
            dataProviderPagingItem.OrderFieldDirection = orderFieldDirection;

            if (this.FilterList.Count > 0)
            {
                this.FilterList[0] = dataProviderPagingItem;
            }
            else
            {
                this.FilterList.Add(dataProviderPagingItem);
            }
        }

        public DataProviderResultFilterItem GetFilterItem(int resultSetID)
        {
            return this.GetFilterItem(resultSetID, null);   
        }

        public DataProviderResultFilterItem GetFilterItem(Type pagingType)
        {
            return this.GetFilterItem(0, pagingType);
        }

        private DataProviderResultFilterItem GetFilterItem(int resultSetID, Type pagingType)
        {
            DataProviderResultFilterItem dataProviderFilterItem = null;

            if (this.FilterList?.Count > 0)
            {
                if (resultSetID > 0)
                {
                    dataProviderFilterItem = this.FilterList.Where(x => x.DataProviderResultSetID == resultSetID).SingleOrDefault();
                }
                else if (pagingType != null)
                {
                    dataProviderFilterItem = this.FilterList.Where(x => x.DataProviderResultType == pagingType).SingleOrDefault();
                }
            }

            return dataProviderFilterItem;
        }

        public DataProviderResultFilter()
        {
            this.FilterList = new List<DataProviderResultFilterItem>();
        }
    }
}
