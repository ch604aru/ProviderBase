using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace ProviderBase.Framework.Handlers
{
    public enum AjaxResultStatus
    {
        Exception = -2,
        Failed = -1,
        Unassigned = 0,
        Success = 1,
        Redirect = 2
    }

    public class AjaxResult : IAsyncResult
    {
        public List<object> AjaxData { get; set; }
        public List<string> Data { get; set; }
        public AjaxResultStatus Status { get; set; }
        public string Message { get; set; }
        public string Action { get; set; }
        public string Redirect { get; set; }

        private bool c_completed;
        private Object c_state;
        private AsyncCallback c_callback;
        private HttpContext c_context;


        bool IAsyncResult.IsCompleted
        {
            get
            {
                return c_completed;
            }
        }

        WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get
            {
                return null;
            }
        }

        Object IAsyncResult.AsyncState
        {
            get
            {
                return c_state;
            }

        }

        bool IAsyncResult.CompletedSynchronously
        {
            get
            {
                return false;
            }
        }

        public AjaxResult(AsyncCallback callback, HttpContext context, Object state)
        {
            c_callback = callback;
            c_context = context;
            c_state = state;
            c_completed = false;

            this.AjaxData = new List<object>();
            this.Data = new List<string>();
            this.Message = "";
            this.Status = AjaxResultStatus.Unassigned;
            this.Action = "";
            this.Redirect = "";
        }
    }
}
