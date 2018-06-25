using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderBase.Framework.Entities
{
    public class PagingItem
    {
        public bool IncludeFirst { get; set; }

        public bool IncludeLast { get; set; }

        public bool IncludeNext { get; set; }

        public bool IncludePrevious { get; set; }

        public bool LoopPaging { get; set; }

        public int PagingUpperBand { get; set; }

        public int PagingLowerBand { get; set; }

        public string PagingItemCSSClass { get; set; }

        public string pagingItemSelectedCSSClass { get; set; }

        public PagingItem()
        {
            this.IncludeFirst = false;
            this.IncludeLast = false;
            this.IncludeNext = false;
            this.IncludePrevious = false;
            this.LoopPaging = false;
            this.PagingUpperBand = 0;
            this.PagingLowerBand = 0;
            this.PagingItemCSSClass = "";
            this.pagingItemSelectedCSSClass = "";
        }
    }
}
