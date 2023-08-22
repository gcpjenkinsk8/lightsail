using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class PairViewModel: DARViewModel
    {
        public string ID { get; set; }
        public string UniqueID { get; set; }
        public string AssetID { get; set; }
        public string QuoteAssetID { get; set; }
        public string DARName { get; set; }

        public string Exchange { get; set;}
        public string ExchangePairName { get; set; }

        public int SourceId { get; set; }
        public string AssetTicker { get; set; }

        public string AssetDarId { get; set; }

        public string AssetName { get; set; }

        public string CurrencyTicker { get; set; }

        public string CurrencyDarId { get; set; }

        public string CurrencyName { get; set; }

        public string ExchangeVettingStatus { get; set; }
        public string ExchangeVettingStatusDescription { get; set; }
        public string AssetTierCode { get; set; }
        public string AssetTierDescription { get; set; }
        public string QuoteAssetTierCode { get; set; }
        public string QuoteAssetTierDescription { get; set; }

        public string AssetLegacyId { get; set; }
        public string AssetLegacyDARAssetId { get; set; }
        public string ExchangeLegacyId { get; set; }    
        public string QuoteAssetLegacyId { get; set; }
        public string QuoteAssetLegacyDARAssetId { get; set; }


        public override string GetDescription()
        {
            return $"{DARName} - {Exchange} - {AssetTicker}";
        }
    }
}
