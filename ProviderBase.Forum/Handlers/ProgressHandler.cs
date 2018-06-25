using System;
using ProviderBase.Framework.Handlers;
using ProviderBase.Framework.Forum.Entities;
using System.Collections.Generic;
using System.Linq;
using ProviderBase.Data.Providers;

namespace ProviderBase.Framework.Forum.Handlers
{
    public class ProgressHandler : BaseHandler
    {
        public void GetProgressSummary()
        {
            AjaxResult.Status = AjaxResultStatus.Success;
        }
    }
}
