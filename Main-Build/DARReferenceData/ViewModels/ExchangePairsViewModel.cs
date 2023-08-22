using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class ExchangePairsViewModel : DARViewModel
    {
        public string DARPairID { get; set; }
        public string DARExchangeID { get; set; }

        public string Exchange { get; set; }
        public string ExchangePair { get; set; }
        public string DARAssetID { get; set; }

        public string Asset { get; set; }
        public string DARCurrencyID { get; set; }

        public string Currency { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public override string GetDescription()
        {
            return $"{DARPairID} {ExchangePair} {DARExchangeID} {DARAssetID}";
        }
    }
}
