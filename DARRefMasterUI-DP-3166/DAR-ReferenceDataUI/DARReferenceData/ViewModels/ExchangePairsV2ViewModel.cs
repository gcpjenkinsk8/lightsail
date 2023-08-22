using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class ExchangePairsV2ViewModel : DARViewModel
    {
        public string ExchangePairID { get; set; }
        public long legacyExchangeID { get; set; }
        public long legacyAssetID { get; set; }
        public string DARExchangeID { get; set; }

        public string ExchangeName { get; set; }
        public string ExchangePair { get; set; }
        public string DARAssetID { get; set; }
        public string AssetTicker { get; set; }
        public string AssetName { get; set; }
        public long legacyCurrencyID { get; set; }
        public string DARCurrencyID { get; set; }

        public string CurrencyTicker { get; set; }
        public string blockchain { get; set; }
        public string contractAddress { get; set; }
        public string darPairID { get; set; }

        public string CurrencyName { get; set; }

        public string Operation { get; set; }

        public long Deleted { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal LoadTimestamp { get; set; }
        public override string GetDescription()
        {
            return $"{legacyExchangeID} - {ExchangeName} - {DARExchangeID} - {DARAssetID}";
        }
    }
}
