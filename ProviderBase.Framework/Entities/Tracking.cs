using ProviderBase.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using KeyType = ProviderBase.Data.Entities.DataProviderKeyType;
using FieldAction = ProviderBase.Data.Entities.DataProviderResultFieldAction;

namespace ProviderBase.Framework.Entities
{
    [DataProviderTable("TrackingLevel_T")]
    public enum TrackingLevel
    {
        Error = -1,
        Unassigned = 0,
        Success = 1
    }

    [DataProviderTable("TrackingType_T")]
    public enum TrackingType
    {
        Unassigned = 0,
        System = 1,
        User = 2
    }

    [DataProviderTable("Tracking_T")]
    public class Tracking
    {
        [DataProviderResultField("TrackingID", KeyType.PrimaryKey, FieldAction.Select, FieldAction.Where)]
        public int TrackingID { get; set; }

        [DataProviderResultField("Message", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string Message { get; set; }

        [DataProviderResultField("Status", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int Status { get; set; }

        [DataProviderResultField("UserID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public int UserID { get; set; }

        [DataProviderResultField("MethodName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string MethodName { get; set; }

        [DataProviderResultField("AssemblyName", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string AssemblyName { get; set; }

        [DataProviderResultField("AssemblyType", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string AssemblyType { get; set; }

        [DataProviderResultField("RequestData", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string RequestData { get; set; }

        [DataProviderResultField("ReturnData", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string ReturnData { get; set; }

        [DataProviderResultField("TrackingLevelID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public TrackingLevel TrackingLevelID { get; set; }

        [DataProviderResultField("TrackingTypeID", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public TrackingType TrackingTypeID { get; set; }

        [DataProviderResultField("StackTrace", FieldAction.Select, FieldAction.Insert, FieldAction.Update)]
        public string StackTrace { get; set; }

        public Tracking()
        {
            this.TrackingID = 0;
            this.Message = "";
            this.Status = 0;
            this.UserID = 0;
            this.MethodName = "";
            this.AssemblyName = "";
            this.AssemblyType = "";
            this.RequestData = "";
            this.ReturnData = "";
            this.TrackingLevelID = TrackingLevel.Unassigned;
            this.TrackingTypeID = TrackingType.Unassigned;
            this.StackTrace = "";
        }

        public bool TrackingInsertLevel()
        {
            TrackingLevel trackingLevelConfig = ProviderBase.Data.Utility.GetAppSetting("TrackingLevelID", TrackingLevel.Error);
            return (this.TrackingLevelID <= trackingLevelConfig);
        }
    }
}
