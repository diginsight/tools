using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TelemetryViewer
{
    public class LocalCommands
    {
        public static readonly RoutedUICommand QueryRest = new RoutedUICommand("QueryRest", "QueryRest", typeof(LocalCommands));
        public static readonly RoutedUICommand QuerySdk = new RoutedUICommand("QuerySdk", "QuerySdk", typeof(LocalCommands));
    }
}
