using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class AssetCustodianViewModel: DARViewModel
    {
     
        public string ID { get; set; }

        public string DARCustodianID { get; set; }

        public string DARAssetID { get; set; }
        public string AssetName { get; set; }

        public string Custodian { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }

        public override string GetDescription()
        {
            return $"{DARAssetID} - {AssetName} - {Custodian}";
        }
    }
}
