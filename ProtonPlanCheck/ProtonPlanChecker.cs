using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;


namespace ProtonPlanCheck
{
    public class PlanChecker
    {
        public IEnumerable<PlanCheckResult> Check(IonPlanSetup plan)
        {
            var planCheckResults = new List<PlanCheckResult>();
            //start with different checks
            planCheckResults.Add(CheckCourseName(plan));
            planCheckResults.Add(CheckPlanName(plan));
            planCheckResults.Add(CheckPlanModel(plan));
            //planCheckResults = planCheckResults.Concat(CheckCalcOption(plan)).ToList();
            planCheckResults = planCheckResults.Concat(CheckImage(plan)).ToList();
            planCheckResults.Add(CheckSameISO(plan));
            planCheckResults.Add(CheckISOInteger(plan));
            planCheckResults.Add(CheckSameRangeShift(plan));
            planCheckResults.Add(CheckSupportStructure(plan));
            planCheckResults.Add(CheckCTCalibration(plan));
            planCheckResults.Add(CheckTableCoords(plan));

            //then some field checks
            planCheckResults=planCheckResults.Concat(CheckLabelGantryAngle(plan)).ToList();
            return planCheckResults;
        }

        //check course name
        PlanCheckResult CheckCourseName(IonPlanSetup plan)
        {
            var planCheckResult = new PlanCheckResult
            {
                Item = "Course Name",
                Expected = "",
                CurrentPlan = plan.Course.Id,
                Pass = PlanCheckResult.CheckResult.Pass,
                Activated = true,
                Comments = ""
            };

            if (!CourseNameRule(plan.Course.Id))
            {
                planCheckResult.Pass = PlanCheckResult.CheckResult.Warning;
                planCheckResult.Comments = "Course Name is not right.";
            }
            return planCheckResult;
        }

        bool CourseNameRule(string planName)
        {
            //course name rules apply here
            string[] possibleCourseName =
            {
                "BRAIN", "LUNG", "CSI", "BREAST", "PROSTATE", "LIVER",
                "PANCREAS", "MANTLE", "PELVIS", "H&N", "ESOPHAGUS",
                "RECTUM", "ANAL", "ABDOMEN"
            }; // Legit name e.g. C1 BRAIN
            return (Array.Exists(possibleCourseName, s => planName.Contains(s))
                && planName.StartsWith("C")
                && char.IsDigit(planName[1]));
        }

        //check plan name
        PlanCheckResult CheckPlanName(IonPlanSetup plan)
        {
            var planCheckResult = new PlanCheckResult
            {
                Item = "Plan Name",
                Expected = "",
                CurrentPlan = plan.Id,
                Pass = PlanCheckResult.CheckResult.Pass,
                Activated = true,
                Comments = ""
            };

            if (!PlanNameRule(plan.Id))
            {
                planCheckResult.Pass = PlanCheckResult.CheckResult.Warning;
                planCheckResult.Comments = "Plan Name is not right.";
            }
            return planCheckResult;
        }

        bool PlanNameRule(string planName)
        {
            //Plan Name Rules apply here
            string[] possiblePlanName =
            {
                "INITIAL",
                "CD1",
                "CD2",
                "INITIAL-P",
            };
            return Array.Exists(possiblePlanName, s => s == planName);
        }

        //check plan calculation model
        PlanCheckResult CheckPlanModel(IonPlanSetup plan)
        {
            var planCheckResult = new PlanCheckResult
            {
                Item = "Proton Dose Calculation Model",
                Expected = "15.5",
                CurrentPlan = plan.ProtonCalculationModel,
                Pass = PlanCheckResult.CheckResult.Pass,
                Activated = true,
                Comments = ""
            };
            if (!planCheckResult.CurrentPlan.Contains(planCheckResult.Expected))
            {
                planCheckResult.Pass = PlanCheckResult.CheckResult.Fail;
                planCheckResult.Comments = "Old model is used.";
            }
            return planCheckResult;
        }

        //check plan calculation option
        List<PlanCheckResult> CheckCalcOption(IonPlanSetup plan)
        {
            List<PlanCheckResult> planCheckResults = new List<PlanCheckResult>();

            Dictionary<string, string> currentCalcOptions = plan.ProtonCalculationOptions;
            Dictionary<string, string> defaultCalcOptions = new DefaultCalcOptions();

            foreach (string s in defaultCalcOptions.Keys)
            {
                if (currentCalcOptions[s].ToUpper() != defaultCalcOptions[s].ToUpper())
                {
                    var planCheckResult = new PlanCheckResult();
                    planCheckResult.Item = s;
                    planCheckResult.Expected = defaultCalcOptions[s];
                    planCheckResult.CurrentPlan = currentCalcOptions[s];
                    planCheckResult.Pass = PlanCheckResult.CheckResult.Warning;
                    planCheckResult.Comments = "Calculation Option is not same as default";
                    planCheckResults.Add(planCheckResult);
                }
            }
            if (planCheckResults.Count==0)
            {
                var planCheckResult = new PlanCheckResult();
                planCheckResult.Item = "Calculation Options Checked";
                planCheckResult.Expected = "Default";
                planCheckResult.CurrentPlan = "Default";
                planCheckResult.Pass = PlanCheckResult.CheckResult.Pass;
                planCheckResult.Comments = "";
                planCheckResults.Add(planCheckResult);

            }

            return planCheckResults;
        }

        //15.5 does not expose proton optimization model. Check this in next version.
        List<PlanCheckResult> CheckOptiOption(IonPlanSetup plan)
        {
            List<PlanCheckResult> planCheckResults = new List<PlanCheckResult>();


            return planCheckResults;
        }

        //Check Image (Orientation, CT thickness, or missing CT, total number, etc.)
        List<PlanCheckResult> CheckImage(IonPlanSetup plan)
        {
            List<PlanCheckResult> planCheckResults = new List<PlanCheckResult>();
            

            Image image = plan.StructureSet.Image;

            //Orientation
            PatientOrientation patientOrientation = image.ImagingOrientation;
            
            if (patientOrientation==PatientOrientation.HeadFirstSupine)
            {
                PlanCheckResult planCheckResult = new PlanCheckResult();
                planCheckResult.Item = "Patient Orientation";
                planCheckResult.Expected = PatientOrientation.HeadFirstSupine.ToString();
                planCheckResult.CurrentPlan = patientOrientation.ToString();
                planCheckResult.Pass = PlanCheckResult.CheckResult.Pass;
                planCheckResult.Comments = "";
                planCheckResults.Add(planCheckResult);
            }
            else
            {
                PlanCheckResult planCheckResult = new PlanCheckResult();
                planCheckResult.Item = "Patient Orientation";
                planCheckResult.Expected = "";
                planCheckResult.CurrentPlan = patientOrientation.ToString();
                planCheckResult.Pass = PlanCheckResult.CheckResult.Warning;
                planCheckResult.Comments = "Patient is NOT HeadFirstSupine, please double check patient orientation.";
                planCheckResults.Add(planCheckResult);
            }
            

            //CT thickness
            double CTThickness = image.ZRes;
            double defaultCTThickness = 2.5;
            
            if (CTThickness-defaultCTThickness<1e-7)
            {
                PlanCheckResult planCheckResult = new PlanCheckResult();
                planCheckResult.Item = "CT Thickness (mm)";
                planCheckResult.Expected = "2.5";
                planCheckResult.CurrentPlan = CTThickness.ToString();
                planCheckResult.Pass = PlanCheckResult.CheckResult.Pass;
                planCheckResult.Comments = "";
                planCheckResults.Add(planCheckResult);
            }
            else
            {
                PlanCheckResult planCheckResult = new PlanCheckResult();
                planCheckResult.Item = "CT Thickness (mm)";
                planCheckResult.Expected = "2.5";
                planCheckResult.CurrentPlan = CTThickness.ToString("0.00");
                planCheckResult.Pass = PlanCheckResult.CheckResult.Warning;
                planCheckResult.Comments = "CT thickness is not same as default. Please check.";
                planCheckResults.Add(planCheckResult);
            }
            

            //CT total slices < 300
            double CTSlices = image.ZSize;
            double maxSlice = 300;
            
            if (CTSlices <= maxSlice)
            {
                PlanCheckResult planCheckResult = new PlanCheckResult();
                planCheckResult.Item = "CT Slices";
                planCheckResult.Expected = "<300";
                planCheckResult.CurrentPlan = CTSlices.ToString();
                planCheckResult.Pass = PlanCheckResult.CheckResult.Pass;
                planCheckResult.Comments = "";
                planCheckResults.Add(planCheckResult);
            }
            else
            {
                PlanCheckResult planCheckResult = new PlanCheckResult();
                planCheckResult.Item = "CT Slices";
                planCheckResult.Expected = "<300";
                planCheckResult.CurrentPlan = CTSlices.ToString();
                planCheckResult.Pass = PlanCheckResult.CheckResult.Warning;
                planCheckResult.Comments = "CT slice is more than 300";
                planCheckResults.Add(planCheckResult);
            }

            return planCheckResults;
        }

        //check if all beams are same iso center
        PlanCheckResult CheckSameISO(IonPlanSetup plan)
        {
            PlanCheckResult planCheckResult = new PlanCheckResult();
            planCheckResult.Item = "Same ISO";
            planCheckResult.Expected = "All beams ISO are same";
            IEnumerable<IonBeam> ionBeams = plan.IonBeams;
            var firstBeamISO = ionBeams.First().IsocenterPosition;
            bool isSame = ionBeams.All(b => b.IsocenterPosition.Equals(firstBeamISO)) ? true : false;
            planCheckResult.CurrentPlan = isSame ? "All beams ISO are same" : "Some beam ISO is different";
            planCheckResult.Pass = isSame ? PlanCheckResult.CheckResult.Pass : PlanCheckResult.CheckResult.Warning;
            planCheckResult.Comments = isSame ? "Isocenter is:(" 
                + firstBeamISO.x.ToString("0.00") + ","
                + firstBeamISO.y.ToString("0.00") + ","
                + firstBeamISO.z.ToString("0.00") + ")."
                : "Please check the ISO Center for all beams";
            return planCheckResult;
        }

        //check if iso center are integer
        PlanCheckResult CheckISOInteger(IonPlanSetup plan)
        {
            PlanCheckResult planCheckResult = new PlanCheckResult();
            planCheckResult.Item = "ISO Integer";
            planCheckResult.Expected = "ISO is integer";
            IEnumerable<IonBeam> ionBeams = plan.IonBeams;
            var firstBeamISO = ionBeams.First().IsocenterPosition;
            bool isInteger = (
                Math.Abs(firstBeamISO.x % 1) < Double.Epsilon * 100 &&
                Math.Abs(firstBeamISO.y % 1) < Double.Epsilon * 100 &&
                Math.Abs(firstBeamISO.z % 1) < Double.Epsilon * 100);
            planCheckResult.CurrentPlan = isInteger ? "ISO is integer" : "ISO is not integer";
            planCheckResult.Pass = isInteger ? PlanCheckResult.CheckResult.Pass : PlanCheckResult.CheckResult.Warning;
            planCheckResult.Comments = isInteger ? "" : "Please check the ISO Center for all beams";
            return planCheckResult;
        }

        //check if all beams are same range shift
        PlanCheckResult CheckSameRangeShift(IonPlanSetup plan)
        {
            PlanCheckResult planCheckResult = new PlanCheckResult();
            planCheckResult.Item = "Same Range Shift";
            planCheckResult.Expected = "All beams range shift are same";
            IEnumerable<IonBeam> ionBeams = plan.IonBeams;
            IEnumerable<RangeShifter> firstBeamRangeShifters = ionBeams.First().RangeShifters;
            bool isSame = ionBeams.All(b => b.RangeShifters.SequenceEqual(firstBeamRangeShifters)) ? true : false;
            planCheckResult.CurrentPlan = isSame ? "All beams range shift are same" : "Some beam range shift is different";
            planCheckResult.Pass = isSame ? PlanCheckResult.CheckResult.Pass : PlanCheckResult.CheckResult.Warning;
            planCheckResult.Comments = isSame ? "" : "Please check the range shift for all beams";
            return planCheckResult;
        }

        //check Support Structure
        PlanCheckResult CheckSupportStructure(IonPlanSetup plan)
        {
            PlanCheckResult planCheckResult = new PlanCheckResult();
            IEnumerable<Structure> structures = plan.StructureSet.Structures;

            bool hasSupport = false;
            foreach(Structure s in structures)
            {
                IEnumerable<StructureCodeInfo> structureCode = s.StructureCodeInfos;
                bool isSupport = structureCode.Any(sc => sc.Code == "Support");
                if (isSupport) hasSupport = true;
            }

            planCheckResult.Item = "Support Structure";
            planCheckResult.Expected = "Support Structure exists";
            planCheckResult.CurrentPlan = hasSupport ? "Support Structure exists" : "Support Structure not exists";
            planCheckResult.Pass = hasSupport ? PlanCheckResult.CheckResult.Pass : PlanCheckResult.CheckResult.Warning;
            planCheckResult.Comments = hasSupport ? "" : "Please check the support structure";
            return planCheckResult;
        }

        //Check CT Calibration Curve
        PlanCheckResult CheckCTCalibration(IonPlanSetup plan)
        {
            PlanCheckResult planCheckResult = new PlanCheckResult();
            string CTCalibrationCurrent = plan.StructureSet.Image.Series.ImagingDeviceId;
            string CTCalibrationExpected = "GE CT";
            planCheckResult.Item = "CT Calibration Curve";
            planCheckResult.Expected = CTCalibrationExpected;
            planCheckResult.CurrentPlan = CTCalibrationCurrent;
            planCheckResult.Pass = (CTCalibrationCurrent == CTCalibrationExpected) ? PlanCheckResult.CheckResult.Pass : PlanCheckResult.CheckResult.Fail;
            planCheckResult.Comments = (CTCalibrationCurrent == CTCalibrationExpected) ? "" : "Please check the CT calibration curve";
            return planCheckResult;
        }

        //Check Table coordinates should be 0,0,0
        PlanCheckResult CheckTableCoords(IonPlanSetup plan)
        {
            
            bool tableCoordsZero = true;
            IEnumerable<IonBeam> beams = plan.IonBeams;
            foreach (IonBeam b in beams)
            {
                if (Math.Abs(b.IonControlPoints.First().TableTopLongitudinalPosition - 0.0) > Double.Epsilon * 100)
                    tableCoordsZero = false;
                if (Math.Abs(b.IonControlPoints.First().TableTopLateralPosition - 0.0) > Double.Epsilon * 100)
                    tableCoordsZero = false;
                if (Math.Abs(b.IonControlPoints.First().TableTopVerticalPosition - 0.0) > Double.Epsilon * 100)
                    tableCoordsZero = false;
            }
            PlanCheckResult planCheckResult = new PlanCheckResult();
            planCheckResult.Item = "Check Table Coords";
            planCheckResult.Expected = "All table coords are zero";
            planCheckResult.CurrentPlan = tableCoordsZero ? "All table coords are zero" : "some table coords are not zero";
            planCheckResult.Pass = tableCoordsZero ? PlanCheckResult.CheckResult.Pass : PlanCheckResult.CheckResult.Warning;
            planCheckResult.Comments = tableCoordsZero ? "" : "Please check the table coordinations";
            return planCheckResult;
        }

        //check fields
        List<PlanCheckResult> CheckLabelGantryAngle(IonPlanSetup plan)
        {
            List<PlanCheckResult> planCheckResults = new List<PlanCheckResult>();
            IEnumerable<IonBeam> ionBeams = plan.IonBeams;

            foreach(IonBeam b in ionBeams)
            {
                double gantry = b.IonControlPoints.First().GantryAngle;
                double couch = b.IonControlPoints.First().PatientSupportAngle;

                FieldNameChecker fieldNameChecker = new FieldNameChecker(
                    b.Id,
                    gantry,
                    couch,
                    plan.StructureSet.Image.ImagingOrientation);

                bool pass = fieldNameChecker.IsLegitName || fieldNameChecker.IsAngleMatch;

                PlanCheckResult planCheckResult = new PlanCheckResult();
                planCheckResult.Item = "Check " + b.Id + "...";
                planCheckResult.Expected = "";
                planCheckResult.CurrentPlan = pass ? "Pass!" : "Not Pass";
                planCheckResult.Pass = pass ? PlanCheckResult.CheckResult.Pass : PlanCheckResult.CheckResult.Warning;
                planCheckResult.Comments = pass ? "" : fieldNameChecker.Warning;
                planCheckResults.Add(planCheckResult);

                //check the gantry and couch agnle is integer
                if (Math.Abs(gantry % 1) >= (Double.Epsilon * 100))
                {
                    planCheckResult.Item = b.Id + " Gantry";
                    planCheckResult.Expected = "Integer";
                    planCheckResult.CurrentPlan = gantry.ToString("0.00");
                    planCheckResult.Pass = PlanCheckResult.CheckResult.Warning;
                    planCheckResult.Comments ="Gantry angle is not integer.";
                    planCheckResults.Add(planCheckResult);
                }
                if (Math.Abs(couch % 1) >= (Double.Epsilon * 100))
                {
                    planCheckResult.Item = b.Id + " Couch";
                    planCheckResult.Expected = "Integer";
                    planCheckResult.CurrentPlan = couch.ToString("0.00");
                    planCheckResult.Pass = PlanCheckResult.CheckResult.Warning;
                    planCheckResult.Comments = "Couch angle is not integer.";
                    planCheckResults.Add(planCheckResult);
                }
            }

            return planCheckResults;
        }
    }
}
