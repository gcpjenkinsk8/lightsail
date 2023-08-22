using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{

    public partial class PrincipalMarketPriceDar
    {
        public string Ticker { get; set; }
        public string Methodology { get; set; }
     
        public long WindowStart { get; set; }
        public long WindowEnd { get; set; }
        public decimal UsdPrice { get; set; }
        public decimal PriceVolume { get; set; }
        public decimal PrincipalMarketVolume { get; set; }
        public long EffectiveTime { get; set; }
        public string PriceId { get; set; }
        public string DarIdentifier { get; set; }
        public string DarExchangeName { get; set; }
        public string PricingTier { get; set; }
        public string AssetName { get; set; }
        public string Currency { get; set; }

        public string DarExchangeID { get; set; }

    }
}
