using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class ExchangePairViewModel: DARViewModel
    {
		public long ID { get; set; }

		public string UniqueID { get; set; }

		public string PairID { get; set; }

		
        public string ExchangePairName { get; set; }

        public int? ExchangePairNumberId { get; set; }
        public string ExchangePairStringId { get; set; }
        public string ExchangePairShortName { get; set; }
        public string ExchangePairLongName { get; set; }
        public string ExchangeAssetStringId { get; set; }
        public int? ExchangeAssetNumberId { get; set; }
        public string ExchangeAssetShortName { get; set; }
        public string ExchangeAssetLongName { get; set; }
        public string ExchangeCurrencyStringId { get; set; }
        public int? ExchangeCurrencyNumberId { get; set; }
        public string ExchangeCurrencyShortName { get; set; }
        public string ExchangeCurrencyLongName { get; set; }

        public string DARExchangePairShortName { get; set; }

        public string DARAssetId { get; set; }

        public string DARCurrencyId { get; set; }

        public string DARExchangeId { get; set; }

        public bool? IsAvailable { get; set; }
        public override string GetDescription()
        {
            return $"{PairID} - {DARExchangeId} - {ExchangePairName}";
        }
    }
}
