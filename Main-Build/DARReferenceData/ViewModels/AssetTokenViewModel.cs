using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class AssetTokenViewModel : DARViewModel
    {
        public string ID { get; set; }

        public string AssetID { get; set; }

        public string TokenId { get; set; }

        public string BlockChainId { get; set; }

        public string DARAssetID { get; set; }
        public string DARTicker { get; set; }
        public string AssetName { get; set; }
        public string DARTokenID { get; set; }
        public string TokenName { get; set; }
        public string BlockChain { get; set; }
        public string TokenContractAddress { get; set; }
        public string ConsensusMechanism { get; set; }
        public string HashAlgorithm { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }

        public override string GetDescription()
        {
            return $"{DARAssetID} - {AssetName} - {TokenName}";
        }
    }
}