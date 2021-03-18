#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.InformationProtection;
using Microsoft.InformationProtection.Policy;
using Microsoft.InformationProtection.Policy.Actions;
#endregion

namespace Common
{
    public struct ExecutionStateOptions
    {
        //public List<KeyValuePair<string, string>> metadata;
        public Dictionary<string, string> metadata;
        public Microsoft.InformationProtection.Label newLabel;
        public string contentIdentifier;
        public ActionSource actionSource;
        public DataState dataState;
        public AssignmentMethod assignmentMethod;
        public bool isDowngradeJustified;
        //public string templateId;
        public ProtectionDescriptor protectionDescriptor;
        //public ContentFormat contentFormat;
        public ActionType supportedActions;
        public bool generateAuditEvent;
        public string downgradeJustification;
    }

    public class ExecutionStateImplementation : ExecutionState
    {
        private ExecutionStateOptions _executionStateOptions;

        public ExecutionStateImplementation(ExecutionStateOptions executionStateOptions)
        {
            using (var sec = this.GetCodeSection(new { executionStateOptions = executionStateOptions.GetLogString() }))
            {
                _executionStateOptions = executionStateOptions;
            }
        }

        public override string GetContentFormat()
        {
            using (var sec = this.GetCodeSection())
            {
                //var ret = _executionStateOptions.contentFormat;
                //sec.Result = ret;
                return null;
            }
        }

        public override string GetContentIdentifier()
        {
            using (var sec = this.GetCodeSection())
            {
                var ret = _executionStateOptions.contentIdentifier;
                sec.Result = ret;
                return ret;
            }
        }

        public override List<MetadataEntry> GetContentMetadata(List<string> names, List<string> namePrefixes)
        {
            using (var sec = this.GetCodeSection(new { names = names.GetLogString(), namePrefixes = namePrefixes.GetLogString() }))
            {
                var filteredMetadata = new Dictionary<string, string>();

                sec.Debug(new { namePrefixes = namePrefixes.GetLogString() });
                foreach (var namePrefix in namePrefixes)
                {
                    sec.Debug($"namePrefix: {namePrefix}, _executionStateOptions.metadata: {_executionStateOptions.metadata.GetLogString()}");
                    if (_executionStateOptions.metadata != null)
                    {
                        foreach (var prop in _executionStateOptions.metadata)
                        {
                            if (prop.Key.StartsWith(namePrefix))
                            {
                                filteredMetadata.Add(prop.Key, prop.Value); sec.Debug($"filteredMetadata.Add({prop.Key}, {prop.Value}); completed");
                            }
                        }
                    }
                }

                sec.Debug(new { names = names.GetLogString() });
                foreach (var name in names)
                {
                    string value = string.Empty;
                    var ok = _executionStateOptions.metadata!=null? _executionStateOptions.metadata.TryGetValue(name, out value): false;
                    if (ok) { filteredMetadata.Add(name, value); sec.Debug($"filteredMetadata.Add({name}, {value}); completed"); }
                }

                var result = new List<MetadataEntry>();
                sec.Debug(new { filteredMetadata = filteredMetadata.GetLogString() });
                foreach (var item in filteredMetadata)
                {
                    result.Add(new MetadataEntry(item.Key, item.Value ?? "", new MetadataVersion(0, MetadataVersionFormat.DEFAULT))); sec.Debug($"result.Add(new MetadataEntry({item.Key}, {item.Value}, 0));"); // 
                }

                sec.Result = result.GetLogString();
                return result;
            }
        }

        public override Microsoft.InformationProtection.Label GetNewLabel()
        {
            using (var sec = this.GetCodeSection())
            {
                var newLabel = _executionStateOptions.newLabel;
                sec.Result = newLabel.GetLogString();
                return newLabel;
            }
        }

        public override AssignmentMethod GetNewLabelAssignmentMethod()
        {
            using (var sec = this.GetCodeSection())
            {
                var ret = _executionStateOptions.assignmentMethod;
                sec.Result = ret.GetLogString();
                return ret;
            }
        }

        public override ProtectionDescriptor GetProtectionDescriptor()
        {
            using (var sec = this.GetCodeSection())
            {
                //var desc = new ProtectionDescriptor(_executionStateOptions.templateId) { AllowOfflineAccess = true };

                sec.Result = _executionStateOptions.protectionDescriptor.GetLogString();
                return _executionStateOptions.protectionDescriptor;
            }
        }

        /// <summary>The UPE SDK will always notify client of 'JUSTIFY', 'METADATA', and 'REMOVE*' actions. 
        ///  However an application can
        ///  choose not to support specific actions that may appear in a policy. 
        ///  (For instance, A policy may define a label to require both protection and a watermark, 
        ///  but the application could decide not to support watermarks by not including ADD_WATERMARK here. 
        ///  If that were the case, 'mip::PolicyEngine::ComputeActions' would never return AddWatermark actions.)</summary>
        /// <returns></returns>
        public override ActionType GetSupportedActions()
        {
            using (var sec = this.GetCodeSection())
            {
                var ret = _executionStateOptions.supportedActions;
                sec.Result = ret.GetLogString();
                return ret;
            }
        }

        public override bool IsDowngradeJustified(out string justificationMessage)
        {
            using (var sec = this.GetCodeSection())
            {
                justificationMessage = _executionStateOptions.downgradeJustification;
                sec.Debug($"justificationMessage:{justificationMessage}");

                var ret = _executionStateOptions.isDowngradeJustified;
                sec.Result = ret;
                return ret;
            }
        }
    }
}
