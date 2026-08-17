#region using
using Common;
using Microsoft.InformationProtection;
using Microsoft.InformationProtection.File;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Identity = Common.Identity;
using Label = Microsoft.InformationProtection.Label;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
#endregion

namespace MipDocumentInspector
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : WindowBase
    {
        #region const
        const string CONFIGVALUE_DISABLEFOOTERLABELSONPDF = "DisableFooterLabelsOnPdf"; const bool DEFAULTVALUE_DISABLEFOOTERLABELSONPDF = true;
        const string CONFIGVALUE_LABELISPROTECTED = "Label.{LabelId}.isProtected"; const bool DEFAULTVALUE_LABELISPROTECTED = false;
        const string CONFIGVALUE_PROTECTEDDOCUMENTSUFFIX = "ProtectedDocumentSuffix"; const string DEFAULTVALUE_PROTECTEDDOCUMENTSUFFIX = "";
        const string CONFIGVALUE_REPLACEORIGINALFILE = "ReplaceOriginalFile"; const bool DEFAULTVALUE_REPLACEORIGINALFILE = true;
        const string CONFIGVALUE_BACKUPORIGINALFILE = "BackupOriginalFile"; const bool DEFAULTVALUE_BACKUPORIGINALFILE = false;
        const string CONFIGVALUE_CLIENTID = "ClientId"; const string DEFAULTVALUE_CLIENTID = "";
        const string CONFIGVALUE_APPNAME = "AppName"; const string DEFAULTVALUE_APPNAME = "";
        const string CONFIGVALUE_APPVERSION = "AppVersion"; const string DEFAULTVALUE_APPVERSION = "";
        #endregion

        AuthenticationHelperMIP _authenticationHelper;
        MIPHelper _mip;
        private static string clientId; // = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appName; //  = ConfigurationManager.AppSettings["app:Name"];
        private static string appVersion; //  = ConfigurationManager.AppSettings["app:Version"];

        #region .ctor
        public MainWindow()
        {
            using (var sec = this.GetCodeSection())
            {
                ConfigurationHelper.Init(TraceManager.Configuration);
                InitializeComponent();
            }
        }
        #endregion

        #region Exception
        public Exception Exception
        {
            get { return (Exception)GetValue(ExceptionProperty); }
            set { SetValue(ExceptionProperty, value); }
        }
        public static readonly DependencyProperty ExceptionProperty = DependencyProperty.Register("Exception", typeof(Exception), typeof(MainWindow), new PropertyMetadata());
        #endregion
        #region Identity
        public Identity Identity
        {
            get { return (Identity)GetValue(IdentityProperty); }
            set { SetValue(IdentityProperty, value); }
        }
        public static readonly DependencyProperty IdentityProperty = DependencyProperty.Register("Identity", typeof(Identity), typeof(MainWindow), new PropertyMetadata());
        #endregion

        #region Labels
        public List<Label> Labels
        {
            get { return (List<Label>)GetValue(LabelsProperty); }
            set { SetValue(LabelsProperty, value); }
        }
        public static readonly DependencyProperty LabelsProperty = DependencyProperty.Register("Labels", typeof(List<Label>), typeof(MainWindow), new PropertyMetadata(null));
        #endregion
        #region LeafLabels
        public List<Label> LeafLabels
        {
            get { return (List<Label>)GetValue(LeafLabelsProperty); }
            set { SetValue(LeafLabelsProperty, value); }
        }
        public static readonly DependencyProperty LeafLabelsProperty = DependencyProperty.Register("LeafLabels", typeof(List<Label>), typeof(MainWindow), new PropertyMetadata(null));
        #endregion

        #region DocumentPath
        public string DocumentPath
        {
            get { return (string)GetValue(DocumentPathProperty); }
            set { SetValue(DocumentPathProperty, value); }
        }
        public static readonly DependencyProperty DocumentPathProperty = DependencyProperty.Register("DocumentPath", typeof(string), typeof(MainWindow), new PropertyMetadata(null, DocumentPathChanged));
        public static void DocumentPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pthis = d as MainWindow;
            var application = App.Current as ApplicationBase;
            application.IsPersistentStateDirty = true;
        }
        #endregion
        #region OutputDocumentPath
        public string OutputDocumentPath
        {
            get { return (string)GetValue(OutputDocumentPathProperty); }
            set { SetValue(OutputDocumentPathProperty, value); }
        }
        public static readonly DependencyProperty OutputDocumentPathProperty = DependencyProperty.Register("OutputDocumentPath", typeof(string), typeof(MainWindow), new PropertyMetadata(null, OutputDocumentPathChanged));
        public static void OutputDocumentPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pthis = d as MainWindow;
            var application = App.Current as ApplicationBase;
            application.IsPersistentStateDirty = true;
        }
        #endregion

        #region DocumentLabel
        public Label DocumentLabel
        {
            get { return (Label)GetValue(DocumentLabelProperty); }
            set { SetValue(DocumentLabelProperty, value); }
        }
        public static readonly DependencyProperty DocumentLabelProperty = DependencyProperty.Register("DocumentLabel", typeof(Label), typeof(MainWindow), new PropertyMetadata(null));
        #endregion
        #region SelectedLabel
        public Label SelectedLabel
        {
            get { return (Label)GetValue(SelectedLabelProperty); }
            set { SetValue(SelectedLabelProperty, value); }
        }
        public static readonly DependencyProperty SelectedLabelProperty = DependencyProperty.Register("SelectedLabel", typeof(Label), typeof(MainWindow), new PropertyMetadata(null));
        #endregion
        #region Output
        public string Output
        {
            get { return (string)GetValue(OutputProperty); }
            set { SetValue(OutputProperty, value); }
        }
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(string), typeof(MainWindow), new PropertyMetadata(null, OutputChanged));
        public static void OutputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pthis = d as MainWindow;
            var application = App.Current as ApplicationBase;
            application.IsPersistentStateDirty = true;
        }
        #endregion

        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                clientId = ConfigurationHelper.GetClassSetting<App, string>(CONFIGVALUE_CLIENTID, DEFAULTVALUE_CLIENTID); // , CultureInfo.InvariantCulture
                appName = ConfigurationHelper.GetClassSetting<App, string>(CONFIGVALUE_APPNAME, DEFAULTVALUE_APPNAME); // , CultureInfo.InvariantCulture
                appVersion = ConfigurationHelper.GetClassSetting<App, string>(CONFIGVALUE_APPVERSION, DEFAULTVALUE_APPVERSION); // , CultureInfo.InvariantCulture
                sec.Debug(new { clientId, appName, appVersion });

                this.DocumentPath = @"E:\temp\SampleDocumentNoLabel.docx";
                this.OutputDocumentPath = @"E:\temp\SampleDocumentNoLabel.enc13.docx";

                var appInfo = new ApplicationInfo()
                {
                    ApplicationId = clientId,
                    ApplicationName = appName,
                    ApplicationVersion = appVersion
                };
                sec.Debug(new { appInfo = appInfo.GetLogString() });
                var mip = _mip = new MIPHelper(appInfo);

                _authenticationHelper = new AuthenticationHelperMIP(appInfo.ApplicationId, Application.Current.MainWindow); sec.Debug(new { _authenticationHelper = _authenticationHelper.GetLogString() });
                var task = _authenticationHelper.GetUserIdentityAsync((identity) =>
                {
                    using (var sec1 = this.GetNamedSection("GetUserIdentityAsyncCallback"))
                    {
                        this.Identity = identity;

                        var fileEngine = mip.CreateFileEngine(_authenticationHelper); sec.Debug($"mip.CreateFileEngine(authenticationHelper) returned {fileEngine.GetLogString()}");

                        // List all labels available to the engine created in Action
                        var labels = mip.ListLabels();
                        sec.Debug($"found {labels.Count()} labels:");
                        this.Labels = labels.ToList();

                        var leafLabels = new List<Label>();
                        labels.ToList().ForEach(lab =>
                        {
                            sec.Debug(new { label = lab });
                            if (lab.Children == null || lab.Children.Count == 0) { leafLabels.Add(lab); }
                            else
                            {
                                leafLabels.AddRange(lab.Children);
                            }
                        });

                        sec.Debug($"found {leafLabels.Count()} labels:");
                        this.LeafLabels = leafLabels.ToList();
                        this.LeafLabels.ForEach(lab =>
                        {
                            sec.Debug(new { label = lab });
                        });

                        if (File.Exists(this.DocumentPath)) { Commands.GetDescriptor.Execute(null, this); }
                    }
                });
            }
        }

        // Commands
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

                        _mip.CreateFileEngine(_authenticationHelper); sec.Debug($"_mip.CreateFileEngine(_authenticationHelper); completed");

                        //_mip.CreatePolicyEngine(_authenticationHelper); sec.Debug($"_mip.CreatePolicyEngine(); completed");

                        // List all labels available to the engine created in Action
                        IEnumerable<Label> labels = _mip.FileEngine.SensitivityLabels;
                        sec.Debug($"found {labels.Count()} labels:");
                        this.Labels = labels.ToList();

                        List<Label> leafLabels = new List<Label>();
                        labels.ToList()?.ForEach(l =>
                        {
                            sec.Debug(new { label = l });
                            if (l.Children == null || l.Children.Count == 0) { leafLabels.Add(l); }
                            else
                            {
                                leafLabels.AddRange(l.Children);
                            }
                        });
                        this.LeafLabels = leafLabels.ToList();

                        if (this.Exception is ClientException clientException && clientException.Code == ExceptionCodes.PRESSLOGIN)
                        {
                            this.Exception = null;
                        }

                        if (!string.IsNullOrEmpty(this.DocumentPath))
                        {
                            //RefreshDocumentProperties(null);
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
                this.Labels = null;
                this.LeafLabels = null;
            }
        }
        private void ToggleMenuCanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = true; }
        private void ToggleMenuExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                //this.Identity = null;
                //_authenticationHelper.Logout(); sec.Debug($"_authenticationHelper.Logout(); completed");

                //var message = this.GetResourceValue<string>("Info.PressLogin", "Press Login to enter your credentials");
                //this.Exception = new ClientException(message) { Code = ExceptionCodes.PRESSLOGIN };
                //this.Labels = null;
                //this.LeafLabels = null;
            }
        }
        private void SelectLabelCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            //if (string.IsNullOrEmpty(this.DocumentPath)) { return; }
            //if (IsProtectedView) { return; }
            if (this.Exception != null)
            {
                var exception = this.Exception as ClientException;
                if (exception == null || exception.ExceptionType.HasFlag(ExceptionType.NonRemovable)) { return; }
            }
            e.CanExecute = true;
        }
        private void SelectLabelExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                var targetLabel = e.Parameter as Label;
                if (targetLabel == null) { return; }

                if (this.SelectedLabel != null && this.SelectedLabel.Id == targetLabel.Id) { this.SelectedLabel = null; return; }

                this.SelectedLabel = targetLabel;
                sec.Debug(new { SelectedLabel = this.SelectedLabel.GetLogString() });
            }
        }
        private void GetDescriptorCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            //if (string.IsNullOrEmpty(this.DocumentPath)) { return; }
            //if (IsProtectedView) { return; }
            //if (this.Exception != null)
            //{
            //    var exception = this.Exception as ClientException;
            //    if (exception == null || exception.ExceptionType.HasFlag(ExceptionType.NonRemovable)) { return; }
            //}
            e.CanExecute = true;
        }
        private void GetDescriptorExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                try
                {
                    // Read label from the previously labeled file. Set file options from FileOptions struct. Used to set various parameters for FileHandler
                    var options = new Common.FileOptions
                    {
                        FileName = this.DocumentPath,
                        OutputName = this.DocumentPath,
                        ActionSource = ActionSource.Manual,
                        AssignmentMethod = AssignmentMethod.Standard,
                        // ActionSource = ActionSource.Automatic,
                        // AssignmentMethod = AssignmentMethod.Auto,
                        DataState = DataState.Rest,
                        GenerateChangeAuditEvent = true,
                        IsAuditDiscoveryEnabled = true,
                        //LabelId = this.SelectedLabel.Id
                    };
                    sec.Debug(new { options = options.GetLogString() });
                    var contentLabel = _mip.GetLabel(options); sec.Debug($"_mip.GetLabel({options.GetLogString()}); returned {contentLabel.GetLogString()}");

                    var label = contentLabel?.Label != null ? LeafLabels.FirstOrDefault(l => l.Id == contentLabel.Label.Id) : null;
                    this.DocumentLabel = label;
                    this.SelectedLabel = label;
                    sec.Debug(new { this.DocumentLabel });

                    var descriptor = _mip.GetProtectionDescriptor(options); sec.Debug($"_mip.GetProtectionDescriptor(options); returned {descriptor.GetLogString()}");

                    var protectionDescriptorSurrogate = new ProtectionDescriptorSurrogate(descriptor);
                    var descriptorString = SerializationHelper.SerializeJsonObject(protectionDescriptorSurrogate);
                    this.Output = descriptorString;
                }
                catch (Exception ex)
                {
                    sec.Exception(ex);
                }
            }
        }
        private void ApplyLabelCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            //if (string.IsNullOrEmpty(this.DocumentPath)) { return; }
            //if (IsProtectedView) { return; }
            if (this.Exception != null)
            {
                var exception = this.Exception as ClientException;
                if (exception == null || exception.ExceptionType.HasFlag(ExceptionType.NonRemovable)) { return; }
            }
            e.CanExecute = true;
        }
        private void ApplyLabelExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                var suffix = ConfigurationHelper.GetClassSetting<MainWindow, string>(CONFIGVALUE_PROTECTEDDOCUMENTSUFFIX, DEFAULTVALUE_PROTECTEDDOCUMENTSUFFIX); // ProtectedDocumentSuffix, CultureInfo.InvariantCulture
                var backupOriginalFile = ConfigurationHelper.GetClassSetting<MainWindow, bool>(CONFIGVALUE_BACKUPORIGINALFILE, DEFAULTVALUE_BACKUPORIGINALFILE); // replaceOriginalFile, CultureInfo.InvariantCulture
                var replaceOriginalFile = ConfigurationHelper.GetClassSetting<MainWindow, bool>(CONFIGVALUE_REPLACEORIGINALFILE, DEFAULTVALUE_REPLACEORIGINALFILE); // replaceOriginalFile, CultureInfo.InvariantCulture

                var timestamp = DateTime.Now.ToString("yyyymmddhhmmss");
                var directoryName = System.IO.Path.GetDirectoryName(this.DocumentPath); sec.Debug(new { directoryName });
                var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(this.DocumentPath); sec.Debug(new { fileNameWithoutExtension });
                var application = System.Windows.Application.Current;
                var extension = FileHelper.GetExtension(DocumentPath); sec.Debug(new { extension });

                var targetLabel = this.SelectedLabel ?? this.DocumentLabel;
                var tempFilename = System.IO.Path.GetTempFileName();
                var intermediateSourceName = $"{tempFilename}.{timestamp}.src.{extension}";
                var intermediateTargetName = $"{tempFilename}.{timestamp}.dst.{extension}";

                List<string> ownerRigths;

                var labelingOptions = new LabelingOptions()
                {
                    AssignmentMethod = AssignmentMethod.Privileged,
                    IsDowngradeJustified = true,
                    JustificationMessage = ""
                };

                var documentLabelID = this.DocumentLabel != null ? this.LeafLabels.IndexOf(this.DocumentLabel) : 0;
                var targetLabelID = this.LeafLabels.IndexOf(targetLabel);

                var startFileName = this.DocumentPath;
                var documentPath = this.DocumentPath;
                var handlerTemp = System.Threading.Tasks.Task.Run(async () => await _mip.FileEngine.CreateFileHandlerAsync(documentPath,    // inputFilePath
                                                                                                                           documentPath,    // actualFilePath
                                                                                                                           false,           //isAuditDiscoveryEnabled
                                                                                                                           null)).Result;   // fileExecutionStat

                var fileOptions = new Common.FileOptions
                {
                    FileName = this.DocumentPath,
                    OutputName = intermediateSourceName,
                    // ActionSource = ActionSource.Automatic,
                    // AssignmentMethod = AssignmentMethod.Auto,
                    ActionSource = ActionSource.Manual,
                    AssignmentMethod = AssignmentMethod.Privileged,
                    DataState = DataState.Rest,
                    GenerateChangeAuditEvent = true,
                    IsAuditDiscoveryEnabled = false,
                    LabelId = targetLabel.Id
                };
                sec.Debug(new { options = fileOptions.GetLogString() });
                using (handlerTemp)
                {
                    // Store protection handler from file
                    var protectionHandler = handlerTemp?.Protection;
                    //Check if the user has the 'Edit' right to the file
                    if (protectionHandler != null && protectionHandler.AccessCheck("Edit"))
                    {
                        // Decrypt file to temp path
                        startFileName = System.Threading.Tasks.Task.Run(async () => await handlerTemp.GetDecryptedTemporaryFileAsync()).Result;
                        sec.Debug($"startFileName: {startFileName}");
                    }

                    var documentFolder = System.IO.Path.GetDirectoryName(this.DocumentPath);
                    fileOptions.FileName = $@"{startFileName}";
                    fileOptions.OutputName = intermediateTargetName;

                    // if (string.IsNullOrEmpty(this.EmployeeAccounts) && string.IsNullOrEmpty(this.ExternalAccounts) && string.IsNullOrEmpty(this.OwnerAccounts))
                    var tmpIsProtected = CONFIGVALUE_LABELISPROTECTED.Replace("{LabelId}", LabelHelper.GetLabelId(targetLabel));
                    bool isProtected = ConfigurationHelper.GetClassSetting<MainWindow, bool>(tmpIsProtected, DEFAULTVALUE_LABELISPROTECTED);

                    if (isProtected == false)
                    {
                        var ret = _mip.SetLabel(fileOptions, labelingOptions); sec.Debug($"_mip.SetLabel(options); returned {ret}");
                    }
                    else
                    {
                        var newRights = new List<UserRights>();
                        ownerRigths = MIPHelper.GetOwnerRigths();

                        var userUpn = this.Identity?.Upn; var userEmail = this.Identity?.Email;
                        sec.Debug(new { userUpn, userEmail });
                        // var ownerAccounts = this.OwnerAccounts?.Split(';')?.Where(s => !string.IsNullOrEmpty(s))?.Except(new[] { userUpn, userEmail }, StringComparer.InvariantCultureIgnoreCase)?.Select(s => s?.Trim())?.ToList() ?? new List<string>();

                        // TODO Get the protection descriptor 
                        //var protectionDescriptorSurrogate = new ProtectionDescriptorSurrogate(descriptor);
                        var protectionDescriptorString = this.Output; sec.Debug(new { protectionDescriptorString });
                        var protectionDescriptorSurrogate = SerializationHelper.DeserializeJsonObject<ProtectionDescriptorSurrogate>(protectionDescriptorString);

                        var userRights = protectionDescriptorSurrogate.UserRights?.Select(ur => new UserRights(ur?.Users, ur?.Rights))?.ToList();
                        var userRoles = protectionDescriptorSurrogate.UserRoles?.Select(ur => new UserRoles(ur?.Users?.ToList(), ur?.Roles?.ToList()))?.ToList();

                        var protectionDescriptor = new ProtectionDescriptor(userRights) { AllowOfflineAccess = protectionDescriptorSurrogate.AllowOfflineAccess };
                        var result = _mip.SetLabelWithProtection(fileOptions, labelingOptions, protectionDescriptor); sec.Debug($"_mip.SetProtection(options, protectionDescriptor); returned {result}");
                    }
                }

                if (backupOriginalFile)
                {
                    var backupFilePath = $"{directoryName}\\{fileNameWithoutExtension}.{suffix}.bkp";
                    for (var i = 0; File.Exists(backupFilePath); i++) { backupFilePath = $"{directoryName}\\{fileNameWithoutExtension}.{suffix}.{i}.bkp"; }
                    File.Copy(this.DocumentPath, backupFilePath);
                }

                var targetFilePath = $"{directoryName}\\{fileNameWithoutExtension}.{suffix}.{extension}";


                try
                {
                    if (replaceOriginalFile)
                    {
                        targetFilePath = this.DocumentPath;
                        File.Delete(this.DocumentPath);
                    }
                    File.Move(intermediateTargetName, targetFilePath);

                    //RefreshDocumentProperties(targetLabel);
                }
                catch (Exception ex)
                {
                    sec.Exception(ex);
                    string message = null;
                    if (ex is UnauthorizedAccessException uaEx)
                    {
                        message = $"Impossibile applicare la protezione al file {targetFilePath}.\r\nIl file non è accessibile o non si dispone delle autorizzazioni per modificarlo ({ex.GetType().Name}).";
                    }
                    else { message = $"Impossibile applicare la protezione al file {targetFilePath}.\r\nInformazioni aggiuntive - {ex.GetType().Name}: {ex.Message}."; }
                    this.Exception = new ClientException(message) { ExceptionLevel = ExceptionLevel.Warning, ExceptionType = ExceptionType.Default };
                }


            }
        }

        private object getIdentityDescription_ConvertEvent(DependencyObject source, object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var identity = value as Identity;
            if (identity == null) { return "login"; }
            if (!string.IsNullOrEmpty(identity.Name)) { return identity.Name; }
            if (!string.IsNullOrEmpty(identity.Upn)) { return identity.Upn; }

            return identity.Email;
        }
        private object getLabelTooltip_ConvertEvent(DependencyObject source, object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var label = value as Label;
            if (label == null) { return DependencyProperty.UnsetValue; }

            var labelTooltip = $"{label.Name.ToLocalize()}\r\n{label.Description}";
            return labelTooltip;
        }
        private object getMenuItemBackground_ConvertEvent(DependencyObject source, object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int i = 0;
            var label = values != null && values.Length > i ? values[i] as Label : null; i++;
            var documentLabel = values != null && values.Length > i ? values[i] as Label : null; i++;
            var selectedLabel = values != null && values.Length > i ? values[i] as Label : null; i++;
            if (label == null) { return Brushes.White; }
            //if (documentLabel == null) { return Brushes.White; }

            if (selectedLabel == null)
            {
                if (documentLabel == null) { return Brushes.White; }
                if (label.Id == documentLabel.Id) { return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC0DDEB")); }
                if (label.Children != null && label.Children.Count > 0)
                {
                    var childLabel = label.Children.FirstOrDefault(l => l.Id == documentLabel.Id);
                    if (childLabel != null) { return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC0DDEB")); }
                }
            }
            else
            {
                if (label.Id == selectedLabel.Id) { return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC0DDEB")); }
                if (label.Children != null && label.Children.Count > 0)
                {
                    var childLabel = label.Children.FirstOrDefault(l => l.Id == selectedLabel.Id);
                    if (childLabel != null) { return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC0DDEB")); }
                }
            }

            return Brushes.White;
        }
        private object isDocumentLabel_ConvertEvent(DependencyObject source, object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int i = 0;
            var label = values != null && values.Length > i ? values[i] as Label : null; i++;
            var documentLabel = values != null && values.Length > i ? values[i] as Label : null; i++;
            var selectedLabel = values != null && values.Length > i ? values[i] as Label : null; i++;
            if (label == null) { return false; }

            if (selectedLabel == null)
            {
                if (documentLabel == null) { return false; }
                if (label.Id == documentLabel.Id) { return true; }
            }
            else
            {
                if (label.Id == selectedLabel.Id) { return true; }
            }

            return false;
        }
        private object getLabelChildren_ConvertEvent(DependencyObject source, object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var label = value as Label;
            if (string.IsNullOrEmpty(this.DocumentPath)) { return null; }
            if (label == null || label.Children == null || label.Children.Count == 0) { return null; }

            var isPdfDocument = this.DocumentPath.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase);
            var disableFooterLabelsOnPdf = ConfigurationHelper.GetClassSetting<MainWindow, bool>(CONFIGVALUE_DISABLEFOOTERLABELSONPDF, DEFAULTVALUE_DISABLEFOOTERLABELSONPDF); // , CultureInfo.InvariantCulture
            if (isPdfDocument && disableFooterLabelsOnPdf)
            {
                return label.Children.Where(c => c.Name.IndexOf("Without footer", StringComparison.InvariantCultureIgnoreCase) < 0)?.ToList();
            }

            return label.Children;
        }
        private object getLabelName_ConvertEvent(DependencyObject source, object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            using (var sec = this.GetCodeSection(new { value }))
            {
                var label = value as Label;
                if (label == null) { return null; }

                sec.Result = label.Name;
                if (string.IsNullOrEmpty(this.DocumentPath)) { return label.Name; }

                return label.Name;
            }
        }
        private object getLabelDescription_ConvertEvent(DependencyObject source, object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var label = value as Label;
            if (label == null) { return null; }

            var labelName = label.Name;
            if (label.Parent != null)
            {
                var parentLabelName = label.Parent.Name;
                return $@"{parentLabelName.ToLocalize()} \ {labelName.ToLocalize()}";
            }

            labelName = labelName.ToLocalize();
            return labelName;
        }
        private object getDocumentName_ConvertEvent(DependencyObject source, object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var documentPath = value as string;
            if (string.IsNullOrEmpty(documentPath)) { return null; }

            var fileName = System.IO.Path.GetFileName(documentPath);
            return fileName;
        }

        private void mainWindow_Drop(object sender, DragEventArgs e)
        {
            using (var sec = this.GetCodeSection(new { sender = sender.GetLogString(), e = e.GetLogString() }))
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // Note that you can have more than one file.
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    // Assuming you have one file that you care about, pass it off to whatever
                    // handling code you have defined.
                    this.DocumentPath = files[0];
                }
            }
        }
    }


    #region MainWindowState
    public class MainWindowState
    {
        #region DocumentPath
        public string DocumentPath { get; set; }
        #endregion
        #region OutputDocumentPath
        public string OutputDocumentPath { get; set; }
        #endregion
        #region Output
        public string Output { get; set; }
        #endregion
    }
    #endregion
    #region MainWindowStateProvider
    public class MainWindowStateProvider : ClassStateProvider<MainWindow, MainWindowState>
    {
        #region .ctor
        public MainWindowStateProvider()
        {
            this.True2Surrogate = delegate (MainWindow t)
            {
                MainWindowState state = new MainWindowState()
                {
                    DocumentPath = t.DocumentPath,
                    OutputDocumentPath = t.OutputDocumentPath,
                    Output = t.Output
                };
                return state;
            };

            this.Surrogate2True = delegate (MainWindow t, MainWindowState s)
            {
                if (t == null) { t = new MainWindow(); }
                t.DocumentPath = s.DocumentPath;
                t.OutputDocumentPath = s.OutputDocumentPath;
                t.Output = s.Output;

                return t;
            };
        }
        #endregion
    }
    #endregion
}
