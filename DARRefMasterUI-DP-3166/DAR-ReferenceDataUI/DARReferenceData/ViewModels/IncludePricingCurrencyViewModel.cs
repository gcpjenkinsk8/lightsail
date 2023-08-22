using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class IncludePricingCurrencyViewModel : DARViewModel
    {
        public string darMnemonicFamily { get; set; }

        public long priceTier { get; set; }

        public string currencyTicker { get; set; }
        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        public override string GetDescription()
        {
            return $"{darMnemonicFamily} - {priceTier} - {currencyTicker}";
        }
    }
}
