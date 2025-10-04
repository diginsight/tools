#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace Common
{
    public class UserHelper
    {
        const string CONFIGVALUE_INTERNALDOMAINS = "InternalDomains"; const string DEFAULTVALUE_INTERNALDOMAINS = "fstechnology.it;anas.it";

        private static List<string> _internalDomains;

        static UserHelper() {
            var internalDomains = ConfigurationHelper.GetClassSetting<MIPHelper, string>(CONFIGVALUE_INTERNALDOMAINS, DEFAULTVALUE_INTERNALDOMAINS); // , CultureInfo.InvariantCulture
            _internalDomains = internalDomains?.Split(';')?.Where(s => !string.IsNullOrEmpty(s))?.Select(s => s?.Trim())?.ToList();
        }

        public static bool IsInternalEmail(string u)
        {
            return _internalDomains != null && _internalDomains.Any(id => u.EndsWith(id, StringComparison.InvariantCultureIgnoreCase));
        }
        public static bool IsExternalEmail(string u)
        {
            return _internalDomains == null || _internalDomains.All(id => u.EndsWith(id, StringComparison.InvariantCultureIgnoreCase) == false);
        }
        public static bool IsDistributionList(string user, IList<GroupsProfile> groupProfiles)
        {
            var groupProfile = groupProfiles != null ? groupProfiles.FirstOrDefault(gp => gp.email == user) : null;
            var group = groupProfile != null && groupProfile.value != null && groupProfile.value.Length > 0 ? groupProfile.value[0] : null;
            if (group != null && group.mailEnabled == true && group.securityEnabled == false)
            {
                return true;
            }
            return false;
        }
    }
}
