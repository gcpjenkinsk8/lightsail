using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class MarketCapViewModel: DARViewModel
    {
        [JsonProperty("darAssetID")]
        public string DARAssetID { get; set; }

        [JsonProperty("darAssetTicker")]
        public string DARTicker { get; set; }
        
        [JsonIgnore]
        public string AssetName { get; set; }
        
        [JsonIgnore]
        public decimal HourlyPrice { get; set; }

        [JsonProperty("marketCap")]
        public decimal MarketCap { get; set; }

        [JsonProperty("quoteCurrency")]
        public string QuoteCurrency { get; set; }

        [JsonIgnore]
        public string PriceID { get; set; }
        [JsonIgnore]
        public decimal OutstandingSupply { get; set; }
        [JsonIgnore]
        public string OutstandingSupplyLoadTime { get; set; }

        [JsonProperty("effectiveTime")]
        public string LoadTime { get; set; }

        [JsonProperty("errors")]
        public string Errors { get; set; }

  

        public override string GetDescription()
        {
            return $"{DARTicker} - {AssetName} - {HourlyPrice} - {MarketCap} - {PriceID} - {OutstandingSupply}";
        }
    }
}
