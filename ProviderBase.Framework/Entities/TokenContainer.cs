using System.Collections.Generic;

namespace ProviderBase.Framework.Entities
{
    public class TokenContainer
    {
        public List<TokenItem> TokenItemList { get; set; }

        public TokenContainer()
        {
            this.TokenItemList = new List<TokenItem>();
        }
    }
}
