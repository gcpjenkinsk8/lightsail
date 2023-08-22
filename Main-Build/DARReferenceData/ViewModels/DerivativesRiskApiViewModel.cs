using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class DerivativesRiskApiViewModel
    {

        [JsonProperty("contractTicker")]
        public string ContractTicker { get; set; }

        [JsonProperty("darContractID")]
        public string DARContractID { get; set; }

        [JsonProperty("underlierDARTicker")]
        public string UnderlierDARTicker { get; set; }

        [JsonProperty("underlierDARAssetID")]
        public string UnderlierDARAssetID { get; set; }


        [JsonProperty("contractType")]
        public string ContractType { get; set; }

        [JsonProperty("optionType")]
        public string OptionType { get; set; }

        [JsonProperty("contractExchange")]
        public string ContractExchange { get; set; }

        [JsonProperty("DARContractExchangeID")]
        public string ContractExchangeDARID { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("vega")]
        public string Vega { get; set; }

        [JsonProperty("theta")]
        public string Theta { get; set; }


        [JsonProperty("rho")]
        public string Rho { get; set; }


        [JsonProperty("gamma")]
        public string Gamma { get; set; }

        [JsonProperty("delta")]
        public string Delta { get; set; }

        [JsonProperty("openInterest")]
        public string OpenInterest { get; set; }
    }
}
