using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class CustodianViewModel : DARViewModel
    {
        public string DARCustodianID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }

        public override string GetDescription()
        {
            return $"{Name} - {Description} - {DARCustodianID}";
        }
    }
}
