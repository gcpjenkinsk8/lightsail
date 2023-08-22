using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DARReferenceData.ViewModels
{
    public class BTCBlockChainTxn
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("transactions")]
        public List<BTCBlockChainTxnViewModel> Transactions { get; set; }

    }
    public class BTCBlockChainTxnViewModel 
    {

        [JsonProperty("darAssetID")]
        public string DARAssetID { get; set; }

        [JsonProperty("darTicker")]
        public string DARTicker { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        [JsonProperty("blockIndex")]
        public string BlockIndex { get; set; }

        [JsonProperty("blockNumber")]
        public string BlockNumber { get; set; }

        [JsonProperty("blockTimestamp")]
        public DateTime blockTimestamp { get; set; }

        [JsonProperty("fee")]
        public string Fee { get; set; }

        [JsonProperty("fromAddress")]
        public string FromAddress { get; set; }

        [JsonProperty("toAddress")]
        public string ToAddress { get; set; }

        [JsonProperty("txnHash")]
        public string TxnHash { get; set; }
        [JsonProperty("spentTxnHash")]
        public string SpentTxnHash { get; set; }

        public string GetDescription()
        {
            return $"{TxnHash} - {BlockHash} - {Amount}  - {Fee}";
        }
    }

    public class BTCBlockChainPos
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("positions")]
        public List<BTCBlockChainBalanceViewModel> Positions { get; set; }

    }
    public class BTCBlockChainBalanceViewModel 
    {
        [JsonProperty("darAssetID")]
        public string DARAssetID { get; set; }

        [JsonProperty("darTicker")]
        public string DARTicker { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
        
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        [JsonProperty("blockNumber")]
        public string BlockNumber { get; set; }

        [JsonProperty("blockTimestamp")]
        public DateTime blockTimestamp { get; set; }

        [JsonProperty("txnHash")]
        public string TxnHash { get; set; }

        [JsonProperty("positionTimestamp ")]
        public DateTime PositionTimestamp { get; set; }



        


        public string GetDescription()
        {
            return $"{TxnHash} - {BlockHash} - {Address}  - {Amount}";
        }
    }
}