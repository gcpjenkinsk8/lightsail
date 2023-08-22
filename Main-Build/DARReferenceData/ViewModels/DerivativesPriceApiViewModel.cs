using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DARReferenceData.ViewModels
{
    public class DerivativesPriceApiViewModel
    {
        [JsonProperty("darExchangeID")]
        public string DarExchangeID { get; set; }

        [JsonProperty("darContractID")]
        public string DARContractID { get; set; }

        [JsonProperty("underlierDARAssetID")]
        public string UnderlierDARAssetID { get; set; }


        [JsonProperty("currencyTicker")]
        public string CurrencyTicker { get; set; }

        [JsonProperty("bestBidPrice")]
        public decimal BestBidPrice { get; set; }

        [JsonProperty("bestBidPriceUSD")]
        public decimal BestBidPriceUSD { get; set; }

        [JsonProperty("bestAskPrice")]
        public decimal BestAskPrice { get; set; }

        [JsonProperty("bestAskPriceUSD")]
        public decimal BestAskPriceUSD { get; set; }

        [JsonProperty("bestBidSize")]
        public decimal BestBidSize { get; set; }

        [JsonProperty("bestBidSizeUSD")]
        public decimal BestBidSizeUSD { get; set; }

        [JsonProperty("bestAskSize")]
        public decimal BestAskSize { get; set; }

        [JsonProperty("bestAskSizeUSD")]
        public decimal BestAskSizeUSD { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("priceUSD")]
        public decimal PriceUSD { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("sizeUSD")]
        public string SizeUSD { get; set; }

        

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("markPrice")]
        public decimal markPrice { get; set; }

        [JsonProperty("markPriceUSD")]
        public decimal MarkPriceUSD { get; set; }

        [JsonProperty("indexPrice")]
        public decimal indexPrice { get; set; }

        
        [JsonProperty("tradeDate")]
        public string TradeDate { get; set; }
    }
}
