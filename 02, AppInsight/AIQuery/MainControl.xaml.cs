#region using
using Common;
using Common.ComponentBase;
using Microsoft.Azure.ApplicationInsights.Query;
using Microsoft.Azure.OperationalInsights;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
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
#endregion

namespace ApplicationInsightQuery
{
    /// <summary>Interaction logic for MainControl.xaml</summary>
    public partial class MainControl : UserControl
    {
        public static Type T = typeof(MainControl);
        const string APPINSIGHTAPI = "https://api.applicationinsights.io/{version}/apps/{appID}/{operation}/{path}"; // ?{parameters}
        const string CONFIGVALUE_CLIENTID = "ClientId"; const string DEFAULTVALUE_CLIENTID = "";
        const string CONFIGVALUE_APPNAME = "AppName"; const string DEFAULTVALUE_APPNAME = "";
        const string CONFIGVALUE_APPVERSION = "AppVersion"; const string DEFAULTVALUE_APPVERSION = "";
        AuthenticationHelper _authenticationHelper;
        Reference<bool> _locPersistState = new Reference<bool>(true);

        public MainControl()
        {
            InitializeComponent();

            this.Operation = "events"; // "metrics";
            this.Path = "$all?$top=10"; // "requests/count";
            this.Parameters = "";
        }
        private async void mainControl_Initialized(object sender, EventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                this.ClientID = await ConfigurationHelper.GetClassSettingAsync<MainControl, string>(CONFIGVALUE_CLIENTID, SettingAccessType.Secret, DEFAULTVALUE_CLIENTID); // , CultureInfo.InvariantCulture
                this.AppName = ConfigurationHelper.GetClassSetting<MainControl, string>(CONFIGVALUE_APPNAME, DEFAULTVALUE_APPNAME); // , CultureInfo.InvariantCulture SettingAccessType.SecretWithCredential, 
                this.AppVersion = ConfigurationHelper.GetClassSetting<MainControl, string>(CONFIGVALUE_APPVERSION, DEFAULTVALUE_APPVERSION); // , CultureInfo.InvariantCulture
                sec.Debug(new { this.ClientID, this.AppName, this.AppVersion });

                var environment = TraceManager.EnvironmentName;
                var panelInfo = new SettingsPanelInfo<SettingsAppInsightKeyControl>() { Name = "AppInsight", Description = "AppInsight", InternalName = "", Position = 0, Type = null };
                Commands.RegisterPanel.Execute(panelInfo, settingsControl);


                sec.Debug(new { this.ClientID });
                // _authenticationHelper = new AuthenticationHelper(this.ClientID, Application.Current.MainWindow); sec.Debug(new { _authenticationHelper = _authenticationHelper.GetLogString() });
                // var task = _authenticationHelper.GetUserIdentityAsync((identity) =>
                // {
                //     using (var sec1 = this.GetNamedSection("GetUserIdentityAsyncCallback"))
                //     {
                //         this.Identity = identity;

                //     }
                // });

                this.GlobalPropertiesBinding = (App.Current as ApplicationBase).SetPRopertyBinding(selectRibbonButton_ConvertEvent, default(object[]), "AIApplicationID", "AIApiKey");
                _locPersistState.Value = false;
            }
        }
        private void mainControl_Unloaded(object sender, RoutedEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                var binding = this.GlobalPropertiesBinding as IDisposable;
                this.GlobalPropertiesBinding = null;

                if (binding != null) { binding.Dispose(); }
            }
        }

        #region GlobalPropertiesBinding
        public object GlobalPropertiesBinding
        {
            get { return (object)GetValue(SelectRibbonButtonBindingProperty); }
            set { SetValue(SelectRibbonButtonBindingProperty, value); }
        }
        public static readonly DependencyProperty SelectRibbonButtonBindingProperty = DependencyProperty.Register("SelectRibbonButtonBinding", typeof(object), typeof(MainControl), new PropertyMetadata(null, SelectRibbonButtonBindingChanged));
        public static void SelectRibbonButtonBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        #endregion
        #region TenantID
        public string TenantID
        {
            get { return (string)GetValue(TenantIDProperty); }
            set { SetValue(TenantIDProperty, value); }
        }
        public static readonly DependencyProperty TenantIDProperty = DependencyProperty.Register("TenantID", typeof(string), typeof(MainControl), new PropertyMetadata(null, TenantIDChanged));
        public static void TenantIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        #endregion
        #region ClientID
        public string ClientID
        {
            get { return (string)GetValue(ClientIDProperty); }
            set { SetValue(ClientIDProperty, value); }
        }
        public static readonly DependencyProperty ClientIDProperty = DependencyProperty.Register("ClientID", typeof(string), typeof(MainControl), new PropertyMetadata(null, ClientIDChanged));
        public static void ClientIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        #endregion
        #region ClientSecret
        public string ClientSecret
        {
            get { return (string)GetValue(ClientSecretProperty); }
            set { SetValue(ClientSecretProperty, value); }
        }
        public static readonly DependencyProperty ClientSecretProperty = DependencyProperty.Register("ClientSecret", typeof(string), typeof(MainControl), new PropertyMetadata(null, ClientSecretChanged));
        public static void ClientSecretChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        #endregion
        #region AppName
        public string AppName
        {
            get { return (string)GetValue(AppNameProperty); }
            set { SetValue(AppNameProperty, value); }
        }
        public static readonly DependencyProperty AppNameProperty = DependencyProperty.Register("AppName", typeof(string), typeof(MainControl), new PropertyMetadata(null, AppNameChanged));
        public static void AppNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        #endregion
        #region AppVersion
        public string AppVersion
        {
            get { return (string)GetValue(AppVersionProperty); }
            set { SetValue(AppVersionProperty, value); }
        }
        public static readonly DependencyProperty AppVersionProperty = DependencyProperty.Register("AppVersion", typeof(string), typeof(MainControl), new PropertyMetadata(null, AppVersionChanged));
        public static void AppVersionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        #endregion
        #region AIApplicationID
        public string AIApplicationID
        {
            get { return (string)GetValue(AIApplicationIDProperty); }
            set { SetValue(AIApplicationIDProperty, value); }
        }
        public static readonly DependencyProperty AIApplicationIDProperty = DependencyProperty.Register("AIApplicationID", typeof(string), typeof(MainControl), new PropertyMetadata(null, AIApplicationIDChanged));
        public static async void AIApplicationIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            using (var sec = TraceManager.GetCodeSection(T))
            {
                var pthis = d as MainControl;
                if (string.IsNullOrEmpty(e.NewValue as string)) { return; }
                await pthis.PersistControlState();
            }
        }
        #endregion
        #region AIApiKey
        public string AIApiKey
        {
            get { return (string)GetValue(AIApiKeyProperty); }
            set { SetValue(AIApiKeyProperty, value); }
        }
        public static readonly DependencyProperty AIApiKeyProperty = DependencyProperty.Register("AIApiKey", typeof(string), typeof(MainControl), new PropertyMetadata(null, AIApiKeyChanged));
        public static async void AIApiKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            using (var sec = TraceManager.GetCodeSection(T))
            {
                var pthis = d as MainControl;
                if (string.IsNullOrEmpty(e.NewValue as string)) { return; }
                await pthis.PersistControlState();
            }
        }
        #endregion
        #region Identity
        public Identity Identity
        {
            get { return (Identity)GetValue(IdentityProperty); }
            set { SetValue(IdentityProperty, value); }
        }
        public static readonly DependencyProperty IdentityProperty = DependencyProperty.Register("Identity", typeof(Identity), typeof(MainControl), new PropertyMetadata());
        #endregion
        #region Exception
        public Exception Exception
        {
            get { return (Exception)GetValue(ExceptionProperty); }
            set { SetValue(ExceptionProperty, value); }
        }
        public static readonly DependencyProperty ExceptionProperty = DependencyProperty.Register("Exception", typeof(Exception), typeof(MainControl), new PropertyMetadata());
        #endregion

        #region ShowSettingsPanel
        public bool ShowSettingsPanel
        {
            get { return (bool)GetValue(ShowSettingsPanelProperty); }
            set { SetValue(ShowSettingsPanelProperty, value); }
        }
        public static readonly DependencyProperty ShowSettingsPanelProperty = DependencyProperty.Register("ShowSettingsPanel", typeof(bool), typeof(MainControl), new PropertyMetadata(false));
        #endregion

        #region Operation
        public string Operation
        {
            get { return (string)GetValue(OperationProperty); }
            set { SetValue(OperationProperty, value); }
        }
        public static readonly DependencyProperty OperationProperty = DependencyProperty.Register("Operation", typeof(string), typeof(MainControl), new PropertyMetadata(null, OperationChanged));
        public static void OperationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        #endregion
        #region Path
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(MainControl), new PropertyMetadata(null, PathChanged));
        public static void PathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        #endregion
        #region Parameters
        public string Parameters
        {
            get { return (string)GetValue(ParametersProperty); }
            set { SetValue(ParametersProperty, value); }
        }
        public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(string), typeof(MainControl), new PropertyMetadata(null, ParametersChanged));
        public static void ParametersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        #endregion

        #region Output
        public string Output
        {
            get { return (string)GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(string), typeof(MainControl), new PropertyMetadata(null, OutputChanged));
        public static void OutputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        #endregion

        private void LoginToggleCanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void LoginToggleExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                if (Identity == null) { Commands.Login.Execute(null, this); }
                else { Commands.Logout.Execute(null, this); }
            }
        }
        private void LoginCanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private async void LoginExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                if (_authenticationHelper == null) { return; }
                await _authenticationHelper.GetUserIdentityAsync((identity) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Identity = identity;
                        if (this.Identity == null)
                        {
                            var message = this.GetResourceValue<string>("Info.PressLogin", "Press Login to enter your credentials");
                            this.Exception = new ClientException(message) { Code = ExceptionCodes.PRESSLOGIN };
                            return;
                        }

                        CommandManager.InvalidateRequerySuggested();
                    });
                }); //, PromptBehavior.Always
            }
        }
        private void LogoutCanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void LogoutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                this.Identity = null;
                _authenticationHelper.Logout(); sec.Debug($"_authenticationHelper.Logout(); completed");

                var message = this.GetResourceValue<string>("Info.PressLogin", "Press Login to enter your credentials");
                this.Exception = new ClientException(message) { Code = ExceptionCodes.PRESSLOGIN };
            }
        }
        #region QueryRestCanExecute
        private void QueryRestCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //Application.Current.Windows.Count
            e.CanExecute = true;
            e.Handled = true;
            return;
        }
        #endregion
        #region QueryRestCommand
        private async void QueryRestCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (CodeSection sec = this.GetCodeSection())
            {
                var accessToken = "";
                var client = HttpManager.GetHttpClient("application/json", accessToken);

                sec.Debug(new { this.Operation });
                sec.Debug(new { this.Path });
                sec.Debug(new { this.Parameters });
                sec.Debug(new { this.AIApplicationID, this.AIApiKey });

                var url = APPINSIGHTAPI.Replace("{version}", "v1").Replace("{appID}", this.AIApplicationID).Replace("{operation}", this.Operation).Replace("{path}", this.Path).Replace("{parameters}", this.Parameters); sec.Debug(new { url });
                client.DefaultRequestHeaders.Add("X-Api-Key", this.AIApiKey);

                var response = await client.InvokeAsync(client.GetAsync, url); sec.Debug($"await client.InvokeAsync(client.GetAsync, {url}); returned {response.GetLogString()}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions { IgnoreReadOnlyProperties = true, IgnoreNullValues = true, ReadCommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true, WriteIndented = true };
                    var aiQueryResultString = default(string);
                    switch (this.Operation)
                    {
                        case "events":
                            {
                                var aiQueryResult = JsonSerializer.Deserialize<AIEventsResult>(stringContent, options);
                                aiQueryResultString = JsonSerializer.Serialize(aiQueryResult, options);
                            }; break;
                        case "metrics":
                            {
                                var aiQueryResult = JsonSerializer.Deserialize<AIMetricsResult>(stringContent, options);
                                aiQueryResultString = JsonSerializer.Serialize(aiQueryResult, options);
                            }; break;
                    }

                    this.Output = aiQueryResultString;

                    sec.Information($"getting group profile from distribution list, security group, etc, etc", "User");
                }
            }
        }
        #endregion
        #region QuerySdkCanExecute
        private void QuerySdkCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //Application.Current.Windows.Count
            e.CanExecute = true;
            e.Handled = true;
            return;
        }
        #endregion
        #region QuerySdkCommand
        private async void QuerySdkCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (CodeSection sec = this.GetCodeSection())
            {
                // log analytics query
                var workspaceId = "9b9c0d00-b0c2-46ef-bdbc-26fe1742302b";
                var clientId = $"{this.AIApplicationID}";
                var clientSecret = $"{this.AIApiKey}";
                // AAD settings, domain == tenant
                var domain = "microsoft.onmicrosoft.com";
                var authEndpoint = "https://login.microsoftonline.com";
                var tokenAudience = "https://api.loganalytics.io/";
                var adSettings = new ActiveDirectoryServiceSettings
                {
                    AuthenticationEndpoint = new Uri(authEndpoint),
                    TokenAudience = new Uri(tokenAudience),
                    ValidateAuthority = true
                };

                var credentials = ApplicationTokenProvider.LoginSilentAsync(domain, clientId, clientSecret, adSettings).GetAwaiter().GetResult();
                // New up a client with credentials and LA workspace Id
                var client = new OperationalInsightsDataClient(credentials);
                client.WorkspaceId = workspaceId;
                var results = client.Query("union * | take 5");
            }
        }
        #endregion

        #region LanguagesCanExecute
        private void LanguagesCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //Application.Current.Windows.Count
            e.CanExecute = true;
            e.Handled = true;
            return;
        }
        #endregion
        #region LanguagesCommand
        private void LanguagesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (CodeSection sec = this.GetCodeSection())
            {
                ShowSettingsPanel = !ShowSettingsPanel;
                if (ShowSettingsPanel)
                {
                    Commands.Reset.Execute(null, settingsControl);
                    Commands.Languages.Execute(null, settingsControl);
                }
            }
        }
        #endregion
        #region ChangeCultureCanExecute
        private void ChangeCultureCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
            return;
        }
        #endregion
        #region ChangeCultureCommand
        private void ChangeCultureCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (CodeSection sec = this.GetCodeSection())
            {
                if (e.Parameter is CultureInfo ci)
                {
                    CultureInfo.CurrentCulture = ci;
                    TranslationSource.Instance.CurrentCulture = CultureInfo.CurrentCulture;
                    //CurrentCulture = CultureInfo.CurrentCulture.ThreeLetterISOLanguageName;
                    //((App)App.Current).IsPersistentStateInvalid = true;
                }
            }
        }

        #endregion
        #region SettingsCanExecute
        private void SettingsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //Application.Current.Windows.Count
            e.CanExecute = true;
            e.Handled = true;
            return;
        }
        #endregion
        #region SettingsCommand
        private void SettingsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (CodeSection sec = this.GetCodeSection())
            {
                ShowSettingsPanel = !ShowSettingsPanel;
                if (ShowSettingsPanel)
                {
                    Commands.Reset.Execute(null, settingsControl);
                }
            }
        }
        #endregion
        #region HideSettingsCanExecute
        private void HideSettingsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
            return;
        }
        #endregion
        #region HideSettingsCommand
        private void HideSettingsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (CodeSection sec = this.GetCodeSection())
            {
                ShowSettingsPanel = false;
            }
        }
        #endregion

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                // log analytics query
                // var workspaceId = "9b9c0d00-b0c2-46ef-bdbc-26fe1742302b";
                // var clientId = $"{this.AIApplicationID}";
                // var clientSecret = $"{this.AIApiKey}";
                // // AAD settings, domain == tenant
                // var domain = "microsoft.onmicrosoft.com";
                // var authEndpoint = "https://login.microsoftonline.com";
                // var tokenAudience = "https://api.loganalytics.io/";
                // var adSettings = new ActiveDirectoryServiceSettings
                // {
                //     AuthenticationEndpoint = new Uri(authEndpoint),
                //     TokenAudience = new Uri(tokenAudience),
                //     ValidateAuthority = true
                // };

                // var creds = ApplicationTokenProvider.LoginSilentAsync(domain, clientId, clientSecret, adSettings).GetAwaiter().GetResult();
                // // New up a client with credentials and LA workspace Id
                // var client = new OperationalInsightsDataClient(creds);
                // client.WorkspaceId = workspaceId;
                // var results = client.Query("union * | take 5");
            }
        }
        private void mainControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //using (var sec = this.GetCodeSection())
            //{
            if (e.Delta > 0)
            {
                if (!(Application.Current is App application)) { return; }
                application.Zoom += GlobalConstants.ZOOMDELTA;
                //TraceManager.Debug($"application.Zoom: {application.Zoom}");
            }
            else
            {
                if (!(Application.Current is App application)) { return; }
                application.Zoom -= GlobalConstants.ZOOMDELTA;
                //TraceManager.Debug($"application.Zoom: {application.Zoom}");
            }
            //}
        }
        #region onChangeIsMouseOver_ConvertEvent
        private object onChangeIsMouseOver_ConvertEvent(DependencyObject source, object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMouseOver = (bool)value;
            if (isMouseOver == false)
            {
                Commands.HideSettings.Execute(null, this);
            }
            return DependencyProperty.UnsetValue;
        }
        #endregion
        #region getTextStatusInfo_ConvertEvent
        private object getTextStatusInfo_ConvertEvent(DependencyObject source, object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var text = value as string;
            if (string.IsNullOrEmpty(text)) { return ""; }

            var len = text.Length;
            var lines = text.Count(f => f == '\r');

            return $"Lines: {lines}, Chars: {len}";
        }
        #endregion
        #region getIdentityDescription_ConvertEvent
        private object getIdentityDescription_ConvertEvent(DependencyObject source, object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var identity = value as Identity;
            if (identity == null) { return "login"; }
            if (!string.IsNullOrEmpty(identity.Name)) { return identity.Name; }
            if (!string.IsNullOrEmpty(identity.Upn)) { return identity.Upn; }

            return identity.Email;
        }
        #endregion
        #region selectRibbonButton_ConvertEvent
        private object selectRibbonButton_ConvertEvent(object source, object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var app = App.Current as App;

            if (!string.IsNullOrEmpty(app.AIApplicationID)) { this.AIApplicationID = app.AIApplicationID; }
            if (!string.IsNullOrEmpty(app.AIApiKey)) { this.AIApiKey = app.AIApiKey; }

            return null;
        } 
        #endregion

        #region PersistControlState
        private async Task PersistControlState()
        {
            if (_locPersistState == null || _locPersistState.Value) { return; }
            using (var sec = this.GetCodeSection())
            {
                var environment = TraceManager.EnvironmentName;
                string username = Environment.UserName;
                string instanceName = string.Format(GlobalConstants.ENVIRONMENTUSER_FORMAT, environment, username, mainControl.Name);

                var provider = new MainControlStateProvider();
                await ABCActivator.Persist<MainControl, MainControlState>(this, instanceName + ".Location", null, provider);
            }
        } 
        #endregion
    }
}
