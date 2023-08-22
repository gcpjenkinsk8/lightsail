using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class ServListViewModel: DARViewModel
    {
        string DARMnemonic { get; set; }
        long LegacyExchangeID { get; set; }
        string ExchangePairName { get; set; }
        string QuoteCurrency { get; set; }
        long DARExchangeVettingStatus { get; set; } 
        long QuoteCurrencyPriceTier { get; set; }
        DateTime LoadTime { get; set; }
        DateTime StartTime { get; set; }
        DateTime EndTime { get; set; }
        string DARAssetID { get; set; }
        string DARCurrencyID { get; set; }
        string DARExchangeID { get; set; }
        string DARPairID { get; set; }

        public override string GetDescription()
        {
            return $"{DARMnemonic} - {ExchangePairName} - {DARAssetID} - {DARExchangeID}";
        }
    }
}
