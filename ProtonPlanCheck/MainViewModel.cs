using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace ProtonPlanCheck
{
    public class MainViewModel : ViewModelBase
    {
        //Expose the view content here.
        public IonPlanSetup IonPlanSetup { get; set; }
        public IEnumerable<PlanCheckResult> PlanCheckResults { get; set; }
        public IEnumerable<StructureCheckResult> StructureCheckResults { get; set; }

        public string TestString { get; set; }

        public void PlanCheck()
        {
            var planChecker = new PlanChecker();
            PlanCheckResults = planChecker.Check(IonPlanSetup);

            

        }
        public void StructureCheck()
        {
            var strucureChecker = new StructureChecker();
            StructureCheckResults = strucureChecker.Check(IonPlanSetup);
        }
    }
}
