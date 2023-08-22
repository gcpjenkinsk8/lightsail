using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DARReferenceData.ViewModels
{
    public class PoolAssets
    {
        [JsonProperty("tokenDARTicker")]
        public string TokenDARTicker { get; set; }
        
        [JsonProperty("tokenDARAssetID")]
        public string TokenDARAssetID { get; set; }

        [JsonProperty("tokenBalance")]
        public decimal TokenBalance { get; set; }

        [JsonProperty("tokenValueInPool")]
        public decimal TokenValueInPool { get; set; }

        [JsonProperty("tokenPrice")]
        public decimal TokenPrice { get; set; }

        [JsonProperty("tokenQuoteCurrency")]
        public string TokenQuoteCurrency { get; set; }

        [JsonProperty("tokenWindowStart")]
        public string WindowStart { get; set; }

        [JsonProperty("tokenWindowEnd")]
        public string WindowEnd { get; set; }
    }

    public class LiquidityPoolViewModel 
    {
        [JsonProperty("darPoolID")]
        public string DarPoolID { get; set; }

        [JsonProperty("darPoolTicker")]
        public string DarPoolTicker { get; set; }

        [JsonProperty("darPoolName")]
        public string DarPoolName { get; set; }


        [JsonProperty("darPoolDescription")]
        public string DarPoolDescription { get; set; }

        [JsonProperty("poolBalance")]
        public decimal PoolBalance { get; set; }


        [JsonProperty("poolTokenPrice")]
        public decimal PoolTokenPrice { get; set; }

        [JsonProperty("poolValueTotal")]
        public decimal PoolValueTotal { get; set; }
        
        [JsonProperty("poolQuoteCurrency")]
        public string PoolQuoteCurrency { get; set; }

        [JsonProperty("methodologyCode")]
        public string MethodologyCode { get; set; }

        [JsonProperty("poolAssets")]
        public IEnumerable<PoolAssets> PoolAssets { get; set; }


        [JsonProperty("effectiveTime")]
        public string EffectiveTime { get; set; }

        [JsonProperty("errors")]
        public IEnumerable<string> Errors { get; set; }


        [JsonIgnore]
        public string tokenDARTicker { get; set; }

        [JsonIgnore]
        public string tokenDARAssetID { get; set; }

        [JsonIgnore]
        public decimal tokenBalance { get; set; }

        [JsonIgnore]
        public decimal tokenPrice { get; set; }

        [JsonIgnore]
        public decimal tokenValueInPool { get; set; }

        [JsonIgnore]
        public string tokenQuoteCurrency { get; set; }

        [JsonIgnore]
        public string windowStart { get; set; }

        [JsonIgnore]
        public string windowEnd { get; set; }


    }
}