using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class DerivativesViewModel: DARViewModel
    {
        public string DARDerivativeID { get; set; }
        public string UnderlierDARTicker { get; set; }
        public string UnderlierDARAssetID { get; set; }
        public string ContractType { get; set; }
        public string OptionType { get; set; }
        public string ContractTicker { get; set; }
        public string DARContractID { get; set; }
        public string ContractExchange { get; set; }
        public string ContractExchangeDARID { get; set; }
        public string Status { get; set; }
        public string TradingHours { get; set; }
        public decimal MinimumTickSize { get; set; }
        public string SettlementTime { get; set; }
        public string SettlementType { get; set; }
        public string SettlementCurrency { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int ContractSize { get; set; }
        public string InitialMargin { get; set; }
        public string MaintenanceMargin { get; set; }
        public string MarkPrice { get; set; }
        public string DeliveryPrice { get; set; }
        public string DeliveryMethod { get; set; }
        public string FeesURL { get; set; }
        public string PositionLimit { get; set; }
        public string PositionLimitURL { get; set; }
        public string BlockTradeMinimum { get; set; }
        public string LinktoTAndC {get;set;}
        public string FundingRates { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }

        public override string GetDescription()
        {
            return $"DARContractID: {DARContractID}";
        }
    }
}
