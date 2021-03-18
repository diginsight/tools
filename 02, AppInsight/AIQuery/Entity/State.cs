using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationInsightQuery
{
    #region MainControlState
    public class MainControlState
    {
        #region AIApplicationID
        public string AIApplicationID { get; set; }
        #endregion
        #region AIApiKey
        public string AIApiKey { get; set; }
        #endregion
    }
    #endregion
    #region MainControlStateProvider
    public class MainControlStateProvider : ClassStateProvider<MainControl, MainControlState>
    {
        #region .ctor
        public MainControlStateProvider()
        {
            this.True2Surrogate = delegate (MainControl t)
            {
                MainControlState state = new MainControlState() { AIApplicationID = t.AIApplicationID, AIApiKey = t.AIApiKey };
                return state;
            };

            this.Surrogate2True = delegate (MainControl t, MainControlState s)
            {
                if (t == null) { t = new MainControl(); }

                t.AIApplicationID = s.AIApplicationID;
                t.AIApiKey = s.AIApiKey;

                return t;
            };
        }
        #endregion

    }
    #endregion

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
