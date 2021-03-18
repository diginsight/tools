#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; 
#endregion

namespace MipDocumentInspector
{
    public class LocalCommands
    {
        public static readonly RoutedUICommand SelectDefaultLabel = new RoutedUICommand("SelectDefaultLabel", "SelectDefaultLabel", typeof(LocalCommands));
        public static readonly RoutedUICommand SelectLabel = new RoutedUICommand("SelectLabel", "SelectLabel", typeof(LocalCommands));
    }
}
