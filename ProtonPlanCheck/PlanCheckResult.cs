using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace ProtonPlanCheck
{
    public class PlanCheckResult
    {
        public enum CheckResult { Pass, Fail, Warning}
        //e.g. Plan Name, 1 Lung, Not Compliant to TG203, here is comment, blahblah
        public string Item { get; set; }
        public string Expected { get; set; }
        public string CurrentPlan { get; set; }
        public CheckResult Pass { get; set; }
        public bool Activated { get; set; }
        public string Comments { get; set; }
    }
}
