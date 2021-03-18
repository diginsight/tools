using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelemetryViewer
{
    #region ApplicationState
    public class ApplicationState
    {
        #region AIApplicationID
        public string AIApplicationID { get; set; }
        #endregion
        #region AIApiKey
        public string AIApiKey { get; set; }
        #endregion
    }
    #endregion
    #region ApplicationStateProvider
    public class ApplicationStateProvider : ClassStateProvider<App, ApplicationState>
    {
        #region .ctor
        public ApplicationStateProvider()
        {
            this.True2Surrogate = delegate (App t)
            {
                ApplicationState state = new ApplicationState() { AIApplicationID = t.AIApplicationID, AIApiKey = t.AIApiKey };
                return state;
            };

            this.Surrogate2True = delegate (App t, ApplicationState s)
            {
                //if (t == null) { t = new App(); }
                t.AIApplicationID = s.AIApplicationID;
                t.AIApiKey = s.AIApiKey;

                return t;
            };
        }
        #endregion

    }
    #endregion
}
