using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtonPlanCheck
{
    public class FieldOrientation
    {
        public enum LeftorRight { NA = 0, Left, Right }
        public enum AntorPost { NA = 0, Anterior, Posterior }
        public enum SuporInf { NA = 0, Superior, Inferior }

        private LeftorRight _LeftRight;
        private AntorPost _AntPost;
        private SuporInf _SupInf;
        private string _FieldName;

        public LeftorRight LeftRight
        {
            get
            {
                return _LeftRight;
            }
        }
        public AntorPost AntPost
        {
            get
            {
                return _AntPost;
            }
        }
        public SuporInf SupInf
        {
            get
            {
                return _SupInf;
            }
        }
        public string FieldName
        {
            get
            {
                return _FieldName;
            }
        }

        public FieldOrientation()
        {
            _LeftRight = 0;
            _AntPost = 0;
            _SupInf = 0;
            _FieldName = "NA";
        }

        public FieldOrientation(string fieldName)
        {
            _FieldName = fieldName;

            if (fieldName == "VERTEX")
            { _LeftRight = 0; _AntPost = 0; _SupInf = SuporInf.Superior; }
            else if (fieldName .Contains("ANT"))
            { _LeftRight = 0; _AntPost = AntorPost.Anterior; _SupInf =0; }
            else if (fieldName.Contains("POST"))
            { _LeftRight = 0; _AntPost = AntorPost.Posterior; _SupInf = 0; }
            else if (fieldName.Contains("LAT") && fieldName.Length == 6)
            {
                if (fieldName.Substring(0, 3) == "LT ")
                { _LeftRight = LeftorRight.Left; _AntPost = 0; _SupInf = 0; }
                else if (fieldName.Substring(0, 3) == "RT ")
                { _LeftRight = LeftorRight.Right; _AntPost = 0; _SupInf = 0; }
                else { }//throw exception something is wrong
            }
            else if (fieldName.Length==3 || fieldName.Length==4)
            {
                if (fieldName.Substring(fieldName.Length - 1, 1) == "O")
                {
                    if (fieldName.Contains("L"))
                        _LeftRight = LeftorRight.Left;
                    else if (fieldName.Contains("R"))
                        _LeftRight = LeftorRight.Right;
                    else
                        _LeftRight = LeftorRight.NA;

                    if (fieldName.Contains("A"))
                        _AntPost = AntorPost.Anterior;
                    else if (fieldName.Contains("P"))
                        _AntPost = AntorPost.Posterior;
                    else
                        _AntPost = AntorPost.NA;

                    if (fieldName.Contains("S"))
                        _SupInf = SuporInf.Superior;
                    else if (fieldName.Contains("I"))
                        _SupInf = SuporInf.Inferior;
                    else
                        _SupInf = SuporInf.NA;
                }
                else { }//throw exception something is wrong
            }
        }

        public FieldOrientation(LeftorRight lr, AntorPost ap, SuporInf si)
        {
            _LeftRight = lr;
            _AntPost = ap;
            _SupInf = si;
            _FieldName = FieldNameByOrientation(lr, ap, si);
        }

        private string FieldNameByOrientation(LeftorRight lr, AntorPost ap, SuporInf si)
        {
            StringBuilder fieldName = new StringBuilder();
            if (lr == 0 && ap == 0 && si == 0) fieldName.Append("NA");
            else if (lr == 0 && ap == 0)
                fieldName.Append(ap == AntorPost.Anterior ? "VERTEX" : "NA");
            else if (si == 0 && ap == 0)
                fieldName.Append(lr == LeftorRight.Left ? "LT LAT" : "RT LAT");
            else
            {
                switch (lr)
                {
                    case LeftorRight.Left: fieldName.Append("L"); break;
                    case LeftorRight.Right: fieldName.Append("R"); break;
                    case LeftorRight.NA: break;

                }
                switch (ap)
                {
                    case AntorPost.Anterior: fieldName.Append("A"); break;
                    case AntorPost.Posterior: fieldName.Append("P"); break;
                    case AntorPost.NA: break;

                }

                switch (si)
                {
                    case SuporInf.Superior: fieldName.Append("S"); break;
                    case SuporInf.Inferior: fieldName.Append("I"); break;
                    case SuporInf.NA: break;
                }
                fieldName.Append("O");
            }
            return fieldName.ToString();
        }


    }
}
