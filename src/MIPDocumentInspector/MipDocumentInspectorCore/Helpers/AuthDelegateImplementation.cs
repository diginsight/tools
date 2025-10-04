#region using
using System;
using System.Configuration;
using Microsoft.InformationProtection;
using Common;
using System.Windows;
#endregion

namespace Common
{
    public struct FileOptions
    {
        public string FileName;
        public string OutputName;
        public string LabelId;
        public DataState DataState;
        public AssignmentMethod AssignmentMethod;
        public ActionSource ActionSource;
        public bool IsAuditDiscoveryEnabled;
        public bool GenerateChangeAuditEvent;
    }
    public class AuthenticationHelperMIP : AuthenticationHelper, IAuthDelegate
    {
        public AuthenticationHelperMIP(string applicationId, Window window) : base(applicationId, window) { }

        public string AcquireToken(Microsoft.InformationProtection.Identity identity, string authority, string resource, string claims)
        {
            return ((AuthenticationHelper)this).AcquireToken(ToIdentity(identity), authority, resource, claims);
        }

        public static Identity ToIdentity(Microsoft.InformationProtection.Identity identity)
        {
            var identityRet = new Identity(identity.Email, identity.Name);
            return identityRet;
        }
        public static Microsoft.InformationProtection.Identity ToIdentityMIP(Identity identity)
        {
            var identityRet = new Microsoft.InformationProtection.Identity(identity.Upn, identity.Name);
            return identityRet;
        }
    }
}
