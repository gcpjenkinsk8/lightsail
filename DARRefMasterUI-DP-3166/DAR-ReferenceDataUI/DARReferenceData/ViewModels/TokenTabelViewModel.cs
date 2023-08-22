using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class TokenTableViewModel: DARRoleViewModel
    {

        public long legacyID { get; set; }
        public string darTicker { get; set; }

        public string Name { get; set; }
        public string ftseStatus { get; set; }
        public string indexStatus { get; set; }
        public string exchangeSourceStatus { get; set; }
        public string otherPricing { get; set; }

        public string darAssetID { get; set; }

        public override string GetDescription()
        {
            return $"{Name} - {darAssetID} - {darTicker}";
        }
    }
}
