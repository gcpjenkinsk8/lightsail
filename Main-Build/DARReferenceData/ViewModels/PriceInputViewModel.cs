using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class PriceInputViewModel
    {
        public string Name { get; set; }
        public string Pair { get; set; }
        public string Ticker { get; set; }
        public decimal AvgUSDPrice { get; set; }
        public int TradeCount { get; set; }

        public decimal USDVolume { get; set; }

    }
}
