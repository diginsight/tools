using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class UserProfileHelper
    {
        const string CONFIGVALUE_PROFILEDATAFOLDER = "ProfileDataFolder"; const string DEFAULTVALUE_PROFILEDATAFOLDER = null;

        public static string GetTempFolder() {
            var tempPath = default(string);
            
            var profileDataFolder = ConfigurationHelper.GetClassSetting<UserProfileHelper, string>(CONFIGVALUE_PROFILEDATAFOLDER, DEFAULTVALUE_PROFILEDATAFOLDER); // , CultureInfo.InvariantCulture
            if (string.IsNullOrEmpty(profileDataFolder)) {
                tempPath = Path.GetTempPath();
                return tempPath.TrimEnd('\\');
            }

            var specialFolder = (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), profileDataFolder);
            tempPath = Environment.GetFolderPath(specialFolder);

            return tempPath;
        }
    }
}
