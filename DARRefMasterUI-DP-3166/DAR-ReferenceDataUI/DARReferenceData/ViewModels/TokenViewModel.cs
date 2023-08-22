using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class TokenViewModel: DARViewModel
    {
        public string DARTokenID { get; set; }

        public string DARAssetID { get; set; }

        public string DARBlockchainID { get; set; }
        public string TokenContractAddress { get; set; }

        public string TokenName { get; set; }
        public string TokenDescription { get; set; }

        public string DARTicker { get; set; }
        public string TokenBlockchainBase { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }

        public override string GetDescription()
        {
            return $"{DARAssetID} - {DARTicker} - {TokenName}";
        }
    }
}
