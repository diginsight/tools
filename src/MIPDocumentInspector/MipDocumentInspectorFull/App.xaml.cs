#region using
using Common;
using Microsoft.Identity.Client;
using Microsoft.InformationProtection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
#endregion

namespace MipDocumentInspector
{
    /// <summary>Interaction logic for App.xaml</summary>
    public partial class App : ApplicationBase, IProvideLogString
    {
        private static Type T = typeof(App);
        private static string _environment = null;

        public App()
        {

        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            LogStringExtensions.RegisterLogstringProvider(App.Current as App);
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                CultureInfo.CurrentCulture = new CultureInfo("it-IT"); // TODO: get current system culture 
                TranslationSource.Instance.CurrentCulture = CultureInfo.CurrentCulture;

                var environment = TraceManager.EnvironmentName;
                var applicationWindow = new MainWindow();
                
                string username = Environment.UserName;
                string instanceName = string.Format(GlobalConstants.ENVIRONMENTUSER_FORMAT, environment, username, applicationWindow.Name);

                var exRestore = await ABCActivator.RestoreState<Window, PositionState, PositionStateProvider>(applicationWindow, $"{instanceName}.Location", null);
                exRestore = await ABCActivator.RestoreState<MainWindow, MainWindowState, MainWindowStateProvider>(applicationWindow, $"{instanceName}.Default", null);

                this.MainWindow = applicationWindow;
                sec.Debug(new { applicationWindow = applicationWindow.GetLogString() });

                applicationWindow.Show();

                this.OnIdleProcessing += App_OnIdleProcessing;
            }
        }

        private void App_OnIdleProcessing(object sender, EventArgs e)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null) { return; }
            if (dispatcher.HasShutdownStarted || dispatcher.Thread == null || !dispatcher.Thread.IsAlive || dispatcher.Thread.ThreadState != System.Threading.ThreadState.Running) { return; }

            dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action<Exception>(async (ex) => await PersistApplicationState(ex)), null);
        }

        #region PersistApplicationState
        public async Task PersistApplicationState(Exception ex)
        {
            ApplicationBase application = this;
            if (application == null || application.IsStartupComplete == false) { return; }

            //if (application != null && application.IsPersistentStateDirty)
            //{
            var applicationWindow = application.MainWindow;
            var environment = TraceManager.EnvironmentName;
            string username = Environment.UserName;
            string instanceName = string.Format(GlobalConstants.ENVIRONMENTUSER_FORMAT, environment, username, applicationWindow.Name);

            var stateProvider = new MainWindowStateProvider();
            await ABCActivator.Persist<MainWindow, MainWindowState>(application.MainWindow as MainWindow, $"{instanceName}.Default", null, stateProvider);

            application.IsPersistentStateDirty = false;
            //}
        }
        #endregion

        public string ToLogString(object t, HandledEventArgs arg)
        {
            switch (t)
            {
                case ApplicationInfo obj: arg.Handled = true; return global::Common.LogstringHelper.ToLogStringInternal(obj);
                case Label obj: arg.Handled = true; return global::Common.LogstringHelper.ToLogStringInternal(obj);
                case Common.FileOptions obj: arg.Handled = true; return global::Common.LogstringHelper.ToLogStringInternal(obj);
                //case ExecutedRoutedEventArgs obj: arg.Handled = true; return global::Common.LogstringHelper.ToLogStringInternal(obj);

                // MSAL
                //case Common_Authentication::Common.Identity obj: arg.Handled = true; return Common_Authentication::Common.LogstringHelper.ToLogStringInternal(obj);
                //case IPublicClientApplication obj: arg.Handled = true; return Common_Authentication::Common.LogstringHelper.ToLogStringInternal(obj);
                //case IAccount obj: arg.Handled = true; return Common_Authentication::Common.LogstringHelper.ToLogStringInternal(obj);
                //case AuthenticationResult obj: arg.Handled = true; return Common_Authentication::Common.LogstringHelper.ToLogStringInternal(obj);
                default: break;
            }
            return null;
        }
    }
}
