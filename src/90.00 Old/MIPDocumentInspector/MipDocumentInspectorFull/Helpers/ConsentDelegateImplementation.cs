#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.InformationProtection; 
#endregion

namespace Common
{
    /// <summary>Certain SDK operations require that the user consents to the action as it may log PII. 
    /// IConsentDelegate allows developers to impement a consent flow that meets their application and business requirement.
    /// This sample always passes back Consent.Accept, but the user could be presented with options that show them the URL
    /// And allow them to accept or reject consent.</summary>
    class ConsentDelegateImplementation : IConsentDelegate
    {
        public Consent GetUserConsent(string url)
        {
            using (var sec = this.GetCodeSection(new { url }))
            {
                return Consent.Accept;
            }
        }
    }
}
