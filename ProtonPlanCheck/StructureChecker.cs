using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace ProtonPlanCheck
{
    public class StructureChecker
    {
        public IEnumerable<StructureCheckResult> Check(IonPlanSetup plan)
        {
            var structureCheckResults = new List<StructureCheckResult>();

            structureCheckResults = structureCheckResults.Concat(CheckHUOveride(plan)).ToList();
            structureCheckResults = structureCheckResults.Concat(CheckMissingPlane(plan)).ToList();
            structureCheckResults = structureCheckResults.Concat(CheckLateral(plan)).ToList();
            return structureCheckResults;
        }

        private string[] StructureTypeToCheck = {"ORGAN", "PTV", "CTV", "EXTERNAL"};

        List<StructureCheckResult> CheckHUOveride(IonPlanSetup plan)
        {
            IEnumerable<Structure> structures = plan.StructureSet.Structures;
            List<StructureCheckResult> structureCheckResults = new List<StructureCheckResult>();
            foreach (Structure s in structures)
            {
                if (StructureTypeToCheck.Contains(s.DicomType.ToUpper()))
                {
                    double assignedHU = 0.0;
                    if (s.GetAssignedHU(out assignedHU))
                    {
                        StructureCheckResult structureCheckResult = new StructureCheckResult();
                        structureCheckResult.StructureID = s.Id;
                        structureCheckResult.Comments = "HU Assigned";
                        structureCheckResult.Parameters.Add("HU", assignedHU.ToString("0.00"));
                        structureCheckResults.Add(structureCheckResult);
                    }
                }
            }

            if(structureCheckResults.Count==0)
            {
                StructureCheckResult structureCheckResult = new StructureCheckResult();
                structureCheckResult.StructureID = "";
                structureCheckResult.Comments = "No HU Assigned Structures";
                structureCheckResults.Add(structureCheckResult);
            }

            return structureCheckResults;
        }

        List<StructureCheckResult> CheckMissingPlane(IonPlanSetup plan)
        {
            IEnumerable<Structure> structures = plan.StructureSet.Structures;
            List<StructureCheckResult> structureCheckResults = new List<StructureCheckResult>();

            foreach (Structure s in structures)
            {
                if (StructureTypeToCheck.Contains(s.DicomType.ToUpper()))
                {
                    if (s.HasSegment)
                    {
                        int changed = 0;
                        bool lastplaneSegment = s.GetContoursOnImagePlane(0).Length != 0;
                        bool currentplaneSegment = false;

                        for (int i = 1; i < plan.StructureSet.Image.ZSize; i++)
                        {
                            currentplaneSegment = (s.GetContoursOnImagePlane(i).Length != 0);
                            if (lastplaneSegment != currentplaneSegment)
                                changed++;
                            lastplaneSegment = currentplaneSegment;
                        }
                        if (changed > 2)
                        {
                            StructureCheckResult structureCheckResult = new StructureCheckResult();
                            structureCheckResult.StructureID = s.Id;
                            structureCheckResult.Comments = "Has missing contour!";
                            structureCheckResults.Add(structureCheckResult);
                        }
                    }
                }
            }
            if (structureCheckResults.Count == 0)
            {
                StructureCheckResult structureCheckResult = new StructureCheckResult();
                structureCheckResult.StructureID = "";
                structureCheckResult.Comments = "No missing contour found.";
                structureCheckResults.Add(structureCheckResult);
            }
            return structureCheckResults;
        }

        List<StructureCheckResult> CheckLateral(IonPlanSetup plan)
        {
            IEnumerable<Structure> structures = plan.StructureSet.Structures;
            List<StructureCheckResult> structureCheckResults = new List<StructureCheckResult>();

            char[] wordSplit = { ' ', '_', '-' };

            foreach (Structure s in structures)
            {
                if (StructureTypeToCheck.Contains(s.DicomType.ToUpper()))
                {
                    string[] structureNames = s.Id.ToUpper().Split(wordSplit);
                    if (structureNames.Contains("LT") || structureNames.Contains("L"))
                    {
                        if (s.CenterPoint.x < 0)
                        {
                            StructureCheckResult structureCheckResult = new StructureCheckResult();
                            structureCheckResult.StructureID = s.Id;
                            structureCheckResult.Comments = "May have wrong lateral!";
                            structureCheckResults.Add(structureCheckResult);
                        }
                    }
                    if (structureNames.Contains("RT") || structureNames.Contains("R"))
                    {
                        if (s.CenterPoint.x > 0)
                        {
                            StructureCheckResult structureCheckResult = new StructureCheckResult();
                            structureCheckResult.StructureID = s.Id;
                            structureCheckResult.Comments = "May have wrong lateral!";
                            structureCheckResults.Add(structureCheckResult);
                        }
                    }
                }
            }

            if (structureCheckResults.Count == 0)
            {
                StructureCheckResult structureCheckResult = new StructureCheckResult();
                structureCheckResult.StructureID = "";
                structureCheckResult.Comments = "all contour lateral is right.";
                structureCheckResults.Add(structureCheckResult);
            }
            return structureCheckResults;

        }
    }
}
