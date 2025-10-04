using Microsoft.InformationProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MipDocumentInspector
{

    public class ProtectionDescriptorSurrogate
    {
        public ProtectionDescriptorSurrogate() { }
        public ProtectionDescriptorSurrogate(ProtectionDescriptor pd)
        {
            if (pd != null)
            {
                this.AllowOfflineAccess = pd.AllowOfflineAccess;
                this.ContentId = pd.ContentId;
                this.ContentValidUntil = pd.ContentValidUntil;
                this.Description = pd.Description;
                this.DoubleKeyUrl = pd.DoubleKeyUrl;
                this.EncryptedAppData = pd.EncryptedAppData;
                this.LabelId = pd.LabelId;
                this.Name = pd.Name;
                this.Owner = pd.Owner;
                this.ProtectionType = pd.ProtectionType;
                this.Referrer = pd.Referrer;
                this.SignedAppData = pd.SignedAppData;
                this.TemplateId = pd.TemplateId;

                var userRights = pd.UserRights != null ? pd.UserRights.Select(ur => new UserRightsSurrogate(ur.Users?.ToList(), ur.Rights?.ToList())).ToList() : null;
                this.UserRights = userRights;

                var userRoles = pd.UserRoles != null ? pd.UserRoles.Select(ur => new UserRolesSurrogate(ur.Users?.ToList(), ur.Roles?.ToList())).ToList() : null;
                this.UserRoles = pd.UserRoles;
            }
        }
        public ProtectionDescriptorSurrogate(string templateId) { }
        public ProtectionDescriptorSurrogate(List<UserRights> userRights) { }
        public ProtectionDescriptorSurrogate(List<UserRoles> userRoles) { }

        public Dictionary<string, string> EncryptedAppData { get; set; }
        public DateTime? ContentValidUntil { get; set; }
        public string Referrer { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }


        public Dictionary<string, string> SignedAppData { get; set; }
        public string Owner { get; set; }

        public string LabelId { get; set; }
        public string TemplateId { get; set; }
        public List<UserRightsSurrogate> UserRights { get; set; }
        public List<UserRoles> UserRoles { get; set; }
        public ProtectionType ProtectionType { get; set; }

        public string ContentId { get; set; }
        public string DoubleKeyUrl { get; set; }
    }

    public class UserRightsSurrogate
    {
        public UserRightsSurrogate() { }
        public UserRightsSurrogate(List<string> users, List<string> rights)
        {
            this.Users = users;
            this.Rights = rights;
        }

        public List<string> Users { get; set; }
        public List<string> Rights { get; set; }
    }

    //
    // Summary:
    //     Represents a group of users and the rights associated with them
    public class UserRolesSurrogate
    {
        public UserRolesSurrogate() { }
        public UserRolesSurrogate(List<string> users, List<string> roles)
        {
            this.Users = users;
            this.Roles = roles;
        }

        public List<string> Users { get; set; }
        public List<string> Roles { get; set; }
    }


}
