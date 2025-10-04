#region using
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Metrics = System.Collections.Generic.Dictionary<string, object>; // $$$
#endregion

namespace MIPDocumentInspector
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            using (var sec = this.GetCodeSection())
            { 
                InitializeComponent();
                this.DocumentPath = @"E:\temp\SampleDocumentNoLabel.enc13.docx";
            }
        }

        public string DocumentPath
        {
            get { return (string)GetValue(DocumentPathProperty); }
            set { SetValue(DocumentPathProperty, value); }
        }
        public static readonly DependencyProperty DocumentPathProperty = DependencyProperty.Register("DocumentPath", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
            }
        }
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                try
                {




                }
                catch (Exception ex)
                {
                    sec.Exception(ex);
                }


                // report button 
                // var recorder = Trace.Listeners.OfType<TraceListener>().FirstOrDefault(l => l is RecorderTraceListener) as RecorderTraceListener;
                // var entries = recorder.GetItems();
            }
        }

        public void SampleMethod()
        {
            using (var sec = this.GetCodeSection())
            {
                Thread.Sleep(100);
                SampleMethodNested();
                SampleMethodNested1();

            }
        }
        public void SampleMethodNested()
        {
            Thread.Sleep(100);
        }
        public void SampleMethodNested1()
        {
            Thread.Sleep(10);
        }

    }
}
