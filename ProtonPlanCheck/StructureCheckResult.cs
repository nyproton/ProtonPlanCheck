using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtonPlanCheck
{
    public class StructureCheckResult
    {
        public string StructureID { get; set; }
        public string Comments { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
