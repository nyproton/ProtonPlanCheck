using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.Types;

namespace ProtonPlanCheck
{


    public class FieldNameChecker
    {

        public string FieldName { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Postfix { get; set; }
        public double GantryAngle { get; set; }
        public double CouchAngle { get; set; }
        public string Warning { get; set; }
        public bool IsLegitName { get; set; }
        public bool IsAngleMatch { get; set; }
        public FieldOrientation Orientation;
        public PatientOrientation PatientPosition;

        private string NameRule
            = @"^([0-9]{1,2}) ?(([LR][AP][SI][O])|([AP][SI][O])|([LR][SI][O])|([LR][AP][O])|VERTEX|((?:LT|RT) (?:LAT|MED))|ANT|POST)(?: (\w+)?)?";

        public FieldNameChecker(string fieldName, double gantry, double couch, PatientOrientation patientOrientation=PatientOrientation.HeadFirstSupine)
        {
            this.FieldName = fieldName;
            this.GantryAngle = gantry;
            this.CouchAngle = couch;
            this.PatientPosition = patientOrientation;
            fieldNameDecoder(fieldName);
            if (IsLegitName)
            {
                this.Orientation = new FieldOrientation(Name);
                IsAngleMatch = GantryAngleMatch(this.Orientation, this.GantryAngle, this.CouchAngle, this.PatientPosition);
            }

        }

        private void fieldNameDecoder(string fieldName)
        {
            Match m = Regex.Match(fieldName, NameRule);
            if (m.Success)
            {
                IsLegitName = true;
                Warning = "Good Legit Field Name";
                ID = m.Groups[1].Value;
                Name = m.Groups[2].Value;
                Postfix = m.Groups[8].Value;
            }
            else
            {
                IsLegitName = false;
                Warning = "Not Legit Field Name";
            }
            return;
        }


        private bool GantryAngleMatch(FieldOrientation fieldOrientation, double gantry, double couch, PatientOrientation patientOrientation)
        {
            bool checkLR=false;
            bool checkAP=false;
            bool checkSI=false;
            switch (patientOrientation)
            {
                case PatientOrientation.HeadFirstSupine:
                    {
                        //check some special case
                        switch (fieldOrientation.FieldName)
                        {
                            case "VERTEX":
                                if ((couch == 90.00 && gantry == 270.00) ||
                                    (couch == 270.00 & gantry == 90.00))
                                    checkLR = checkAP = checkSI = true;
                                break;

                            case "ANT":
                                if (gantry == 0.00 && couch == 0.00)
                                    checkLR = checkAP = checkSI = true;
                                break;
                            case "POST":
                                if (gantry == 180.00 && couch == 0.00)
                                    checkLR = checkAP = checkSI = true;
                                break;
                            case "LT LAT":
                                if (gantry == 90.00 && couch == 0.00)
                                    checkLR = checkAP = checkSI = true;
                                break;
                            case "RT LAT":
                                if (gantry == 270.00 && couch == 0.00)
                                    checkLR = checkAP = checkSI = true;
                                break;
                            default:
                                //check Left Right
                                if (gantry > 0.00 && gantry < 180.00 && (couch != 90.00 || couch != 270.00))
                                    checkLR = fieldOrientation.LeftRight == FieldOrientation.LeftorRight.Left;
                                else if (gantry > 180.00 && gantry < 359.99)
                                    checkLR = fieldOrientation.LeftRight == FieldOrientation.LeftorRight.Right;
                                else
                                    checkLR = fieldOrientation.LeftRight == FieldOrientation.LeftorRight.NA;

                                //check Antieror Postieror
                                if ((gantry > 270.00 && gantry < 359.99) || (gantry >= 0.00 && gantry < 90.00))
                                    checkAP = fieldOrientation.AntPost == FieldOrientation.AntorPost.Anterior;
                                else if (gantry > 90.00 && gantry < 270.00)
                                    checkAP = fieldOrientation.AntPost == FieldOrientation.AntorPost.Posterior;
                                else
                                    checkAP = fieldOrientation.AntPost == FieldOrientation.AntorPost.NA;

                                //check Superior and Inferior
                                if ((gantry > 0.00 && gantry < 180.00 && couch > 270.00 && couch < 359.99) ||
                                    (gantry > 180.00 && gantry < 359.99 && couch > 0.00 && couch < 90.00))
                                    checkSI = fieldOrientation.SupInf == FieldOrientation.SuporInf.Superior;
                                else if ((gantry > 0.00 && gantry < 180.00 && couch > 0.00 && couch < 90.00) ||
                                    (gantry > 180.00 && gantry < 359.99 && couch > 270.00 && couch < 359.99))
                                    checkSI = fieldOrientation.SupInf == FieldOrientation.SuporInf.Inferior;
                                else
                                    checkSI = fieldOrientation.SupInf == FieldOrientation.SuporInf.NA;

                                break;
                        }
                        break;
                    }
            
                case PatientOrientation.HeadFirstProne:
                    break;
                case PatientOrientation.FeetFirstSupine:
                    break;
                case PatientOrientation.FeetFirstProne:
                    break;
            }
            if (!checkLR) Warning += System.Environment.NewLine + "Left Right mismatch";
            if (!checkAP) Warning += System.Environment.NewLine + "Anterior Posterior mismatch";
            if (!checkSI) Warning += System.Environment.NewLine + "Superior Inferior mismatch";
            return checkLR && checkAP && checkSI; }


    }
}
