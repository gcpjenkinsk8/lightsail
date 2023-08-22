using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class NonPricingServListViewModel : DARRoleViewModel
    {
        public string darMnemonic { get; set; }
        public string darAssetID { get; set; }

        public string note { get; set; }

    }
}
