using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DARReferenceData.ViewModels
{
    public class ShortAsset
    {
        [JsonProperty("darAssetID")]
        public string DARAssetID { get; set; }
        
        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }





    }

    public class ClientAccessViewModel 
    {
        [JsonProperty("clientName")]
        public string ClientName { get; set; }

        [JsonProperty("hasAccessToAllPricedAssets")]
        public string HasAccessToAllPricedAssets { get; set; }

        [JsonProperty("hasAccessToHourlyPrice")]
        public string HasAccessToHourlyPrice { get; set; }


        [JsonProperty("hasAccessToLatestPrice")]
        public string HasAccessToLatestPrice { get; set; }

        [JsonProperty("hasAccessToDerivativePrices")]
        public string HasAccessToDerivativePrices { get; set; }


        [JsonProperty("hasAccessToLPTokenPrices")]
        public string HasAccessToLPTokenPrices { get; set; }

        [JsonProperty("hasAccessToNFTPrices")]
        public string HasAccessToNFTPrices { get; set; }

        [JsonProperty("assets")]
        public IEnumerable<ShortAsset> Assets { get; set; }

        [JsonIgnore]
        public string DARAssetID { get; set; }

        [JsonIgnore]
        public string AssetName { get; set; }

        [JsonIgnore]
        public string DARTicker { get; set; }


    }
}