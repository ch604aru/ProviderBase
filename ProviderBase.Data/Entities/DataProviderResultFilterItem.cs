using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderBase.Data.Entities
{
    public enum OrderFieldDirection
    {
        Unassigned = 0,
        Ascending = 1,
        Desending = 2
    }

    public class DataProviderResultFilterItem
    {
        public int DataProviderResultSetID { get; set; }

        public Type DataProviderResultType { get; set; }

        public int PageCurrent { get; set; }

        public int PageSize { get; set; }

        public int PageTotal { get; set; }

        public int PageTotalCount { get; set; }

        public string FilterField { get; set; }

        public string FilterFieldValue { get; set; }

        public string OrderField { get; set; }

        public OrderFieldDirection OrderFieldDirection { get; set; }

        public string OrderFieldDirectionString
        {
            get
            {
                switch (this.OrderFieldDirection)
                {
                    case OrderFieldDirection.Ascending:
                        return "ASC";

                    case OrderFieldDirection.Desending:
                        return "DESC";

                    case OrderFieldDirection.Unassigned:
                    default:
                        return "";
                }
            }
        }

        public DataProviderResultFilterItem()
        {
            this.DataProviderResultSetID = 0;
            this.DataProviderResultType = null;
            this.PageCurrent = 0;
            this.PageSize = 0;
            this.PageTotal = 0;
            this.PageTotalCount = 0;
            this.FilterField = "";
            this.FilterFieldValue = "";
            this.OrderField = "";
            this.OrderFieldDirection = OrderFieldDirection.Unassigned;
        }
    }
}
