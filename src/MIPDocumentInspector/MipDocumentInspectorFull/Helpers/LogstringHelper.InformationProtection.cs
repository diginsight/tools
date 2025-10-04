#region using
using Common;
using Microsoft.InformationProtection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
#endregion

namespace Common
{
    public partial class LogstringHelper
    {
        public static string ToLogStringInternal(ApplicationInfo pthis)
        {
            string logString = $"{{ApplicationInfo:{{ApplicationName:{pthis.ApplicationName},ApplicationVersion:{pthis.ApplicationVersion},ApplicationId:{pthis.ApplicationId}}}}}";
            return logString;
        }
        public static string ToLogStringInternal(Label pthis)
        {
            string logString = $"{{Label:{{Name:{pthis.Name},Description:{pthis.Description},Id:{pthis.Id},Sensitivity:{pthis.Sensitivity},Tooltip:{pthis.Tooltip},IsActive:{pthis.IsActive},ActionSource:{pthis.ActionSource.GetLogString()},AutoTooltip:{pthis.AutoTooltip},Children:{pthis.Children.GetLogString()},Color:{pthis.Color},CustomSettings:{pthis.CustomSettings.GetLogString()}}}}}";
            return logString;
        }
        public static string ToLogStringInternal(FileOptions pthis)
        {
            string logString = $"{{FileOptions:{{FileName:{pthis.FileName},IsAuditDiscoveryEnabled:{pthis.IsAuditDiscoveryEnabled},LabelId:{pthis.LabelId},OutputName:{pthis.OutputName},ActionSource:{pthis.ActionSource.GetLogString()},AssignmentMethod:{pthis.AssignmentMethod.GetLogString()},DataState:{pthis.DataState.GetLogString()}}}}}";
            return logString;
        }
    }
}
