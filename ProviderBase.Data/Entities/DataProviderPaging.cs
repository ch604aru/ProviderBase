using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderBase.Data.Entities
{
    public abstract class DataProviderPaging
    {
        [DataProviderResultField("PageCurrent")]
        public int PageCurrent { get; set; }

        [DataProviderResultField("PageSize")]
        public int PageSize { get; set; }

        public int PageTotal
        { get
            {
                return (int)Math.Ceiling((double)PageTotalCount / (double)PageSize);
            }
        }

        [DataProviderResultField("PageTotalCount")]
        public int PageTotalCount { get; set; }

        public DataProviderPaging() : base()
        {
            this.PageCurrent = 0;
            this.PageSize = 0;
            this.PageTotalCount = 0;
        }
    }
}
