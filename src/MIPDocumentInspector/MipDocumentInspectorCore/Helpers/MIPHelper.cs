#region using
using Common;
using Microsoft.InformationProtection;
using Microsoft.InformationProtection.File;
using Microsoft.InformationProtection.Policy;
using Microsoft.InformationProtection.Policy.Actions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
#endregion

namespace Common
{
    public class MIPHelper
    {
        #region const
        const string CONFIGVALUE_LOADMIPFROMAPPLICATIONPATH = "LoadMIPFromApplicationPath"; const bool DEFAULTVALUE_LOADMIPFROMAPPLICATIONPATH = true;
        const string CONFIGVALUE_OWNERRIGHTSLABEL = "OwnerRights"; const string DEFAULTVALUE_OWNERRIGHTSLABEL = "OWNER";
        const string CONFIGVALUE_LABELPUBLICRIGHTS = "Label.Public.Rigths"; const string DEFAULTVALUE_LABELPUBLICRIGHTS = "";
        //const string CONFIGVALUE_LABELOWNERRIGHTS = "Label.{LabelId}.Owner.Rights"; const string DEFAULTVALUE_LABELOWNERRIGHTS = "";
        const string CONFIGVALUE_LABELINTERNALRIGHTS = "Label.{LabelId}.Internal.Rights"; const string DEFAULTVALUE_LABELINTERNALRIGHTS = "";
        const string CONFIGVALUE_LABELEXTERNALRIGHTS = "Label.{LabelId}.External.Rights"; const string DEFAULTVALUE_LABELEXTERNALRIGHTS = "";
        const string CONFIGVALUE_LABELISPROTECTED = "Label.{LabelId}.isProtected"; const bool DEFAULTVALUE_LABELISPROTECTED = false;
        const string CONFIGVALUE_LABELOWNERISREADONLY = "Label.{LabelId}.Owner.IsReadOnly"; const bool DEFAULTVALUE_LABELOWNERISREADONLY = true;
        const string CONFIGVALUE_LABELINTERNALISREADONLY = "Label.{LabelId}.Internal.IsReadOnly"; const bool DEFAULTVALUE_LABELINTERNALISREADONLY = true;
        const string CONFIGVALUE_LABELEXTERNALISREADONLY = "Label.{LabelId}.External.IsReadOnly"; const bool DEFAULTVALUE_LABELEXTERNALISREADONLY = true;
        // const string CONFIGVALUE_LABELOWNERACCOUNTS = "Label.{LabelId}.Owner.Accounts"; const string DEFAULTVALUE_LABELOWNERACCOUNTS = "";
        const string CONFIGVALUE_LABELINTERNALACCOUNTS = "Label.{LabelId}.Internal.Accounts"; const string DEFAULTVALUE_LABELINTERNALACCOUNTS = "";
        const string CONFIGVALUE_LABELEXTERNALACCOUNTS = "Label.{LabelId}.External.Accounts"; const string DEFAULTVALUE_LABELEXTERNALACCOUNTS = "";
        const string CONFIGVALUE_LABELINTERNALTEXT = "Label.{LabelId}.Internal.Description"; const string DEFAULTVALUE_LABELINTERNALTEXT = "";
        const string CONFIGVALUE_LABELEXTERNALTEXT = "Label.{LabelId}.External.Description"; const string DEFAULTVALUE_LABELEXTERNALTEXT = "";
        const string CONFIGVALUE_LABELOWNERSTEXT = "Label.{LabelId}.Owners.Description"; const string DEFAULTVALUE_LABELOWNERSTEXT = "";
        #endregion

        #region internal state
        private ApplicationInfo _appInfo;
        private MipContext _mipContext;
        private IFileEngine _fileEngine;
        private IPolicyEngine _policyEngine;
        private IPolicyProfile _policyProfile;
        private IFileProfile _fileProfile;
        private List<Label> _labels;
        private List<Label> _leafLabels;
        #endregion
        public ApplicationInfo AppInfo { get => _appInfo; set => _appInfo = value; }
        public MipContext MipContext { get => _mipContext; set => _mipContext = value; }
        public IFileProfile FileProfile { get => _fileProfile; set => _fileProfile = value; }
        public IPolicyProfile PolicyProfile { get => _policyProfile; set => _policyProfile = value; }
        public IFileEngine FileEngine { get => _fileEngine; set => _fileEngine = value; }
        public IPolicyEngine PolicyEngine { get => _policyEngine; set => _policyEngine = value; }
        public List<Label> Labels { get => _labels; set => _labels = value; }
        public List<Label> LeafLabels { get => _leafLabels; set => _leafLabels = value; }

        /// <summary>Constructor for Action class. Pass in AppInfo to simplify passing settings to AuthDelegate.</summary>
        /// <param name="appInfo"></param>
        public MIPHelper(ApplicationInfo appInfo, string assemblyPath = null)
        {
            using (var sec = this.GetCodeSection(new { appInfo = appInfo.GetLogString(), assemblyPath }))
            {
                this.AppInfo = appInfo;

                // Set path to bins folder.
                //var mipVersion = ".1.7.151";
                var mipVersion = "";
                var baseDirectory = !string.IsNullOrEmpty(assemblyPath) ? assemblyPath : AppDomain.CurrentDomain.BaseDirectory;
                var path = Path.Combine(baseDirectory, Environment.Is64BitProcess ? $"x64{mipVersion}" : $"x86{mipVersion}");
                sec.Debug(new { path });
                //var path = Path.Combine(
                //  Directory.GetParent(Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath)).FullName,
                //   Environment.Is64BitProcess ? "bin\\x64" : "bin\\x86");
                //var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + (Environment.Is64BitProcess ? "x64" : "x86");

                //// Initialize MIP for File API.
                var loadMipFromApplicationPath = ConfigurationHelper.GetClassSetting<MIPHelper, bool>(CONFIGVALUE_LOADMIPFROMAPPLICATIONPATH, DEFAULTVALUE_LOADMIPFROMAPPLICATIONPATH);
                if (loadMipFromApplicationPath)
                {
                    MIP.Initialize(MipComponent.File, path); sec.Debug($"MIP.Initialize({MipComponent.File}, {path}); completed");
                }
                else
                {
                    MIP.Initialize(MipComponent.File); sec.Debug($"MIP.Initialize({MipComponent.File}); completed");
                }

                // Create profile
                this.CreateFileProfile(); sec.Debug($"CreateFileProfile(); returned {this.FileProfile.GetLogString()}");

                // Create profile.
                _policyProfile = CreatePolicyProfile(appInfo);

                //// Create engine providing Identity from authDelegate to assist with service discovery.
                //engine = CreatePolicyEngine(authenticationHelper.Identity);

            }
        }

        /// <summary>Null refs to engine and profile and release all MIP resources.</summary>
        ~MIPHelper()
        {
            using (var sec = this.GetCodeSection())
            {
                _fileEngine = null;
                _fileProfile = null;
                _policyProfile = null;
                _mipContext = null;
            }
        }

        public Label GetLabelById(string labelId)
        {
            using (var sec = this.GetCodeSection(new { labelId }))
            {
                var label = _fileEngine.GetLabelById(labelId);
                sec.Result = label;
                return label;
            }
        }

        /// <summary>Creates an IFileProfile and returns.
        /// IFileProfile is the root of all MIP SDK File API operations. Typically only one should be created per app.</summary>
        /// <param name="appInfo"></param>
        /// <param name="authDelegate"></param>
        /// <returns></returns>
        public IFileProfile CreateFileProfile()
        {
            using (var sec = this.GetCodeSection(new { appInfo = this.AppInfo.GetLogString() }))
            {
                if (_mipContext == null)
                {
                    var tempPath = UserProfileHelper.GetTempFolder(); sec.Debug(new { tempPath });
                    var cacheFolder = $"{tempPath}\\mip_data";
                    if (!Directory.Exists(cacheFolder)) { Directory.CreateDirectory(cacheFolder); }
                    if (!Directory.Exists($"{cacheFolder}\\mip")) { Directory.CreateDirectory($"{cacheFolder}\\mip"); }
                    _mipContext = MIP.CreateMipContext(this.AppInfo, cacheFolder, LogLevel.Trace, null, null); sec.Debug($"MIP.CreateMipContext({this.AppInfo.GetLogString()}, '{cacheFolder}', LogLevel.Trace, null, null); returned {_mipContext.GetLogString()}");
                }

                // Initialize file profile settings to create/use local state.                
                var profileSettings = new FileProfileSettings(_mipContext, CacheStorageType.OnDiskEncrypted, new ConsentDelegateImplementation());

                //// Use MIP.LoadFileProfileAsync() providing settings to create IFileProfile. IFileProfile is the root of all SDK operations for a given application.
                var profile = Task.Run(async () =>
                {
                    var res = await MIP.LoadFileProfileAsync(profileSettings); TraceManager.Debug($"await MIP.LoadFileProfileAsync(profileSettings); returned {res}");
                    return res;
                }).Result;

                //var task = MIP.LoadFileProfileAsync(profileSettings); TraceManager.Debug($"await MIP.LoadFileProfileAsync(profileSettings); returned {task}");
                //var profile = task.Result;

                this.FileProfile = profile;
                sec.Result = profile.GetLogString();
                return profile;
            }
        }
        /// <summary>Creates a file engine, associating the engine with the specified identity. 
        /// File engines are generally created per-user in an application. 
        /// IFileEngine implements all operations for fetching labels and sensitivity types.
        /// IFileHandlers are added to engines to perform labeling operations.</summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public IFileEngine CreateFileEngine(AuthenticationHelperMIP authenticationHelper)
        {
            using (var sec = this.GetCodeSection())
            {
                var identity = authenticationHelper.Identity;
                sec.Debug(new { Identity = identity.GetLogString() });

                // If the profile hasn't been created, do that first. 
                if (this.FileProfile == null) { this.FileProfile = CreateFileProfile(); }

                // Create file settings object. Passing in empty string for the first parameter, engine ID, will cause the SDK to generate a GUID.
                // Locale settings are supported and should be provided based on the machine locale, particular for client applications.
                var engineSettings = new FileEngineSettings(identity.Upn, authenticationHelper, "", "en-US")
                {
                    // Provide the identity for service discovery.
                    Identity = AuthenticationHelperMIP.ToIdentityMIP(identity) 
                };
                sec.Debug(new { engineSettings = engineSettings.GetLogString() });

                // Add the IFileEngine to the profile and return.
                var engine = Task.Run(async () =>
                {
                    var res = await _fileProfile.AddEngineAsync(engineSettings); TraceManager.Debug($"await profile.AddEngineAsync({engineSettings.GetLogString()}); returned {res.GetLogString()}");
                    return res;
                }).Result;
                this.FileEngine = engine;

                sec.Result = engine.GetLogString();
                return engine;
            }
        }        
        /// <summary>Method creates a file handler and returns to the caller. 
        /// IFileHandler implements all labeling and protection operations in the File API.</summary>
        /// <param name="options">Struct provided to set various options for the handler.</param>
        /// <returns></returns>
        public IFileHandler CreateFileHandler(FileOptions options)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString() }))
            {
                // Create the handler using options from FileOptions. 
                // Assumes that the engine was previously created and stored in private engine object.
                // There's probably a better way to pass/store the engine, but this is a sample ;)
                var exception = default(Exception);
                var handler = Task.Run(async () =>
                {
                    try
                    {
                        var ret = await _fileEngine.CreateFileHandlerAsync(options.FileName, options.FileName, options.IsAuditDiscoveryEnabled);
                        TraceManager.Debug($"await engine.CreateFileHandlerAsync({options.FileName}, {options.FileName}, {options.IsAuditDiscoveryEnabled}); returned {ret.GetLogString()}");
                        return ret;
                    }
                    catch (Exception ex)
                    {
                        TraceManager.Exception(ex);
                        exception = ex;
                    }
                    return null;
                }).Result;
                if (exception != null) { throw exception; }
                return handler;
            }
        }
        /// <summary>
        /// Creates an IFileProfile and returns.
        /// IFileProfile is the root of all MIP SDK File API operations. Typically only one should be created per app.
        /// </summary>
        /// <param name="appInfo"></param>
        /// <param name="authDelegate"></param>
        /// <returns></returns>
        private IPolicyProfile CreatePolicyProfile(ApplicationInfo appInfo)
        {
            using (var sec = this.GetCodeSection(new { appInfo = appInfo.GetLogString() }))
            {
                // Initialize MipContext
                if (_mipContext == null)
                {
                    var tempPath = UserProfileHelper.GetTempFolder(); sec.Debug(new { tempPath });
                    _mipContext = MIP.CreateMipContext(this.AppInfo, $"{tempPath}\\mip_data", LogLevel.Trace, null, null); sec.Debug($"MIP.CreateMipContext({this.AppInfo.GetLogString()}, '{tempPath}mip_data', LogLevel.Trace, null, null); returned {_mipContext.GetLogString()}");
                }

                // Initialize file profile settings to create/use local state.                
                var profileSettings = new PolicyProfileSettings(_mipContext, CacheStorageType.OnDiskEncrypted);

                // Use MIP.LoadFileProfileAsync() providing settings to create IFileProfile. 
                // IFileProfile is the root of all SDK operations for a given application.
                var profile = Task.Run(async () =>
                {
                    var ret = await MIP.LoadPolicyProfileAsync(profileSettings); TraceManager.Debug($"await MIP.LoadPolicyProfileAsync({profileSettings.GetLogString()}); returned {ret.GetLogString()}");
                    return ret;
                }
                ).Result;

                sec.Result = profile.GetLogString();
                return profile;
            }
        }
        /// <summary>
        /// Creates a file engine, associating the engine with the specified identity. 
        /// File engines are generally created per-user in an application. 
        /// IFileEngine implements all operations for fetching labels and sensitivity types.
        /// IFileHandlers are added to engines to perform labeling operations.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public IPolicyEngine CreatePolicyEngine(AuthenticationHelperMIP authenticationHelper)
        {
            using (var sec = this.GetCodeSection(new { authenticationHelper = authenticationHelper.GetLogString() }))
            {
                // If the profile hasn't been created, do that first. 
                if (_policyProfile == null)
                {
                    _policyProfile = CreatePolicyProfile(_appInfo);
                }

                // Create file settings object. Passing in empty string for the first parameter, engine ID, will cause the SDK to generate a GUID.
                // Passing in a email address or other unique value helps to ensure that the cached engine is loaded each time for the same user.
                // Locale settings are supported and should be provided based on the machine locale, particular for client applications.
                var email = authenticationHelper.Identity?.Email;
                var engineSettings = new PolicyEngineSettings(email, authenticationHelper, "", "en-US")
                {
                    // Provide the identity for service discovery.
                    Identity = AuthenticationHelperMIP.ToIdentityMIP(authenticationHelper.Identity)
                };

                // Add the IFileEngine to the profile and return.
                _policyEngine = Task.Run(async () => await _policyProfile.AddEngineAsync(engineSettings)).Result;

                sec.Result = _policyEngine.GetLogString();
                return _policyEngine;
            }
        }
        /// <summary>
        /// Method creates a file handler and returns to the caller. 
        /// IFileHandler implements all labeling and protection operations in the File API.        
        /// </summary>
        /// <param name="options">Struct provided to set various options for the handler.</param>
        /// <returns></returns>
        public IPolicyHandler CreatePolicyHandler(ExecutionStateOptions options)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString() }))
            {
                // Create the handler using options from FileOptions. 
                // Assumes that the engine was previously created and stored in private engine object.
                // There's probably a better way to pass/store the engine, but this is a sample ;)
                var handler = _policyEngine.CreatePolicyHandler(options.generateAuditEvent);

                sec.Result = handler.GetLogString();
                return handler;
            }
        }

        /// <summary>Read the label from a file provided via FileOptions.</summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public ContentLabel GetLabel(FileOptions options)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString() }))
            {
                using (var handler = CreateFileHandler(options))
                {
                    var res = handler.Label;
                    sec.Result = res.GetLogString();
                    return res;
                }
            }
        }
        public ContentLabel GetLabel(IFileHandler handler)
        {
            using (var sec = this.GetCodeSection(new { handler = handler.GetLogString() }))
            {
                var res = handler.Label;
                sec.Result = res.GetLogString();
                return res;
            }
        }

        /// <summary>List all labels from the engine and return in IEnumerable<Label></summary>
        /// <returns></returns>
        public IEnumerable<Label> ListLabels()
        {
            using (var sec = this.GetCodeSection())
            {
                // Get labels from the engine and return.
                // For a user principal, these will be user specific.
                // For a service principal, these may be service specific or global.
                var res = _fileEngine.SensitivityLabels;

                sec.Result = res.GetLogString();
                return res;
            }
        }
        /// <summary>Set the label on the given file. 
        /// Options for the labeling operation are provided in the FileOptions parameter.</summary>
        /// <param name="options">Details about file input, output, label to apply, etc.</param>
        /// <returns></returns>
        public bool SetLabel(FileOptions options, LabelingOptions labelingOptions)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString() }))
            {
                // LabelingOptions allows us to set the metadata associated with the labeling operations
                // Review the API Spec at https://aka.ms/mipsdkdocs for details
                // LabelingOptions labelingOptions = new LabelingOptions() { AssignmentMethod = options.AssignmentMethod }

                using (var handler = CreateFileHandler(options))
                {

                    // Use the SetLabel method on the handler, providing label ID and LabelingOptions
                    // The handler already references a file, so those details aren't needed.
                    try
                    {
                        handler.SetLabel(_fileEngine.GetLabelById(options.LabelId), labelingOptions, new ProtectionSettings());
                    }
                    catch (Microsoft.InformationProtection.Exceptions.JustificationRequiredException)
                    {
                        Console.Write("Please provide justification: ");
                        string justification = Console.ReadLine();

                        labelingOptions.IsDowngradeJustified = true;
                        labelingOptions.JustificationMessage = justification;

                        handler.SetLabel(_fileEngine.GetLabelById(options.LabelId), labelingOptions, new ProtectionSettings());
                    }

                    // The change isn't committed to the file referenced by the handler until CommitAsync() is called.
                    // Pass the desired output file name in to the CommitAsync() function.
                    var result = Task.Run(async () =>
                    {
                        var res = await handler.CommitAsync(options.OutputName); TraceManager.Debug($"await handler.CommitAsync(options.OutputName); returned {res.GetLogString()}");
                        return res;
                    }).Result;

                    // If the commit was successful and GenerateChangeAuditEvents is true, call NotifyCommitSuccessful()
                    if (result && options.GenerateChangeAuditEvent)
                    {
                        // Submits and audit event about the labeling action to Azure Information Protection Analytics 
                        handler.NotifyCommitSuccessful(options.FileName);
                        sec.Debug($"handler.NotifyCommitSuccessful({options.FileName}); completed");
                    }

                    sec.Result = result;
                    return result;
                }
            }
        }
        public bool SetLabelWithProtection(FileOptions options, LabelingOptions labelingOptions, ProtectionDescriptor protectionDescriptor)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString() }))
            {
                var handler = CreateFileHandler(options);
                sec.Debug($"CreateFileHandler({options.GetLogString()}); returned {handler.GetLogString()}");

                using (handler)
                {
                    //// Use the SetLabel method on the handler, providing label ID and LabelingOptions
                    //// The handler already references a file, so those details aren't needed.
                    var protectionSettings = new ProtectionSettings()
                    {
                        DelegatedUserEmail = null,
                        PFileExtensionBehavior = PFileExtensionBehavior.Default,
                    };

                    handler.SetProtection(protectionDescriptor, protectionSettings);
                    sec.Debug($"handler.SetProtection({protectionDescriptor.GetLogString()}, {protectionSettings.GetLogString()}); completed");

                    try
                    {
                        var label = _fileEngine.GetLabelById(options.LabelId);

                        //handler.SetProtection(   )

                        handler.SetLabel(label, labelingOptions, protectionSettings);
                        sec.Debug($"handler.SetLabel({label.GetLogString()}, {labelingOptions.GetLogString()}, {protectionSettings.GetLogString()}); completed");
                    }
                    catch (Microsoft.InformationProtection.Exceptions.JustificationRequiredException)
                    {
                        Console.Write("Please provide justification: ");
                        string justification = Console.ReadLine();

                        labelingOptions.IsDowngradeJustified = true;
                        labelingOptions.JustificationMessage = justification;

                        handler.SetLabel(_fileEngine.GetLabelById(options.LabelId), labelingOptions, new ProtectionSettings());
                    }

                    // The change isn't committed to the file referenced by the handler until CommitAsync() is called.
                    // Pass the desired output file name in to the CommitAsync() function.
                    var result = Task.Run(async () =>
                    {
                        var res = await handler.CommitAsync(options.OutputName); TraceManager.Debug($"await handler.CommitAsync(options.OutputName); returned {res.GetLogString()}");
                        return res;
                    }).Result;

                    // If the commit was successful and GenerateChangeAuditEvents is true, call NotifyCommitSuccessful()
                    if (result && options.GenerateChangeAuditEvent)
                    {
                        // Submits and audit event about the labeling action to Azure Information Protection Analytics 
                        handler.NotifyCommitSuccessful(options.FileName); sec.Debug($"handler.NotifyCommitSuccessful({options.FileName}); completed");
                    }

                    sec.Result = result;
                    return result;
                }
            }
        }

        public ProtectionDescriptor GetProtectionDescriptor(FileOptions options)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString() }))
            using (var handler = this.CreateFileHandler(options))
            {
                // get the ProtectionDescriptor 
                var descriptor = handler?.Protection?.ProtectionDescriptor;
                return descriptor;
            }
        }
        public string GetOwner(FileOptions options)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString() }))
            using (var handler = this.CreateFileHandler(options))
            {
                var descriptor = handler?.Protection?.ProtectionDescriptor;
                return descriptor?.Owner;
            }
        }
        public string GetOwner(ProtectionDescriptor protectionDescriptor)
        {
            using (var sec = this.GetCodeSection(new { protectionDescriptor = protectionDescriptor.GetLogString() }))
            {
                return protectionDescriptor?.Owner;
            }
        }
        public List<string> GetCoownerUsers(FileOptions options, List<string> ownerRights)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString(), ownerRights = ownerRights.GetLogString() }))
            using (var handler = this.CreateFileHandler(options))
            {
                var contentLabel = this.GetLabel(handler);
                var DocumentLabel = contentLabel?.Label;
                var ownerUsers = new List<string>();
                // get the ProtectionDescriptor 
                var descriptor = handler?.Protection?.ProtectionDescriptor;
                if (descriptor != null)
                {
                    var documentRights = descriptor.UserRights;

                    documentRights?.ForEach(documentRight =>
                    {
                        // if full access  => get internal users
                        if (ownerRights != null && ownerRights.All(ir => documentRight.Rights != null && documentRight.Rights.Contains(ir, StringComparer.InvariantCultureIgnoreCase)))
                        {
                            if (documentRight.Users != null)
                            {
                                var users = documentRight.Users.Where(u => UserHelper.IsInternalEmail(u)).ToList();
                                ownerUsers.AddRange(users);
                            }
                        }
                    });
                }
                return ownerUsers;
            }
        }
        public List<string> GetCoownerUsers(ProtectionDescriptor protectionDescriptor, List<string> ownerRights)
        {
            using (var sec = this.GetCodeSection(new { protectionDescriptor = protectionDescriptor.GetLogString(), ownerRights = ownerRights.GetLogString() }))
            {
                var ownerUsers = new List<string>();
                if (protectionDescriptor != null)
                {
                    var documentRights = protectionDescriptor.UserRights;

                    documentRights?.ForEach(r =>
                    {
                        if (ownerRights != null && ownerRights.All(ir => r.Rights != null && r.Rights.Contains(ir, StringComparer.InvariantCultureIgnoreCase)))
                        {
                            if (r.Users != null)
                            {
                                var users = r.Users.Where(u => UserHelper.IsInternalEmail(u)).ToList();
                                ownerUsers.AddRange(users);
                            }
                        }
                    });
                }
                return ownerUsers;
            }
        }
        public List<string> GetCoownerUsers(List<UserRights> userRights, List<string> ownerRights)
        {
            using (var sec = this.GetCodeSection(new { userRights = userRights.GetLogString(), ownerRights = ownerRights.GetLogString() }))
            {
                var ownerUsers = new List<string>();
                if (userRights != null)
                {
                    userRights?.ForEach(r =>
                    {
                        if (ownerRights != null && ownerRights.All(ir => r.Rights != null && r.Rights.Contains(ir, StringComparer.InvariantCultureIgnoreCase)))
                        {
                            if (r.Users != null)
                            {
                                var users = r.Users.Where(u => UserHelper.IsInternalEmail(u)).ToList();
                                ownerUsers.AddRange(users);
                            }
                        }
                    });
                }
                return ownerUsers;
            }
        }

        public List<string> GetInternalUsers(FileOptions options, List<string> internalRights)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString(), internalRights = internalRights.GetLogString() }))
            using (var handler = this.CreateFileHandler(options))
            {
                var contentLabel = this.GetLabel(handler);
                var DocumentLabel = contentLabel?.Label;
                var internalUsers = new List<string>();
                // get the ProtectionDescriptor 
                var descriptor = handler?.Protection?.ProtectionDescriptor;
                if (descriptor != null)
                {
                    var rights = descriptor.UserRights;

                    rights?.ForEach(r =>
                    {
                        // if full access  => get internal users
                        if (internalRights != null && internalRights.All(ir => r.Rights != null && r.Rights.Contains(ir, StringComparer.InvariantCultureIgnoreCase)))
                        {
                            if (r.Users != null)
                            {
                                var users = r.Users.Where(u => UserHelper.IsInternalEmail(u)).ToList();
                                internalUsers.AddRange(users);
                            }
                        }
                    });
                }
                return internalUsers;
            }
        }
        public List<string> GetInternalUsers(ProtectionDescriptor protectionDescriptor, List<string> internalRights)
        {
            using (var sec = this.GetCodeSection(new { protectionDescriptor = protectionDescriptor.GetLogString(), internalRights = internalRights.GetLogString() }))
            {
                var internalUsers = new List<string>();
                // get the ProtectionDescriptor 
                var descriptor = protectionDescriptor;
                if (descriptor != null)
                {
                    var rights = descriptor.UserRights;

                    rights?.ForEach(r =>
                    {
                        // if full access  => get internal users
                        if (internalRights != null && internalRights.All(ir => r.Rights != null && r.Rights.Contains(ir, StringComparer.InvariantCultureIgnoreCase)))
                        {
                            if (r.Users != null)
                            {
                                var users = r.Users.Where(u => UserHelper.IsInternalEmail(u)).ToList();
                                internalUsers.AddRange(users);
                            }
                        }
                    });

                }
                return internalUsers;
            }
        }
        public List<string> GetExternalUsers(FileOptions options, List<string> externalRights)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString(), externalRights = externalRights.GetLogString() }))
            using (var handler = this.CreateFileHandler(options))
            {
                var contentLabel = this.GetLabel(handler);
                var DocumentLabel = contentLabel?.Label;
                var externalUsers = new List<string>();
                // get the ProtectionDescriptor 
                var descriptor = handler?.Protection?.ProtectionDescriptor;
                if (descriptor != null)
                {
                    var rights = descriptor.UserRights;

                    rights?.ForEach(r =>
                    {
                        // if full access  => get internal users
                        if (externalRights != null && externalRights.All(ir => r.Rights != null && r.Rights.Contains(ir, StringComparer.InvariantCultureIgnoreCase)))
                        {
                            if (r.Users != null)
                            {
                                var users = r.Users.Where(u => UserHelper.IsExternalEmail(u)).ToList();
                                externalUsers.AddRange(users);
                            }
                        }
                    });

                }
                return externalUsers;
            }
        }
        public List<string> GetExternalUsers(ProtectionDescriptor protectionDescriptor, List<string> externalRights)
        {
            using (var sec = this.GetCodeSection(new { protectionDescriptor = protectionDescriptor.GetLogString(), externalRights = externalRights.GetLogString() }))
            {
                var externalUsers = new List<string>();
                var descriptor = protectionDescriptor;
                if (descriptor != null)
                {
                    var rights = descriptor.UserRights;

                    rights?.ForEach(r =>
                    {
                        // if full access  => get internal users
                        if (externalRights != null && externalRights.All(ir => r.Rights != null && r.Rights.Contains(ir, StringComparer.InvariantCultureIgnoreCase)))
                        {
                            if (r.Users != null)
                            {
                                var users = r.Users.Where(u => UserHelper.IsExternalEmail(u)).ToList();
                                externalUsers.AddRange(users);
                            }
                        }
                    });

                }
                return externalUsers;
            }
        }

        public static List<string> GetOwnerRigths()
        {
            List<string> ownerRigths;
            var tmp_ownerRigths = ConfigurationHelper.GetClassSetting<MIPHelper, string>(CONFIGVALUE_OWNERRIGHTSLABEL, DEFAULTVALUE_OWNERRIGHTSLABEL);
            ownerRigths = tmp_ownerRigths?.Split(';')?.Where(s => !string.IsNullOrEmpty(s))?.Select(s => s?.Trim())?.ToList();
            return ownerRigths;
        }
        public static List<string> GetInternalRigths(Label targetLabel)
        {
            List<string> internalRights;
            var internalRightsLabelSetting = CONFIGVALUE_LABELINTERNALRIGHTS.Replace("{LabelId}", LabelHelper.GetLabelId(targetLabel));
            var internalRightsString = ConfigurationHelper.GetClassSetting<MIPHelper, string>(internalRightsLabelSetting, DEFAULTVALUE_LABELINTERNALRIGHTS);
            internalRights = internalRightsString?.Split(';')?.Where(s => !string.IsNullOrEmpty(s))?.Select(s => s?.Trim())?.ToList();
            return internalRights;
        }
        public static List<string> GetExternalRights(Label targetLabel)
        {
            List<string> externalRights;
            var externalRightsLabelSetting = CONFIGVALUE_LABELEXTERNALRIGHTS.Replace("{LabelId}", LabelHelper.GetLabelId(targetLabel));
            var externalRightsString = ConfigurationHelper.GetClassSetting<MIPHelper, string>(externalRightsLabelSetting, DEFAULTVALUE_LABELEXTERNALRIGHTS);
            externalRights = externalRightsString?.Split(';')?.Where(s => !string.IsNullOrEmpty(s))?.Select(s => s?.Trim())?.ToList();
            return externalRights;
        }

        public ReadOnlyCollection<Microsoft.InformationProtection.Policy.Actions.Action> ComputeActions(ExecutionStateOptions options)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString() }))
            {
                var handler = CreatePolicyHandler(options);
                ExecutionStateImplementation state = new ExecutionStateImplementation(options);

                var actions = handler.ComputeActions(state); sec.Debug($"handler.ComputeActions({state.GetLogString()}); returned {actions?.GetLogString()}");

                if (actions.Count == 0 && options.generateAuditEvent)
                {
                    handler.NotifyCommittedActions(state); sec.Debug($"handler.NotifyCommittedActions(state); completed");
                }

                sec.Result = actions.GetLogString();
                return actions;
            }
        }
        public bool ComputeActionLoop(ExecutionStateOptions options)
        {
            using (var sec = this.GetCodeSection(new { options = options.GetLogString() }))
            {
                var state = new ExecutionStateImplementation(options);

                var handler = CreatePolicyHandler(options); sec.Debug($"CreatePolicyHandler(options); return {handler.GetLogString()}");
                var actions = handler.ComputeActions(state); sec.Debug($"handler.ComputeActions(state); returned {actions.GetLogString()}");

                while (actions != null && actions.Count > 0)
                {
                    sec.Debug($"Action Count: {actions.Count}");

                    foreach (var action in actions)
                    {
                        switch (action.ActionType)
                        {
                            case ActionType.Metadata:
                                var derivedMetadataAction = (MetadataAction)action;

                                if (derivedMetadataAction.MetadataToRemove.Count > 0)
                                {
                                    sec.Debug("*** Action: Remove Metadata.");
                                    //Rather than iterate, in the same we just remove it all.
                                    options.metadata.Clear();
                                }

                                if (derivedMetadataAction.MetadataToAdd.Count > 0)
                                {
                                    sec.Debug("*** Action: Apply Metadata.");
                                    //Iterate through metadata and add to options
                                    foreach (var item in derivedMetadataAction.MetadataToAdd)
                                    {
                                        options.metadata.Add(item.Key, item.Value);
                                        Console.WriteLine("*** Added: {0} - {1}", item.Key, item.Value);
                                    }
                                }

                                break;

                            case ActionType.ProtectByTemplate:
                                var derivedProtectbyTemplateAction = (ProtectByTemplateAction)action;
                                //options.templateId = derivedProtectbyTemplateAction.TemplateId;
                                sec.Debug($"*** Action: Protect by Template: {derivedProtectbyTemplateAction.TemplateId}");

                                break;

                            case ActionType.RemoveProtection:

                                var derivedRemoveProtectionAction = (RemoveProtectionAction)action;
                                //options.templateId = string.Empty;
                                sec.Debug("*** Action: Remove Protection.");

                                break;

                            case ActionType.Justify:

                                var derivedJustificationAction = (JustifyAction)action;
                                sec.Debug("*** Justification Required! Provide Justification: ");
                                string justificationMessage = Console.ReadLine();

                                options.isDowngradeJustified = true;
                                options.downgradeJustification = justificationMessage;

                                break;

                            case ActionType.AddContentFooter:
                            // Any other actions must be explicitly defined after this.

                            default:


                                break;
                        }
                    }

                    state = new ExecutionStateImplementation(options);
                    actions = handler.ComputeActions(state);
                    Console.WriteLine("*** Remaining Action Count: {0}", actions.Count);
                }
                if (options.generateAuditEvent && actions.Count == 0)
                {
                    handler.NotifyCommittedActions(state);
                }

                sec.Result = true;
                return true;
            }
        }


        //public IList<Label> GetLabelsWithPolicyEngine(AuthenticationHelper authenticationHelper)
        //{
        //    using (var sec = this.GetCodeSection(new { authenticationHelper = authenticationHelper.GetLogString() }))
        //    {
        //        if (this.PolicyEngine == null)
        //        {
        //            var policyEngine = this.CreatePolicyEngine(authenticationHelper); sec.Debug($"this.CreatePolicyEngine(authenticationHelper); {policyEngine.GetLogString()}");
        //            this.PolicyEngine = policyEngine;
        //        }

        //        sec.Information("List all labels available to the engine created in Action");
        //        var labels = this.PolicyEngine.SensitivityLabels;
        //        sec.Debug($"found {labels.Count()} labels:");
        //        this.Labels = labels.ToList();

        //        var leafLabels = new List<Label>();
        //        labels.ToList()?.ForEach(l =>
        //        {
        //            sec.Debug(new { label = l.GetLogString() });
        //            if (l.Children == null || l.Children.Count == 0) { leafLabels.Add(l); }
        //            else
        //            {
        //                leafLabels.AddRange(l.Children);
        //            }
        //        });
        //        this.LeafLabels = leafLabels.ToList();
        //        sec.Debug($"Leaf labels {this.LeafLabels?.Count}: ");
        //        leafLabels.ToList()?.ForEach(l => { sec.Debug(new { label = l.GetLogString() }); });

        //        return labels;
        //    }
        //}
        public IList<Label> GetLabelsWithFileEngine(AuthenticationHelperMIP authenticationHelper)
        {
            using (var sec = this.GetCodeSection(new { authenticationHelper = authenticationHelper.GetLogString() }))
            {
                if (this.FileEngine == null)
                {
                    var fileEngine = this.CreateFileEngine(authenticationHelper); sec.Debug($"this.CreateFileEngine(authenticationHelper); {fileEngine.GetLogString()}");
                    this.FileEngine = fileEngine;
                }

                sec.Information("List all labels available to the engine created in Action");
                var labels = this.FileEngine.SensitivityLabels;
                sec.Debug($"found {labels.Count()} labels:");
                this.Labels = labels.ToList();

                var leafLabels = new List<Label>();
                labels.ToList()?.ForEach(l =>
                {
                    sec.Debug(new { label = l.GetLogString() });
                    if (l.Children == null || l.Children.Count == 0) { leafLabels.Add(l); }
                    else
                    {
                        leafLabels.AddRange(l.Children);
                    }
                });
                this.LeafLabels = leafLabels.ToList();
                sec.Debug($"Leaf labels {this.LeafLabels?.Count}: ");
                leafLabels.ToList()?.ForEach(l => { sec.Debug(new { label = l.GetLogString() }); });

                return labels;
            }
        }

        public static void ResetMIPCache(string user = null)
        {
            using (var sec = TraceManager.GetCodeSection(new { user }))
            {
                // get folders 
                //var userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); sec.Debug(new { userProfileFolder });
                var publicProfileFolder = Environment.GetEnvironmentVariable("PUBLIC"); sec.Debug(new { publicProfileFolder });

                var userProfilesDirectory = Directory.GetParent(publicProfileFolder);
                var userProfilesDirectories = userProfilesDirectory.GetDirectories(); sec.Debug(new { userProfilesDirectories });
                // users C:\Users\{user}\AppData\Local\Microsoft\MSIP

                foreach (var userFolder in userProfilesDirectories)
                {
                    var userFolderPath = userFolder.FullName;
                    var userFolderName = userFolder.Name;
                    if (!string.IsNullOrEmpty(user) && !user.Equals(userFolderName, StringComparison.InvariantCultureIgnoreCase)) { continue; }

                    sec.Debug(new { userFolderPath });
                    try
                    {
                        var msipFolder = $@"{userFolderPath}\AppData\Local\Microsoft\MSIP";
                        if (Directory.Exists(msipFolder))
                        {
                            Directory.Delete(msipFolder, true);
                        }
                    }
                    catch (Exception ex) { sec.Debug(ex); } // throw; 
                }
            }
        }
    }
}
