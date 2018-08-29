using ProtonPlanCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace VMS.TPS
{
    public class Script
    {
        public void Execute(ScriptContext scriptContext, Window mainWindow)
        {
            Run(scriptContext.CurrentUser,
                scriptContext.Patient,
                scriptContext.StructureSet,
                scriptContext.PlanSetup,
                scriptContext.PlansInScope,
                scriptContext.PlanSumsInScope,
                scriptContext.IonPlanSetup,
                scriptContext.IonPlansInScope,
                mainWindow);
        }

        public void Run(
            User user,
            Patient patient,
            StructureSet structureSet,
            PlanSetup planSetup,
            IEnumerable<PlanSetup> planSetupsInScope,
            IEnumerable<PlanSum> planSumsInScope,
            IonPlanSetup ionPlanSetup,
            IEnumerable<IonPlanSetup> ionPlanSetupsInScope,
            Window mainWindow)
        {
            var mainViewModel = new MainViewModel();
            mainViewModel.IonPlanSetup = ionPlanSetup;

            mainViewModel.PlanCheck();
            mainViewModel.StructureCheck();

            var mainView = new MainView();
            //patient.BeginModifications();
            //StructureSet ss = ionPlanSetup.StructureSet;
            //Structure s = ss.Structures.First();
            //SegmentVolume temp1 = s.SegmentVolume.Margin(10);
            //SegmentVolume temp2 = temp1.Margin(-10);
            //temp1.

            //if (ss.CanAddStructure("PTV", "temp1"))
            //{
            //    Structure temp1 = ss.AddStructure("PTV", "temp1");
            //    Structure temp2 = ss.AddStructure("PTV", "temp2");
            //    temp1.SegmentVolume = s.Margin(10).Margin(-10);
            //    temp2.SegmentVolume = s.Margin(-10).Margin(10);
            //    mainView.MyTestBox.Text = s.Volume.ToString() + " " + temp1.Volume.ToString() + " " + temp2.Volume.ToString();
            //}
            //else
            //    mainView.MyTestBox.Text = ss.CanAddStructure("PTV", "temp1").ToString();

            //mainView.MyTestBox.Text = ionPlanSetup.StructureSet.Structures.First().Id
                //+ ionPlanSetup.StructureSet.Structures.First().CenterPoint.x.ToString("0.00")
                //+ ionPlanSetup.StructureSet.Structures.First().CenterPoint.y.ToString("0.00")
                //+ ionPlanSetup.StructureSet.Structures.First().CenterPoint.z.ToString("0.00");

            mainView.ViewModel = mainViewModel;
            mainWindow.Content = mainView;
            mainWindow.SizeToContent = SizeToContent.WidthAndHeight;
            mainWindow.ResizeMode = ResizeMode.NoResize;
        }
    }
}
