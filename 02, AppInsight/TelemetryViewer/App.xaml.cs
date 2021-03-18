using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TelemetryViewer
{
    /// <summary>Interaction logic for App.xaml</summary>
    public partial class App : ApplicationBase
    {
        const string CONFIGVALUE_AIAPPLICATIONID = "AIApplicationID"; const string DEFAULTVALUE_AIAPPLICATIONID = "";
        const string CONFIGVALUE_AIAPIKEY = "AIApiKey"; const string DEFAULTVALUE_AIAPIKEY = "";

        private static Type T = typeof(App);
        Reference<bool> _locPersistState = new Reference<bool>(true);

        #region AIApplicationID
        public string AIApplicationID
        {
            get { return GetValue(() => AIApplicationID); }
            set { SetValue(() => AIApplicationID, value); AIApplicationIDChanged(); }
        }
        void AIApplicationIDChanged()
        {
        }
        #endregion
        #region AIApiKey
        public string AIApiKey
        {
            get { return GetValue(() => AIApiKey); }
            set { SetValue(() => AIApiKey, value); AIApiKeyChanged(); }
        }
        void AIApiKeyChanged()
        {
        }
        #endregion

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                CultureInfo.CurrentCulture = new CultureInfo("it-IT"); // TODO: get current system culture 
                TranslationSource.Instance.CurrentCulture = CultureInfo.CurrentCulture;
                ResourcesHelper.LoadApplicationResources(null);

                this.AIApplicationID = ConfigurationHelper.GetClassSetting<MainControl, string>(CONFIGVALUE_AIAPPLICATIONID, DEFAULTVALUE_AIAPPLICATIONID); // , CultureInfo.InvariantCulture
                this.AIApiKey = ConfigurationHelper.GetClassSetting<MainControl, string>(CONFIGVALUE_AIAPIKEY, DEFAULTVALUE_AIAPIKEY); // , CultureInfo.InvariantCulture

                var mainControl = new MainControl();
                var applicationWindow = new ApplicationWindow(mainControl)
                {
                    Width = 800,
                    Height = 450,
                    //ResizeMode = ResizeMode.CanResize,
                    //WindowState = WindowState.Normal,
                    //WindowStyle = WindowStyle.None,
                    DragMode = WindowBase.WindowDragMode.Full,
                    WindowChrome = new System.Windows.Shell.WindowChrome() { CaptionHeight = 0, ResizeBorderThickness = new Thickness(5) }
                };
                applicationWindow.Name = mainControl.Name;

                var username = Environment.UserName;
                var environment = TraceManager.EnvironmentName;
                var instanceName = string.Format(GlobalConstants.ENVIRONMENTUSER_FORMAT, environment, username, applicationWindow.Name);
                var exRestore = await ABCActivator.RestoreState<Window, PositionState, PositionStateProvider>(applicationWindow, $"{instanceName}.Location", null);

                instanceName = string.Format(GlobalConstants.ENVIRONMENTUSER_FORMAT, environment, username, this.MainWindow.Name);
                exRestore = await ABCActivator.RestoreState<App, ApplicationState, ApplicationStateProvider>(this, $"{instanceName}.AppState", null);

                this.MainWindow = applicationWindow;
                sec.Debug(new { applicationWindow = applicationWindow.GetLogString() });

                applicationWindow.Show(); sec.Debug($"applicationWindow.Show(); completed");

                _locPersistState.Value = false;
            }
        }

        #region PersistControlState
        private async Task PersistControlState()
        {
            if (_locPersistState == null || _locPersistState.Value) { return; }
            using (var sec = this.GetCodeSection())
            {
                var environment = TraceManager.EnvironmentName;
                string username = Environment.UserName;
                string instanceName = string.Format(GlobalConstants.ENVIRONMENTUSER_FORMAT, environment, username, this.MainWindow.Name);

                var provider = new ApplicationStateProvider();
                await ABCActivator.Persist<App, ApplicationState>(this, instanceName + ".AppState", null, provider);
            }
        }
        #endregion
    }
}
