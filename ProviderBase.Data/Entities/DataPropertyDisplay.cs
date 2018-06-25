using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderBase.Data.Entities
{
    public enum DataPropertyDisplayType
    {
        Description
    };

    public class DataPropertyDisplay : Attribute
    {
        public DataPropertyDisplayType Type { get; set; }

        public DataPropertyDisplay(DataPropertyDisplayType type)
        {
            this.Type = type;
        }
    }
}
